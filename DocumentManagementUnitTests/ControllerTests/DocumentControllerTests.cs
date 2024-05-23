using DocumentManagement.Server.Controllers;
using DocumentManagement.Server.Models;
using DocumentManagement.Server.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace DocumentManagementUnitTests.ControllerTests
{
    [TestClass]
    public class DocumentControllerTests
    {
        [TestMethod]
        public async Task GetDocuments_Returns_OkResult()
        {
            // Arrange
            var documentServiceMock = new Mock<IDocumentService>();
            var expectedDocuments = new List<Document>();
            documentServiceMock.Setup(service => service.GetDocuments(It.IsAny<int>(), It.IsAny<int>()))
                                .ReturnsAsync(expectedDocuments);
            documentServiceMock.Setup(service => service.GetDocumentCount())
                                .ReturnsAsync(10);

            var controller = new DocumentController(documentServiceMock.Object);

            // Act
            var result = await controller.GetDocuments(1, 10);

            // Assert
            Assert.IsNotNull(result);
            var objectResult = result.Result as ObjectResult;
            Assert.AreEqual(StatusCodes.Status200OK, objectResult.StatusCode);
        }

        [TestMethod]
        public async Task GetDocuments_Returns_InternalServerError_On_Exception()
        {
            // Arrange
            var documentServiceMock = new Mock<IDocumentService>();
            documentServiceMock.Setup(service => service.GetDocuments(It.IsAny<int>(), It.IsAny<int>()))
                                .ThrowsAsync(new Exception("Test exception"));

            var loggerMock = new Mock<ILogger<DocumentController>>();
            var controller = new DocumentController(documentServiceMock.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            // Act
            var result = await controller.GetDocuments(1, 10);

            // Assert
            Assert.IsNotNull(result);
            var objectResult = result.Result as ObjectResult;
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        }

        [TestMethod]
        public async Task UploadFiles_Success()
        {
            // Arrange
            var documentServiceMock = new Mock<IDocumentService>();
            documentServiceMock.Setup(service => service.UploadFilesAsync(It.IsAny<List<IFormFile>>(), It.IsAny<string>()))
                               .ReturnsAsync(true);
            documentServiceMock.Setup(service => service.TargetDirectory).Returns("Doc");
            documentServiceMock.Setup(service => service.SaveFileMetadataAsync(It.IsAny<List<IFormFile>>(), It.IsAny<Guid>()))
                               .ReturnsAsync(true);

            var controller = new DocumentController(documentServiceMock.Object);

            var files = new List<IFormFile>
            {
                new FormFile(Stream.Null, 0, 0, "file1", "file1.txt"),
                new FormFile(Stream.Null, 0, 0, "file2", "file2.txt"),
            };

            // Act
            var result = await controller.UploadFiles(files) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
        }

        [TestMethod]
        public async Task UploadFiles_NoFilesUploaded()
        {
            // Arrange
            var documentServiceMock = new Mock<IDocumentService>();
            var controller = new DocumentController(documentServiceMock.Object);

            // Act
            var result = await controller.UploadFiles(null) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
        }


        [TestMethod]
        public async Task DownloadFile_FileNotFound()
        {
            // Arrange
            var documentServiceMock = new Mock<IDocumentService>();
            var controller = new DocumentController(documentServiceMock.Object);
            var documentId = 123;

            documentServiceMock.Setup(service => service.GetFilePathAsync(documentId))
                               .ReturnsAsync(string.Empty);
            documentServiceMock.Setup(service => service.DownloadFileAsync(string.Empty))
                               .ReturnsAsync((byte[])null);

            // Act
            var result = await controller.DownloadFile(documentId) as NotFoundObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
        }

        [TestMethod]
        public async Task DownloadFile_ExceptionThrown()
        {
            // Arrange
            var documentServiceMock = new Mock<IDocumentService>();
            var controller = new DocumentController(documentServiceMock.Object);
            var documentId = 123;

            var exceptionMessage = "Test exception message";
            documentServiceMock.Setup(service => service.GetFilePathAsync(documentId))
                               .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await controller.DownloadFile(documentId) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, result.StatusCode);
            Assert.AreEqual($"An error occurred while downloading file with documentId: {documentId}", result.Value);
        }
    }
}
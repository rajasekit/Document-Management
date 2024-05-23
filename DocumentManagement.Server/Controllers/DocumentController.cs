using DocumentManagement.Server.Models.Dto;
using DocumentManagement.Server.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace DocumentManagement.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : Controller
    {
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedDocumentResponse>> GetDocuments([FromQuery] int page, [FromQuery] int pageSize)
        {
            try
            {
                var documents = await _documentService.GetDocuments(page, pageSize);

                var totalCount = await _documentService.GetDocumentCount();

                var result = new PaginatedDocumentResponse()
                {
                    Documents = documents,
                    TotalCount = totalCount
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while retrieving documents.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving documents.");
            }
        }

        [HttpPost]
        [Route("upload")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadFiles(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                Log.Warning("No files uploaded.");
                return BadRequest("No files uploaded.");
            }

            var caseNumber = Guid.NewGuid();
            var directoryPath = Path.Combine(_documentService.TargetDirectory, caseNumber.ToString());

            try
            {
                var fileUploadResponse = await _documentService.UploadFilesAsync(files, directoryPath);

                bool SaveFileMetadataResponse = false;

                if (fileUploadResponse)
                {
                    SaveFileMetadataResponse = await _documentService.SaveFileMetadataAsync(files, caseNumber);
                }

                if (!fileUploadResponse || !SaveFileMetadataResponse)
                {
                    await _documentService.RollbackUploadsAsync(directoryPath);
                    Log.Error("File upload failed. Transaction rolled back.");
                    return StatusCode(StatusCodes.Status500InternalServerError, "File upload failed. Transaction rolled back.");
                }

                return Ok(new { count = files.Count });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "File upload failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, "File upload failed.");
            }
        }

        [HttpGet("download/{documentId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DownloadFile(int documentId)
        {
            try
            {
                var filePath = await _documentService.GetFilePathAsync(documentId);
                var fileData = await _documentService.DownloadFileAsync(filePath);

                if (fileData != null)
                {
                    return File(fileData, "application/octet-stream", Path.GetFileName(filePath));
                }
                else
                {
                    Log.Warning("File not found.");
                    return NotFound("File not found.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"An error occurred while downloading file with documentId: {documentId}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while downloading file with documentId: {documentId}");
            }
        }
        
    }
}

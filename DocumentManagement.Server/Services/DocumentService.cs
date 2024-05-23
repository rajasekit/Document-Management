using DocumentManagement.Server.Helper;
using DocumentManagement.Server.Models;
using DocumentManagement.Server.Repository.Interface;
using DocumentManagement.Server.Services.Interface;
using Serilog;

namespace DocumentManagement.Server.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IConfiguration _configuration;

        public string TargetDirectory { get; set; }

        public DocumentService(IDocumentRepository documentRepository, IConfiguration configuration)
        {
            _documentRepository = documentRepository;            
            _configuration = configuration;
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), _configuration["CaseTransactionDirectoryPath"]);
            TargetDirectory = createDirectory(filePath);
        }

        public async Task<IEnumerable<Models.Document>> GetDocuments(int page, int pageSize)
        {
            try
            {
                return await _documentRepository.GetDocuments(page, pageSize);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while getting documents.");
                throw; 
            }
        }

        public async Task<int> GetDocumentCount()
        {
            try
            {
                return await _documentRepository.GetDocumentCount();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while getting document count.");
                throw; 
            }
        }

        public async Task<string> GetFilePathAsync(int documentId)
        {
            try
            {
                return await _documentRepository.GetFilePathAsync(documentId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error occurred while getting file path for documentId: {documentId}");
                throw; 
            }
        }

        public async Task<bool> UploadFilesAsync(List<IFormFile> files, string directoryPath)
        {
            try
            {
                createDirectory(directoryPath);
                foreach (var file in files)
                {
                    var filePath = Path.Combine(Path.Combine(directoryPath, file.FileName));


                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        memoryStream.Position = 0;

                        using (var encryptedStream = new MemoryStream())
                        {
                            FileEncryptionHelper.EncryptStream(memoryStream, encryptedStream);
                            var encryptedData = encryptedStream.ToArray();
                            await System.IO.File.WriteAllBytesAsync(filePath, encryptedData);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while uploading files.");
                return false;
            }


            return true;
        }

        public async Task RollbackUploadsAsync(string directoryPath)
        {
            try
            {
                if (Directory.Exists(directoryPath))
                {
                    Directory.Delete(directoryPath, true);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error occurred while deleting the directory: {directoryPath}");
                throw;
            }
        }

        public async Task<bool> SaveFileMetadataAsync(List<IFormFile> files, Guid caseNumber)
        {
            try
            {
                var caseTransaction = new CaseTransaction
                {
                    CaseNumber = caseNumber
                };

                var documents = new List<Models.Document>();

                foreach (var file in files)
                {
                    documents.Add(new Models.Document
                    {
                        FileName = file.FileName,
                        FileType = file.ContentType,
                        FileSize = file.Length,
                        FilePath = Path.Combine(TargetDirectory, caseNumber.ToString(), file.FileName),
                        UploadedBy = "Dev",
                        UploadDate = DateTime.Now
                    });
                }

                return await _documentRepository.SaveFileMetadataAsync(caseTransaction, documents);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while saving file metadata.");
                throw; 
            }
        }

        public async Task<byte[]> DownloadFileAsync(string filePath)
        {
            try
            {
                using (var encryptedStream = new MemoryStream(await System.IO.File.ReadAllBytesAsync(filePath)))
                {
                    using (var decryptedStream = new MemoryStream())
                    {
                        FileEncryptionHelper.DecryptStream(encryptedStream, decryptedStream);
                        decryptedStream.Position = 0;
                        return decryptedStream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error occurred while downloading file: {filePath}");
                throw; 
            }
        }

        private string createDirectory(string directoryPath)
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                return directoryPath;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error occurred while creating directory: {directoryPath}");
                throw; 
            }
        }





    }
}

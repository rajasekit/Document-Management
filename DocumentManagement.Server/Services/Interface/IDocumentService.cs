using DocumentManagement.Server.Models;

namespace DocumentManagement.Server.Services.Interface
{
    public interface IDocumentService
    {
        public string TargetDirectory { get; set; }
        Task<IEnumerable<Document>> GetDocuments(int page, int pageSize);
        Task<int> GetDocumentCount();
        Task<string> GetFilePathAsync(int documentId);
        Task<bool> UploadFilesAsync(List<IFormFile> files, string directoryPath);
        Task RollbackUploadsAsync(string directoryPath);
        Task<bool> SaveFileMetadataAsync(List<IFormFile> files, Guid caseNumber);
        Task<byte[]> DownloadFileAsync(string filePath);
    }
}

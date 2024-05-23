using DocumentManagement.Server.Models;

namespace DocumentManagement.Server.Repository.Interface
{
    public interface IDocumentRepository
    {
        Task<IEnumerable<Document>> GetDocuments(int page, int pageSize);
        Task<int> GetDocumentCount();
        Task<string> GetFilePathAsync(int documentId);
        Task<bool> SaveFileMetadataAsync(CaseTransaction caseTransaction, List<Document> documents);        
    }
}

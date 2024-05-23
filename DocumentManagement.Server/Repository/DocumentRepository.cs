using DocumentManagement.Server.Data;
using DocumentManagement.Server.Models;
using DocumentManagement.Server.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DocumentManagement.Server.Repository
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly ApplicationDbContext _context;

        public DocumentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Document>> GetDocuments(int page, int pageSize)
        {
            try
            {
                var documents = await _context.Documents
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

                return documents;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while getting documents.");
                throw;
            }
        }

        public async Task<int> GetDocumentCount()
        {
            try
            {
                var totalCount = await _context.Documents.CountAsync();

                return totalCount;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while getting document count.");
                throw;
            }
        }

        public async Task<string> GetFilePathAsync(int documentId)
        {
            try
            {
                var document = await _context.Documents
                    .FirstOrDefaultAsync(d => d.DocumentID == documentId);

                if (document != null)
                {
                    return document.FilePath;
                }
                else
                {
                    throw new FileNotFoundException("Document not found.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"An error occurred while retrieving the file path for document ID: {documentId}");
                throw;
            }
        }

        public async Task<bool> SaveFileMetadataAsync(CaseTransaction caseTransaction, List<Document> documents)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.CaseTransactions.Add(caseTransaction);
                    await _context.SaveChangesAsync();

                    foreach (var document in documents)
                    {
                        document.CaseTransactionID = caseTransaction.CaseTransactionID;
                    }

                    _context.Documents.AddRange(documents);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    Log.Information("Records inserted successfully within a transaction.");
                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Log.Error(ex, "An error occurred while saving file metadata.");
                    return false;
                }
            }
        }

        
    }
}

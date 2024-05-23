namespace DocumentManagement.Server.Models.Dto
{
    public class PaginatedDocumentResponse
    {
        public IEnumerable<Document> Documents { get; set; }
        public int TotalCount { get; set; }
    }
}

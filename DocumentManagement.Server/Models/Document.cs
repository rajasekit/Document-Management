namespace DocumentManagement.Server.Models
{
    public class Document
    {
        public int DocumentID { get; set; }
        public int CaseTransactionID { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public string FilePath { get; set; }
        public string UploadedBy { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.Now;
    }
}

using System.ComponentModel.DataAnnotations;

namespace HattmakarenWebbAppGrupp03.Models
{
    public class Email
    {
        [Key]
        public int Id { get; set; }
        public string Subject { get; set; }
        public string SenderEmail { get; set; }
        public string Body { get; set; }
        public DateTime ReceivedDate { get; set; }
        public bool IsRead { get; set; }
        public string MessageId { get; set; } // För att undvika att samma mail sparas två gånger
    }
}
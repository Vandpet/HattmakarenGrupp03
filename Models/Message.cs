using System.ComponentModel.DataAnnotations;

namespace HattmakarenWebbAppGrupp03.Models
{
    public class Message
    {
        public int Id { get; set; }

        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; } = null!;

        public int SenderEmployeeId { get; set; }
        public Employee SenderEmployee { get; set; } = null!;

        [Required]
        [StringLength(5000)]
        public string Content { get; set; } = null!;

        public DateTime SentAtUtc { get; set; }

        public bool IsSystemMessage { get; set; }
    }
}
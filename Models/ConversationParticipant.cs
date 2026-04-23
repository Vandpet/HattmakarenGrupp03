namespace HattmakarenWebbAppGrupp03.Models
{
    public class ConversationParticipant
    {
        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; } = null!;

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        public DateTime JoinedAtUtc { get; set; }

        public DateTime? LastReadAtUtc { get; set; }

        public bool IsArchived { get; set; }
    }
}
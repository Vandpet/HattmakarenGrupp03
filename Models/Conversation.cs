using System.ComponentModel.DataAnnotations;

namespace HattmakarenWebbAppGrupp03.Models
{
    public class Conversation
    {
        public int Id { get; set; }

        [StringLength(200)]
        public string? Title { get; set; }

        public bool IsBroadcast { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public int CreatedByEmployeeId { get; set; }
        public Employee CreatedByEmployee { get; set; } = null!;

        public ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
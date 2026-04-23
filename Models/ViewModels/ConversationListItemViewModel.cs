namespace HattmakarenWebbAppGrupp03.ViewModels.Messages
{
    public class ConversationListItemViewModel
    {
        public int ConversationId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string LastMessagePreview { get; set; } = string.Empty;
        public string LastSenderName { get; set; } = string.Empty;
        public DateTime? LastMessageSentAtUtc { get; set; }
        public int UnreadCount { get; set; }
        public bool IsBroadcast { get; set; }
    }
}
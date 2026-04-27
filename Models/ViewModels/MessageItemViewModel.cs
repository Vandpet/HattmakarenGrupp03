namespace HattmakarenWebbAppGrupp03.ViewModels.Messages
{
    public class MessageItemViewModel
    {
        public int MessageId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime SentAtUtc { get; set; }
        public bool IsMine { get; set; }
    }
}
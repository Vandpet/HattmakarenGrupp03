namespace HattmakarenWebbAppGrupp03.ViewModels.Messages
{
    public class ConversationThreadViewModel
    {
        public int ConversationId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public List<string> Participants { get; set; } = new();
        public List<MessageItemViewModel> Messages { get; set; } = new();

        public string NewMessageContent { get; set; } = string.Empty;
    }
}
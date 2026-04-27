using System.ComponentModel.DataAnnotations;

namespace HattmakarenWebbAppGrupp03.ViewModels.Messages
{
    public class CreateConversationViewModel
    {
        public List<int> SelectedEmployeeIds { get; set; } = new();

        [StringLength(200)]
        public string? Title { get; set; }

        [Required]
        [StringLength(5000)]
        [Display(Name = "Message")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Send to everyone")]
        public bool SendToAll { get; set; }

        public List<EmployeeSelectionViewModel> AvailableEmployees { get; set; } = new();
    }
}
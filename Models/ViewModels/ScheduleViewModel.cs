namespace HattmakarenWebbAppGrupp03.Models.ViewModels
{
    public class ScheduleViewModel
    {
        public string ViewType { get; set; } = "week"; // day, week, month
        public DateTime SelectedDate { get; set; } = DateTime.Today;
        public bool IsPersonal { get; set; }

        public List<ScheduleItemViewModel> Items { get; set; } = new();
    }

    public class ScheduleItemViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string Type { get; set; } = string.Empty;
        public string ColorClass { get; set; } = string.Empty;

        public int? OrderId { get; set; }
        public int? EmployeeId { get; set; }

        public string EmployeeName { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsDelayed { get; set; }
    }
}
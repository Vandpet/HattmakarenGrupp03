namespace HattmakarenWebbAppGrupp03.Models.ViewModels
{
    public class SchedulePageViewModel
    {
        public bool IsPersonal { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int SelectedDay { get; set; }

        public List<CalendarDayViewModel> Days { get; set; } = new();
        public List<OrderGroupViewModel> Orders { get; set; } = new();
    }

    public class CalendarDayViewModel
    {
        public DateTime Date { get; set; }
        public bool IsCurrentMonth { get; set; }
        public bool IsSelected { get; set; }
        public bool HasItems { get; set; }
        public string ColorClass { get; set; } = "";
    }

    public class OrderGroupViewModel
    {
        public int OrderId { get; set; }
        public string Title { get; set; } = "";
        public string CustomerName { get; set; } = "";
        public string Status { get; set; } = "";
        public string ColorClass { get; set; } = "";
        public List<TaskItemViewModel> Tasks { get; set; } = new();
    }

    public class TaskItemViewModel
    {
        public string Title { get; set; } = "";
        public string AssignedTo { get; set; } = "";
        public string Status { get; set; } = "";
        public bool IsDone { get; set; }
    }
}
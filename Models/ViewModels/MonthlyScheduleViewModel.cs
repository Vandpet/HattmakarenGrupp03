namespace HattmakarenWebbAppGrupp03.Models.ViewModels
{
    public class MonthScheduleViewModel
    {
        public bool IsPersonal { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }

        public List<WeekRowViewModel> Weeks { get; set; } = new();
        public List<UnscheduledTaskViewModel> UnscheduledTasks { get; set; } = new();
        public List<EmployeeViewModel> Employees { get; set; } = new();
    }

    public class WeekRowViewModel
    {
        public List<CalendarCellViewModel> Days { get; set; } = new();
    }

    public class CalendarCellViewModel
    {
        public DateTime Date { get; set; }
        public bool IsCurrentMonth { get; set; }
        public bool IsToday { get; set; }

        public List<CalendarEventViewModel> Events { get; set; } = new();
    }

    public class CalendarEventViewModel
    {
        public int OrderId { get; set; }
        public int HatId { get; set; }

        public string Title { get; set; } = "";
        public string HatName { get; set; } = "";

        public string Status { get; set; } = "";
        public string ColorClass { get; set; } = "";
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = "";
    }

    public class UnscheduledTaskViewModel
    {
        public int OrderId { get; set; }
        public int HatId { get; set; }
        public string Title { get; set; } = "";
        public string HatName { get; set; } = "";
        public string Status { get; set; } = "";
        public string ColorClass { get; set; } = "";
    }

    public class EmployeeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }

    public class OrderScheduleViewModel
    {
        public int OrderId { get; set; }
        public string OrderTitle { get; set; } = "";

        public List<HatScheduleItemViewModel> Hats { get; set; } = new();
    }

    public class HatScheduleItemViewModel
    {
        public int HatOrderId { get; set; }
        public int HatId { get; set; }
        public string HatName { get; set; } = "";
        public int Amount { get; set; }

        public string Status { get; set; } = "";
        public string ColorClass { get; set; } = "";
    }
}
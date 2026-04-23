namespace HattmakarenWebbAppGrupp03.Models.ViewModels
{
    public class MonthScheduleViewModel
    {
        public bool IsPersonal { get; set; }
        public bool IsAdmin { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int WeekNumber { get; set; }
		public DateTime SelectedDate { get; set; }
		public string ViewMode { get; set; } = "month";


		public List<WeekRowViewModel> Weeks { get; set; } = new();
        public List<UnscheduledTaskViewModel> UnscheduledTasks { get; set; } = new();
        public List<EmployeeViewModel> Employees { get; set; } = new();
    }

    public class WeekRowViewModel
    {
        public int WeekNumber { get; set; }
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
        public int Amount { get; set; }
        public DateTime? PrelDeliveryDate { get; set; }

        public string ViewMode { get; set; } = "month";

    }

    public class UnscheduledTaskViewModel
    {
        public int OrderId { get; set; }
        public int HatId { get; set; }
        public string Title { get; set; } = "";
        public string HatName { get; set; } = "";
        public string Status { get; set; } = "";
        public string ColorClass { get; set; } = "";
        public int Amount { get; set; }

        public DateTime? PrelDeliveryDate { get; set; }
        public string CustomerName { get; set; } = "";
        public string Description { get; set; } = "";
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
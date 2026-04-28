    namespace HattmakarenWebbAppGrupp03.Models.ViewModels
    {
        public class DashViewModel
        {
            public decimal TodaySales { get; set; }
            public decimal WeekSales { get; set; } 
            public decimal QuarterSales { get; set; }

            public List<TodayScheduleItemViewModel> TodayActivities { get; set; } = new();
            public List<TodayScheduleItemViewModel> TodayOrderTasks { get; set; } = new();
        }

        public class TodayScheduleItemViewModel
        {
            public string Title { get; set; } = "";
            public string Type { get; set; } = "";
            public string Status { get; set; } = "";
            public string EmployeeName { get; set; } = "";
            public int Amount { get; set; }
            public TimeSpan? Time { get; set; }
    }
    }
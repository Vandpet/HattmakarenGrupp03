namespace HattmakarenWebbAppGrupp03.Models.ViewModels
{
    public class StatisticsViewModel
    {
        public int TotalSoldHats { get; set; } = 0;
        public int TotalRevenue { get; set; } = 0;
        public IEnumerable<Hat> AllHats { get; set; } = new List<Hat>();
        
        public IEnumerable<HatOrder> AllHatOrders { get; set; } = new List<HatOrder>();

    }
}



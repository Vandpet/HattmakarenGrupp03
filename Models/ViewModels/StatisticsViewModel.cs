namespace HattmakarenWebbAppGrupp03.Models.ViewModels
{
    public class StatisticsViewModel
    {
        public int TotalSoldHats { get; set; } = 0;
        public int TotalRevenue { get; set; } = 0;
        public IEnumerable<HatStatisticsRow> hatStats { get; set; } = new List<HatStatisticsRow>();
        public IEnumerable<Customer> Customers { get; set; } = new List<Customer>();

    }

    public class HatStatisticsRow
    {
        public int HatId { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Sales { get; set; }
    }
}



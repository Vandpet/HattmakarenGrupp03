namespace HattmakarenWebbAppGrupp03.Models
{
    public class CustomerManager
    {
        public int EId { get; set; }
        public Employee Employee { get; set; }

        public int CId { get; set; }
        public Customer Customer { get; set; }
        public string Description { get; set; }
    }

}

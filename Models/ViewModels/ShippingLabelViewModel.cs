namespace HattmakarenWebbAppGrupp03.Models.ViewModels
{
    public class ShippingLabelViewModel
    {
        public int OId { get; set; }

        // Hattmakaren-info (X = fylls i manuellt)
        public string CompanyAddress { get; set; }
        public string CompanyPostalCode { get; set; }
        public string CompanyCity { get; set; }
        public string CompanyCountry { get; set; }

        // Hämtas automatiskt från order/kund
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerCity { get; set; }
        public string CustomerCountry { get; set; }
        public DateTime DateCreated { get; set; }

        // X-fält
        public string Contents { get; set; }       // Innehåll
        public double Weight { get; set; }         // Vikt
        public double DeliveryPrice { get; set; }  // Fraktpris
        public bool ShipOutsideCountry { get; set; } // Avgör moms-avdrag

        public IEnumerable<HatOrder> HatOrders { get; set; }
    }
}

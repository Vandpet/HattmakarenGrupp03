namespace HattmakarenWebbAppGrupp03.Models.ViewModels
{
    public class HatCreateViewModel
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Size { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? Status { get; set; }
        public bool StandardHat { get; set; }

        public string Description { get; set; }

        // användaren fyller i dessa
        public List<MaterialInputViewModel> Materials { get; set; } = new();
    }


    public class MaterialInputViewModel
    {
        public string Name { get; set; }
        public double Amount { get; set; }
        public string MeasuringUnits { get; set; }
        public decimal Price { get; set; }
    }
}
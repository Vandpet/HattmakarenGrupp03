namespace HattmakarenWebbAppGrupp03.Models.ViewModels
{
    public class HatCreateViewModel
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Size { get; set; }
        public string? PicturePath { get; set; }
        public string? Status { get; set; }
        public bool StandardHat { get; set; }

        // Listan med alla material som visas (checkboxar)
        public List<MaterialSelectionViewModel> AvailableMaterials { get; set; } = new();

        // Listan med ID:n för de material användaren har kryssat i
        public List<int> SelectedMaterialIds { get; set; } = new();
    }

    public class MaterialSelectionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
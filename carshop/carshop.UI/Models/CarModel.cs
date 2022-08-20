namespace carshop.carshop.UI.Models
{
    public class CarModel
    {
        public int Id { get; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string Gearbox { get; set; }
        public decimal Price { get; set; }
        public decimal Insurance { get; set; }
    }
}

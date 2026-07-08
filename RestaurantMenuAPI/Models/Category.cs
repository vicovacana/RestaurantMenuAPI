namespace RestaurantMenuAPI.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    }
}

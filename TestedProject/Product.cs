namespace TestedProject
{
    public class Product
    {
        public Product( int id, string description, double price )
        {
            Id = id;
            Description = description;
            Price = price;
        }


        public int Id { get; }
        public string Description { get; set; }
        public double Price { get; set; }
    }
}
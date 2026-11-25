namespace Backend.Api.Models;

public class Product
{
    public int ProductID { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int UnitsInStock { get; set; }
    public bool Discontinued { get; set; } = false;
}

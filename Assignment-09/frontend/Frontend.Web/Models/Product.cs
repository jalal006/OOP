using System.ComponentModel.DataAnnotations;

namespace Frontend.Web.Models;

public class Product
{
    public int ProductID { get; set; }

    [Display(Name = "Product name")]
    [Required(ErrorMessage = "{0} Must Be Filled In.")]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Category { get; set; } = string.Empty;

    [Range(0.0, double.MaxValue, ErrorMessage = "Unit price cannot be negative.")]
    [Display(Name = "Unit price")]
    public decimal UnitPrice { get; set; }

    [Range(0, int.MaxValue)]
    [Display(Name = "Units in stock")]
    public int UnitsInStock { get; set; }

    public bool Discontinued { get; set; }
}

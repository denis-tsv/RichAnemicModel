namespace Entities;

public class ShoppingCart
{
    public Product Product { get; set; } = null!;

    public void RenameProduct(Product product)
    {
        product.Name = "NewName"; // ok
    }
}
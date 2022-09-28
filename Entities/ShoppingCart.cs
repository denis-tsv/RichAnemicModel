namespace Entities;

public class ShoppingCart
{
    public void RenameProduct(Product product)
    {
        product.Name = "NewName"; // ok
    }
}
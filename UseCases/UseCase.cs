using Entities;

namespace UseCases;

public class UseCase
{
    public void AssignEntityProperty()
    {
        var product = new Product
        {
            Name = "Name" // ok
        };

        product.Name = "NewName"; // fail

        product.Rename("NewName"); // ok
    }
}
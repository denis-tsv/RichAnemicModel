namespace Entities;

public class Product
{
    public string Name { get; set; } = null!;

    public void Rename(string name)
    {
        Name = name;
    }
}
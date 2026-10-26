using InterfacesDemo.Interfaces;

namespace InterfacesDemo.Classes;

public class Cat : IAnimal
{
    public void Speak()
    {
        Console.WriteLine("Meow!");
    }
}

using InterfacesDemo.Interfaces;

namespace InterfacesDemo.Classes;

public class Dog : IAnimal
{
    public void Speak()
    {
        Console.WriteLine("Woof!");
    }
}

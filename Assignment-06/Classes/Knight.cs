using InterfacesDemo.Interfaces;

namespace InterfacesDemo.Classes;

public class Knight : IAttack, IDefend
{
    void IAttack.Attack()
    {
        Console.WriteLine("Knight attacks with a sword.");
    }

    void IDefend.Attack()
    {
        Console.WriteLine("Knight raises a shield to defend.");
    }
}

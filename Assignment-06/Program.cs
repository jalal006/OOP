using InterfacesDemo.Classes;
using InterfacesDemo.Interfaces;

IAnimal[] animals =
{
    new Dog(),
    new Cat()
};

foreach (IAnimal animal in animals)
{
    animal.Speak();
}

Console.WriteLine();

ICombatant[] fighters =
{
    new Warrior { Name = "Aragorn", Health = 100, AttackPower = 25 },
    new Mage { Name = "Merlin", Health = 80, AttackPower = 40 }
};

foreach (ICombatant fighter in fighters)
{
    fighter.Attack();
    fighter.Defend();
    fighter.TakeDamage(10);
}

Console.WriteLine();

ICharacter character = fighters[0];
character.TakeDamage(5);

Mage mage = fighters[1] as Mage;
mage.Heal(30);

Console.WriteLine();
Console.WriteLine("Choose your fighter:");
Console.WriteLine("1. Warrior");
Console.WriteLine("2. Mage");
Console.Write("Choice: ");
string choice = Console.ReadLine();

ICombatant player;

if (choice == "2")
{
    player = new Mage { Name = "Player Mage", Health = 80, AttackPower = 40 };
}
else
{
    player = new Warrior { Name = "Player Warrior", Health = 100, AttackPower = 25 };
}

player.Attack();
player.Defend();

Console.WriteLine();

Knight knight = new Knight();

IAttack attack = knight;
attack.Attack();

IDefend defend = knight;
defend.Attack();

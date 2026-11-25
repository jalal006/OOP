using System;

namespace InheritanceMinigame.Classes
{
    public class Goblin : Character
    {

    public Goblin(string name) : base(name, 60, 10)
        {
            BattleCry = "Get them!";
        }

    public override void PerformAttack(Character target)
        {
            Console.WriteLine($"{Name} shouts '{BattleCry}' and slashes with a rusty dagger!");
            target.TakeDamage(AttackPower);
            
            if (Random.Shared.Next(0, 100) < 30)
            {
                Console.WriteLine($"{Name} strikes a second time out of nowhere!");
                target.TakeDamage(AttackPower / 2);
            }
        }
    }
}

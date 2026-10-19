using System;

namespace InheritanceMinigame.Classes
{
    public sealed class Mage : Character
    {
        public int Mana { get; private set; } = 50;

        public Mage(string name) : base(name, 80, 25)
        {
            BattleCry = "Magic will prevail!";
        }

        public override void PerformAttack(Character target)
        {
            if (Mana >= 20)
            {
                Mana -= 20;
                Console.WriteLine($"{Name} shouts '{BattleCry}' and casts a massive Fireball at {target.Name}! (Mana left: {Mana})");
                target.TakeDamage(AttackPower);
            }
            else
            {
                Console.WriteLine($"{Name} shouts '{BattleCry}' but is out of mana! They whack {target.Name} weakly with a staff.");
                target.TakeDamage(5);
            }
        }
    }
}

namespace InheritanceMinigame.Classes
{
    public class EliteWarrior : Warrior
    {
        public EliteWarrior(string name) : base($"Elite Member {name}")
        {
            AttackPower *= 2;
            ShieldBlock *= 2;
        }
    }
}

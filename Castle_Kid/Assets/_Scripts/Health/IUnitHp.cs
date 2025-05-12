namespace _Scripts.Health
{
    public interface IUnitHp // bassicly only utility is to be able to getComponent the item that can take damage
    {
        public bool CanDie { get; set; }
        
        public int CurrentHp { get; set; }
        public int MaxHp { get; set; }

        public void TakeDamage(int damage) {}
        public virtual void Die() {}
        public void GainHealth(int healthGained) {}
        public virtual void GainFullLife() {}
    }
}

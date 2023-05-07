namespace Content.Scripts.Weapons.Energy
{
    public interface IDamagable
    {
        public float Armor { get; }
        public float Health { get; }

        public void TakeDamage(float damage);
    }
}
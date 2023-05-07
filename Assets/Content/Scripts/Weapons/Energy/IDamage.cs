namespace Content.Scripts.Weapons.Energy
{
    public interface IDamage
    {
        public float Damage { get; }

        public void AddDamage(IDamagable damagable);
    }
}
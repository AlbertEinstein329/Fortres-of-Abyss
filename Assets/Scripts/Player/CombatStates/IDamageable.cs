namespace AbyssFirstPersonController
{
    public interface IDamageable
    {
        float CurrentHealth { get; }
        void TakeDamage(float damage);
    }
}
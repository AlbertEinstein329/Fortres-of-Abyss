namespace AbyssFirstPersonController
{
    public interface IDamageable
    {
        DamageResult TakeDamage(DamageInfo info);
    }
}
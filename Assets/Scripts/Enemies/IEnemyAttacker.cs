namespace AbyssFirstPersonController
{
    using System;

    public interface IEnemyAttacker
    {
        event Action KilledTarget;
        void ScaleDamage(float multiplier);
    }
}
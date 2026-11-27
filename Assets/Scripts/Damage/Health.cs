using UnityEngine;
using System;

namespace AbyssFirstPersonController
{
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField, Min(1f)] private float maxHealth = 100f;

        public float MaxHealth => maxHealth;
        public float CurrentHealth { get; private set; }
        public event Action Changed;
        public float Ratio => CurrentHealth / maxHealth;
        public bool IsDead => CurrentHealth <= 0f;
        public event Action<DamageInfo> Damaged;
        public Func<DamageInfo, float> DamageModifier { get; set; }
        public event Action Died;

        private CombatLevel combatLevel;
        private int TargetLevel => combatLevel != null ? combatLevel.Level : 1;

        private void Awake()
        {
            CurrentHealth = maxHealth;
            TryGetComponent(out combatLevel);
        }

        public void ScaleMaxHealth(float multiplier)
        {
            maxHealth *= multiplier;
            Restore();
        }

        public DamageResult TakeDamage(DamageInfo info)
        {
            if (IsDead) return DamageResult.None(TargetLevel);

            float finalDamage = DamageModifier != null ? DamageModifier(info) : info.amount;

            if (finalDamage <= 0f) return DamageResult.None(TargetLevel);

            float previousHealth = CurrentHealth;
            float dealt = Mathf.Min(finalDamage, previousHealth);

            CurrentHealth = Mathf.Max(CurrentHealth - finalDamage, 0f);
            bool killed = IsDead;

            Damaged?.Invoke(info);
            Changed?.Invoke();

            if (killed) Died?.Invoke();
            return new DamageResult(dealt, dealt / maxHealth, TargetLevel, killed);
        }

        public void Restore()
        {
            CurrentHealth = maxHealth;
            Changed?.Invoke();
        }
    }
}
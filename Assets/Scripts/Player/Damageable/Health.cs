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

        private void Awake()
        {
            CurrentHealth = maxHealth;
        }

        public void TakeDamage(DamageInfo info)
        {
            if (IsDead) return;

            float finalDamage = DamageModifier != null ? DamageModifier(info) : info.amount;

            if (finalDamage <= 0f) return;

            CurrentHealth = Mathf.Max(CurrentHealth - finalDamage, 0f);
            Damaged?.Invoke(info);
            Changed?.Invoke();

            if (IsDead) Died?.Invoke();
        }

        public void Restore()
        {
            CurrentHealth = maxHealth;
            Changed?.Invoke();
        }
    }
}
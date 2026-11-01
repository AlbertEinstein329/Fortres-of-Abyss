namespace AbyssFirstPersonController
{
    using System;
    using UnityEngine;

    public class PlayerBlock : MonoBehaviour
    {
        [SerializeField, Range(0f, 180f)] private float blockHalfAngle = 80f;
        [SerializeField, Range(0f, 1f)] private float blockDamageMultiplier = 0f;

        private Health health;
        private PlayerCombatController combat;

        public event Action<DamageInfo> Blocked;

        private void Awake()
        {
            health = GetComponent<Health>();
            combat = GetComponent<PlayerCombatController>();

            if (health == null) { Fail("The player needs a Health component."); return; }
            if (combat == null) { Fail("The player needs PlayerCombatController component."); return; }
        }

        private void OnEnable()
        {
            if (health != null) health.DamageModifier = ModifyDamage;
        }

        private void OnDisable()
        {
            if (health != null) health.DamageModifier = null;
        }

        private float ModifyDamage(DamageInfo info)
        {
            if (!combat.IsBlocking || !IsFromFront(info.sourcePosition))
            {
                return info.amount;
            }

            Blocked?.Invoke(info);
            return info.amount * blockDamageMultiplier;
        }

        private bool IsFromFront(Vector3 sourcePosition)
        {
            Vector3 direction = sourcePosition - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.01f) return true;
            return Vector3.Angle(transform.forward, direction) <= blockHalfAngle;
        }

        private void Fail(string message)
        {
            Debug.LogError(message, this);
            enabled = false;
        }

    }
}
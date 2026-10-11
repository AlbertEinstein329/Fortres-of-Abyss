namespace AbyssFirstPersonController
{
    using System.Collections.Generic;
    using UnityEngine;

    public class SwordWeapon : WeaponBase
    {
        [SerializeField] private CombatAction primaryAction;
        [SerializeField] private CombatAction secondaryAction;
        [SerializeField] private CombatAction skill1Action;

        public override CombatAction GetAction(CombatSlot slot)
        {
            CombatAction action;
            switch (slot)
            {
                case CombatSlot.Primary:
                    action = primaryAction;                  
                    break;
                case CombatSlot.Secondary:
                    action = secondaryAction;
                    break;
                case CombatSlot.Skill1:
                    action = skill1Action;
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(slot), slot, null);
            }
            return string.IsNullOrEmpty(action.animatorTrigger) ? null : action;
        }

        public override void AnimEvent_ActionHit()
        {
            if (currentAction == null) return;

            Vector3 center = hitOrigin.position + hitOrigin.forward * (currentAction.attackRange - currentAction.attackRadius);
            Collider[] hitColliders = Physics.OverlapSphere(center, currentAction.attackRadius, enemyMask);

            HashSet<IDamageable> damageablesHit = new HashSet<IDamageable>();

            foreach (Collider hitCollider in hitColliders)
            {
                IDamageable damageable = hitCollider.GetComponentInParent<IDamageable>();
                if (damageable != null && damageablesHit.Add(damageable))
                {
                    damageable.TakeDamage(currentAction.attackDamage);
                
                }
            }
            if (damageablesHit.Count > 0)
            {           
                //Debug.Log($"Action '{currentAction.name}' hit {damageablesHit.Count} target(s) at context {startContext}.");
                combat.ReportHit(new HitReport(currentAction, startContext, damageablesHit.Count));
            }
            else
            {
                //Debug.Log($"Action '{currentAction.name}' missed. Context {startContext} discarded.");
            }
        }

        private void OnDrawGizmosSelected()
        {   
            if (hitOrigin == null || primaryAction == null) return;

            float radius = currentAction != null ? currentAction.attackRadius : primaryAction.attackRadius;
            float range = currentAction != null ? currentAction.attackRange : primaryAction.attackRange;
            
            Vector3 gizmoCenter = hitOrigin.position +hitOrigin.forward * (range - radius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(gizmoCenter, radius);
        }
    }
}

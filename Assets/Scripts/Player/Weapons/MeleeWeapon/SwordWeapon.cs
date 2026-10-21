namespace AbyssFirstPersonController
{
    using System.Collections.Generic;
    using UnityEngine;

    public class SwordWeapon : WeaponBase
    {
        [SerializeField] private CombatAction primaryAction;
        [SerializeField] private CombatAction secondaryAction;
        [SerializeField] private CombatAction skill1Action;
        [SerializeField] private CombatAction blockAction;

        private enum ChargePhase { None, Charging, Swinging }

        private ChargePhase chargePhase;
        private float chargeStartTime;
        private float chargeRatio;
        private bool swingFinished;
        private float swingStartTime;

        public override WeaponType Type => WeaponType.Sword;
        private float CurrentChargeRatio => chargePhase == ChargePhase.Swinging ? chargeRatio : 1f;

        public override CombatAction GetAction(CombatSlot slot)
        {
            CombatAction action;
            switch (slot)
            {
                case CombatSlot.Block:
                    return blockAction;
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

        public override void BeginAction(CombatAction action, LocomotionContext context)
        {
            if (action.kind == ActionKind.Instant)
            {
                base.BeginAction(action, context);
                return;
            }

            currentAction = action;
            startContext = context;

            if (action == blockAction) return;

            chargePhase = ChargePhase.Charging;
            chargeStartTime = Time.time;
        }

        public override ActionStatus Tick(ICombatInputManager input)
        {
            if (currentAction == null) return ActionStatus.Finished;

            return currentAction == blockAction ? TickBlock(input) : TickHeavy(input);
        }
        
        private ActionStatus TickBlock(ICombatInputManager input)
        {
            if (input.Block) return ActionStatus.Running;

            currentAction = null;
            return ActionStatus.Finished;
        }

        private ActionStatus TickHeavy(ICombatInputManager input)
        {
            if (chargePhase == ChargePhase.Swinging)
            {
                if (swingFinished) return EndHeavy(ActionStatus.Finished);

                if (SwingTimeOut())
                {
                    Debug.LogWarning($"Heavy attack '{currentAction.name}' timed out: the end Animation Event never arrived.");
                    return EndHeavy(ActionStatus.Finished);
                }

                return ActionStatus.Running;
            }

            if (input.Block) return EndHeavy(ActionStatus.Cancelled);
            if (!input.SecondaryHeld) ReleaseHeavy();

            return ActionStatus.Running;
        }

        private void ReleaseHeavy()
        {
            swingStartTime = Time.time;
            chargeRatio = Mathf.Clamp01((Time.time - chargeStartTime) / currentAction.fullChargeTime);
            chargePhase = ChargePhase.Swinging;
            swingFinished = false;
            base.BeginAction(currentAction, combat.fpscontroller.currentLocomotionContext);
        }

        private ActionStatus EndHeavy(ActionStatus actionStatus)
        {
            chargePhase = ChargePhase.None;
            currentAction = null;

            return actionStatus;
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
                    damageable.TakeDamage(new DamageInfo(CalculateDamage(), hitOrigin.position));
                }

            }
            if (damageablesHit.Count > 0)
            {
                //Debug.Log($"Action '{currentAction.name}' hit {damageablesHit.Count} target(s) at context {startContext}.");
                combat.ReportHit(new HitReport(currentAction, startContext, damageablesHit.Count, Type, CurrentChargeRatio));
            }
            else
            {
                //Debug.Log($"Action '{currentAction.name}' missed. Context {startContext} discarded.");
            }
        }

        public override void AnimEvent_ActionEnd()
        {
            if (chargePhase == ChargePhase.Swinging)
            {
                swingFinished = true;
                return;
            }
            base.AnimEvent_ActionEnd();
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

        private float CalculateDamage()
        {

            float multiplier = Mathf.Lerp(currentAction.minChargeDamageMultiplier, 1f, CurrentChargeRatio);
            return currentAction.attackDamage * multiplier;
        }

        private bool SwingTimeOut()
        {
            float limit = currentAction.maxDuration / Mathf.Max(currentAction.speed, 0.01f) + 0.5f;
            return Time.time - swingStartTime >= limit;
        }

        public override bool TryInterrupt(InterruptReason reason)
        {
            if (currentAction == null || chargePhase == ChargePhase.Swinging)
                return false;

            if (reason == InterruptReason.Dash && !currentAction.interruptibleByDash)
            {
                return false;
            }

            chargePhase = ChargePhase.None;
            return base.TryInterrupt(reason);
        }
    }
}

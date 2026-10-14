namespace AbyssFirstPersonController
{
    using UnityEngine;

    public class CombatChargingState : CombatBaseState
    {
        private readonly CombatAction action;
        private ActionStatus status;

        public CombatChargingState(PlayerCombatController currentContext, CombatStateFactory combatStateFactory, CombatAction action)
            : base(currentContext, combatStateFactory)
        {
            this.action = action;
        }

        public override void EnterState()
        {
            status = ActionStatus.Running;
            ctx.EquippedWeapon.BeginAction(action, ctx.fpscontroller.currentLocomotionContext);
            
        }

        public override void UpdateState()
        {
            status = ctx.EquippedWeapon.Tick(ctx.combatInput);
            CheckSwitchStates();
        }

        public override void ExitState() { }

        public override void CheckSwitchStates()
        {
            if (status != ActionStatus.Running)
            {
                SwitchState(combatStateFactory.Idle());
            }
        }

        public override void OnInterruptRequested(InterruptReason reason)
        {
            if (ctx.EquippedWeapon.TryInterrupt(reason))
            {
                SwitchState(combatStateFactory.Idle());
            }
        }
    }
}
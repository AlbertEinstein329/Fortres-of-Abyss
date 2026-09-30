namespace AbyssFirstPersonController
{
    using UnityEngine;

    public class CombatIdleState : CombatBaseState
    {
        public CombatIdleState(PlayerCombatController currentContext, CombatStateFactory combatStateFactory)
            : base(currentContext, combatStateFactory) { }
    

        public override void EnterState()
        { }

        public override void UpdateState()
        {
            CheckSwitchStates();
        }

        public override void ExitState() { }

        public override void CheckSwitchStates()
        {
            if (ctx.fpscontroller.allowsCombatActions && ctx.CombatInput.PrimaryAttack)
            {
                SwitchState(combatStateFactory.Attack());
            }
        }






    }
}

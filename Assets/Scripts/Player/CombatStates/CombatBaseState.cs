namespace AbyssFirstPersonController
{
    public abstract class CombatBaseState
    {
        protected PlayerCombatController ctx;
        protected CombatStateFactory combatStateFactory;

        protected CombatBaseState(PlayerCombatController currentContext, CombatStateFactory combatStateFactory)
        {
            this.ctx = currentContext;
            this.combatStateFactory = combatStateFactory;
        }


        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void ExitState();
        public abstract void CheckSwitchStates();

        protected void SwitchState(CombatBaseState newState)
        {
            ExitState();
            newState.EnterState();
            ctx.CurrentState = newState;
        }
    }
}
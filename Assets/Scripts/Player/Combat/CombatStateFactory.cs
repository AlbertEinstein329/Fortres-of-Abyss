namespace AbyssFirstPersonController
{
    public class CombatStateFactory
    {
        PlayerCombatController context;

        public CombatStateFactory(PlayerCombatController currentContext)
        {
            context = currentContext;
        }
        public CombatBaseState Idle() => new CombatIdleState(context, this);
        public CombatBaseState Attack(CombatAction action) => new CombatAttackingState(context, this, action);
        public CombatBaseState Charge(CombatAction action) => new CombatChargingState(context, this, action);
    }

}


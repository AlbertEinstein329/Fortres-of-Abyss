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
        public CombatBaseState Attack() => new CombatAttackingState(context, this);
    }

}


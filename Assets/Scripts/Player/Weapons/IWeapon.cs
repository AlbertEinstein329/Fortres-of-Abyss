namespace AbyssFirstPersonController
{
    public interface IWeapon
    {
        void Initialize(PlayerCombatController combat);
        CombatAction GetAction(CombatSlot slot);
        void BeginAction(CombatAction action, LocomotionContext context);
        ActionStatus Tick(ICombatInputManager input);
        void Interrupt();
    }
}

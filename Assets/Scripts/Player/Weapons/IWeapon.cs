namespace AbyssFirstPersonController
{
    public interface IWeapon
    {
        WeaponType Type { get; }
        void Initialize(PlayerCombatController combat);
        CombatAction GetAction(CombatSlot slot);
        void BeginAction(CombatAction action, LocomotionContext context);
        ActionStatus Tick(ICombatInputManager input);
        bool TryInterrupt(InterruptReason reason);
    }
}

namespace AbyssFirstPersonController
{
    public readonly struct HitReport
    {
        public readonly CombatAction action;
        public readonly LocomotionContext startContext;
        public readonly int targetsHit;
        public readonly float chargeRatio;
        public readonly WeaponType weaponType;

        public HitReport(CombatAction action, LocomotionContext startContext, int targetsHit, WeaponType weaponType, float chargeRatio = 1f)
        {
            this.action = action;
            this.startContext = startContext;
            this.targetsHit = targetsHit;
            this.weaponType = weaponType;
            this.chargeRatio = chargeRatio;
        }
    }
}
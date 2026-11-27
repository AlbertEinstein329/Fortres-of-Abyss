namespace AbyssFirstPersonController
{
    public readonly struct HitReport
    {
        public readonly CombatAction action;
        public readonly LocomotionContext startContext;
        public readonly DamageResult[] targets;
        public readonly float chargeRatio;
        public readonly WeaponType weaponType;

        public int TargetsHit => targets != null ? targets.Length : 0;

        public HitReport(CombatAction action, LocomotionContext startContext, DamageResult[] targets, WeaponType weaponType, float chargeRatio = 1f)
        {
            this.action = action;
            this.startContext = startContext;
            this.targets = targets;
            this.weaponType = weaponType;
            this.chargeRatio = chargeRatio;
        }
    }
}
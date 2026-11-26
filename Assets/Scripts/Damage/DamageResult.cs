namespace AbyssFirstPersonController
{
    public readonly struct DamageResult
    {
        public readonly float dealt;
        public readonly float fraction;
        public readonly int targetLevel;
        public readonly bool killed;

        public DamageResult(float dealt, float fraction, int targetLevel, bool killed)
        {
            this.dealt = dealt;
            this.fraction = fraction;
            this.targetLevel = targetLevel;
            this.killed = killed;
        }

        public static DamageResult None(int targetLevel) => new DamageResult(0f, 0f, targetLevel, false);
    }
}
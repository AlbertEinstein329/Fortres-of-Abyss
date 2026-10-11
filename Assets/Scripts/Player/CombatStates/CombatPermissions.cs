namespace AbyssFirstPersonController
{
    [System.Flags]
    public enum CombatPermissions
    {
        None = 0,
        Primary = 1 << 0,
        Secondary = 1 << 1,
        Skill = 1 << 2,
        All = Primary | Secondary | Skill
    }

}

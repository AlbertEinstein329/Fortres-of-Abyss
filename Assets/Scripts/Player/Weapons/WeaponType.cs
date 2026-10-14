namespace AbyssFirstPersonController
{
    [System.Flags]
    public enum WeaponType
    {
        None = 0,   // in a skill requirement: any weapon
        Sword = 1 << 0,
        Axe = 1 << 1,
        Bow = 1 << 2,
        Dagger = 1 << 3,
        Staff = 1 << 4,
        Wand = 1 << 5,
    }
}
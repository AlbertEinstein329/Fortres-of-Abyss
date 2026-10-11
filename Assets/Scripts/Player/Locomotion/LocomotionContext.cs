namespace AbyssFirstPersonController
{
    [System.Flags]
    public enum LocomotionContext
    {
        None = 0,
        Grounded = 1 << 0,
        Falling = 1 << 1,
        Jumping = 1 << 2, 
        Sliding = 1 << 3,
        Swimming = 1 << 4,
        Dashing = 1 << 5,
        Crouching = 1 << 6,
        LedgeGrabbing = 1 << 7,
        All = Grounded | Falling | Jumping | Sliding | Swimming | Dashing | Crouching | LedgeGrabbing
    }
}

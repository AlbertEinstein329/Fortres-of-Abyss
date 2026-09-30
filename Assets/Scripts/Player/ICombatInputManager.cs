using UnityEngine;

namespace AbyssFirstPersonController
{
    public interface ICombatInputManager
    {
        bool PrimaryAttack { get; }
        bool SecondaryAttack { get; }
        bool SecondaryHeld { get; }
        bool SecondaryReleased { get; }
    }
}

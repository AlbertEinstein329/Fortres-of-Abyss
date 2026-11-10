using UnityEngine;

namespace AbyssFirstPersonController
{
    public interface ICombatInputManager
    {
        bool PrimaryAttack { get; }
        bool SecondaryAttack { get; }
        bool SecondaryHeld { get; }
        bool SecondaryReleased { get; }
        bool Block { get; }
        bool Skill1 { get; }
    }
}

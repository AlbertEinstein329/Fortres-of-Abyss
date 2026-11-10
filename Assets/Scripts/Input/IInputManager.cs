using UnityEngine;

namespace AbyssFirstPersonController
{
    public interface IInputManager
    {
        Vector2 MoveInput { get; }
        Vector2 LookInput { get; }
        bool Jump { get; }
        bool Sprint { get; }
        bool Crouch { get; }
        bool Slide { get; }
        bool Dash { get; }

    }
}
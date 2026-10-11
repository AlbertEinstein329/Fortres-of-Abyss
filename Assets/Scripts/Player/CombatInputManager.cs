namespace AbyssFirstPersonController
{
    using UnityEngine;
#if ENABLE_INPUT_SYSTEM
    using UnityEngine.InputSystem;
#endif

    public class CombatInputManager : MonoBehaviour, ICombatInputManager
    {
        public bool PrimaryAttack
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
#else
                return Input.GetMouseButtonDown(0);
#endif
            }
        }

        public bool SecondaryAttack
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame;
#else
                return Input.GetMouseButtonDown(1);
#endif
            }
        }

        public bool SecondaryHeld
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return Mouse.current != null && Mouse.current.rightButton.isPressed;
#else
                return Input.GetMouseButton(1);
#endif
            }
        }

        public bool SecondaryReleased
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return Mouse.current != null && Mouse.current.rightButton.wasReleasedThisFrame;
#else
                return Input.GetMouseButtonUp(1);
#endif
            }
        }

        public bool Skill1
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return Keyboard.current != null && Keyboard.current.qKey.wasPressedThisFrame;
#else
                return Input.GetKeyDown(KeyCode.Q);
#endif
            }
        }

    }
}

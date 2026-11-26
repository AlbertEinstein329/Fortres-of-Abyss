namespace AbyssFirstPersonController
{
    using System;
    using UnityEngine;

    public class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] private float range = 2.5f;
        [SerializeField] private LayerMask interactMask;
        private IInputManager input;
        private Transform cameraTransform;
        private IInteractable current;

        public event Action<IInteractable> FocusChanged;

        private void Awake()
        {
            input = GetComponent<IInputManager>();
            FirstPersonController fps = GetComponent<FirstPersonController>();
            cameraTransform = fps != null ? fps.playerCamera : null;

            if (input == null) { Fail("PlayerInteractor needs an IInputManager component."); return; }
            if (cameraTransform == null) { Fail("PlayerInteractor needs a FirstPersonController component with a playerCamera assigned."); return; }
        }

        private void Update()
        {
            IInteractable target = FindInteractable();
            SetFocus(target);
            if (current != null && input.Interact)
            {
                current.Interact(this);
                SetFocus(null);
            }
        }

        private IInteractable FindInteractable()
        {
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            if (!Physics.Raycast(ray, out RaycastHit hit, range, interactMask))
            {
                return null;
            }

            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
            if (interactable == null || !interactable.CanInteract)
            {
                return null;
            }

            return interactable;
        }

        private void SetFocus(IInteractable target)
        {
            if (target == current) { return; }

            current = target;
            FocusChanged?.Invoke(current);
        }

        private void Fail(string message)
        {
            Debug.LogError(message, this);
            enabled = false;
        }
    }
}
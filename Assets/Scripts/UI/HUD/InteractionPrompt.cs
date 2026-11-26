namespace AbyssFirstPersonController
{
    using TMPro;
    using UnityEngine;

    public class InteractionPrompt : MonoBehaviour
    {
        [SerializeField] private PlayerInteractor interactor;
        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text label;

        private void Awake()
        {
            if (interactor == null) Fail("InteractionPrompt requires a PlayerInteractor reference.");
            if (panel == null) Fail("InteractionPrompt requires a panel GameObject.");
            if (label == null) Fail("InteractionPrompt requires a TMP_Text label.");
            if (!enabled) return;

            panel.SetActive(false);
        }

        private void OnEnable()
        {
            if (interactor != null)
            {
                interactor.FocusChanged += OnFocusChanged;
            }
        }

        private void OnDisable()
        {
            if (interactor != null)
            {
                interactor.FocusChanged -= OnFocusChanged;
            }
        }

        private void OnFocusChanged(IInteractable target)
        {
            if (target == null)
            {
                panel.SetActive(false);
                return;
            }

            label.text = $"[F] {target.Prompt}";
            panel.SetActive(true);
        }

        private void Fail(string message)
        {
            Debug.LogError(message, this);
            enabled = false;
        }
    }
}
namespace AbyssFirstPersonController
{
    using System;
    using UnityEngine;

    public class Bonfire : MonoBehaviour, IInteractable
    {
        [SerializeField] private Transform respawnPoint;
        [SerializeField] private GameObject litVisual;
        [SerializeField] private bool restoresHealth = true;

        public static event Action<Bonfire> Rested;
        private bool isLit;

        public string Prompt => isLit ? "Rest" : "Light Bonfire";
        public bool CanInteract => enabled;

        private void Awake()
        {
            if (respawnPoint == null) Fail("Bonfire requires a respawnPoint Transform.");
            if (litVisual == null) Fail("Bonfire requires a litVisual GameObject.");
            if (!enabled) return;

            litVisual.SetActive(false);
        }

        public void Interact(PlayerInteractor interactor)
        {
            if (interactor.TryGetComponent(out PlayerRespawn respawn))
            {
                respawn.SetCheckpoint(respawnPoint);
            }

            if (restoresHealth)
            {
                if (interactor.TryGetComponent(out Health health)) health.Restore();
                if (interactor.TryGetComponent(out Stamina stamina)) stamina.Restore();
            }

            if (!isLit)
            {
                litVisual.SetActive(true);
                isLit = true;
            }

            Rested?.Invoke(this);
        }

        private void Fail(string message)
        {
            Debug.LogError(message, this);
            enabled = false;
        }
    }

}
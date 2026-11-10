namespace AbyssFirstPersonController
{
    using System.Collections;
    using UnityEngine;

    public class PlayerRespawn : MonoBehaviour
    {
        [SerializeField] private Transform defaultCheckpoint;
        [SerializeField, Min(0f)] private float respawnDelay = 3f;

        private Health health;
        private FirstPersonController movement;
        private PlayerCombatController combat;
        private CharacterController characterController;
        private Transform currentCheckpoint;

        private void Awake()
        {
            health = GetComponent<Health>();
            movement = GetComponent<FirstPersonController>();
            combat = GetComponent<PlayerCombatController>();
            characterController = GetComponent<CharacterController>();

            if (defaultCheckpoint == null) { Fail("PlayerRespawn needs a default checkpoint."); return; }
            if (health == null) { Fail("The player needs a Health component."); return; }
            if (movement == null) { Fail("The player needs FirstPersonController component."); return; }
            if (combat == null) { Fail("The player needs PlayerCombatController component."); return; }
            if (characterController == null) { Fail("The player needs CharacterController component."); return; }

            currentCheckpoint = defaultCheckpoint;
        }

        public void SetCheckpoint(Transform checkpoint) => currentCheckpoint = checkpoint;

        private void OnEnable() { if (health != null) health.Died += OnDied; }
        private void OnDisable() { if (health != null) health.Died -= OnDied; }

        private void OnDied() => StartCoroutine(RespawnRoutine());

        private IEnumerator RespawnRoutine()
        {
            movement.enabled = false;
            combat.enabled = false;

            yield return new WaitForSeconds(respawnDelay);

            characterController.enabled = false;
            transform.SetPositionAndRotation(currentCheckpoint.position, currentCheckpoint.rotation);
            characterController.enabled = true;

            health.Restore();
            movement.ResetForRespawn();
            combat.ResetForRespawn();

            movement.enabled = true;
            combat.enabled = true;
        }

        private void Fail(string message)
        {
            Debug.LogError(message, this);
            enabled = false;
        }
    }
}

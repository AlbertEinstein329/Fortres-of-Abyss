using UnityEngine;
namespace AbyssFirstPersonController
{
    public class TargetDummy : MonoBehaviour
    {
        private Health health;
        public float CurrentHealth => health != null ? health.CurrentHealth : 0f;

        private void Awake()
        {
            health = GetComponent<Health>();

            if (health == null)
            {
                Debug.LogError("TargetDummy needs a Health component.", this);
            }
        }

        private void OnEnable()
        {
            if (health != null) health.Died += OnDied;
        }

        private void OnDisable()
        {
            if (health != null) health.Died -= OnDied;
        }

        private void OnDied()
        {
            gameObject.SetActive(false);
        }
    }
}


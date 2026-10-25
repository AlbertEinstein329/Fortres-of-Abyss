namespace AbyssFirstPersonController
{
    using UnityEngine;
    using UnityEngine.UI;

    public class PlayerHealthBar : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private Image fillImage;

        private void Awake()
        {
            if (health == null)
            {
                Debug.LogError("PlayerHealthBar no tiene asignado el componente Health.", this);
                enabled = false;
                return;
            }

            if (fillImage == null)
            {
                Debug.LogError("PlayerHealthBar no tiene asignada la Image de relleno.", this);
                enabled = false;
                return;
            }
        }

        private void Start()
        {
            Refresh();
        }

        private void OnEnable()
        {
            if (health != null) health.Changed += Refresh;
        }

        private void OnDisable()
        {
            if (health != null) health.Changed -= Refresh;
        }

        private void Refresh()
        {
            if (fillImage != null && health != null)
            {
                fillImage.fillAmount = health.Ratio;
            }
        }
    }
}
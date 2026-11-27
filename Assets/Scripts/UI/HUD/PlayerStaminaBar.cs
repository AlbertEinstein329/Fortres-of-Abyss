namespace AbyssFirstPersonController
{
    using UnityEngine;
    using UnityEngine.UI;
    public class PlayerStaminaBar : MonoBehaviour
    {
        [SerializeField] private Stamina stamina;
        [SerializeField] private Image fillImage;
        private void Awake()
        {
            if (stamina == null)
            {
                Debug.LogError("PlayerStaminaBar no tiene asignado el componente Stamina.", this);
                enabled = false;
                return;
            }
            if (fillImage == null)
            {
                Debug.LogError("PlayerStaminaBar no tiene asignada la Image de relleno.", this);
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
            if (stamina != null) stamina.Changed += Refresh;
        }
        private void OnDisable()
        {
            if (stamina != null) stamina.Changed -= Refresh;
        }
        private void Refresh()
        {
                fillImage.fillAmount = stamina.Ratio;
        }
    }
}

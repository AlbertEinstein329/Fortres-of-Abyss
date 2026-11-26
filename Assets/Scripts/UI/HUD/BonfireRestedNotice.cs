namespace AbyssFirstPersonController
{
    using TMPro;
    using UnityEngine;

    public class BonfireRestedNotice : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private float displayTime = 2f;

        private float hideTime;

        private void Awake()
        {
            if (label == null)
            {
                Debug.LogError("BonfireRestedNotice requires a TMP_Text label.", this);
                enabled = false;
                return;
            }

            label.gameObject.SetActive(false);
        }

        private void OnEnable() => Bonfire.Rested += OnBonfireRested;
        private void OnDisable() => Bonfire.Rested -= OnBonfireRested;

        private void Update()
        {
            if (label.gameObject.activeSelf && Time.time >= hideTime)
            {
                label.gameObject.SetActive(false);
            }
        }

        private void OnBonfireRested(Bonfire bonfire)
        {
            label.text = "Checkpoint saved";
            label.gameObject.SetActive(true);
            hideTime = Time.time + displayTime;
        }
    }
}

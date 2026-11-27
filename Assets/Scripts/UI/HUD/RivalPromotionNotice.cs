using UnityEngine;
using TMPro;

namespace AbyssFirstPersonController
{
    public class RivalPromotionNotice : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text label;
        [SerializeField] private GameObject panel;

        [Header("Settings")]
        [SerializeField, Min(0.1f)] private float displayTime = 4f;

        private float hideTime;

        private void Awake()
        {
            if (label == null) { Fail("RivalPromotionNotice needs a TMP_Text reference."); return; }
            if (panel == null) { Fail("RivalPromotionNotice needs a panel reference."); return; }
            panel.SetActive(false);
        }

        private void OnEnable() => RivalRank.Promoted += OnPromoted;
        private void OnDisable() => RivalRank.Promoted -= OnPromoted;

        private void Update()
        {
            if (panel.activeSelf && Time.time >= hideTime)
            {
                panel.SetActive(false);
            }
        }

        private void OnPromoted(RivalRank rank)
        {
            label.text = $"The goblin who killed you was promoted.\n{rank.DisplayName} · Level {rank.Level}";
            panel.SetActive(true);
            hideTime = Time.time + displayTime;
        }

        private void Fail(string message)
        {
            Debug.LogError(message, this);
            enabled = false;
        }
    }
}
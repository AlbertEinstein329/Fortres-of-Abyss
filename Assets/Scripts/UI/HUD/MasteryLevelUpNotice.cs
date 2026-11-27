using UnityEngine;
using TMPro;

namespace AbyssFirstPersonController
{
    public class MasteryLevelUpNotice : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerMastery mastery;
        [SerializeField] private TMP_Text label;
        [SerializeField] private GameObject panel;

        [Header("Settings")]
        [SerializeField, Min(0.1f)] private float displayTime = 2.5f;

        private float hideTime;

        private void Awake()
        {
            if (label == null) { Fail("MasteryLevelUpNotice needs a TMP_Text reference."); return; }
            if (mastery == null) { Fail("MasteryLevelUpNotice needs a PlayerMastery reference."); return; }
            if (panel == null) { Fail("MasteryLevelUpNotice needs a panel reference."); return; }
            panel.SetActive(false);
        }

        private void OnEnable()
        {
            if (mastery != null)
            {
                mastery.LevelChanged += OnLevelChanged;
            }
        }

        private void OnDisable()
        {
            if (mastery != null)
            {
                mastery.LevelChanged -= OnLevelChanged;
            }
        }

        private void Update()
        {
            if (panel.activeSelf && Time.time >= hideTime)
            {
                panel.SetActive(false);
            }
        }

        private void OnLevelChanged(MasteryType type, int newLevel)
        {
            label.text = $"{type} mastery {newLevel}";
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
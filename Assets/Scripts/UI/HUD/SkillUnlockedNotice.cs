using UnityEngine;
using TMPro;

namespace AbyssFirstPersonController
{

    public class SkillUnlockedNotice : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SkillProgress skillProgress;
        [SerializeField] private TMP_Text label;
        [SerializeField] private GameObject panel;

        [Header("Settings")]
        [SerializeField, Min(0.1f)] private float displayTime = 3.5f;

        private float hideTime;

        private void Awake()
        {
            // Fail-fast: validate all required Inspector references before hiding the panel.
            if (skillProgress == null) { Fail("SkillUnlockedNotice needs a SkillProgress reference."); return; }
            if (label == null) { Fail("SkillUnlockedNotice needs a TMP_Text reference."); return; }
            if (panel == null) { Fail("SkillUnlockedNotice needs a panel reference."); return; }
            panel.SetActive(false);
        }

        private void OnEnable()
        {
            // Guard against null in case Awake disabled this component due to missing references.
            if (skillProgress != null)
            {
                skillProgress.SkillUnlocked += OnUnlocked;
            }
        }

        private void OnDisable()
        {
            if (skillProgress != null)
            {
                skillProgress.SkillUnlocked -= OnUnlocked;
            }
        }

        private void Update()
        {
            if (panel.activeSelf && Time.time >= hideTime)
            {
                panel.SetActive(false);
            }
        }

        private void OnUnlocked(SkillDefinition skill)
        {
            label.text = $"New skill learned: {skill.skillName}\nPress Q to use it";
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
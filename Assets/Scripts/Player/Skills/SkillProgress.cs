namespace AbyssFirstPersonController
{
    using System.Collections.Generic;
    using UnityEngine;
    public class SkillProgress : MonoBehaviour
    {
        [SerializeField] private SkillsData skillsData;
        private PlayerCombatController combat;
        private readonly Dictionary<int, int> progress = new Dictionary<int, int>();
        private readonly HashSet<int> unlocked = new HashSet<int>();
        public bool IsUnlocked(int skillId) => unlocked.Contains(skillId);

        private void Awake()
        {
            combat = GetComponent<PlayerCombatController>();
            
        }
        private void OnEnable()
        {
            combat.HitConfirmed += OnHitConfirmed;
        }
        private void OnDisable()
        {
            combat.HitConfirmed -= OnHitConfirmed;
        }

        private void OnHitConfirmed(HitReport report)
        {
            foreach (SkillDefinition skill in skillsData.skills)
            {
                if (unlocked.Contains(skill.skillId)) continue;

                if(!skill.IsSatisfiedBy(report)) continue;

                progress.TryGetValue(skill.skillId, out int count);
                count++;
                progress[skill.skillId] = count;
                Debug.Log($"{skill.skillName}: {count}/{skill.requirementCount}");

                if (count >= skill.requirementCount)
                {
                    unlocked.Add(skill.skillId);
                    Debug.Log($"Skill Unlocked: {skill.skillName}");
                }

            }
        }
    }
}

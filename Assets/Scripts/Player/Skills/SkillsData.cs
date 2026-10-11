namespace AbyssFirstPersonController
{
    using System;
    using UnityEngine;

    [Serializable]
    public class SkillDefinition
    {
        public int skillId;
        public string skillName;
        public LocomotionContext requiredContexts = LocomotionContext.All;
        public Sprite skillIcon;
        public int requirementCount = 1;
        public int minTargetsPerHit = 1;
        public int requiredActionSkillId = 0;
        
        [Range(0f, 1f)] public float minChargeRatio = 0f;
        [Range(0f, 1f)] public float maxChargeRatio = 1f;

        public bool IsSatisfiedBy(HitReport report)
        {
            return ContextMatches(report) && ChargeMatches(report) && MinTargetsMatches(report) && ActionMatches(report);
        }

        private bool ContextMatches(HitReport report)
        {
            return (requiredContexts & report.startContext) != 0;
        }

        private bool ChargeMatches(HitReport report)
        {
            return report.chargeRatio >= minChargeRatio && report.chargeRatio <= maxChargeRatio;
        }

        private bool MinTargetsMatches(HitReport report)
        {
            return report.targetsHit >= minTargetsPerHit;
        }

        private bool ActionMatches(HitReport report)
        {
            return requiredActionSkillId == 0 || report.action.skillId == requiredActionSkillId;
        }
    }


    [CreateAssetMenu(fileName = "SkillsData", menuName = "Scriptable Objects/SkillsData")]
    public class SkillsData : ScriptableObject
    {
        public SkillDefinition[] skills;
    }
}

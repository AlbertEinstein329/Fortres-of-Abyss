using UnityEngine;

namespace AbyssFirstPersonController
{
    public class MasteryXpGranter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerCombatController combat;
        [SerializeField] private PlayerMastery mastery;

        [Header("Settings")]
        [SerializeField, Min(0.01f)] private float minXpRatio = 0.1f;
        [SerializeField, Min(0.01f)] private float maxXpRatio = 2f;
        [SerializeField, Min(0f)] private float multiTargetBonus = 0.1f;
        [SerializeField, Min(0f)] private float xpScale = 5f;

        private void Awake()
        {
            if (combat == null) { Fail("MasteryXpGranter needs a PlayerCombatController reference."); return; }
            if (mastery == null) { Fail("MasteryXpGranter needs a PlayerMastery reference."); return; }
        }

        private void OnEnable()
        {
            if (combat != null)
            {
                combat.HitConfirmed += OnHitConfirmed;
            }
        }

        private void OnDisable()
        {
            if (combat != null)
            {
                combat.HitConfirmed -= OnHitConfirmed;
            }
        }

        private void OnHitConfirmed(HitReport report)
        {
            MasteryType type = ToMasteryType(report.weaponType);
            if (type == MasteryType.None) return;

            int currentLevel = mastery.GetLevel(type);
            float xpGained = HitXp(report, currentLevel);

            mastery.AddXp(type, xpGained);

            //Debug.Log($"[Mastery] {type} +{xpGained:F2} XP | Nivel actual: {mastery.GetLevel(type)}");
        }

        private float HitXp(HitReport report, int masteryLevel)
        {
            if(report.targets == null || report.TargetsHit <= 0) return 0f;
            
            float totalTargetXp = 0f;
            foreach (DamageResult target in report.targets)
            {
                totalTargetXp += TargetXp(target, masteryLevel);
            }

            float averageXp = totalTargetXp / report.TargetsHit;
            float multiBonus = 1f + (report.TargetsHit - 1) * multiTargetBonus;

            return averageXp * multiBonus * xpScale;
        }

        private float TargetXp(DamageResult target, int masteryLevel)
        {
            float baseXp = mastery.Curve.XpBase(target.targetLevel);
            float ratio = XpRatio(target.targetLevel, masteryLevel);

            return baseXp * target.fraction * ratio;
        }

        private float XpRatio(int targetLevel, int masteryLevel)
        {
            float rawRatio = (float)targetLevel / Mathf.Max(1, masteryLevel);
            return Mathf.Clamp(rawRatio, minXpRatio, maxXpRatio);
        }

        private static MasteryType ToMasteryType(WeaponType weapon)
        { 
            switch (weapon)
            {
                case WeaponType.Sword:
                    return MasteryType.Sword;
                case WeaponType.Bow:
                    return MasteryType.Bow;
                default:
                    return MasteryType.None;
            }
        }

        private void Fail(string message)
        {
            Debug.LogError(message, this);
            enabled = false;
        }
    }
}

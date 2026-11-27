namespace AbyssFirstPersonController
{
    using System;
    using UnityEngine;

    [RequireComponent(typeof(CombatLevel), typeof(Health))]
    public class RivalRank : MonoBehaviour
    {
        public static event Action<RivalRank> Promoted;

        [SerializeField] private string[] possibleNames = { "Grak", "Snikt", "Molg", "Vrask", "Zubb", "Rizzik" };
        [SerializeField] private string[] rankTitles = { "Debt Collector", "Bailiff", "Loan Shark" };
        [SerializeField, Min(1)] private int levelsPerPromotion = 2;
        [SerializeField, Min(1f)] private float healthMultiplier = 1.25f;
        [SerializeField, Min(1f)] private float damageMultiplier = 1.2f;
        [SerializeField, Min(1f)] private float sizeMultiplier = 1.08f;

        private CombatLevel combatLevel;
        private Health health;
        private IEnemyAttacker attacker;

        public int Rank { get; private set; }
        public string RivalName { get; private set; }
        public int Level => combatLevel.Level;
        public bool IsMaxRank => Rank >= rankTitles.Length;

        public string DisplayName
        {
            get
            {
                if (Rank == 0) return gameObject.name;

                string title = rankTitles[Rank - 1];
                return $"{RivalName}, {title}";
            }
        }

        private void Awake()
        {
            attacker = GetComponent<IEnemyAttacker>();
            combatLevel = GetComponent<CombatLevel>();
            health = GetComponent<Health>();

            if (attacker == null) { Fail("Enemy GameObject needs an IEnemyAttacker."); return; }
            if (combatLevel == null) { Fail("Enemy GameObject needs CombatLevel component."); return; }
            if (health == null) { Fail("Enemy GameObject needs Health component."); return; }

        }

        private void OnEnable()
        {
            if (attacker != null)
            {
                attacker.KilledTarget += Promote;
            }
        }

        private void OnDisable()
        {
            if (attacker != null)
            {
                attacker.KilledTarget -= Promote;
            }
        }

        private void Promote()
        {
            if (IsMaxRank) return;

            Rank++;

            if (string.IsNullOrEmpty(RivalName) && possibleNames.Length > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, possibleNames.Length);
                RivalName = possibleNames[randomIndex];
            }

            combatLevel.AddLevels(levelsPerPromotion);
            health.ScaleMaxHealth(healthMultiplier);
            attacker.ScaleDamage(damageMultiplier);
            transform.localScale *= sizeMultiplier;

            Promoted?.Invoke(this);
        }

        private void Fail(string message)
        {
            Debug.LogError(message, this);
            enabled = false;
        }
    }
}
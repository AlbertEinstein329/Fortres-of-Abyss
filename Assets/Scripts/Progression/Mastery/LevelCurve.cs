namespace AbyssFirstPersonController
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "LevelCurve", menuName = "Abyss/Level Curve")]
    public class LevelCurve : ScriptableObject
    {
        [SerializeField, Min(2)] private int maxLevel = 100;
        [SerializeField, Min(0.01f)] private float baseXp = 5f;
        [SerializeField, Min(1f)] private float growth = 1.0723f;
        [SerializeField, Min(1f)] private float levelsCostMultiplier = 13f;

        public int MaxLevel => maxLevel;

        public float XpBase(int level)
        {
            level = Mathf.Max(1, level);
            return baseXp * Mathf.Pow(growth, level - 1);
        }

        public float XpToNextLevel(int level)
        {
            level = Mathf.Max(1, level);

            if (level >= maxLevel)
            {
                return float.PositiveInfinity;
            }

            return levelsCostMultiplier * XpBase(level);
        }
    }
}
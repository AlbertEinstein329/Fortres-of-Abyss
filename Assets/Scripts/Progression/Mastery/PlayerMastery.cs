namespace AbyssFirstPersonController
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class PlayerMastery : MonoBehaviour
    {
        [Serializable]
        private class MasteryEntry
        {
            public MasteryType type;
            [Min(1)] public int level = 1;
            [Min(0f)] public float xp;
        }

        [SerializeField] private LevelCurve levelCurve;
        [SerializeField] private List<MasteryEntry> masteries = new List<MasteryEntry>();

        public event Action<MasteryType, int> LevelChanged;
        public LevelCurve Curve => levelCurve;

        private void Awake()
        {
            if (levelCurve == null)
            {
                Debug.LogError("PlayerMastery requiere una referencia a LevelCurve.", this);
                enabled = false;
            }
        }

        public int GetLevel(MasteryType type)
        {
            return masteries.Find(m => m.type == type)?.level ?? 1;
        }

        public void AddXp(MasteryType type, float amount)
        {
            if (type == MasteryType.None || amount <= 0f) return;

            MasteryEntry entry = GetOrCreateEntry(type);
            if (entry.level >= levelCurve.MaxLevel) return;

            entry.xp += amount;

            while (entry.level < levelCurve.MaxLevel && entry.xp >= levelCurve.XpToNextLevel(entry.level))
            {
                entry.xp -= levelCurve.XpToNextLevel(entry.level);
                entry.level++;
                LevelChanged?.Invoke(type, entry.level);
            }

            if (entry.level >= levelCurve.MaxLevel) entry.xp = 0f;
        }

        private MasteryEntry GetOrCreateEntry(MasteryType type)
        {
            MasteryEntry entry = masteries.Find(m => m.type == type);
            if (entry == null)
            {
                entry = new MasteryEntry { type = type, level = 1, xp = 0f };
                masteries.Add(entry);
            }
            return entry;
        }
    }
}

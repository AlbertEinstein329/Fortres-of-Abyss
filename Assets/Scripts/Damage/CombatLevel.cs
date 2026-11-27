using UnityEngine;

namespace AbyssFirstPersonController
{
    public class CombatLevel : MonoBehaviour
    {
        [SerializeField, Min(1)] private int level = 1;
        public int Level => level;

        public void AddLevels(int amount) => level = Mathf.Min(level + Mathf.Max(0, amount), 100);
    }
}

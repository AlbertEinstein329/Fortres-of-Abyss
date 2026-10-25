namespace AbyssFirstPersonController
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
    public class EnemyData : ScriptableObject
    {
        [Header("Movement")]
        [Min(0.1f)] public float moveSpeed = 3.5f;
        [Min(1f)] public float detectionRange = 12f;
        [Min(1f)] public float loseTargetRange = 18f;
        [Min(0.5f)] public float stoppingDistance = 1.8f;
        [Min(0.05f)] public float repathInterval = 0.2f;

        [Header("Attack")]
        [Min(0.5f)] public float attackRange = 2.2f;
        [Min(0f)] public float attackDamage = 10f;
        [Min(0.05f)] public float windupTime = 0.6f;
        [Min(0.05f)] public float recoveryTime = 1.0f;
        [Range(0f, 180f)] public float attackHalfAngle = 60f;

        private void OnValidate()
        {
            attackRange = Mathf.Max(attackRange, stoppingDistance + 0.2f);
            loseTargetRange = Mathf.Max(loseTargetRange, detectionRange + 1f);
        }
    }
}
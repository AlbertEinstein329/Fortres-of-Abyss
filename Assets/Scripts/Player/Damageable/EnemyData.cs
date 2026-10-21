namespace AbyssFirstPersonController
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
    public class EnemyData : ScriptableObject
    {
        [Min(0.1f)] public float moveSpeed = 3.5f;
        [Min(1f)] public float detectionRange = 12f;
        [Min(1f)] public float loseTargetRange = 18f;
        [Min(0.5f)] public float stoppingDistance = 1.8f;
        [Min(0.05f)] public float repathInterval = 0.2f;
    }
}
namespace AbyssFirstPersonController
{
    using UnityEngine;
    using UnityEngine.AI;

    public class EnemyAnimator : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private float speedDampTime = 0.1f;

        private NavMeshAgent agent;
        private static readonly int SpeedHash = Animator.StringToHash("Speed");

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();

            if (agent == null) { Fail("EnemyAnimator needs a NavMeshAgent component."); return; }
            if (animator == null) { Fail("EnemyAnimator needs an Animator reference."); return; }
        }

        private void Update()
        {
            animator.SetFloat(SpeedHash, agent.velocity.magnitude, speedDampTime, Time.deltaTime);
        }

        private void Fail(string message)
        {
            Debug.LogError(message, this);
            enabled = false;
        }
    }
}
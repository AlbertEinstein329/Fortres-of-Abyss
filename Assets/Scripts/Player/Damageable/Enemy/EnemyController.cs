namespace AbyssFirstPersonController
{
    using UnityEngine;
    using UnityEngine.AI;

    public class EnemyController : MonoBehaviour
    {
        private enum State { Idle, Chase, Dead }

        [SerializeField] private EnemyData data;

        private NavMeshAgent agent;
        private Health health;
        private Transform target;
        private State state;
        private float nextRepathTime;
        private float distance;

        private void Awake()
        {
            health = GetComponent<Health>();
            agent = GetComponent<NavMeshAgent>();

            FirstPersonController player = FindAnyObjectByType<FirstPersonController>();
            target = player != null ? player.transform : null;

            if (data == null) { Fail("EnemyController needs an EnemyData asset."); return; }
            if (agent == null) { Fail("EnemyController needs a NavMeshAgent."); return; }
            if (health == null) { Fail("EnemyController needs a Health component."); return; }
            if (target == null) { Fail("EnemyController could not find the player."); return; }

            agent.speed = data.moveSpeed;
            agent.stoppingDistance = data.stoppingDistance;
        }
        private void Fail(string message)
        {
            Debug.LogError(message, this);
            enabled = false;
        }

        private void Update()
        {
            distance = Vector3.Distance(transform.position, target.position);
            if (!agent.isOnNavMesh) return;

            switch (state)
            {
                case State.Idle:
                    UpdateIdle();
                    break;
                case State.Chase:
                    UpdateChase();
                    break;
                default:
                    break;
            }
        }

        private void OnEnable()
        {
            if (health != null) health.Died += OnDied;
        }

        private void OnDisable()
        {
            if (health != null) health.Died -= OnDied;
        }

        private void OnDied()
        {
            state = State.Dead;
            gameObject.SetActive(false);
        }

        private void UpdateIdle()
        {
            if (distance <= data.detectionRange)
            {
                state = State.Chase;
            }
        }
        
        private void UpdateChase()
        {
            if (distance >= data.loseTargetRange)
            {
                state = State.Idle;
                agent.ResetPath();
                return;
            }

            RepathIfNeeded();

            if (distance <= data.stoppingDistance) FaceTarget();
        }
        private void FaceTarget()
        {
            if (distance <= data.stoppingDistance)
            {
                Vector3 direction = (target.position - transform.position);
                direction.y = 0f;

                if (direction.sqrMagnitude < 0.01f) return;

                Quaternion look = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, look, 10f * Time.deltaTime);
            }
        }

        private void RepathIfNeeded()
        {
            if (Time.time < nextRepathTime) return;
            agent.SetDestination(target.position);
            nextRepathTime = Time.time + data.repathInterval;
        }

        private void OnDrawGizmosSelected()
        {
            if (data == null) return;

            Gizmos.color = Color.orange;
            Gizmos.DrawWireSphere(transform.position, data.detectionRange);

            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(transform.position, data.loseTargetRange);
        }
    }
}

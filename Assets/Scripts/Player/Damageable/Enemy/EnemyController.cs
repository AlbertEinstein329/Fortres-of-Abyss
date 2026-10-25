namespace AbyssFirstPersonController
{
    using UnityEngine;
    using UnityEngine.AI;

    public class EnemyController : MonoBehaviour
    {
        private enum State { Idle, Chase, Attack, Dead }
        private enum AttackPhase { Windup, Recovery }

        [SerializeField] private EnemyData data;

        private NavMeshAgent agent;
        private Health health;
        private Health targetHealth;
        private Transform target;
        private State state;
        private AttackPhase attackPhase;
        private float nextRepathTime;
        private float distance;
        private float phaseEndTime;
        private Renderer bodyRenderer;
        private Color baseColor;

        private float WindupProgress => 1f - (phaseEndTime - Time.time) / data.windupTime;

        private void Awake()
        {
            health = GetComponent<Health>();
            agent = GetComponent<NavMeshAgent>();
            bodyRenderer = GetComponentInChildren<Renderer>();

            FirstPersonController player = FindAnyObjectByType<FirstPersonController>();
            target = player != null ? player.transform : null;
            targetHealth = target != null ? target.GetComponent<Health>() : null;

            if (data == null) { Fail("EnemyController needs an EnemyData asset."); return; }
            if (agent == null) { Fail("EnemyController needs a NavMeshAgent."); return; }
            if (health == null) { Fail("EnemyController needs a Health component."); return; }
            if (target == null) { Fail("EnemyController could not find the player."); return; }
            if (bodyRenderer == null) { Fail("EnemyController needs a Renderer component."); return; }
            if (targetHealth == null) { Fail("Player needs a Health component."); return; }

            agent.speed = data.moveSpeed;
            agent.stoppingDistance = data.stoppingDistance;
            baseColor = bodyRenderer.material.color;
        }

        private void Fail(string message)
        {
            Debug.LogError(message, this);
            enabled = false;
        }

        private void Update()
        {
            if (targetHealth.IsDead)
            {
                if (state != State.Idle && state != State.Dead) GoIdle();
                return;
            }

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
                case State.Attack:
                    UpdateAttack();
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
            ResetTelegraph();
            gameObject.SetActive(false);
        }

        private void GoIdle()
        {
            state = State.Idle;
            agent.ResetPath();
            ResetTelegraph();
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
                GoIdle();
                return;
            }

            if (distance <= data.attackRange)
            {
                StartAttack();
                return;
            }

            RepathIfNeeded();
        }

        private void StartAttack()
        {
            state = State.Attack;
            attackPhase = AttackPhase.Windup;
            phaseEndTime = Time.time + data.windupTime;
            agent.ResetPath();
        }

        private void UpdateAttack()
        {
            FaceTarget();

            if (Time.time < phaseEndTime)
            {
                if (attackPhase == AttackPhase.Windup)
                {
                    ShowTelegraph(WindupProgress);
                }
                return;
            }

            if (attackPhase == AttackPhase.Windup) BeginRecovery();
            else EndAttack();
        }

        private void BeginRecovery()
        {
            Strike();
            ResetTelegraph();
            attackPhase = AttackPhase.Recovery;
            phaseEndTime = Time.time + data.recoveryTime;
        }

        private bool IsTargetInStrikeZone()
        {
            if (distance > data.attackRange) return false;

            Vector3 direction = target.position - transform.position;
            direction.y = 0f;

            return Vector3.Angle(transform.forward, direction) <= data.attackHalfAngle;
        }

        private void Strike()
        {
            if (!IsTargetInStrikeZone()) return;

            targetHealth.TakeDamage(new DamageInfo(data.attackDamage, transform.position));
        }

        private void EndAttack()
        {
            state = State.Chase;
        }

        private void FaceTarget()
        {
            Vector3 direction = target.position - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.01f) return;

            Quaternion look = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, 10f * Time.deltaTime);
        }

        private void RepathIfNeeded()
        {
            if (Time.time < nextRepathTime) return;
            agent.SetDestination(target.position);
            nextRepathTime = Time.time + data.repathInterval;
        }

        private void ShowTelegraph(float progress)
        {
            bodyRenderer.material.color = Color.Lerp(baseColor, Color.red, Mathf.Clamp01(progress));
        }

        private void ResetTelegraph()
        {
            bodyRenderer.material.color = baseColor;
        }

        private void OnDrawGizmosSelected()
        {
            if (data == null) return;

            Gizmos.color = Color.orange;
            Gizmos.DrawWireSphere(transform.position, data.detectionRange);

            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(transform.position, data.loseTargetRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, data.attackRange);
        }
    }
}

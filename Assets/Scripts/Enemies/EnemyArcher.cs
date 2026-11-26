namespace AbyssFirstPersonController
{
    using System;
    using UnityEngine;

    public class EnemyArcher : MonoBehaviour, IEnemyAttacker
    {
        private enum State { Idle, Aiming, Recovery, Dead }

        [SerializeField] private EnemyData data;
        [SerializeField] private Transform shootPoint;
        [SerializeField] private LineRenderer aimLine;
        [SerializeField] private LayerMask obstacleMask;
        [SerializeField, Min(0.05f)] private float aimLockTime = 0.35f;
        [SerializeField, Min(0.1f)] private float hitRadius = 0.6f;
        [SerializeField] private Vector3 targetOffset = new Vector3(0f, 1.2f, 0f);

        public event Action KilledTarget;

        private Health health;
        private Health targetHealth;
        private Transform target;
        private State state;
        private float phaseEndTime;
        private Vector3 lockedDirection;
        private float damageMultiplier = 1f;

        private Vector3 TargetPoint => target.position + targetOffset;

        public void ScaleDamage(float multiplier) => damageMultiplier *= multiplier;

        private void Awake()
        {
            health = GetComponent<Health>();

            FirstPersonController player = FindAnyObjectByType<FirstPersonController>();
            target = player != null ? player.transform : null;
            targetHealth = target != null ? target.GetComponent<Health>() : null;

            if (data == null) { Fail("EnemyArcher needs an EnemyData asset."); return; }
            if (shootPoint == null) { Fail("EnemyArcher needs a shootPoint."); return; }
            if (aimLine == null) { Fail("EnemyArcher needs an aimLine."); return; }
            if (health == null) { Fail("EnemyArcher needs a Health component."); return; }
            if (target == null) { Fail("EnemyArcher could not find the player."); return; }
            if (targetHealth == null) { Fail("Player needs a Health component."); return; }

            aimLine.enabled = false;
        }

        private void Fail(string message)
        {
            Debug.LogError(message, this);
            enabled = false;
        }

        private void Update()
        {
            if (state == State.Dead) return;

            if (targetHealth.IsDead)
            {
                if (state != State.Idle && state != State.Dead) GoIdle();
                return;
            }

            switch (state)
            {
                case State.Idle:
                    UpdateIdle();
                    break;
                case State.Aiming:
                    UpdateAiming();
                    break;
                case State.Recovery:
                    UpdateRecovery();
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
            aimLine.enabled = false;
            gameObject.SetActive(false);
        }

        private void GoIdle()
        {
            state = State.Idle;
            aimLine.enabled = false;
        }

        private void UpdateIdle()
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance <= data.detectionRange && HasLineOfSight())
            {
                StartAiming();
            }
        }

        private void StartAiming()
        {
            state = State.Aiming;
            phaseEndTime = Time.time + data.windupTime;
            lockedDirection = (TargetPoint - shootPoint.position).normalized;
            aimLine.enabled = true;
            ShowAimLine(Color.white);
        }

        private void UpdateAiming()
        {
            FaceTarget();

            float distance = Vector3.Distance(transform.position, target.position);
            if (distance > data.loseTargetRange)
            {
                GoIdle();
                return;
            }

            // Track the player until the final aimLockTime window, where the shot direction locks in.
            bool isTracking = (phaseEndTime - Time.time) > aimLockTime;
            if (isTracking)
            {
                lockedDirection = (TargetPoint - shootPoint.position).normalized;
                ShowAimLine(Color.white);
            }
            else
            {
                ShowAimLine(Color.red);
            }

            if (Time.time >= phaseEndTime)
            {
                Shoot();
                BeginRecovery();
            }
        }

        private void Shoot()
        {
            if (IsTargetOnShotLine())
            {
                DamageResult result = targetHealth.TakeDamage(new DamageInfo(data.attackDamage * damageMultiplier, transform.position));
                if (result.killed) KilledTarget?.Invoke();
            }

            Debug.DrawRay(shootPoint.position, lockedDirection * data.loseTargetRange, Color.yellow, 1f);
        }

        private void BeginRecovery()
        {
            state = State.Recovery;
            aimLine.enabled = false;
            phaseEndTime = Time.time + data.recoveryTime;
        }

        private void UpdateRecovery()
        {
            if (Time.time >= phaseEndTime)
            {
                GoIdle();
            }
        }

        private void FaceTarget()
        {
            Vector3 direction = target.position - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.01f) return;

            Quaternion look = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, 10f * Time.deltaTime);
        }

        private void ShowAimLine(Color color)
        {
            aimLine.startColor = color;
            aimLine.endColor = color;
            aimLine.SetPosition(0, shootPoint.position);
            aimLine.SetPosition(1, shootPoint.position + lockedDirection * data.loseTargetRange);
        }

        private bool HasLineOfSight()
        {
            return !Physics.Linecast(shootPoint.position, TargetPoint, obstacleMask);
        }

        private bool IsTargetOnShotLine()
        {
            Vector3 toTarget = TargetPoint - shootPoint.position;
            float along = Vector3.Dot(toTarget, lockedDirection);
            if (along < 0f || along > data.loseTargetRange) return false;

            Vector3 closestPoint = shootPoint.position + lockedDirection * along;
            if (Vector3.Distance(closestPoint, TargetPoint) > hitRadius) return false;

            return !Physics.Raycast(shootPoint.position, lockedDirection, along, obstacleMask);
        }
    }
}
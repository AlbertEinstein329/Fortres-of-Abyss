namespace AbyssFirstPersonController
{
    using UnityEngine;


    public class BowWeapon : WeaponBase
    {
        [SerializeField] private CombatAction drawAction;
        [SerializeField] private LayerMask hitMask;
        
        [Header("Draw Pose")]
        [SerializeField] private Transform bowModel;
        [SerializeField] private Vector3 drawOffset = new Vector3(0f, 0f, -0.15f);
        [SerializeField, Min(0.01f)] private float fullDrawTime = 1f;
        [SerializeField, Range(0f, 1f)] private float minDrawDamageMultiplier = 0.25f;
        [SerializeField] private float drawPoseSpeed = 4f;
        [SerializeField] private float releasePoseSpeed = 12f;


        private bool isConfigured;
        private Vector3 restPosition;
        private float drawStartTime;
        private float shownPose;
        private float DrawRatio => Mathf.Clamp01((Time.time - drawStartTime) / fullDrawTime);
        public override WeaponType Type => WeaponType.Bow;

        public override void Initialize(PlayerCombatController combat)
        {
            base.Initialize(combat);

            isConfigured = hitOrigin != null && bowModel != null;
            if (isConfigured)
            {
                restPosition = bowModel.localPosition;
            }
            else
            {
                if (hitOrigin == null)
                {
                    Debug.LogError("BowWeapon needs hitOrigin: an unrotated empty child of the camera.", this);
                }
                if (bowModel == null)
                {
                    Debug.LogError("BowWeapon needs bowModel", this);
                }
            }
        }

        public override CombatAction GetAction(CombatSlot slot)
        {
            if (!isConfigured) return null;
            switch (slot)
            {
                case CombatSlot.Secondary:
                    return drawAction;
                case CombatSlot.Primary:
                case CombatSlot.Block:
                case CombatSlot.Skill1:
                    return null;
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(slot), slot, null);
            }
        }

        public override void BeginAction(CombatAction action, LocomotionContext context)
        {
            currentAction = action;
            drawStartTime = Time.time;
        }

        private void Update()
        {
            if (!isConfigured) return;

            float targetPose = currentAction != null ? DrawRatio : 0f;
            float speed = targetPose > shownPose ? drawPoseSpeed : releasePoseSpeed;
            shownPose = Mathf.MoveTowards(shownPose, targetPose, speed * Time.deltaTime);

            bowModel.localPosition = Vector3.Lerp(restPosition, restPosition + drawOffset, shownPose);
        }

        public override ActionStatus Tick(ICombatInputManager input)
        {
            if (currentAction == null) return ActionStatus.Finished;

            if (input.PrimaryAttack) return FireArrow();
            if (!input.SecondaryHeld) return CancelDraw();

            return ActionStatus.Running;
        }


        private ActionStatus FireArrow()
        {
            LocomotionContext fireContext = combat.fpscontroller.currentLocomotionContext;
            float chargeRatio = DrawRatio;

            if (TryShoot(out IDamageable target))
            {
                target.TakeDamage(CalculateDamage(chargeRatio));
                combat.ReportHit(new HitReport(currentAction, fireContext, 1, Type, chargeRatio));
            }

            currentAction = null;
            return ActionStatus.Finished;
        }

        private float CalculateDamage(float chargeRatio)
        {
            float multiplier = Mathf.Lerp(minDrawDamageMultiplier, 1f, chargeRatio);
            return currentAction.attackDamage * multiplier;
        }

        private ActionStatus CancelDraw()
        {
            currentAction = null;
            return ActionStatus.Cancelled;
        }

        private bool TryShoot(out IDamageable target)
        {
            target = null;
            Ray ray = new Ray(hitOrigin.position, hitOrigin.forward);
            float range = currentAction.attackRange;

            if (!Physics.Raycast(ray, out RaycastHit hit, range, hitMask))
            {
                DrawShotRay(ray, range, Color.red);
                return false;
            }

            DrawShotRay(ray, hit.distance, Color.green);
            target = hit.collider.GetComponentInParent<IDamageable>();
            return target != null;
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void DrawShotRay(Ray ray, float distance, Color color)
        {
            Debug.DrawRay(ray.origin, ray.direction * distance, color, 1f);
        }

        public override bool TryInterrupt(InterruptReason reason)
        {
            if (reason == InterruptReason.Dash) return false;

            return base.TryInterrupt(reason);
        }

    }
}
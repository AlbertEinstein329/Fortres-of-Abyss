namespace AbyssFirstPersonController
{
    using UnityEngine;

    public abstract class WeaponBase : MonoBehaviour, IWeapon
    {
        [Header("Weapon Settings")]
        [SerializeField] protected Animator animator;
        [SerializeField] protected Transform hitOrigin;
        [SerializeField] protected LayerMask enemyMask;

        protected PlayerCombatController combat;
        protected CombatAction currentAction;
        protected LocomotionContext startContext;

        protected static readonly int SpeedHash = Animator.StringToHash("AttackSpeed");

        public virtual void Initialize(PlayerCombatController combat)
        {
            this.combat = combat;
        }

        public abstract CombatAction GetAction(CombatSlot slot);

        public virtual void BeginAction(CombatAction action, LocomotionContext context)
        {
            startContext = context;
            currentAction = action;
            float speed = Mathf.Max(action.speed, 0.01f);
            animator.SetFloat(SpeedHash, speed);
            animator.ResetTrigger(action.animatorTrigger);
            animator.SetTrigger(action.animatorTrigger);
        }

        public virtual void AnimEvent_ActionEnd()
        {
            currentAction = null;
            combat.OnActionFinished();
        }

        public virtual void AnimEvent_ActionHit()
        {
            if (currentAction == null) return;
        }

        public virtual ActionStatus Tick(ICombatInputManager input) => ActionStatus.Finished;

        public virtual void Interrupt() { }


    }

}
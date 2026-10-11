namespace AbyssFirstPersonController
{

    using UnityEngine;

    public class CombatAttackingState : CombatBaseState
    {
        private readonly CombatAction action;
        private float timer;
        private float timeLimit;
        public CombatAttackingState(PlayerCombatController currentContext, CombatStateFactory combatStateFactory, CombatAction action)
            : base(currentContext, combatStateFactory) 
        {
            this.action = action;
        }


        public override void EnterState()
        { 
            ctx.actionFinished = false;
            timer = 0f;
            timeLimit = action.maxDuration / Mathf.Max(action.speed, 0.01f) + 0.5f; // Prevent division by zero
            ctx.EquippedWeapon.BeginAction(action, ctx.fpscontroller.currentLocomotionContext);
        }

        public override void UpdateState()
        {
            timer += Time.deltaTime;
            CheckSwitchStates();
        }

        public override void ExitState() { }

        public override void CheckSwitchStates()
        {
            if (ctx.actionFinished)
            {
                SwitchState(combatStateFactory.Idle());
            }
            else if (timer >= timeLimit)
            {
                //Debug.LogWarning($"Action '{action.name}' exceeded its maximum duration of {action.maxDuration} seconds at speed {action.speed}. Forcing exit.");
                SwitchState(combatStateFactory.Idle());

            }
        }
    }


}

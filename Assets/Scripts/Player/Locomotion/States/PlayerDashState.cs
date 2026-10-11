namespace AbyssFirstPersonController
{
    using UnityEngine;

    public class PlayerDashState : PlayerBaseState
    {
        private float dashTimer;
        private float OrigialFovSpeed;
        private Vector3 dashDirection;

        public PlayerDashState(FirstPersonController currentContext, PlayerStateFactory playerStateFactory)
            : base(currentContext, playerStateFactory) { }

        public override void EnterState()
        {
            ctx.InvokeDashEvent();
            OrigialFovSpeed = ctx.fovChangeSpeed;
            dashTimer = 0f;
            dashDirection = DashDirection();
            // Start Dash Effect
            ctx.fovChangeSpeed = 20f;
            ctx.targetFov = 90f;
        }

        public override void UpdateState()
        {
            ctx.combatPermissionsType = CombatPermissions.Primary;
            ctx.currentLocomotionContext = LocomotionContext.Dashing;
            // Dash physics logic...
            ApplyDash();
            dashTimer += Time.deltaTime;
            CheckSwitchStates();
        }

        public override void ExitState()
        {
            // End Dash Effect
            ctx.targetFov = ctx.normalFov;
            ctx.fovChangeSpeed = OrigialFovSpeed;
        }

        public override void CheckSwitchStates()
        {
            if (dashTimer > 0.25f)
            {
                SwitchState(factory.Grounded());
            }

        }
        public Vector3 DashDirection()
        {
            Vector2 input = ctx.input.MoveInput;
            Vector3 direction = ctx.transform.right * input.x + ctx.transform.forward * input.y;

            if (direction.sqrMagnitude < 0.01f)
            {
                direction = ctx.transform.forward;
            }
            direction.y = 0f;
            return direction.normalized;
        }

        public void ApplyDash()
        {
            Vector3 dashVelocity = dashDirection * ctx.dashSpeed;

            ctx.currentVelocity = dashVelocity;

            if (ctx.isGrounded) dashVelocity.y = -5f;

            ctx.characterController.Move(dashVelocity * Time.deltaTime);
        }
    }
}

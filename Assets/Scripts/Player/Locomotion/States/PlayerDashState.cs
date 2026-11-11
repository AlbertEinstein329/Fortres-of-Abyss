namespace AbyssFirstPersonController
{
    using UnityEngine;

    public class PlayerDashState : PlayerBaseState
    {
        private float dashTimer;
        private float OriginalFovSpeed;
        private Vector3 dashDirection;

        public PlayerDashState(FirstPersonController currentContext, PlayerStateFactory playerStateFactory)
            : base(currentContext, playerStateFactory) { }

        public override void EnterState()
        {
            ctx.InvokeDashEvent();
            OriginalFovSpeed = ctx.fovChangeSpeed;
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
            ctx.fovChangeSpeed = OriginalFovSpeed;
            ctx.nextDashTime = Time.time + ctx.dashCooldown;
        }

        public override void CheckSwitchStates()
        {
            if (dashTimer > ctx.dashDuration)
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

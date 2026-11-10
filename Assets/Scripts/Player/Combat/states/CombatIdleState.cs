namespace AbyssFirstPersonController
{
    using System;
    using UnityEngine;

    public class CombatIdleState : CombatBaseState
    {
        public CombatIdleState(PlayerCombatController currentContext, CombatStateFactory combatStateFactory)
            : base(currentContext, combatStateFactory) { }
    

        public override void EnterState()
        { }

        public override void UpdateState()
        {
            CheckSwitchStates();
        }

        public override void ExitState() { }

        public override void CheckSwitchStates()
        {
            CombatPermissions permissions = ctx.fpscontroller.combatPermissionsType;
            if ((permissions & CombatPermissions.Block) != 0 && ctx.combatInput.Block)
            {
                TryStartAction(CombatSlot.Block);
            }
            else if ((permissions & CombatPermissions.Primary) != 0 && ctx.combatInput.PrimaryAttack)
            {
                TryStartAction(CombatSlot.Primary);
            }
            else if ((permissions & CombatPermissions.Secondary) != 0 && ctx.combatInput.SecondaryAttack)
            {
                TryStartAction(CombatSlot.Secondary);
            }
            else if ((permissions & CombatPermissions.Skill) != 0 && ctx.combatInput.Skill1)
            {
                TryStartAction(CombatSlot.Skill1);
            }
        }

        private void TryStartAction(CombatSlot slot)
        {
            LocomotionContext locoContext = ctx.fpscontroller.currentLocomotionContext;
            CombatAction action = ctx.EquippedWeapon.GetAction(slot);
            if (action == null) return;
            if ((locoContext & action.allowedContexts) == 0) return;
            if (action.skillId != 0 && !ctx.skillProgress.IsUnlocked(action.skillId)) return;
            //Debug.Log($"Starting action {action.name} in context {locoContext}");

            CombatBaseState next = action.kind == ActionKind.Charge
                ? combatStateFactory.Charge(action)
                : combatStateFactory.Attack(action);
            SwitchState(next);
        }
    }
}

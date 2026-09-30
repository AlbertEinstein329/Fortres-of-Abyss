namespace AbyssFirstPersonController
{
    using System;
    using Unity.VisualScripting;
    using UnityEngine;
    
    public class PlayerCombatController : MonoBehaviour
    {
        [Header("Combat State Machine")]
        public CombatBaseState CurrentState;
        public CombatStateFactory States;
        [HideInInspector] public FirstPersonController fpscontroller;
        [HideInInspector] public ICombatInputManager CombatInput;

        private void Awake()
        {
            fpscontroller = GetComponent<FirstPersonController>();
            CombatInput = GetComponent<ICombatInputManager>();
            States = new CombatStateFactory(this);
            CurrentState = States.Idle();
            CurrentState.EnterState();
        }
        private void Update()
        {
            CurrentState.UpdateState();
            CurrentState.CheckSwitchStates();
        }
    }


}


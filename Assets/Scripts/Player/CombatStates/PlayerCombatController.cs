namespace AbyssFirstPersonController
{
    using System;
    using UnityEngine;

    public class PlayerCombatController : MonoBehaviour
    {
        [Header("Combat State Machine")]
        [HideInInspector] public CombatBaseState CurrentState;
        [HideInInspector] public CombatStateFactory States;
        [HideInInspector] public FirstPersonController fpscontroller;
        [HideInInspector] public ICombatInputManager combatInput;
        [HideInInspector] public SkillProgress skillProgress;


        [Header("Weapon")]
        [SerializeField] private WeaponBase equippedWeapon;
        public IWeapon EquippedWeapon => equippedWeapon;

        [HideInInspector] public bool actionFinished;
        public event Action<HitReport> HitConfirmed;
        private void Awake()
        {
            if (equippedWeapon == null)
            {
                Debug.LogError("No weapon equipped on PlayerCombatController.", this);
                enabled = false;
                return;
            }
            equippedWeapon.Initialize(this);
            fpscontroller = GetComponent<FirstPersonController>();
            combatInput = GetComponent<ICombatInputManager>();
            skillProgress = GetComponent<SkillProgress>();
            States = new CombatStateFactory(this);
            CurrentState = States.Idle();
            CurrentState.EnterState();
        }
        private void Update()
        {
            CurrentState.UpdateState();
        }

        public void OnActionFinished()
        {
            actionFinished = true;
        }

        public void ReportHit(HitReport report)
        {
            HitConfirmed?.Invoke(report);
            //Debug.Log($"Hit reported: {report}");
        }
    }

}
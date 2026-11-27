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
        [HideInInspector] public bool actionFinished;

        [Header("Weapon")]
        [SerializeField] private WeaponBase equippedWeapon;
        [SerializeField] private WeaponBase[] weapons;

        public IWeapon EquippedWeapon => equippedWeapon;
        public bool IsBlocking => equippedWeapon != null && equippedWeapon.IsBlocking;
        public event Action<HitReport> HitConfirmed;
        public event Action<WeaponType> WeaponChanged;

        private void Awake()
        {
            // Fail-fast: verify the array exists and contains at least one weapon before proceeding.
            if (weapons == null || weapons.Length == 0)
            {
                Fail("No weapons assigned to PlayerCombatController.");
                return;
            }

            // Validate all slots first so no weapon gets partially initialized if a later slot is null.
            foreach (WeaponBase weapon in weapons)
            {
                if (weapon == null)
                {
                    Fail("One or more weapon slots in PlayerCombatController is empty.");
                    return;
                }
            }

            // Array.IndexOf returns -1 if equippedWeapon is null or missing from the weapons array.
            if (Array.IndexOf(weapons, equippedWeapon) == -1)
            {
                Fail("The equipped weapon is not in the weapons array.");
                return;
            }

            // Initialize every weapon now because inactive GameObjects do not run Awake/Update,
            // and weapons like the bow need to cache their rest pose before being equipped.
            foreach (WeaponBase weapon in weapons)
            {
                weapon.Initialize(this);
                weapon.RootObject.SetActive(weapon == equippedWeapon);
            }

            fpscontroller = GetComponent<FirstPersonController>();
            combatInput = GetComponent<ICombatInputManager>();
            skillProgress = GetComponent<SkillProgress>();
            States = new CombatStateFactory(this);
            CurrentState = States.Idle();
            CurrentState.EnterState();
        }

        public void TryEquip(int index)
        {
            // Guard clauses: ignore out-of-range input keys or redundant equip calls.
            if (index < 0 || index >= weapons.Length) return;
            if (weapons[index] == equippedWeapon) return;

            equippedWeapon.RootObject.SetActive(false);
            equippedWeapon = weapons[index];
            equippedWeapon.RootObject.SetActive(true);

            // Notify listeners (such as HUD icons) only after the new weapon state is fully active.
            WeaponChanged?.Invoke(equippedWeapon.Type);
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
        }

        private void OnEnable()
        {
            if (fpscontroller != null)
            {
                fpscontroller.OnPlayerDash += HandleDash;
            }
        }

        private void OnDisable()
        {
            if (fpscontroller != null)
            {
                fpscontroller.OnPlayerDash -= HandleDash;
            }
        }

        private void HandleDash()
        {
            RequestInterrupt(InterruptReason.Dash);
        }

        public void RequestInterrupt(InterruptReason reason)
        {
            CurrentState.OnInterruptRequested(reason);
        }

        public void ResetForRespawn()
        {
            equippedWeapon.TryInterrupt(InterruptReason.Death);
            actionFinished = false;
            CurrentState = States.Idle();
            CurrentState.EnterState();
        }

        private void Fail(string message)
        {
            Debug.LogError(message, this);
            enabled = false;
        }
    }

}
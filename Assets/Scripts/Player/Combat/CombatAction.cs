namespace AbyssFirstPersonController
{
    using UnityEngine;

    [System.Serializable]
    public class CombatAction
    {
        public LocomotionContext allowedContexts = LocomotionContext.All;
        public ActionKind kind = ActionKind.Instant;
        [Min(0.01f)] public float speed = 1f;
        public float maxDuration = 1f; // seconds at speed 1, used only as a safety limit
        [Min(0.01f)] public float fullChargeTime = 1f;
        [Range(0f, 1f)] public float minChargeDamageMultiplier = 0.3f;
        public string name;
        public string animatorTrigger;
        public float attackRange = 2;
        public float attackRadius = 1;
        public float attackDamage = 10;
        public int skillId = 0;
        public bool interruptibleByDash = true;
    }
}

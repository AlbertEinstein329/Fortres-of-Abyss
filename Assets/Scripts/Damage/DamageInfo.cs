using UnityEngine;

namespace AbyssFirstPersonController
{
    public readonly struct DamageInfo
    {
        public readonly float amount;
        public readonly Vector3 sourcePosition;

        public DamageInfo(float amount, Vector3 sourcePosition)
        {
            this.amount = amount;
            this.sourcePosition = sourcePosition;
        }
    }
}


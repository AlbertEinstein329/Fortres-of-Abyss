namespace AbyssFirstPersonController
{
    using System;
    using UnityEngine;

    public class Stamina : MonoBehaviour
    {
        [SerializeField, Min(1f)] private float maxStamina = 100f;
        [SerializeField, Min(0f)] private float regenPerSecond = 25f;
        [SerializeField, Min(0f)] private float regenDelay = 1f;
        [SerializeField, Min(0f)] private float recoverThreshold = 25f;

        public float MaxStamina => maxStamina;
        public float Current { get; private set; }
        public float Ratio => Current / maxStamina;
        public bool IsExhausted { get; private set; }
        public event Action Changed;
        private float lastSpendTime;

        private void Awake()
        {
            Current = maxStamina;
        }

        private void Update()
        {
            Regenerate();

            if (IsExhausted && Current >= recoverThreshold)
            {
                IsExhausted = false;
            }
        }

        private void Regenerate()
        {
            if (Current >= maxStamina) return;
            if (Time.time - lastSpendTime < regenDelay) return;

            float oldStamina = Current;
            Current = Mathf.Min(Current + regenPerSecond * Time.deltaTime, maxStamina);

            if (!Mathf.Approximately(oldStamina, Current))
            {
                Changed?.Invoke();
            }
        }

        public bool TrySpend(float amount) 
        {
            if (amount <= 0f) return true;
            
            if (Current < amount) return false;

            float oldStamina = Current;
            Current = Mathf.Max(Current - amount, 0f);
            lastSpendTime = Time.time;

            if (!Mathf.Approximately(oldStamina, Current))
            {
                Changed?.Invoke();
            }

            return true;
        }

        public void Drain(float amount) 
        {
            if (amount <= 0f) return;

            float oldStamina = Current;
            Current = Mathf.Max(Current - amount, 0f);
            lastSpendTime = Time.time;

            if (Current <= 0f)
            {
                IsExhausted = true;
            }

            if (!Mathf.Approximately(oldStamina, Current))
            {
                Changed?.Invoke();
            }
        }

        public void Restore() 
        {
            Current = maxStamina;
            IsExhausted = false;
            lastSpendTime = Time.time;
            Changed?.Invoke();
        }
    }
}
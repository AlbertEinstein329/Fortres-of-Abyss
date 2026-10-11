using AbyssFirstPersonController;
using UnityEngine;

public class TargetDummy : MonoBehaviour, IDamageable
{
    [SerializeField] private float Health = 100f;
    public float CurrentHealth { get; private set; }

    private void Awake()
    {
        CurrentHealth = Health;
    }

    public void TakeDamage(float damage)
    {
        if (CurrentHealth <= 0f) return;

        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0f);

        Debug.Log($"TargetDummy took {damage} damage. Current health: {CurrentHealth}");

        if (CurrentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        //Debug.Log("TargetDummy has died.");
        gameObject.SetActive(false);
    }
}


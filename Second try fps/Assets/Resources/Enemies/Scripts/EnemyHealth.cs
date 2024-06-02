using System;
using UnityEngine;

public class EnemyHealth : Health
{
    [SerializeField] private float _maxHealth;

    public event Action OnHealthZero;

    private HealthBar healthBar;
    private float currentHealth;

    private void Awake()
    {
        healthBar = GetComponentInChildren<HealthBar>();
        healthBar.InitializeSelf();
    }
    private void OnEnable()
    {
        currentHealth = _maxHealth;
        healthBar.UpdateHealthBar(currentHealth, _maxHealth);
    }

    public override void RestoreHealth(float gained)
    {
       
    }

    public override void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.UpdateHealthBar(currentHealth, _maxHealth);
        if (currentHealth <= 0) 
            OnHealthZero?.Invoke();
    }
}

using UnityEngine;

public class PlayerHealth : Health, IShootable
{
    [SerializeField] private float _maxHealth;
    [SerializeField] private PlayerHealthIcon _healthIcon;

    private float currentHealth;

    private void Start()
    {
        currentHealth = _maxHealth;
        _healthIcon = FindAnyObjectByType<PlayerHealthIcon>();
    }


    public override void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, _maxHealth);
        _healthIcon.StartCoroutine(_healthIcon.DecreaseHealthBar(currentHealth, _maxHealth));
    }

    public override void RestoreHealth(float gained)
    {
        currentHealth += gained;
        currentHealth = Mathf.Clamp(currentHealth, 0, _maxHealth);
        StartCoroutine(_healthIcon.RestoreHealthBar(currentHealth,_maxHealth));
    }

    public void TakeBullet(float damage)
    {
        TakeDamage(damage);
    }
}

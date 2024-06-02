using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Slider healthBar;
    private Camera camera;


    public void InitializeSelf()
    {
        healthBar = GetComponentInChildren<Slider>();
        camera = Camera.main;
    }

    private void Update()
    {
        healthBar.transform.rotation = camera.transform.rotation;
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if(currentHealth < 0) 
            currentHealth = 0;
        healthBar.value = currentHealth / maxHealth;
    }
}

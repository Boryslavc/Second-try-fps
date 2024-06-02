using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthIcon : MonoBehaviour
{
    [SerializeField] private float _chipDuration;

    public Image BackHealthImage;
    public Image FrontHealthImage;


    public IEnumerator DecreaseHealthBar(float currentHealth, float maxHealth)
    {
        float healthPercentage = currentHealth / maxHealth;

        FrontHealthImage.fillAmount = healthPercentage;
        BackHealthImage.color = Color.red;

        float timeElapsed = 0;
        float timeFraction = timeElapsed / _chipDuration;

        while (timeFraction < _chipDuration)
        {
            BackHealthImage.fillAmount = Mathf.Lerp(BackHealthImage.fillAmount,
                healthPercentage, timeFraction);
            timeElapsed += Time.deltaTime;
            timeFraction = timeElapsed / _chipDuration;
            yield return null;
        }
    }

    public IEnumerator RestoreHealthBar(float currentHealth,float maxHealth)
    {
        float healthPercentage = currentHealth / maxHealth;

        BackHealthImage.color = Color.green;
        BackHealthImage.fillAmount = healthPercentage;

        float currentFrontHealthBar = FrontHealthImage.fillAmount;

        float timeElapsed = 0;
        float timeFraction = timeElapsed / _chipDuration;

        while (timeFraction < _chipDuration)
        {
            FrontHealthImage.fillAmount = Mathf.Lerp(currentFrontHealthBar,
                healthPercentage, timeFraction);
            timeElapsed += Time.deltaTime;
            timeFraction = timeElapsed / _chipDuration;
            yield return null;
        }
    }
}

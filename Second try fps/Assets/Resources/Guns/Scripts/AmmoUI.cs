using TMPro;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    private TMP_Text text;

    private void OnEnable()
    {
        text = GetComponentInChildren<TMP_Text>();
    }

    public void UpdateAmmoUI(int ammoCount)
    {
        text.text = ammoCount.ToString();
    }
}

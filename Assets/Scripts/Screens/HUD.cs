
using TMPro;
using UnityEngine;

public class HUD : Screen
{
    [SerializeField] private TextMeshProUGUI amoText;
    [SerializeField] private TextMeshProUGUI magazineText;

    public void SetAmoText(int currentAmo, int maxAmo)
    {
        amoText.text = currentAmo + "/" + maxAmo;
    }

    public void SetMagazineText(int value)
    {
        magazineText.text = value.ToString();
    }
}

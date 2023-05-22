
using TMPro;
using UnityEngine;

public class HUD : Screen
{
    [SerializeField] private TextMeshProUGUI amoText;

    public void SetAmoText(int currentAmo, int maxAmo)
    {
        amoText.text = currentAmo + "/" + maxAmo;
    }
}

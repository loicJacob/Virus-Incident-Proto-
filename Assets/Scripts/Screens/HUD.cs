
using TMPro;
using UnityEngine;

public class HUD : Screen
{
    [SerializeField] private TextMeshProUGUI amoText;

    private void Start()
    {
        UIManager.Instance.OnUpdateAmo += SetAmoText;
    }

    public override void OnOpen()
    {
        UIManager.Instance.RequestAmoUpdate?.Invoke();
    }

    public void SetAmoText(int currentAmo, int maxAmo)
    {
        amoText.text = currentAmo + "/" + maxAmo;
    }

    private void OnDestroy()
    {
        UIManager.Instance.OnUpdateAmo -= SetAmoText;
    }
}


using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] public Screen HUD;

    public static UIManager Instance => _instance;
    private static UIManager _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
            return;
        }

        _instance = this;
    }

    private void OnDestroy()
    {
        _instance = null;
    }
}

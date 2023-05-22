
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public enum Screens
    {
        HUD,
        RELOAD
    }

    [SerializeField] private Screen HUD;
    [SerializeField] private Screen ReloadScreen;

    private Screen lastScreenOpenned;
    private Dictionary<Screens, Screen> screens = new Dictionary<Screens, Screen>();

    public Action<int, int> OnUpdateAmo;
    public Action RequestAmoUpdate;

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

        screens.Add(Screens.HUD, HUD);
        screens.Add(Screens.RELOAD, ReloadScreen);
    }

    private void Start()
    {
        StartCoroutine(LateStart());
    }

    private IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();
        Init();
    }

    public void Init()
    {
        OpenScreen(Screens.HUD);
    }

    public void OpenScreen(Screens screen)
    {
        lastScreenOpenned = screens[screen];
        lastScreenOpenned.GetComponent<Animator>().SetBool("IsOpen", true);
        lastScreenOpenned.OnOpen();
    }

    public void CloseScreen(Screens screen)
    {
        screens[screen].GetComponent<Animator>().SetBool("IsOpen", false);
    }

    public void SwapScreen(Screens previousScreen, Screens nextScreen)
    {
        CloseScreen(previousScreen);
        OpenScreen(nextScreen);
    }

    private void OnDestroy()
    {
        _instance = null;
    }
}

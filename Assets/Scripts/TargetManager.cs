
using System.Collections;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    public static TargetManager Instance => _instance;
    private static TargetManager _instance;

    [SerializeField] private float timeBeforeResetTarget = 3;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(gameObject);
        else
            _instance = this;
    }

    public void ResetTarget(GameObject target)
    {
        StartCoroutine(WaitBeforeRespawn(target));
    }

    private IEnumerator WaitBeforeRespawn(GameObject target)
    {
        yield return new WaitForSeconds(timeBeforeResetTarget);
        target.SetActive(true);
    }

    private void OnDestroy()
    {
        _instance = null;
    }
}

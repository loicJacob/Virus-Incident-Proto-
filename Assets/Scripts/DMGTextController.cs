
using UnityEngine;
using System.Collections.Generic;

public class DMGTextController : MonoBehaviour
{
    [SerializeField] private Transform poolContainter;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    [SerializeField] private Color colorNormal;
    [SerializeField] private Color colorCritical;

    [SerializeField] private float popDuration = 0.5f;
    [SerializeField] private float stayDuration = 0.1f;
    [SerializeField] private float fadeDuration = 0.1f;

    [SerializeField] private AnimationCurve popCurve;

    private List<Transform> dmgTexts = new List<Transform>();
    private int poolIndex = 0;

    private void Start()
    {
        foreach (Transform item in poolContainter)
            dmgTexts.Add(item);
    }

    public void DisplayText(bool isCriticalHitted)
    {
        poolIndex++;

        if (poolIndex >= dmgTexts.Count)
            poolIndex = 0;

        dmgTexts[poolIndex].gameObject.SetActive(true);
        dmgTexts[poolIndex].position = startPoint.position;
        dmgTexts[poolIndex].rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        Color color = isCriticalHitted? colorCritical : colorNormal;
        dmgTexts[poolIndex].GetComponent<DMGText>().Animate(isCriticalHitted, color, endPoint.position, popDuration, stayDuration, fadeDuration, popCurve);
    }
}

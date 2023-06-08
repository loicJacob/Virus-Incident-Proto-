
using UnityEngine;
using DG.Tweening;
using TMPro;

public class DMGText : MonoBehaviour
{
    private TextMeshProUGUI text;
    private Vector3 initialScale;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        initialScale = transform.localScale;
    }

    public void Animate(bool isCriticalHitted, Color color, Vector3 endPoint, float popDuration, float stayDuration, float fadeDuration, AnimationCurve popCurve)
    {
        Vector3 targetScale = isCriticalHitted ? initialScale + new Vector3(0.1f, 0.1f, 0.1f) : initialScale;

        text.color = color;
        transform.localScale = Vector3.zero;

        transform.DOMoveY(endPoint.y, popDuration).SetEase(popCurve);
        transform.DOScale(targetScale, popDuration)
            .SetEase(popCurve)
            .OnComplete(() =>
            {
                text.DOFade(0, fadeDuration).SetDelay(stayDuration).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
            });
    }
}

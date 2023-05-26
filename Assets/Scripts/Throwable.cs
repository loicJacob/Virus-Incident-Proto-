
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Throwable : MonoBehaviour
{
    [SerializeField] private Image timeRemainSlider;
    [SerializeField] private ParticleSystem effectParticles;
    [SerializeField] private float timeBeforeEffect = 5f;

    private float elapsedTimeEffect = 0;
    private Coroutine waitBeforeEffectCoroutine;
    private Rigidbody rb;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        waitBeforeEffectCoroutine = StartCoroutine(WaitBeforeEffect());
        rb.isKinematic = true;
    }

    public void Throw(Vector3 strength)
    {
        rb.isKinematic = false;
        rb.AddForce(strength);
    }

    private void OnDisable()
    {
        StopCoroutine(waitBeforeEffectCoroutine);
        waitBeforeEffectCoroutine = null;
    }

    private IEnumerator WaitBeforeEffect()
    {
        elapsedTimeEffect = 0;

        while (elapsedTimeEffect < timeBeforeEffect)
        {
            elapsedTimeEffect += Time.deltaTime;
            timeRemainSlider.fillAmount = elapsedTimeEffect / timeBeforeEffect;
            yield return null;
        }

        Instantiate(effectParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}

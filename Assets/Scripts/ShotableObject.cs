
using UnityEngine;

public class ShotableObject : MonoBehaviour
{

    [SerializeField] private ParticleSystem shootedParticles;
    [SerializeField] private float distanceToCriticalHit = 1;

    public virtual void OnHit(Vector3 hitPoint, Vector3 hitNormal, float damage)
    {
        Instantiate(shootedParticles, hitPoint, Quaternion.LookRotation(hitNormal, Vector3.up));
    }

    protected float CheckCriticalHit(Vector3 hitPoint, float damage, float criticalMultiplier)
    {
        Vector2 hitPointHorizontal = new Vector2(hitPoint.x, hitPoint.z);
        Vector2 positionHorizontal = new Vector2(transform.position.x, transform.position.z);

        if (Vector3.Distance(hitPointHorizontal, positionHorizontal) < distanceToCriticalHit)
            return damage * criticalMultiplier;

        return damage;
    }
}

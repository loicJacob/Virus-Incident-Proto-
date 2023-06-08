
using UnityEngine;

public class ShotableObject : MonoBehaviour
{

    [SerializeField] private ParticleSystem shootedParticles;
    [SerializeField] protected float life = 100;
    [SerializeField] private float distanceToCriticalHit = 1;

    protected bool isCriticalHitted = false;

    public virtual void OnHit(Vector3 hitPoint, Vector3 hitNormal, float damage)
    {
        Instantiate(shootedParticles, hitPoint, Quaternion.LookRotation(hitNormal, Vector3.up));


        isCriticalHitted = false;
        life -= CheckCriticalHit(hitPoint, damage, 2);

        if (life <= 0)
            Die();
    }

    private float CheckCriticalHit(Vector3 hitPoint, float damage, float criticalMultiplier)
    {
        Vector2 hitPointHorizontal = new Vector2(hitPoint.x, hitPoint.z);
        Vector2 positionHorizontal = new Vector2(transform.position.x, transform.position.z);

        if (Vector3.Distance(hitPointHorizontal, positionHorizontal) < distanceToCriticalHit)
        {
            isCriticalHitted = true;
            return damage * criticalMultiplier;
        }

        isCriticalHitted = false;
        return damage;
    }

    protected virtual void Die()
    {
        Debug.Log(name + " has been killed");
    }
}

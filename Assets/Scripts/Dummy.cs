
using UnityEngine;

public class Dummy : ShotableObject
{
    [SerializeField] private DMGTextController dmgTextController;

    private float maxLife;

    private void Awake()
    {
        maxLife = life;
    }

    private void OnEnable()
    {
        life = maxLife;
    }

    public override void OnHit(Vector3 hitPoint, Vector3 hitNormal, float damage)
    {
        base.OnHit(hitPoint, hitNormal, damage);
        dmgTextController.DisplayText(isCriticalHitted);
    }

    protected override void Die()
    {
        base.Die();
        TargetManager.Instance.ResetTarget(gameObject);
        gameObject.SetActive(false);
    }
}


using System;
using UnityEngine;

public class Zombie : ShotableObject
{
    [SerializeField] private float life = 100;

    // animation IDs
    private int animIDSpeed;
    private int animIDGrounded;
    private int animIDFreeFall;
    private int animIDMotionSpeed;

    private Animator animator;
    private CharacterController controller;

    private Action DoAction;


    private void Awake()
    {
        SetModeVoid();
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        AssignAnimationIDs();
    }

    private void AssignAnimationIDs()
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDFreeFall = Animator.StringToHash("FreeFall");
        animIDMotionSpeed = Animator.StringToHash("MotionSpeed");

        //Remove this later
        animator.SetBool(animIDFreeFall, false);
    }

    private void Update()
    {
        DoAction.Invoke();
    }

    private void SetModeVoid()
    {
        DoAction = DoActionVoid;
    }

    private void SetModeChase()
    {
        DoAction = DoActionChase;
    }

    private void DoActionVoid()
    {

    }

    private void DoActionChase()
    {

    }

    public override void OnHit(Vector3 hitPoint, Vector3 hitNormal, float damage)
    {
        base.OnHit(hitPoint, hitNormal, damage);

        life -= CheckCriticalHit(hitPoint, damage, 2);

        if (life <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log("Zombie killed");
        Destroy(gameObject);
    }
}

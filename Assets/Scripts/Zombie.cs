
using System;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    [SerializeField] private float life = 100;
    [SerializeField] private int horizontalDistanceToHeadShot = 1;

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

    public void OnHit(Vector3 hitPoint, float damage)
    {
        Vector2 hitPointHorizontal = new Vector2(hitPoint.x, hitPoint.z);
        Vector2 positionHorizontal = new Vector2(transform.position.x, transform.position.z);

        if (Vector3.Distance(hitPointHorizontal, positionHorizontal) < horizontalDistanceToHeadShot)
            damage *= 2;

        life -= damage;

        if (life <= 0)
            Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}

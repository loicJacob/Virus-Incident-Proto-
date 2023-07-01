
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : ShotableObject
{
    [SerializeField] private AudioClip landingAudioClip;
    [SerializeField] private AudioClip[] footstepAudioClips;
    [SerializeField, Range(0, 1)] private float footstepAudioVolume = 0.5f;

    [Header("Patrol Settings")]
    [SerializeField] private float patrolPointOffsetDistance = 1;

    [Header("Detection Settings")]
    [SerializeField] private float visionDistance;
    [SerializeField, Range(0, 360)] private float visionAngle;
    [SerializeField] private LayerMask visionObstructionMask;
    [SerializeField] private LayerMask chasedObjectMask;

    [Header("Attack Settings")]
    [SerializeField] private float distanceToAttack = 2;
    [SerializeField] private float angleToFrontAttack = 90;
    [SerializeField] private float angleToSideAttack = 135;
    [SerializeField, Range(0, 100)] private float frontAttackDmg = 10;
    [SerializeField, Range(0, 100)] private float backAttackDmg = 10;
    [SerializeField, Range(0, 100)] private float sideAttackDmg = 10;

    public float VisionDistance => visionDistance;
    public float VisionAngle => visionAngle;
    public float DistanceToAttack => distanceToAttack;

    // animation IDs
    private int animIDSpeed;
    private int animIDGrounded;
    private int animIDFreeFall;
    private int animIDMotionSpeed;
    private int animIDAttackFront;
    private int animIDAttackLeft;
    private int animIDAttackRight;
    private int animIDAttackBack;

    private Action DoAction;
    private Animator animator;
    private NavMeshAgent navMeshAgent;

    private Transform target;
    private Vector3 patrolTargetPos;

    private float currentAttackDmg = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        SetModeVoid();
    }

    private void Start()
    {
        AssignAnimationIDs();
    }

    private void AssignAnimationIDs()
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDFreeFall = Animator.StringToHash("FreeFall");
        animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        animIDAttackFront = Animator.StringToHash("AttackFront");
        animIDAttackLeft = Animator.StringToHash("AttackLeft");
        animIDAttackRight = Animator.StringToHash("AttackRight");
        animIDAttackBack = Animator.StringToHash("AttackBack");

        //Remove this later
        animator.SetBool(animIDFreeFall, false);
    }

    private void Update()
    {
        DoAction.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            other.GetComponent<Player>().GetHit(currentAttackDmg);
    }

#if UNITY_EDITOR
    public void EditorTestPatrol()
    {
        SetModePatrol();
    }

    public void EditorTestChase()
    {
        SetModeChase();
    }
#endif

    private void SetModeVoid()
    {
        DoAction = DoActionVoid;
    }

    private void SetModePatrol()
    {
        Debug.Log(name + " is in patrol state");
        patrolTargetPos = GetNewPatrolPoint();
        DoAction = DoActionPatrol;
    }

    private void SetModeChase()
    {
        Debug.Log(name + " is in chase state");
        DoAction = DoActionChase;
    }

    private void SetModeAttack(Vector3 toTarget)
    {
        Debug.Log(name + " is in attack state");

        float angleToTarget = Vector3.Angle(transform.forward, toTarget);

        DoAction = DoActionAttack;

        if (angleToTarget < angleToFrontAttack / 2)
        {
            Debug.Log(name + " is attacking to front");
            currentAttackDmg = frontAttackDmg;
            animator.SetTrigger(animIDAttackFront);
        }
        else if (angleToTarget < angleToSideAttack / 2)
        {
            Debug.Log(name + " is attacking to sides");
            Vector3 perp = Vector3.Cross(transform.forward, toTarget);
            float dir = Vector3.Dot(perp, transform.up);

            currentAttackDmg = sideAttackDmg;

            if (dir > 0)
                animator.SetTrigger(animIDAttackRight);
            else
                animator.SetTrigger(animIDAttackLeft);
        }
        else
        {
            Debug.Log(name + " is attacking to back");
            currentAttackDmg = backAttackDmg;
            animator.SetTrigger(animIDAttackBack);
        }
    }

    private void DoActionVoid()
    {
        
    }

    private void DoActionPatrol()
    {
        Move(patrolTargetPos);
        Vector3 toTarget = patrolTargetPos - transform.position;

        if (toTarget.magnitude <= patrolPointOffsetDistance)
            patrolTargetPos = GetNewPatrolPoint();

        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, visionDistance, chasedObjectMask);

        if (rangeChecks.Length != 0)
        {
            target = rangeChecks[0].transform;
            toTarget = target.position - transform.position;

            if (Vector3.Angle(transform.forward, toTarget) < visionAngle / 2)
            {
                if (!Physics.Raycast(transform.position, toTarget.normalized, toTarget.magnitude, visionObstructionMask))
                    SetModeChase();
            }
        }
    }

    private void DoActionChase()
    {
        Vector3 toTarget = target.position - transform.position;

        Move(target.position);

        if (toTarget.magnitude <= distanceToAttack)
            SetModeAttack(toTarget);

        animator.SetFloat(animIDSpeed, navMeshAgent.velocity.magnitude);
        animator.SetFloat(animIDMotionSpeed, 1);
    }

    private void DoActionAttack()
    {

    }

    private void Move(Vector3 targetPos)
    {
        navMeshAgent.destination = targetPos;
    }

    private Vector3 GetNewPatrolPoint()
    {
        return Vector3.zero;
    }

    public override void OnHit(Vector3 hitPoint, Vector3 hitNormal, float damage, float knockBackForce)
    {
        base.OnHit(hitPoint, hitNormal, damage, knockBackForce);

        navMeshAgent.velocity += -hitNormal * knockBackForce;
    }

    // Called in animation frame
    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (footstepAudioClips.Length > 0)
            {
                var index = UnityEngine.Random.Range(0, footstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(footstepAudioClips[index], transform.position, footstepAudioVolume);
            }
        }
    }

    // Called in animation frame
    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(landingAudioClip, transform.position, footstepAudioVolume);
        }
    }

    protected override void Die()
    {
        base.Die();
        Destroy(gameObject);
    }
}

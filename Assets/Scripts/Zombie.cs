
using System;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : ShotableObject
{
    [SerializeField] private AudioClip landingAudioClip;
    [SerializeField] private AudioClip[] footstepAudioClips;
    [SerializeField, Range(0, 1)] private float footstepAudioVolume = 0.5f;

    [SerializeField] private float visionDistance;
    [SerializeField, Range(0, 360)] private float visionAngle;
    [SerializeField] private LayerMask visionObstructionMask;
    [SerializeField] private LayerMask chasedObjectMask;

    public float VisionDistance => visionDistance;
    public float VisionAngle => visionAngle;

    // animation IDs
    private int animIDSpeed;
    private int animIDGrounded;
    private int animIDFreeFall;
    private int animIDMotionSpeed;

    private Action DoAction;
    private Animator animator;
    private NavMeshAgent navMeshAgent;

    private Transform chaseTarget;

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

        //Remove this later
        animator.SetBool(animIDFreeFall, false);
    }

    private void Update()
    {
        DoAction.Invoke();
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
        DoAction = DoActionPatrol;
    }

    private void SetModeChase()
    {
        DoAction = DoActionChase;
    }

    private void DoActionVoid()
    {
        
    }

    private void DoActionPatrol()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, visionDistance, chasedObjectMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 toTarget = target.position - transform.position;
            float toTargetDistance = toTarget.magnitude;
            toTarget.Normalize();

            if (Vector3.Angle(transform.forward, toTarget) < visionAngle / 2)
            {
                if (!Physics.Raycast(transform.position, toTarget, toTargetDistance, visionObstructionMask))
                {
                    chaseTarget = target;
                    SetModeChase();
                }
            }
        }
    }

    private void DoActionChase()
    {
        navMeshAgent.destination = chaseTarget.position;

        animator.SetFloat(animIDSpeed, navMeshAgent.velocity.magnitude);
        animator.SetFloat(animIDMotionSpeed, 1);
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


using System;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : ShotableObject
{
    [SerializeField] private bool setModeChase = false;
    [SerializeField] private bool setModeVoid = false;
    [SerializeField] private bool setPosToCenter = false;
    [SerializeField] private Transform placeHolder_Target;

    [SerializeField] private AudioClip landingAudioClip;
    [SerializeField] private AudioClip[] footstepAudioClips;
    [SerializeField, Range(0, 1)] private float footstepAudioVolume = 0.5f;

    // animation IDs
    private int animIDSpeed;
    private int animIDGrounded;
    private int animIDFreeFall;
    private int animIDMotionSpeed;

    private Animator animator;

    private Action DoAction;
    private NavMeshAgent navMeshAgent;

    private float animationBlend;
    private Vector3 centerPosPlaceHolder;
    private Vector3 targetPosPlaceHolder;

    private void Awake()
    {
        centerPosPlaceHolder = transform.position;
        targetPosPlaceHolder = placeHolder_Target.position;
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

        if (setModeChase)
        {
            Debug.Log(name + " is Chasing " + placeHolder_Target.name);
            setModeChase = false;
            SetModeChase();
        }

        if (setModeVoid)
        {
            Debug.Log(name + " is waiting");
            navMeshAgent.isStopped = true;
            setModeVoid = false;
            SetModeVoid();
        }

        if (setPosToCenter)
        {
            setPosToCenter = false;

            if (placeHolder_Target.position == centerPosPlaceHolder)
                placeHolder_Target.position = targetPosPlaceHolder;
            else
                placeHolder_Target.position = centerPosPlaceHolder;
        }
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
        navMeshAgent.destination = placeHolder_Target.position;
        animationBlend = navMeshAgent.velocity.magnitude;

        if (animationBlend < 0.01f)
            animationBlend = 0f;

        animator.SetFloat(animIDSpeed, animationBlend);
        animator.SetFloat(animIDMotionSpeed, 1);
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
}

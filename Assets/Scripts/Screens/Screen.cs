
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Screen : MonoBehaviour
{
    protected Animator animator;

    virtual protected void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public virtual void OnOpen()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets animators for objects outside buttons
/// </summary>
public class ButtonAnimator : MonoBehaviour
{
    Animator animator;

    /// <summary>
    /// Used for initialization
    /// </summary>
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void Normal()
    {
        animator.SetTrigger("Normal");
    }

    public void Highlighted()
    {
        animator.SetTrigger("Highlighted");
    }

    public void Pressed()
    {
        animator.SetTrigger("Pressed");
    }

    public void Selected()
    {
        animator.SetTrigger("Selected");
    }

    public void Disabled()
    {
        animator.SetTrigger("Disabled");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Destroy an object after an animation
/// </summary>
public class DestroyAfterAnimation : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        Destroy(animator.gameObject);
    }
}

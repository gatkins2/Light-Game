using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionsPopUp : MonoBehaviour
{
    public KeyCode completeKey;
    Animator animator;

    /// <summary>
    /// Use for initialization
    /// </summary>
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Remove popup on key press
        if (Input.GetKeyDown(completeKey))
        {
            FadeOut();
        }
    }

    /// <summary>
    /// Makes the icon pop up
    /// </summary>
    public void PopUp()
    {
        animator.SetTrigger("PopUp");
    }

    /// <summary>
    /// Removes the popup
    /// </summary>
    public void FadeOut()
    {
        animator.SetTrigger("Complete");
    }
}

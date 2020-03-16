using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CustomMenuButton : MonoBehaviour
{
    public bool Selected { get; set; }
    public int FrameBuffer { private get; set; }

    // Start is called before the first frame update
    void Start()
    {
        Selected = false;
        FrameBuffer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (FrameBuffer > 0)
            FrameBuffer--;
        else
            Selected = false;
        if (Selected)
            transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
        else
            transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
    }

    // Perform the button action
    public abstract void ButtonAction();
}

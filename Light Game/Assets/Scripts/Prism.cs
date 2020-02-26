using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prism : MonoBehaviour
{
    [SerializeField]
    Sprite prismSprite, activePrismSprite;

    LineRenderer redLine, yellowLine, greenLine, blueLine;
    LineRenderer[] lines;

    public int FrameBuffer { private get; set; }
    public bool Active { get; set; }
    public bool Split { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        redLine = transform.GetChild(0).GetComponent<LineRenderer>();
        yellowLine = transform.GetChild(1).GetComponent<LineRenderer>();
        greenLine = transform.GetChild(2).GetComponent<LineRenderer>();
        blueLine = transform.GetChild(3).GetComponent<LineRenderer>();
        lines = new LineRenderer[4];
        lines[0] = redLine;
        lines[1] = yellowLine;
        lines[2] = greenLine;
        lines[3] = blueLine;

        FrameBuffer = 0;
        Active = false;
        Split = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Deactivate when not being pointed at
        if (FrameBuffer > 0)
            FrameBuffer--;
        else
            Active = false;
        if (Active)
        {
            foreach (LineRenderer lr in lines)
                lr.enabled = true;
            GetComponent<SpriteRenderer>().sprite = activePrismSprite;
        }
        else
        {
            foreach (LineRenderer lr in lines)
                lr.enabled = false;
            GetComponent<SpriteRenderer>().sprite = prismSprite;
        }
    }

    // Spawn colored copies of the player
    void Refract()
    {

    }
}

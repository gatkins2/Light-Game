using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prism : MonoBehaviour
{
    [SerializeField]
    Sprite prismSprite, activePrismSprite, playerSprite, emptySprite;

    LineRenderer redLine, yellowLine, greenLine, blueLine;
    LineRenderer[] lines;

    Transform redPlayer, yellowPlayer, greenPlayer, bluePlayer;
    Transform[] playerImages;

    Vector2[] points;

    public int FrameBuffer { private get; set; }
    public bool Active { get; set; }
    public bool Split { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        // Get line renderers
        redLine = transform.GetChild(0).GetComponent<LineRenderer>();
        yellowLine = transform.GetChild(1).GetComponent<LineRenderer>();
        greenLine = transform.GetChild(2).GetComponent<LineRenderer>();
        blueLine = transform.GetChild(3).GetComponent<LineRenderer>();
        lines = new LineRenderer[4];
        lines[0] = redLine;
        lines[1] = yellowLine;
        lines[2] = greenLine;
        lines[3] = blueLine;

        // Set colored player sprites
        redPlayer = transform.GetChild(4);
        yellowPlayer = transform.GetChild(5);
        greenPlayer = transform.GetChild(6);
        bluePlayer = transform.GetChild(7);
        playerImages = new Transform[4];
        playerImages[0] = redPlayer;
        playerImages[1] = yellowPlayer;
        playerImages[2] = greenPlayer;
        playerImages[3] = bluePlayer;

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
            for (int i=0; i<lines.Length; i++)
            {
                playerImages[i].GetComponent<SpriteRenderer>().sprite = playerSprite;
                playerImages[i].transform.position = lines[i].GetPosition(lines[i].positionCount - 1);
            }
        }
        else
        {
            foreach (LineRenderer lr in lines)
                lr.enabled = false;
            GetComponent<SpriteRenderer>().sprite = prismSprite;
            foreach (Transform t in playerImages)
                t.GetComponent<SpriteRenderer>().sprite = emptySprite;
        }
    }

    // Spawn colored copies of the player
    void Refract()
    {

    }
}

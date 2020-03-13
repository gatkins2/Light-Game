using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prism : MonoBehaviour
{
    [SerializeField]
    Sprite prismSprite, activePrismSprite, playerSprite, emptySprite;

    [SerializeField]
    GameObject player;

    ColorPointer redLine, yellowLine, greenLine, blueLine;
    ColorPointer[] lines;

    Transform redPlayer, yellowPlayer, greenPlayer, bluePlayer;
    Transform[] playerImages;

    Vector2[] points;

    bool validRefract;

    public int FrameBuffer { private get; set; }
    public bool Active { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        // Get color pointers
        redLine = transform.GetChild(0).GetComponent<ColorPointer>();
        yellowLine = transform.GetChild(1).GetComponent<ColorPointer>();
        greenLine = transform.GetChild(2).GetComponent<ColorPointer>();
        blueLine = transform.GetChild(3).GetComponent<ColorPointer>();
        lines = new ColorPointer[4];
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

        // Scale colored images to 1
        foreach (Transform image in playerImages)
            image.localScale = new Vector3(1 / transform.localScale.x, 1 / transform.localScale.y, transform.localScale.z);

        FrameBuffer = 0;
        Active = false;
        validRefract = false;
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
            // Activate pointers and sprites
            GetComponent<SpriteRenderer>().sprite = activePrismSprite;
            foreach (ColorPointer line in lines)
                line.Active = true;
            for (int i=0; i<lines.Length; i++)
            {
                if (lines[i].FinalObject != null && lines[i].FinalObject.tag == "AttachableSurface")
                {
                    playerImages[i].transform.position = lines[i].TeleportPoint;
                    playerImages[i].GetComponent<SpriteRenderer>().enabled = true;
                    if (lines[i].FinalObject != null)
                        playerImages[i].transform.up = lines[i].ObjectNormal;
                }
                else
                    playerImages[i].GetComponent<SpriteRenderer>().enabled = false;
            }
        }
        else
        {
            // Deactivate pointers and sprites
            GetComponent<SpriteRenderer>().sprite = prismSprite;
            foreach (ColorPointer line in lines)
                line.Active = false;
            foreach (Transform t in playerImages)
                t.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    // Spawn colored copies of the player
    public bool SplitRefract()
    {
        // Check if all colors can spawn
        validRefract = true;
        foreach (Transform image in playerImages)
        {
            if (!image.GetComponent<SpriteRenderer>().enabled)
                validRefract = false;
        }

        // Create player objects at each image
        if (validRefract)
        {
            for (int i=0; i<playerImages.Length; i++)
            {
                GameObject colorPlayer = GameObject.Instantiate(player, playerImages[i].transform.position, playerImages[i].rotation);
                GameObject colorPointer = colorPlayer.transform.GetChild(0).GetChild(0).gameObject;
                Destroy(colorPointer.GetComponent<Pointer>());
                colorPlayer.GetComponent<PlayerMove>().pointer = colorPointer.AddComponent<ColorPointer>();
                colorPlayer.GetComponent<SpriteRenderer>().color = playerImages[i].GetComponent<SpriteRenderer>().color;
                colorPointer.GetComponent<ColorPointer>().normalPointer = lines[i].GetComponent<LineRenderer>().material;
                colorPointer.GetComponent<ColorPointer>().maxRays = 10;
                colorPointer.GetComponent<ColorPointer>().maxLength = 1000;
            }

            return true;
        }

        return false;
    }

    // TODO: Return colors to a single white player object
    public bool CombineRefract()
    {
        return false;
    }
}

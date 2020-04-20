using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    public GameObject BlackHoleExit;                // Other black hole to travel to
    public Dictionary<PlayerColor, Pointer> Pointers { get; set; }    // The pointers attached to the black hole
    public Dictionary<PlayerColor, int> FrameBuffers { get; set; }    // Farme buffers for activating each pointer

    Transform sprite;                               // Black hole sprite to rotate

    // Start is called before the first frame update
    void Start()
    {
        // Get sprite and pointers
        sprite = transform.GetChild(0);
        Pointers = new Dictionary<PlayerColor, Pointer>();

        // Load dictionary
        Pointers.Add(PlayerColor.WHITE, transform.GetChild(1).GetComponent<Pointer>());
        Pointers.Add(PlayerColor.RED, transform.GetChild(2).GetComponent<ColorPointer>());
        Pointers.Add(PlayerColor.YELLOW, transform.GetChild(3).GetComponent<ColorPointer>());
        Pointers.Add(PlayerColor.GREEN, transform.GetChild(4).GetComponent<ColorPointer>());
        Pointers.Add(PlayerColor.BLUE, transform.GetChild(5).GetComponent<ColorPointer>());

        // Initialize frame buffers
        FrameBuffers = new Dictionary<PlayerColor, int>();
        FrameBuffers.Add(PlayerColor.WHITE, 0);
        FrameBuffers.Add(PlayerColor.RED, 0);
        FrameBuffers.Add(PlayerColor.YELLOW, 0);
        FrameBuffers.Add(PlayerColor.GREEN, 0);
        FrameBuffers.Add(PlayerColor.BLUE, 0);
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate sprite
        sprite.RotateAround(transform.position, Vector3.forward, -18f * Time.deltaTime);

        // Set activity
        foreach (KeyValuePair<PlayerColor, Pointer> entry in Pointers)
        {
            if (FrameBuffers[entry.Key] > 0)
                FrameBuffers[entry.Key]--;
            else
                entry.Value.Active = false;
        }
    }
}

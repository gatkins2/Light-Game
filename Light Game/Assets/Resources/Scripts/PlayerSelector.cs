using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelector : MonoBehaviour
{
    public Pointer[] playerPointers;
    public bool PlayerRefracted { get; set; }

    public enum ColorPlayer
    {
        RED = 0,
        YELLOW = 1,
        GREEN = 2,
        BLUE = 3,
        ALL = 4
    }
    public ColorPlayer activePlayer { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        activePlayer = ColorPlayer.RED;
        PlayerRefracted = false;
    }

    // Update is called once per frame
    void Update()
    {
        // On right click move to next player in the cycle
        if (Input.GetMouseButtonDown(1) && PlayerRefracted)
        {
            switch (activePlayer)
            {
                case ColorPlayer.RED:
                case ColorPlayer.YELLOW:
                case ColorPlayer.GREEN:
                    playerPointers[(int)activePlayer].Active = false;
                    activePlayer += 1;
                    playerPointers[(int)activePlayer].Active = true;
                    break;
                case ColorPlayer.BLUE:
                    activePlayer += 1;
                    foreach (Pointer pointer in playerPointers)
                        pointer.Active = true;
                    break;
                case ColorPlayer.ALL:
                    activePlayer = ColorPlayer.RED;
                    foreach (Pointer pointer in playerPointers)
                        pointer.Active = false;
                    playerPointers[(int)activePlayer].Active = true;
                    break;
            }
        }
    }
}

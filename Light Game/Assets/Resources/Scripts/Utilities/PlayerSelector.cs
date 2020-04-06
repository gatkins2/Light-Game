using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelector : MonoBehaviour
{
    public Pointer[] playerPointers;
    public bool PlayerRefracted { get; set; }

    public PlayerColor activePlayer { get; set; }

    RoomController rc;

    // Start is called before the first frame update
    void Start()
    {
        activePlayer = PlayerColor.RED;
        PlayerRefracted = false;
        rc = GetComponent<RoomController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Move to player room if not in it
        //if (!PlayerRefracted && !rc.travelling)
        //{
        //    GameObject player = GameObject.FindGameObjectWithTag("Player");
        //    if (rc.GetRoomFromPoint(transform.position) != rc.GetRoomFromPoint(player.transform.position))
        //        rc.ChangeRoom(player.transform.position);
        //}

        // On right click move to next player in the cycle
        if (Input.GetMouseButtonDown(1) && PlayerRefracted)
        {
            switch (activePlayer)
            {
                case PlayerColor.RED:
                case PlayerColor.YELLOW:
                case PlayerColor.GREEN:
                    playerPointers[(int)activePlayer].Active = false;
                    activePlayer += 1;
                    playerPointers[(int)activePlayer].Active = true;
                    break;
                case PlayerColor.BLUE:

                    // If all colored players in the same room
                    if (rc.GetObjectRoom(playerPointers[0].gameObject) == rc.GetObjectRoom(playerPointers[1].gameObject) &&
                        rc.GetObjectRoom(playerPointers[0].gameObject) == rc.GetObjectRoom(playerPointers[2].gameObject) &&
                        rc.GetObjectRoom(playerPointers[0].gameObject) == rc.GetObjectRoom(playerPointers[3].gameObject))
                    {
                        activePlayer = PlayerColor.ALL;
                        foreach (Pointer pointer in playerPointers)
                            pointer.Active = true;
                    }

                    // Otherwise, skip to red
                    else
                    {
                        playerPointers[(int)activePlayer].Active = false;
                        activePlayer = PlayerColor.RED;
                        playerPointers[(int)activePlayer].Active = true;
                    }

                    break;
                case PlayerColor.ALL:
                    activePlayer = PlayerColor.RED;
                    foreach (Pointer pointer in playerPointers)
                        pointer.Active = false;
                    playerPointers[(int)activePlayer].Active = true;
                    break;
            }

            // Move camera to active player
            if (activePlayer != PlayerColor.ALL)
                GetComponent<RoomController>().ChangeRoom(playerPointers[(int)activePlayer].transform.position);
        }
    }
}

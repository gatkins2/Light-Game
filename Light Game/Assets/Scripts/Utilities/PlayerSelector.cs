using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelector : MonoBehaviour
{
    public Pointer[] playerPointers;
    public bool PlayerRefracted { get; set; }

    public PlayerColor activePlayer { get; set; }

    RoomController rc;

    [SerializeField]
    Canvas playerSelectGUI;
    GameObject[] activeSelectors;

    // Start is called before the first frame update
    void Start()
    {
        // Set initial state
        activePlayer = PlayerColor.RED;
        PlayerRefracted = false;
        rc = GetComponent<RoomController>();

        // Initialize active selectors
        activeSelectors = new GameObject[4];
        for (int i=0; i<activeSelectors.Length; i++)
            activeSelectors[i] = playerSelectGUI.transform.GetChild(i + activeSelectors.Length).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        // On right arrow move to next player in the cycle
        if (Input.GetKeyDown(KeyCode.RightArrow) && PlayerRefracted)
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
                    if (PlayersInSameRoom())
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

            MoveCameraToActivePlayer();
        }

        // On left arrow move to previous player in the cycle
        // On right arrow move to next player in the cycle
        if (Input.GetKeyDown(KeyCode.LeftArrow) && PlayerRefracted)
        {
            switch (activePlayer)
            {
                
                case PlayerColor.YELLOW:
                case PlayerColor.GREEN:
                case PlayerColor.BLUE:
                    playerPointers[(int)activePlayer].Active = false;
                    activePlayer -= 1;
                    playerPointers[(int)activePlayer].Active = true;
                    break;
                case PlayerColor.RED:
                    // If all colored players in the same room
                    if (PlayersInSameRoom())
                    {
                        activePlayer = PlayerColor.ALL;
                        foreach (Pointer pointer in playerPointers)
                            pointer.Active = true;
                    }

                    // Otherwise, skip to blue
                    else
                    {
                        playerPointers[(int)activePlayer].Active = false;
                        activePlayer = PlayerColor.BLUE;
                        playerPointers[(int)activePlayer].Active = true;
                    }

                    break;
                case PlayerColor.ALL:
                    activePlayer = PlayerColor.BLUE;
                    foreach (Pointer pointer in playerPointers)
                        pointer.Active = false;
                    playerPointers[(int)activePlayer].Active = true;
                    break;
            }

            MoveCameraToActivePlayer();
        }

        // Update GUI
        if (PlayerRefracted)
        {
            playerSelectGUI.gameObject.SetActive(true);
            if (activePlayer == PlayerColor.ALL)
            {
                foreach (GameObject selector in activeSelectors)
                    if (selector != null)
                        selector.SetActive(true);
            }
            else
            {
                foreach (GameObject selector in activeSelectors)
                    if (selector != null)
                        selector.SetActive(false);
                if (activeSelectors[(int)activePlayer] != null)
                    activeSelectors[(int)activePlayer].SetActive(true);
            }
        }
        else
        {
            playerSelectGUI.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Checks if all color players are in the same room
    /// </summary>
    /// <returns></returns>
    public bool PlayersInSameRoom()
    {
        return rc.GetObjectRoom(playerPointers[0].gameObject) == rc.GetObjectRoom(playerPointers[1].gameObject) &&
               rc.GetObjectRoom(playerPointers[0].gameObject) == rc.GetObjectRoom(playerPointers[2].gameObject) &&
               rc.GetObjectRoom(playerPointers[0].gameObject) == rc.GetObjectRoom(playerPointers[3].gameObject);
    }

    /// <summary>
    /// Moves to the room with the active color player
    /// </summary>
    public void MoveCameraToActivePlayer()
    {
        // Move camera to active player
        if (activePlayer != PlayerColor.ALL)
            GetComponent<RoomController>().ChangeRoom(playerPointers[(int)activePlayer].transform.position);
    }
}

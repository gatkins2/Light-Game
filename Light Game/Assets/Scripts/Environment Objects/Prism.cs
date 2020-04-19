using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prism : MonoBehaviour
{
    [SerializeField]
    Sprite prismSprite, activePrismSprite;                          // Active / inactive sprites

    [SerializeField]
    GameObject player;                                              // Player prefab

    ColorPointer redLine, yellowLine, greenLine, blueLine;          // Lines that show split refract destinations
    ColorPointer[] lines;

    Pointer whiteLine;                                              // Line that shows combine refract desitnation

    Transform redPlayer, yellowPlayer, greenPlayer, bluePlayer, whitePlayer;     // Colored player images
    List<Transform> playerImages;

    Material[] colorPointerMaterials;                               // Array of materials to give color players

    bool validRefract;                                              // Whether the player is able to refract

    public List<ColorPointer> LinesToCombine;                       // List of colored lines to comine on combine refract
    public int FrameBuffer { private get; set; }                    // Frame buffer for setting activity
    public bool Active { get; set; }                                // Whether the prism is active

    // Start is called before the first frame update
    void Start()
    {
        // Set color pointer materials
        colorPointerMaterials = new Material[4];
        colorPointerMaterials[0] = (Material)Resources.Load("Materials/RedLight");
        colorPointerMaterials[1] = (Material)Resources.Load("Materials/YellowLight");
        colorPointerMaterials[2] = (Material)Resources.Load("Materials/GreenLight");
        colorPointerMaterials[3] = (Material)Resources.Load("Materials/BlueLight");

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

        // Get white pointer
        whiteLine = transform.GetChild(4).GetComponent<Pointer>();

        // Set pointers to inactive
        foreach (ColorPointer line in lines)
            line.Active = false;
        whiteLine.Active = false;

        // Set colored player sprites
        redPlayer = transform.GetChild(5);
        yellowPlayer = transform.GetChild(6);
        greenPlayer = transform.GetChild(7);
        bluePlayer = transform.GetChild(8);
        whitePlayer = transform.GetChild(9);
        playerImages = new List<Transform>();
        playerImages.Add(redPlayer);
        playerImages.Add(yellowPlayer);
        playerImages.Add(greenPlayer);
        playerImages.Add(bluePlayer);

        // Scale images to 1
        foreach (Transform image in playerImages)
            image.localScale = new Vector3(1 / transform.localScale.x, 1 / transform.localScale.y, transform.localScale.z);
        whitePlayer.localScale = new Vector3(1 / transform.localScale.x, 1 / transform.localScale.y, transform.localScale.z);

        // Set default vars
        FrameBuffer = 0;
        Active = false;
        validRefract = false;
        LinesToCombine = new List<ColorPointer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Deactivate when not being pointed at
        if (FrameBuffer > 0)
            FrameBuffer--;
        else if (LinesToCombine.Count < 4)
        {
            Active = false;
        }

        // Active when all 4 color pointers are pointing at prism
        else
        {
            Active = true;
        }

        if (Active)
        {
            // Set sprite to active
            GetComponent<SpriteRenderer>().sprite = activePrismSprite;

            // Activate white line
            if (Camera.main.GetComponent<PlayerSelector>().PlayerRefracted && LinesToCombine.Count >= 4)
            {
                // Calculate line start position
                whiteLine.SetStartPoint(transform.position);
                Vector3 point = transform.position;
                Vector3 direction = Vector3.zero;
                foreach (ColorPointer line in LinesToCombine)
                    direction += (Vector3)(line.path[line.path.Count - 1] - line.path[line.path.Count - 2]).normalized;

                if (direction != Vector3.zero)
                {
                    // Get out of prism
                    while (GetComponent<PolygonCollider2D>().OverlapPoint(point))
                    {
                        point += direction.normalized * 0.01f;
                    }
                    whiteLine.SetStartPoint(point);

                    // Set refracted angle
                    direction = Quaternion.AngleAxis(20, Vector3.forward) * direction;
                    whiteLine.transform.right = direction;

                    whiteLine.Active = true;

                    // Activate white player image
                    if (whiteLine.FinalObject != null && whiteLine.FinalObject.tag == "AttachableSurface")
                    {
                        whitePlayer.transform.position = whiteLine.TeleportPoint;
                        whitePlayer.transform.up = whiteLine.ObjectNormal;

                        // Check collision
                        bool noCollision = true;
                        float halfWidth = whitePlayer.GetComponent<BoxCollider2D>().size.x / 2;
                        float halfHeight = whitePlayer.GetComponent<BoxCollider2D>().size.y / 2;
                        Vector2 startPoint = whitePlayer.transform.position + (transform.up * halfHeight);
                        RaycastHit2D hit = Physics2D.Raycast(startPoint, whitePlayer.transform.right, halfWidth);
                        RaycastHit2D hit2 = Physics2D.Raycast(startPoint, -whitePlayer.transform.right, halfWidth);
                        RaycastHit2D hit3 = Physics2D.Raycast(startPoint, whitePlayer.transform.up, halfHeight);
                        if (hit.collider != null || hit2.collider != null || hit3.collider != null)
                            noCollision = false;

                        if (noCollision)
                            whitePlayer.GetComponent<SpriteRenderer>().enabled = true;
                    }
                    else
                        whitePlayer.GetComponent<SpriteRenderer>().enabled = false;
                }
                else
                    whitePlayer.GetComponent<SpriteRenderer>().enabled = false;
            }

            // Activate colored lines
            else
            {
                foreach (ColorPointer line in lines)
                    line.Active = true;
                
                // Activate colored player images
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].FinalObject != null && lines[i].FinalObject.tag == "AttachableSurface")
                    {
                        playerImages[i].transform.position = lines[i].TeleportPoint;
                        playerImages[i].transform.up = lines[i].ObjectNormal;

                        // Check collision
                        bool noCollision = false;
                        float halfWidth = playerImages[i].GetComponent<BoxCollider2D>().size.x / 2;
                        float halfHeight = playerImages[i].GetComponent<BoxCollider2D>().size.y / 2;
                        Vector2 startPoint = playerImages[i].transform.position + (transform.up * halfHeight);
                        RaycastHit2D hit = Physics2D.Raycast(startPoint, playerImages[i].transform.right, halfWidth);
                        RaycastHit2D hit2 = Physics2D.Raycast(startPoint, -playerImages[i].transform.right, halfWidth);
                        RaycastHit2D hit3 = Physics2D.Raycast(startPoint, playerImages[i].transform.up, halfHeight);
                        if ((hit.collider == null || playerImages.Contains(hit.transform)) &&
                            (hit2.collider == null || playerImages.Contains(hit2.transform)) &&
                            (hit3.collider == null || playerImages.Contains(hit3.transform)))
                            noCollision = true;
                            
                        // Enable player image
                        if (noCollision)
                            playerImages[i].GetComponent<SpriteRenderer>().enabled = true;
                        else
                            playerImages[i].GetComponent<SpriteRenderer>().enabled = false;
                    }
                    else
                        playerImages[i].GetComponent<SpriteRenderer>().enabled = false;
                }
            }
        }
        else
        {
            // Deactivate pointers and sprites
            GetComponent<SpriteRenderer>().sprite = prismSprite;
            foreach (ColorPointer line in lines)
                line.Active = false;
            whiteLine.Active = false;
            foreach (Transform t in playerImages)
                t.GetComponent<SpriteRenderer>().enabled = false;
            whitePlayer.GetComponent<SpriteRenderer>().enabled = false;
        }

        // Clear lines to combine
        LinesToCombine.Clear();
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

        if (validRefract)
        {
            // Create player objects at each image
            Camera.main.GetComponent<PlayerSelector>().playerPointers = new Pointer[4];
            for (int i=0; i<playerImages.Count; i++)
            {
                GameObject colorPlayer = GameObject.Instantiate(player, playerImages[i].transform.position, playerImages[i].rotation);
                GameObject colorPointer = colorPlayer.transform.GetChild(0).GetChild(0).gameObject;
                Destroy(colorPointer.GetComponent<Pointer>());
                colorPlayer.GetComponent<PlayerMove>().pointer = colorPointer.AddComponent<ColorPointer>();
                colorPlayer.GetComponent<SpriteRenderer>().color = playerImages[i].GetComponent<SpriteRenderer>().color;
                colorPointer.GetComponent<ColorPointer>().normalPointer = colorPointerMaterials[i];
                colorPointer.GetComponent<ColorPointer>().errorPointer = (Material)Resources.Load("Materials/ErrorLight");
                colorPointer.GetComponent<ColorPointer>().maxRays = 10;
                colorPointer.GetComponent<ColorPointer>().maxLength = 1000;
                colorPointer.GetComponent<ColorPointer>().color = (PlayerColor)i;
                Camera.main.GetComponent<PlayerSelector>().playerPointers[i] = colorPointer.GetComponent<ColorPointer>();
                if (i != 0)
                    colorPointer.GetComponent<ColorPointer>().Active = false;
            }

            // Dsiable colored lines
            foreach (ColorPointer p in lines)
                p.Active = false;

            // Show arrows popup
            if (!PopUpManager.arrowsPopupShown)
                Camera.main.GetComponent<PopUpManager>().ArrowsPopup(gameObject);

            return true;
        }

        return false;
    }

    // Return colors to a single white player object
    public bool CombineRefract()
    {
        if (whitePlayer.GetComponent<SpriteRenderer>().enabled)
        {
            // Delete colored player objects
            PlayerMove[] coloredPlayers = FindObjectsOfType<PlayerMove>();
            for (int i = 0; i < coloredPlayers.Length; i++)
                Destroy(coloredPlayers[i].gameObject);

            // Create player object at image
            GameObject newPlayer = GameObject.Instantiate(player, whitePlayer.transform.position, whitePlayer.rotation);
            newPlayer.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Pointer>().color = PlayerColor.WHITE;

            // Disable white line
            whiteLine.Active = false;

            // Reset first colored player selected to RED
            Camera.main.GetComponent<PlayerSelector>().activePlayer = PlayerColor.RED;

            return true;
        }

        return false;
    }
}

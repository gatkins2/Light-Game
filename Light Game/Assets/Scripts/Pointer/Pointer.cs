using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Pointer : MonoBehaviour
{
    #region Fields

    public int maxRays;
    public float maxLength;

    public Material normalPointer, errorPointer;

    LineRenderer lr;
    protected Ray ray;
    protected RaycastHit2D hit;
    protected int colorFrameBuffer;
    protected Transform player;

    #endregion

    #region Properties

    public bool Active { get; set; }                      // Whether the pointer is shown
    public Vector2 TeleportPoint { get; protected set; }  // Point to teleport to
    public Transform FinalObject { get; protected set; }  // Final object hit by the pointer
    public Vector2 ObjectNormal { get; protected set; }   // The normal of the final objects surface that the pointer collided with
    public List<Vector2> path { get; protected set; }     // The set of vectors that make the pointer path
    public PlayerColor color;                             // The color of the pointer

    #endregion

    // Use this for initialization
    protected void Awake()
    {
        if (transform.parent.GetComponent<Prism>() != null ||
            transform.parent.GetComponent<BlackHole>() != null)
            player = transform.parent;
        
        else
            player = transform.parent.parent;
        lr = GetComponent<LineRenderer>();
        FinalObject = null;
        colorFrameBuffer = 0;
        Active = true;
    }

    // Update is called once per frame
    protected void Update()
    {
        // Deactivate if game is paused
        if (PauseMenu.GameIsPaused)
            Active = false;

        if (Active)
        {
            // Enable line renderer
            lr.enabled = true;

            // Update line color
            if (colorFrameBuffer > 0)
                colorFrameBuffer--;
            else
                lr.material = normalPointer;

            // Set LightRenderer initial values
            lr.positionCount = 1;
            lr.SetPosition(0, transform.position);

            // Send out initial ray
            ray = new Ray(transform.position, transform.right);
            hit = Physics2D.Raycast(ray.origin, ray.direction, maxLength);
            path = new List<Vector2>();
            path.Add(ray.origin);
            int numRays = 1;
            if (hit.collider != null)
                path.Add(hit.point);

            // Render first line
            AddLineRender();

            // While reflecting or passing through objects
            while (hit.collider != null && 
                (hit.collider.tag == "ReflectingSurface" || hit.collider.tag == "RoomChangeBox" ||
                (hit.collider.tag == "Window" && hit.transform.GetComponent<Window>().windowColor == color)
                ) && numRays < maxRays)
            {
                // Pass through room change boxes and windows
                if (hit.collider.tag == "RoomChangeBox" || (hit.collider.tag == "Window" && hit.transform.GetComponent<Window>().windowColor == color))
                    PassThrough();

                // Reflect off of mirrors
                else if (hit.collider.tag == "ReflectingSurface")
                {
                    Reflect();

                    numRays++;
                    if (hit.collider != null)
                        path.Add(hit.point);

                    // Render line
                    AddLineRender();
                }
            }

            // Warp through black holes
            if (hit.collider != null && hit.collider.tag == "BlackHole")
            {
                BlackHoleWarp();

                // Combine paths
                List<Vector2> path2 = hit.transform.GetComponent<BlackHole>().BlackHoleExit.transform.GetChild(1).GetComponent<Pointer>().path;
                if (path2 != null && path != path2)
                    foreach (Vector2 item in path2)
                        path.Add(item);
            }

            // Refract on prisms
            else if (hit.collider != null && hit.collider.tag == "Prism")
                Refract();

            // Set the final teleport point
            SetTeleportPoint();
        }

        // If inactive
        else
        {
            // Set vars to default or null
            TeleportPoint = player.position;
            FinalObject = null;
            path = null;

            // Disable line renderer
            lr.enabled = false;
        }
    }

    // Set the teleport point and final object from hit data
    protected void SetTeleportPoint()
    {
        // If no collision, in room change box, or max rays reached
        if (hit.collider == null || (hit.point - (Vector2)transform.position).magnitude < 0.1 ||
            hit.collider.tag == "RoomChangeBox" || hit.collider.tag == "ReflectingSurface" || hit.collider.tag == "Prism")

            // Set teleport point to same location
            TeleportPoint = player.position;

        // If on an attachable surface
        else if (hit.collider.tag == "AttachableSurface" || hit.collider.tag == "Window")
            TeleportPoint = hit.point;

        // If on a black hole
        else if (hit.collider.tag == "BlackHole")
        {
            Pointer exitPointer = hit.transform.GetComponent<BlackHole>().BlackHoleExit.GetComponent<BlackHole>().Pointer;
            TeleportPoint = exitPointer.TeleportPoint;
            FinalObject = exitPointer.FinalObject;
            ObjectNormal = exitPointer.ObjectNormal;
        }

        // Set final object
        if (hit.collider != null && hit.collider.tag != "BlackHole")
        {
            FinalObject = hit.transform;
            ObjectNormal = hit.normal;
        }
        else if (hit.collider == null || hit.collider.tag != "BlackHole")
            FinalObject = null;
    }

    // Pass a ray through a room change box
    protected void PassThrough()
    {
        // Move into box
        ray.origin = hit.point;
        int tries = 0;
        BoxCollider2D collider = hit.transform.GetComponent<BoxCollider2D>();
        while (!collider.OverlapPoint(ray.origin) && tries < 1000)
        {
            ray.origin += ray.direction * 0.01f;
            tries++;
        }

        // Move out of box
        while (collider.OverlapPoint(ray.origin))
            ray.origin += ray.direction * 0.01f;
        path.Add(ray.origin);

        // Ray continues on same trajectory
        hit = Physics2D.Raycast(ray.origin, ray.direction, maxLength);
        if (hit.collider != null)
            path.Add(hit.point);
        AddLineRender();
    }

    // Warp a ray through a black hole
    protected void BlackHoleWarp()
    {
        // Get direction from center of first black hole to the hit point
        Vector2 directionFromCenter = (hit.point - (Vector2)hit.transform.position).normalized;

        // Add black hole center and blank value to path
        path.Add((Vector2)hit.transform.position);
        path.Add(new Vector2(float.NaN, float.NaN));

        // Get the point on the edge of the 2nd black hole in the same direction
        GameObject exitBlackHole = hit.transform.GetComponent<BlackHole>().BlackHoleExit;
        CircleCollider2D collider = exitBlackHole.GetComponent<CircleCollider2D>();
        Vector2 edgePoint = (Vector2)exitBlackHole.transform.position + directionFromCenter;

        // Add second black hole center to path
        path.Add(exitBlackHole.transform.position);

        // Move into black hole
        ray.origin = edgePoint;
        int tries = 0;
        while (!collider.OverlapPoint(ray.origin) && tries < 1000)
        {
            ray.origin -= (Vector3)(directionFromCenter * 0.01f);
            tries++;
        }

        // Move out of black hole
        while (collider.OverlapPoint(ray.origin))
            ray.origin += ray.direction * 0.01f;

        // Activate black hole
        BlackHole bhScript = exitBlackHole.GetComponent<BlackHole>();
        bhScript.Active = true;
        bhScript.FrameBuffer = 1;

        // Set exit pointer
        Pointer exitPointer = bhScript.Pointer;
        exitPointer.SetStartPoint(ray.origin);
        exitPointer.SetStartDirection(ray.direction);
        exitPointer.normalPointer = normalPointer;
    }

    // Reflect a ray across a reflectable surface
    protected void Reflect()
    {
        // Create a new ray to the next potential location
        ray = new Ray(hit.point, Vector2.Reflect(ray.direction, hit.normal));

        // Get out of last object's collider
        while (hit.transform.GetComponent<BoxCollider2D>().OverlapPoint(ray.origin))
        {
            ray.origin += ray.direction * 0.01f;
        }

        // Set new hit
        hit = Physics2D.Raycast(ray.origin, ray.direction, maxLength);
    }

    // Refract a ray through a prism
    protected virtual void Refract()
    {
        // Send ray through prism
        int tries = 0;
        ray.origin = hit.point;

        // Move ray into prism
        while (!hit.transform.GetComponent<PolygonCollider2D>().OverlapPoint(ray.origin) && tries < 1000)
        {
            ray.origin += ray.direction * 0.01f;
            tries++;
        }

        if (hit.transform.GetComponent<PolygonCollider2D>().OverlapPoint(ray.origin))
        {
            // Set angle inside prism
            Vector3 refractedDirection = Quaternion.AngleAxis(-20, Vector3.forward) * ray.direction;
            ray.direction = refractedDirection;

            // Move ray out of prism
            while (hit.transform.GetComponent<PolygonCollider2D>().OverlapPoint(ray.origin))
            {
                ray.origin += ray.direction * 0.01f;
            }
            path.Add(ray.origin);

            // Render line through prism
            lr.positionCount++;
            lr.SetPosition(lr.positionCount - 1, ray.origin);

            // Activate prism
            hit.transform.GetComponent<Prism>().Active = true;
            hit.transform.GetComponent<Prism>().FrameBuffer = 1;

            // Activate prism lights
            for (int i = 0; i < 4; i++)
            {
                // Set color pointer positions and directions
                ColorPointer colorPointer = hit.transform.GetChild(i).GetComponent<ColorPointer>();
                colorPointer.SetStartPoint(ray.origin);
                Vector3 colorDirection = Quaternion.AngleAxis(-(5 * i), Vector3.forward) * refractedDirection;
                colorPointer.gameObject.transform.right = colorDirection;
            }
        }
    }

    // Adds a portion of the line and renders it
    protected void AddLineRender()
    {
        lr.positionCount++;
        if (hit.collider == null)
            lr.SetPosition(lr.positionCount - 1, ray.origin + (ray.direction * maxLength));
        else
            lr.SetPosition(lr.positionCount - 1, hit.point);
    }

    // Changes the pointer to red when an invalid move is attempted
    public void ErrorFlash()
    {
        lr.material = errorPointer;
        colorFrameBuffer = 60;
    }

    // Set the start point of the line renderer
    public void SetStartPoint(Vector2 point)
    {
        transform.position = point;
    }

    // Set the start direction of the line renderer
    public void SetStartDirection(Vector2 direction)
    {
        transform.right = direction;
    }
}

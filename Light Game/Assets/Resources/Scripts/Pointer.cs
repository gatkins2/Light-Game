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

    #endregion

    #region Properties

    public bool Active { protected get; set; }            // Whether the pointer is shown
    public Vector2 TeleportPoint { get; protected set; }  // Point to teleport to
    public Transform FinalObject { get; protected set; }  // Final object hit by the pointer
    public Vector2 ObjectNormal { get; protected set; }   // The normal of the final objects surface that the pointer collided with
    public List<Vector2> path { get; protected set; }     // The set of vectors that make the pointer path

    #endregion

    // Use this for initialization
    protected void Awake()
    {
        lr = GetComponent<LineRenderer>();
        FinalObject = null;
        colorFrameBuffer = 0;
        Active = true;
        normalPointer = lr.material;
        errorPointer = Resources.Load<Material>("Materials/RedLight");
    }

    // Update is called once per frame
    protected void Update()
    {
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

            // While reflecting
            while (hit.collider != null && (hit.collider.tag == "ReflectingSurface") && numRays < maxRays)
            {
                // Find next point
                if (hit.collider.tag == "ReflectingSurface")
                    Reflect();

                numRays++;
                if (hit.collider != null)
                    path.Add(hit.point);

                // Render line
                AddLineRender();
            }

            // Refract on prisms
            if (hit.collider != null && hit.collider.tag == "Prism")
                Refract();

            // Set the final teleport point
            SetTeleportPoint();
        }

        // If inactive
        else
        {
            // Set vars to default or null
            TeleportPoint = GetComponentInParent<Transform>().GetComponentInParent<Transform>().position;
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
        if (hit.collider == null ||
            hit.collider.tag == "RoomChangeBox" || hit.collider.tag == "BlackHole" ||
            hit.collider.tag == "ReflectingSurface" || hit.collider.tag == "Prism")

            // Set teleport point to same location
            TeleportPoint = GetComponentInParent<Transform>().GetComponentInParent<Transform>().position;

        // If on an attachable surface
        else if (hit.collider.tag == "AttachableSurface")
            TeleportPoint = hit.point;

        // Set final object
        if (hit.collider != null)
        {
            FinalObject = hit.transform;
            ObjectNormal = hit.normal;
        }
        else
            FinalObject = null;
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
}

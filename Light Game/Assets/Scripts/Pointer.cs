using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    #region Fields

    [SerializeField]
    int maxRays;
    [SerializeField]
    float maxLength;
    [SerializeField]
    Material normalPointer, redPointer;

    LineRenderer lr;
    Ray ray;
    RaycastHit2D hit;
    int colorFrameBuffer;

    #endregion

    #region Properties

    public Vector2 TeleportPoint { get; private set; }  // Point to teleport to
    public Transform FinalObject { get; private set; }  // Final object hit by the pointer
    public Vector2 ObjectNormal { get; private set; }   // The normal of the final objects surface that the pointer collided with
    public Vector2[] path { get; private set; }             // The set of vectors that make the pointer path

    #endregion

    // Use this for initialization
    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        FinalObject = null;
        colorFrameBuffer = 0;
    }

    // Update is called once per frame
    void Update()
    {

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
        path = new Vector2[11];
        path[0] = ray.origin;
        int numRays = 1;
        if (hit.collider != null)
            path[numRays] = hit.point;

        // Render first line
        AddLineRender();
        
        // While reflecting or refracting
        while (hit.collider != null && (hit.collider.tag == "ReflectingSurface" || hit.collider.tag == "Prism") && numRays < maxRays)
        {
            // Find next point
            if (hit.collider.tag == "ReflectingSurface")
                Reflect();
            else if (hit.collider.tag == "Prism")
                PrismRefract();

            numRays++;
            if (hit.collider != null)
                path[numRays] = hit.point;

            // Render line
            AddLineRender();
        }

        // Set the final teleport point
        SetTeleportPoint();
    }

    // Set the teleport point and final object from hit data
    void SetTeleportPoint()
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
    void Reflect()
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
    void PrismRefract()
    {
        // Send ray through prism
        int tries = 0;
        ray.origin = hit.point;
        while (!hit.transform.GetComponent<PolygonCollider2D>().OverlapPoint(ray.origin) && tries < 1000)
        {
            ray.origin += ray.direction * 0.01f;
            tries++;
        }
        if (hit.transform.GetComponent<PolygonCollider2D>().OverlapPoint(ray.origin))
        {
            Vector3 refractedDirection = Quaternion.AngleAxis(-20, Vector3.forward) * ray.direction;
            ray.direction = refractedDirection;
            while (hit.transform.GetComponent<PolygonCollider2D>().OverlapPoint(ray.origin))
            {
                ray.origin += ray.direction * 0.01f;
            }

            // Render line through prism
            lr.positionCount++;
            lr.SetPosition(lr.positionCount - 1, ray.origin);

            // Send ray to final point
            ray.direction = Quaternion.AngleAxis(-20, Vector3.forward) * refractedDirection;

            // Active prism lights
            RaycastHit2D colorHit;
            for (int i = 0; i < 4; i++)
            {
                hit.transform.GetComponent<Prism>().Active = true;
                hit.transform.GetComponent<Prism>().FrameBuffer = 1;
                LineRenderer colorLR = hit.transform.GetChild(i).GetComponent<LineRenderer>();
                colorLR.SetPosition(0, ray.origin);
                colorLR.positionCount = 2;
                Vector3 colorDirection = Quaternion.AngleAxis(-(5 * i), Vector3.forward) * refractedDirection;
                Ray colorRay = new Ray(ray.origin, colorDirection);
                colorHit = Physics2D.Raycast(colorRay.origin, colorDirection);
                colorLR.SetPosition(1, colorHit.point);
            }
        }
        
        hit = Physics2D.Raycast(ray.origin, ray.direction);
    }

    // Adds a portion of the line and renders it
    void AddLineRender()
    {
        lr.positionCount++;
        if (hit.collider == null)
            lr.SetPosition(lr.positionCount - 1, ray.origin + (ray.direction * maxLength));
        else
            lr.SetPosition(lr.positionCount - 1, hit.point);
    }

    // Changes the pointer to red when an invalid move is attempted
    public void FlashRed()
    {
        lr.material = redPointer;
        colorFrameBuffer = 60;
    }
}

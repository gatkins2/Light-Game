﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    #region Fields

    [SerializeField]
    int maxRays;
    [SerializeField]
    float maxLength;

    LineRenderer lr;
    Ray ray;
    RaycastHit2D hit;

    #endregion

    #region Properties

    public Vector2 TeleportPoint { get; private set; }  // Point to teleport to
    public Transform FinalObject { get; private set; }  // Final object hit by the pointer

    #endregion

    // Use this for initialization
    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        FinalObject = null;
    }

    // Update is called once per frame
    void Update()
    {
        // Set LightRenderer initial values
        lr.positionCount = 1;
        lr.SetPosition(0, transform.position);

        // Send out initial ray
        ray = new Ray(transform.position, transform.right);
        hit = Physics2D.Raycast(ray.origin, ray.direction, maxLength);
        int numRays = 1;

        // Render first line
        AddLineRender();
        
        // While reflecting or refracting
        while (hit.collider != null && (hit.collider.tag == "ReflectingSurface" || hit.collider.tag == "Prism") && numRays < maxRays)
        {
            // Find next point
            if (tag == "ReflectingSurface")
                Reflect();
            else if (tag == "Prism")
                PrismRefract();

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
            hit.collider.tag == "RoomChangeBox" ||
            hit.collider.tag == "ReflectingSurface" || hit.collider.tag == "Prism")

            // Set teleport point to same location
            TeleportPoint = GetComponentInParent<Transform>().GetComponentInParent<Transform>().transform.position;

        // If on an attachable surface
        else if (hit.collider.tag == "AttachableSurface")
            TeleportPoint = hit.point;

        // Set final object
        if (hit.collider != null)
            FinalObject = hit.transform;
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
            ray.origin += ray.direction * 0.01f;

        // Set new hit
        hit = Physics2D.Raycast(ray.origin, ray.direction, maxLength);
    }

    // Refract a ray through a prism
    void PrismRefract()
    {

        if (hit.collider != null && hit.transform.tag == "Prism")
        {
            // Send ray through prism
            Vector3 refractedDirection = Quaternion.AngleAxis(-20, Vector3.forward) * ray.direction;
            ray = new Ray(hit.point, refractedDirection);
            while (hit.transform.GetComponent<PolygonCollider2D>().OverlapPoint(ray.origin))
                ray.origin += ray.direction * 0.01f;

            // Render line through prism
            lr.positionCount++;
            lr.SetPosition(lr.positionCount - 1, ray.origin);

            // Send ray to final point
            ray.direction = Quaternion.AngleAxis(-20, Vector3.forward) * refractedDirection;
            hit = Physics2D.Raycast(ray.origin, ray.direction);
            lr.positionCount++;
            lr.SetPosition(lr.positionCount - 1, hit.point);


            ///////////////////////////////////////////////////////////
            /// CODE FOR COLORS
            ///////////////////////////////////////////////////////////
            //if (hit.collider !=null && hit.transform.tag == "Prism")
            //{
            //    // Send ray through prism
            //    Vector3 refractedDirection = Quaternion.AngleAxis(-20, Vector3.forward) * ray.direction;
            //    ray = new Ray(hit.point, refractedDirection);
            //    while(hit.transform.GetComponent<PolygonCollider2D>().OverlapPoint(ray.origin))
            //            ray.origin += ray.direction * 0.01f;

            //    // Active prism lights
            //    RaycastHit2D colorHit;
            //    for (int i=0; i<4; i++)
            //    {
            //        LineRenderer colorLR = hit.transform.GetChild(i).GetComponent<LineRenderer>();
            //        colorLR.SetPosition(0, ray.origin);
            //        colorLR.positionCount = 2;
            //        Vector3 colorDirection = Quaternion.AngleAxis(-(5*i), Vector3.forward) * refractedDirection;
            //        Ray colorRay = new Ray(ray.origin, colorDirection);
            //        colorHit = Physics2D.Raycast(colorRay.origin, colorDirection);
            //        colorLR.SetPosition(1, colorHit.point);
            //    }
            //}
        }
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
}

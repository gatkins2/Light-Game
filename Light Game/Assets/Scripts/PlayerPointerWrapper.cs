using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPointerWrapper : MonoBehaviour
{
	
	// Update is called once per frame
	void Update ()
    {
        // Point from player
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailingLight : MonoBehaviour
{
    Vector2 direction;
    float stepLength;
    int frameBuffer;

    public List<Vector2> path { private get; set; }
    public PlayerMove player { private get; set; }

    // Start is called before the first frame update
    void Start()
    {
        frameBuffer = 0;
        path.RemoveAt(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (frameBuffer > 0)
        {
            transform.position += (Vector3)direction.normalized * stepLength;
            frameBuffer--;
        }

        else
        {
            // If end of path reached
            if (path.Count <= 0)
            {
                player.Travelling = false;
                Destroy(gameObject);
            }

            else
            {
                // Calculate next vector
                direction = path[0] - (Vector2)transform.position;
                path.RemoveAt(0);
                stepLength = direction.magnitude / 10;
                frameBuffer = 10;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }
    }
}

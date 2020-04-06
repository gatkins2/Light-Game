using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailingLight : MonoBehaviour
{
    Vector2 direction;
    float stepLength;

    public List<Vector2> path { private get; set; }
    public PlayerMove player { private get; set; }

    // Start is called before the first frame update
    void Start()
    {
        path.RemoveAt(0);

        // Calculate first vector
        direction = path[0] - (Vector2)transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    // Update is called once per frame
    void Update()
    {
        if (!Camera.main.GetComponent<RoomController>().travelling)
        {
            // If path point reached
            if (((Vector2)transform.position - path[0]).magnitude <= (GameConstants.TrailingLightMoveSpeed * Time.deltaTime))
            {
                // Move to point
                transform.position = path[0];
                path.RemoveAt(0);

                // If end of path reached
                if (path.Count <= 0)
                {
                    player.Enabled = true;
                    Destroy(gameObject);
                }

                else
                {
                    // If vector contains NaN
                    if (float.IsNaN(path[0].x) && float.IsNaN(path[0].y))
                    {
                        // Remove empty point and teleport
                        path.RemoveAt(0);
                        if (path.Count > 0)
                        {
                            transform.position = path[0];
                            path.RemoveAt(0);
                        }
                    }

                    // Calculate next vector
                    direction = path[0] - (Vector2)transform.position;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                }
            }

            else
                // Move towards next point
                transform.position += (Vector3)direction.normalized * GameConstants.TrailingLightMoveSpeed * Time.deltaTime;
        }
    }

    // Called when a collision occurs
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Pause on room change box
        if (collision.gameObject.tag == "RoomChangeBox")
            Camera.main.GetComponent<RoomController>().ChangeRoom(path[path.Count - 1]);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public GameObject player;

    bool updating;
    int updateFrame;
    Vector2 travelDistance;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 position = player.transform.position;
        position.z = transform.position.z;
        transform.position = position;
        updating = false;
        updateFrame = 1;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = transform.position;
        position.z = player.transform.position.z;
        if (position != player.transform.position && !updating)
        {
            updating = true;
            travelDistance = player.transform.position - position;
            travelDistance /= 15;
        }

        else if (updating)
        {
            if (updateFrame < 15)
                transform.position += (Vector3)travelDistance;
            else
            {
                position = player.transform.position;
                position.z = transform.position.z;
                transform.position = position;
                updateFrame = 0;
                updating = false;
            }
            updateFrame++;
        }
    }
}

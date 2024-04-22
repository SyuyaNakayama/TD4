using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : LiveEntity
{
    protected override void LiveEntityUpdate()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            movement += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            movement += new Vector3(0, 0, -1);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            movement += new Vector3(1, 0, 0);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            movement += new Vector3(-1, 0, 0);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            movement = new Vector3(movement.x, 10, movement.z);
        }
    }
}
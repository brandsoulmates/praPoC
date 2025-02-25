﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clickMotion : MonoBehaviour
{
    Vector3 startPos;
    bool movingF;
    bool movingB;
    public float moveSpeed;
    public float moveDist;


    // Use this for initialization
    void Start()
    {
        startPos = transform.localPosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (movingF)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + moveSpeed);
            if(transform.localPosition.z >= startPos.z + moveDist)
            {
                movingF = false;
                movingB = true;
            }
        }

        if (movingB)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z - moveSpeed);
            if (transform.localPosition.z <= startPos.z)
            {
                transform.localPosition = startPos;
                movingF = false;
                movingB = false;

                gameObject.SendMessage("finishClick", SendMessageOptions.DontRequireReceiver);
            }
        }

    }

    public void click()
    {
        movingF = true;
    }


}

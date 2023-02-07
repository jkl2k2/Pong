using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleController : MonoBehaviour
{
    public string axis;
    private void FixedUpdate()
    {
        float verticalValue = Input.GetAxis(axis);
        
        Vector3 force = Vector3.up * verticalValue * 50f;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(force, ForceMode.Force);

        if (rb.position.y >= 25)
        {
            rb.position = new Vector3(rb.position.x, 24.9f, 0f);
        }

        if (rb.position.y <= 10)
        {
            rb.position = new Vector3(rb.position.x, 9.9f, 0f);
        }
    }
}
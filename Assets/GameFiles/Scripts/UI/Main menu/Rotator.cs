﻿using UnityEngine;

public class Rotator : MonoBehaviour
{

    void Update()
    {
        transform.Rotate(new Vector3(0, -0.1f, 0), Space.Self);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerCollision : MonoBehaviour
{
    internal PlayerManager manager;

    public static event Action boatReached;

    private void Awake()
    {
        manager = GetComponent<PlayerManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.tag == "boat")
        {
            boatReached();
        }
    }
}

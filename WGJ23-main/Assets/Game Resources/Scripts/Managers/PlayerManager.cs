using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    internal PlayerInput input;
    internal PlayerMovement movement;
    internal PlayerCollision collision;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        movement = GetComponent<PlayerMovement>();
    }
    void Update()
    {
        
    }
}

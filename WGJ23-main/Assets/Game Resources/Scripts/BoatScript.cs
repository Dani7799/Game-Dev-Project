using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BoatScript : MonoBehaviour
{
    private GameObject player;

    int travelDistance = 10;
    int travelDuration = 2;

    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>().gameObject;
    }
    private void OnEnable()
    {
        PlayerCollision.boatReached += AttachPlayer;
    }


    void AttachPlayer()
    {
        Debug.Log("reached boat");

        player.transform.parent = transform;
        StartCoroutine(MoveBoat());
    }

    IEnumerator MoveBoat()
    {
        Vector3 endPos = transform.position + (transform.forward * travelDistance);

        Debug.Log("End Pos = " + endPos);
        Debug.DrawLine(transform.position, endPos, Color.red, 5f);
        yield return new WaitForSeconds(0.5f);

        transform.DOMove(endPos, travelDuration, false);
    }

    private void OnDisable()
    {
        PlayerCollision.boatReached -= AttachPlayer;
    }
}

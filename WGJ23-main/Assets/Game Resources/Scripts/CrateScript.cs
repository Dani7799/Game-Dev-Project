using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateScript : MonoBehaviour
{

    public static event Action cratePressedA, cratePressedB;
    public static event Action<char> HoldingButton, ReleasedButton;


    private WallButton buttonHeld;

    private void Start()
    {
        PlayerMovement.cratePushed += MoveCrate;
        buttonHeld = null;
    }

    private void OnDisable()
    {
        PlayerMovement.cratePushed -= MoveCrate;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "button")
        {
            buttonHeld = other.GetComponent<WallButton>();
            HoldingButton?.Invoke(buttonHeld.type);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "button")
        {
            ReleasedButton?.Invoke(buttonHeld.type);
            buttonHeld = null;
        }
    }


    
    void MoveCrate(Vector3 rayDir, Vector3 endPosOffset, GameObject hitObj)
    {
        if (hitObj != this.gameObject)
            return;
        //AudioManager.instance.Play("CrateSlide");


        delayExecution(0.1f);

        RaycastHit hit;
        Debug.Log(endPosOffset.magnitude);
        float offset = endPosOffset.magnitude;

        if (Physics.Raycast(transform.position, rayDir, out hit, Mathf.Infinity))
        {

            Debug.Log("CRATE DISTANCE: " + hit.distance);
            if (hit.distance >= 0.6f)
                AudioManager.instance.Play("CrateSlide");

            float hitDistance = 0f;

            Debug.DrawRay(transform.position, Vector3.forward * hit.distance, Color.red);

            bool detectAgain;

            do
            {

                detectAgain = false;
                hitDistance += hit.distance;

                Vector3 endPosition = transform.position;
                if (rayDir == Vector3.forward || rayDir == Vector3.back)
                {
                    endPosition = new Vector3(transform.position.x, transform.position.y, hit.transform.position.z);
                }
                else
                {
                    endPosition = new Vector3(hit.transform.position.x, transform.position.y, transform.position.z);
                }

                Debug.Log("crate ray detects: " + hit.transform.tag + "\n" + "end Pos: " + endPosition + endPosOffset);

                if (hit.transform.tag == "wall" || hit.transform.tag == "obstacleWall")
                {
                    transform.DOMove(endPosition + endPosOffset, 0.03f * (hitDistance - offset));
                }
                else if (hit.transform.tag == "boat")
                {
                    //stop in one box bahind
                    transform.DOMove(endPosition + (endPosOffset * 2), 0.03f * (hitDistance - offset));
                }
                else if (hit.transform.tag == "crate")
                {
                    transform.DOMove(endPosition + endPosOffset, 0.03f * (hitDistance - offset));
                }
                else if (hit.transform.tag == "button")
                {
                    //enable walls here
                    if (hit.transform.GetComponent<WallButton>().type == 'A')
                    {
                        cratePressedA?.Invoke();
                    }
                    else if (hit.transform.GetComponent<WallButton>().type == 'B')
                    {
                        cratePressedB?.Invoke();
                    }

                    if (Physics.Raycast(endPosition, rayDir, out hit, Mathf.Infinity))
                    {
                        detectAgain = true;
                    }
                }
            } while (detectAgain);
        }
    }

    IEnumerator delayExecution(float time)
    {
        yield return new WaitForSeconds(time);
    }


}

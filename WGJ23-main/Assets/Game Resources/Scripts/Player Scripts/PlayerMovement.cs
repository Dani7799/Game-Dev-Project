using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Unity.Burst.CompilerServices;
using QuizCannersUtilities;

public class PlayerMovement : MonoBehaviour
{
    public static event Action buttonPressedA, buttonPressedB;
    public static event Action<Vector3, Vector3, GameObject> cratePushed;

    internal PlayerManager manager;
    [SerializeField]
    private float offset;
    [SerializeField]
    private GameObject SaiyanParticles;

    //animation vars
    private bool isMoving;
    Vector3 posReference;
    [SerializeField]
    private Animator animator;

    private void Awake()
    {
        manager = GetComponent<PlayerManager>();
    }
    private void Start()
    {
        PlayerInput.swiped += MovePlayer;
        PlayerCollision.boatReached += doJump;

        posReference = transform.position;
    }
    private void OnDisable()
    {
        PlayerInput.swiped -= MovePlayer;
        PlayerCollision.boatReached -= doJump;
    }

    // Update is called once per frame
    void Update()
    {
        movementDetection();
    }

    void MovePlayer()
    {

        if (isMoving)
            return;

        AudioManager.instance.Play("Move");
        Instantiate(SaiyanParticles, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z - 1), Quaternion.identity);

        string dir = manager.input.dir;
        Debug.Log(dir);

        RaycastHit hit;

        Vector3 rayDir = Vector3.zero, endPosOffset = Vector3.zero;

        switch (dir)
        {
            case "up":
                rayDir = Vector3.forward;
                endPosOffset.z = -offset;
                
                //Debug.Log("up");
                break;
            case "down":
                rayDir = Vector3.back;
                endPosOffset.z = offset;
                
                //Debug.Log("down");
                break;
            case "left":
                rayDir = Vector3.left;
                endPosOffset.x = offset;
                
                //Debug.Log("left");
                break;
            case "right":
                rayDir = Vector3.right;
                endPosOffset.x = -offset;
                
                //Debug.Log("right");
                break;
            default:
                break;
        }

        transform.LookAt(transform.position + rayDir);

        if (Physics.Raycast(transform.position, rayDir, out hit, Mathf.Infinity))
        {
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
                Debug.Log(hit.transform.tag);

                if (hit.transform.tag == "wall" || hit.transform.tag == "obstacleWall")
                {
                    transform.DOMove(endPosition + endPosOffset, 0.03f * (hitDistance - offset));
                    Debug.Log(hit.transform.position + "," + endPosOffset);
                }
                else if (hit.transform.tag == "boat")
                {
                    //stop in one box bahind
                    transform.DOMove(endPosition + (endPosOffset * 2), 0.03f * (hitDistance - offset));
                    //make the jump forward
                    doJump();
                    transform.DOMove(endPosition + endPosOffset, 0.4f);
                }
                else if (hit.transform.tag == "crate")
                {
                    cratePushed?.Invoke(rayDir, endPosOffset, hit.transform.gameObject);

                    AudioManager.instance.Play("CrateCollide");
                    if (Physics.Raycast(endPosition, rayDir, out hit, 1))
                    {
                        if(hit.transform.tag != "wall" && hit.transform.tag != "obstacleWall" && hit.transform.tag != "crate")
                        {
                            endPosition -= endPosOffset;
                        }
                    }
                    else
                    {
                        endPosition -= endPosOffset;
                    }

                    float timeDelay = 0.03f * (hitDistance);
                    transform.DOMove(endPosition + endPosOffset, timeDelay);

                    //wait until player reaches the crate to call the cratePushed event and move into its position
                    //StartCoroutine(MoveCrateAfterDelay(rayDir, endPosOffset, hit.transform.gameObject ,timeDelay, endPosition));

                }
                else if (hit.transform.tag == "button")
                {
                    //enable walls here
                    if (hit.transform.GetComponent<WallButton>().type == 'A')
                    {
                        buttonPressedA?.Invoke();
                    }
                    else if (hit.transform.GetComponent<WallButton>().type == 'B')
                    {
                        buttonPressedB?.Invoke();
                    }
                    ///////////////////


                    if (Physics.Raycast(endPosition, rayDir, out hit, Mathf.Infinity))
                    {
                        detectAgain = true;
                    }
                }
            } while (detectAgain);
        }
    }

    void movementDetection()
    {
        if (transform.position != posReference)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        posReference = transform.position;

        animator.SetBool("moving", isMoving);
    }

    void doJump()
    {
        animator.SetTrigger("reached_boat");
    }

    IEnumerator MoveCrateAfterDelay(Vector3 rayDir, Vector3 endPosOffset,GameObject hitObj, float timeDelay, Vector3 endPosition)
    {
        cratePushed?.Invoke(rayDir, endPosOffset, hitObj);
        yield return new WaitForSeconds(timeDelay + 0.1f);

        RaycastHit hit, hit2;

        //checking if the crate has moved into a wall before taking the crate's position
        if (Physics.Raycast(endPosition, rayDir, out hit, Mathf.Infinity))
        {
            Debug.Log("2nd ray for crate detects: " + hit.transform.name);
            Debug.Log("hit.distance = " + hit.distance + ", magnitude = " + (transform.position - endPosition).magnitude);
            if (hit.transform.position != transform.position && hit.transform.tag != "wall")
            {
                //checking if crate was on an obstacle wall before it moved
                if(Physics.Raycast(transform.position, rayDir, out hit2, offset))
                {
                    if(hit2.transform.tag == "obstacleWall")
                    {
                        //checking if there is an empty space in front of obstacle wall
                        if(!Physics.Raycast(hit2.transform.position, rayDir, out hit2, offset)){
                            transform.DOMove(endPosition + (rayDir * offset), 0.15f);
                        }
                    }
                }
                else
                {
                    transform.DOMove(endPosition, 0.3f);
                }
            }
        }
    }
}

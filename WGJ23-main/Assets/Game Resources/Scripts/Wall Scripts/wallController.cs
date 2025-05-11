using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class wallController : MonoBehaviour
{
    public char wallType;
    [SerializeField]
    float offset;

    bool up;

    bool wallsBeingHeld = false;
    char heldWalltype = '\0';

    float startPosY;
    private void Start()
    {
        startPosY = this.transform.GetChild(0).position.y;

        CrateScript.HoldingButton += WallsHeld;
        CrateScript.ReleasedButton += WallsReleased;
        if (wallType == 'A')
        {
            PlayerMovement.buttonPressedA += enableWall;
            PlayerMovement.buttonPressedB += disableWall;
            CrateScript.cratePressedA += enableWall;
            CrateScript.cratePressedB += disableWall;
        }
        else if (wallType == 'B')
        {
            PlayerMovement.buttonPressedB += enableWall;
            PlayerMovement.buttonPressedA += disableWall;
            CrateScript.cratePressedB += enableWall;
            CrateScript.cratePressedA += disableWall;
        }
    }
    private void OnDisable()
    {

        CrateScript.HoldingButton -= WallsHeld;
        CrateScript.ReleasedButton -= WallsReleased;

        if (wallType == 'A')
        {
            PlayerMovement.buttonPressedA -= enableWall;
            PlayerMovement.buttonPressedB -= disableWall;
            CrateScript.cratePressedA -= enableWall;
            CrateScript.cratePressedB -= disableWall;
        }
        else if (wallType == 'B')
        {
            PlayerMovement.buttonPressedB -= enableWall;
            PlayerMovement.buttonPressedA -= disableWall;
            CrateScript.cratePressedB -= enableWall;
            CrateScript.cratePressedA -= disableWall;
        }
    }

    void WallsHeld(char type)
    {
        if(!wallsBeingHeld)
        {
            wallsBeingHeld = true;
            heldWalltype = type;
        }
    }

    void WallsReleased(char type)
    {
        if(wallsBeingHeld && heldWalltype == type)
        {
            wallsBeingHeld = false;
            heldWalltype = '\0';
        }
    }


    void enableWall()
    {
        if (wallsBeingHeld || up)
            return;


        AudioManager.instance.Play("WallRise");
        GetComponent<BoxCollider>().enabled = true;
        //put animation here
        transform.GetChild(0).DOMoveY(transform.GetChild(0).position.y + ((startPosY + offset) - transform.GetChild(0).position.y), 0.2f);

        up = true;
    }

    void disableWall()
    {

        if (wallsBeingHeld || !up)
            return;

        GetComponent<BoxCollider>().enabled = false;
        //put animation here too
        if (up) {
            transform.GetChild(0).DOMoveY(transform.GetChild(0).position.y - offset, 0.2f);
        }
        
        up = false;
    }
}

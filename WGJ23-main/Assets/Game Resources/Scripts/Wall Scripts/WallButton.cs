using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallButton : MonoBehaviour
{
    public char type;

    [SerializeField]
    GameObject emitObj;
    [SerializeField]
    Material normalMaterial;
    [SerializeField]
    Material greyMaterial;

    bool wallsBeingHeld = false;

    MeshRenderer sphereRenderer;

    char typeHeld;

    private void Start()
    {

        sphereRenderer = emitObj.GetComponent<MeshRenderer>();
        CrateScript.HoldingButton += WallsHeld;
        CrateScript.ReleasedButton += WallsReleased;
        if (type == 'A')
        {
            PlayerMovement.buttonPressedA += turnOn;
            PlayerMovement.buttonPressedB += turnOff;
            CrateScript.cratePressedA += turnOn;
            CrateScript.cratePressedB += turnOff;

        }
        else if (type == 'B')
        {
            PlayerMovement.buttonPressedB += turnOn;
            PlayerMovement.buttonPressedA += turnOff;
            CrateScript.cratePressedB += turnOn;
            CrateScript.cratePressedA += turnOff;
        }
    }

    private void OnDisable()
    {

        CrateScript.HoldingButton -= WallsHeld;
        CrateScript.ReleasedButton -= WallsReleased;
        if (type == 'A')
        {
            PlayerMovement.buttonPressedA -= turnOn;
            PlayerMovement.buttonPressedB -= turnOff;
            CrateScript.cratePressedA -= turnOn;
            CrateScript.cratePressedB -= turnOff;
        }
        else if (type == 'B')
        {
            PlayerMovement.buttonPressedB -= turnOn;
            PlayerMovement.buttonPressedA -= turnOff;
            CrateScript.cratePressedB -= turnOn;
            CrateScript.cratePressedA -= turnOff;
        }
    }

    void WallsHeld(char heldType)
    {
        if(!wallsBeingHeld)
        {
            wallsBeingHeld = true;
            typeHeld = heldType;
            if (heldType != type)
            {
                sphereRenderer.material = greyMaterial;
            }
        }

    }

    void WallsReleased(char heldType)
    {

        if(wallsBeingHeld && heldType == typeHeld)
        {
            wallsBeingHeld = false;
            typeHeld = '\0';
            if (heldType != type)
            {
                sphereRenderer.material = normalMaterial;
            }
        }
    }

    void turnOn()
    {
        if(!wallsBeingHeld)
        {
            emitObj.GetComponent<Renderer>().material.EnableKeyword("_RIMLIGHTING_ON");
            AudioManager.instance.Play("ButtonSelect");
        }
        else if(wallsBeingHeld && typeHeld != '\0' && typeHeld != type)
        {
            AudioManager.instance.Play("DisabledButton");
        }
    }
    void turnOff()
    {
        if(!wallsBeingHeld)
            emitObj.GetComponent<Renderer>().material.DisableKeyword("_RIMLIGHTING_ON");
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Extant.GameServerShared;

public class PlayerController : MonoBehaviour
{
    public Camera mainCamera;

    private CharacterController inControl;
    private GameController game;

    private bool canInput = true;

    private double inputUp, inputRight;

    private void Start()
    {
        game = this.GetComponent<GameController>();
        mainCamera = GameObject.Find("MainCamera").camera;

        CanInput = true;
    }

    private void Update()
    {
        if (inControl != null)
        {
            if (CanInput)
            {
                double newInputUp, newInputRight;

                newInputRight = (Input.GetKey(KeyCode.D) ? 1.0f : 0.0f) - (Input.GetKey(KeyCode.A) ? 1.0f : 0.0f);
                newInputUp = (Input.GetKey(KeyCode.D) ? 1.0f : 0.0f) - (Input.GetKey(KeyCode.A) ? 1.0f : 0.0f);

                if (newInputRight != inputRight || newInputUp != inputUp)
                {
                    //update server

                    inputUp = newInputUp;
                    inputRight = newInputRight;
                }
            }
        }
        else
        {

        }
    }

    public void SetControl(CharacterController ch)
    {
        inControl = ch;
    }

    public bool CanInput
    {
        set
        {
            canInput = value;
        }
        get
        {
            return canInput;
        }
    }
}
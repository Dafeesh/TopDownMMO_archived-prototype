using UnityEngine;
using System;

class GameInput : MonoComponent
{
    [SerializeField]
    MainCameraController camera = null;
    [SerializeField]
    GameController gameController = null;

    GameObject moveCursor;

    void Start()
    {
        if (camera == null)
            Debug.LogError("GameInput was not given a mainCamera.");
        if (gameController == null)
            Debug.LogError("GameInput was not given a gameController.");

        moveCursor = (GameObject)GameObject.Instantiate(Resources.Load("MoveCursor"));
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit hit;
            Ray ray = camera.Camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;

                moveCursor.transform.position = hit.point;
                gameController.Command_MoveTo(hit.point.x, hit.point.z);
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0.0f)
        {
            camera.CameraFollowDistance += Input.GetAxis("Mouse ScrollWheel")*(-5.0f);
        }
    }
}

using UnityEngine;
using System.Collections;

public class MainCameraController : MonoBehaviour
{
    const float MAX_FOLLOW_DISTANCE = 20.0f;
    const float MIN_FOLLOW_DISTANCE = 2.0f;

    [SerializeField]
    Camera thisCamera = null;

    float cameraFollowDistance = 12.0f;
    GameObject followObject = null;
    Vector3 followObject_offSet = new Vector3(0, 1.5f, 0);
	
	void Start()
	{
        if (thisCamera == null)
            Debug.LogError("MainCameraController was not given reference to its Camera.");

        this.transform.rotation = Quaternion.Euler(new Vector3(45, 45, 0));
	}
	
	void Update()
	{
        if (followObject != null)
        {
            this.transform.position = Vector3.Lerp(
                this.transform.position,
                new Vector3(followObject.transform.position.x - cameraFollowDistance * 0.5f, followObject.transform.position.y + cameraFollowDistance, followObject.transform.position.z - cameraFollowDistance*0.5f),
                2.0f * Time.deltaTime);

            Quaternion rotation = Quaternion.LookRotation((followObject.transform.position + followObject_offSet) - this.transform.position);
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, rotation, 1.0f * Time.deltaTime);
        }
	}

    public Camera Camera
    {
        get
        {
            return thisCamera;
        }
    }

    public float CameraFollowDistance
    {
        set
        {
            if (value < MIN_FOLLOW_DISTANCE)
                cameraFollowDistance = MIN_FOLLOW_DISTANCE;
            else if (value > MAX_FOLLOW_DISTANCE)
                cameraFollowDistance = MAX_FOLLOW_DISTANCE;
            else
                cameraFollowDistance = value;
        }
        get
        {
            return cameraFollowDistance;
        }
    }

    public GameObject FollowObject
    {
        set
        {
            followObject = value;
        }
        get
        {
            return followObject;
        }
    }
}

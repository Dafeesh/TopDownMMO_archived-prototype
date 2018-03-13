using UnityEngine;

class PlayerController : MonoBehaviour
{
    Camera mainCamera = null;

    CharacterController controlledChar = null;

    void Start()
    {
        mainCamera = GameObject.Find("_MainCamera").GetComponent<Camera>();
        if (mainCamera == null)
            Debug.LogError("PlayerController could not find _MainCamera.");
    }

    void Update()
    {
        if (controlledChar != null)
        {
            mainCamera.transform.position = new Vector3(controlledChar.transform.position.x - 15, controlledChar.transform.position.y + 20, controlledChar.transform.position.z - 15);
            mainCamera.transform.LookAt(controlledChar.transform);
        }
    }

    public void SetCharacterControlled(CharacterController cc)
    {
        controlledChar = cc;
    }
}

using UnityEngine;

class PlayerController : MonoBehaviour
{
    [SerializeField]
    Camera mainCamera = null;

    GameCharacterController controlledChar = null;

    void Start()
    {
        if (mainCamera == null)
            Debug.LogError("PlayerController was not given MainCamera.");

        mainCamera.transform.rotation = Quaternion.Euler(new Vector3(45, 45, 0));
    }

    void Update()
    {
        if (controlledChar != null)
        {
            mainCamera.transform.position = Vector3.Lerp(
                mainCamera.transform.position,
                new Vector3(controlledChar.transform.position.x - 15, controlledChar.transform.position.y + 20, controlledChar.transform.position.z - 15),
                2.0f * Time.deltaTime);
        }
    }

    public void SetCharacterControlled(GameCharacterController cc)
    {
        controlledChar = cc;
    }
}

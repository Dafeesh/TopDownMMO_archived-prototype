using UnityEngine;

class PlayerController : MonoComponent
{
    [SerializeField]
    MainCameraController mainCamera = null;

    GameCharacterController controlledChar = null;

    void Start()
    {
        if (mainCamera == null)
            Debug.LogError("PlayerController was not given MainCamera.");
    }

    void Update()
    {

    }

    public void SetCharacterControlled(GameCharacterController cc)
    {
        controlledChar = cc;
        mainCamera.FollowObject = cc.gameObject;
    }
}

using UnityEngine;
using UnityEngine.UI;

class DebugInterfaceController : MonoBehaviour
{
    [SerializeField]
    Text text_WSConnection = null;

    void Start()
    {
        if (text_WSConnection == null)
            Debug.LogError("DebugInterface has no reference to WSConnection text.");
    }

    void Update()
    {

    }

    public string Text_WSConnextion
    {
        set
        {
            text_WSConnection.text = value;
        }
    }
}

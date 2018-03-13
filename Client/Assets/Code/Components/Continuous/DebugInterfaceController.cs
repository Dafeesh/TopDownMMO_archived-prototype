using Extant;
using System.Collections;
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

        //StartCoroutine(ClearConsoleAsync());

        DebugLogger.Global.MessageLogged += Debug.Log;
    }

    void Update()
    {

    }

    void OnDestroy()
    {

    }

    public string Text_WSConnextion
    {
        set
        {
            text_WSConnection.text = value;
        }
    }

    IEnumerator ClearConsoleAsync()
    {
        // wait until console visible
        while (!Debug.developerConsoleVisible)
        {
            yield return null;
        }
        yield return null; // this is required to wait for an additional frame, without this clearing doesn't work (at least for me)
        Debug.ClearDeveloperConsole();
    }
}

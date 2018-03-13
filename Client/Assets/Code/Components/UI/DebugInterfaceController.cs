using Extant;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

class DebugInterfaceController : MonoComponent
{
    [SerializeField]
    InstanceServerConnection WSConnection = null;
    [SerializeField]
    Text WSConnection_text = null;

    [SerializeField]
    MasterServerConnection MSConnection = null;
    [SerializeField]
    Text MSConnection_text = null;

    void Awake()
    {
        if (WSConnection == null)
            Debug.LogError("DebugInterface has no reference to WSCon.");
        if (WSConnection_text == null)
            Debug.LogError("DebugInterface has no reference to WSCon text.");

        if (MSConnection == null)
            Debug.LogError("DebugInterface has no reference to MSCon.");
        if (MSConnection_text == null)
            Debug.LogError("DebugInterface has no reference to MSCon text.");

        WSConnection.StateChanged += OnStateChange_WSCon;
        MSConnection.StateChanged += OnStateChange_MSCon;
        DebugLogger.Global.MessageLogged += Debug.Log;
    }

    void Start()
    {

    }

    void Update()
    {

    }

    void OnDestroy()
    {

    }

    public void OnClick_Disconnect()
    {
        WSConnection.CloseConnection();
    }

    public void OnStateChange_WSCon(ConnectionState state)
    {
        WSConnection_text.text = state.ToString();
    }

    public void OnStateChange_MSCon(ConnectionState state)
    {
        MSConnection_text.text = state.ToString();
    }
}

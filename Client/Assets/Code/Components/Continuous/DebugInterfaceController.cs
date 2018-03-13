using Extant;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

class DebugInterfaceController : MonoComponent
{
    [SerializeField]
    InstanceServerConnection InstServConnection = null;
    [SerializeField]
    Text InstServConnection_text = null;

    [SerializeField]
    MasterServerConnection MastServConnection = null;
    [SerializeField]
    Text MastServConnection_text = null;

    void Awake()
    {
        if (InstServConnection == null)
            Debug.LogError("DebugInterface has no reference to WSCon.");
        if (InstServConnection_text == null)
            Debug.LogError("DebugInterface has no reference to WSCon text.");

        if (MastServConnection == null)
            Debug.LogError("DebugInterface has no reference to MSCon.");
        if (MastServConnection_text == null)
            Debug.LogError("DebugInterface has no reference to MSCon text.");

        InstServConnection.StateChanged += OnStateChange_WSCon;
        MastServConnection.StateChanged += OnStateChange_MSCon;
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
        MastServConnection.CloseConnection();
    }

    public void OnStateChange_WSCon(ConnectionState state)
    {
        InstServConnection_text.text = state.ToString();
    }

    public void OnStateChange_MSCon(ConnectionState state)
    {
        MastServConnection_text.text = state.ToString();
    }
}

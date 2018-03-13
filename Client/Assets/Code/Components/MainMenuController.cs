using Extant.Networking;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    private Text text_ip;
    [SerializeField]
    private Text text_port;

    private GlobalController globalController;

    void Start()
    {
        if (text_ip == null)
            Debug.LogError("MainMenuController has no reference to IP Text.");
        if (text_port == null)
            Debug.LogError("MainMenuController has no reference to Port Text.");

        globalController = GameObject.Find("_GlobalController").GetComponent<GlobalController>();
        if (globalController == null)
            Debug.LogError("MainMenuController could not find _GlobalController.");

    }

    void Update()
    {

    }

    public void _Login(string u)
    {
        NetConnection con = WorldServerConnection.ConnectAsync(new IPEndPoint(IPAddress.Parse(text_ip.text), int.Parse(text_port.text)), u, 111).Result;
        if (con != null)
            globalController.SetWorldServerConnection(con);
    }

    public string InputText_IP
    {
        get
        {
            return text_ip.text;
        }
    }

    public string InputText_Port
    {
        get
        {
            return text_port.text;
        }
    }
}

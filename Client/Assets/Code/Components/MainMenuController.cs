using UnityEngine;
using System.Collections;
using System.Net;

public class MainMenuController : MonoBehaviour
{
    WorldServerConnection wsConnection = null;

    void Start()
    {
        try
        {
            wsConnection = GameObject.Find("Connections").GetComponent<WorldServerConnection>();
        }
        finally
        {
            if (wsConnection == null)
                Debug.LogError("MainMenuController could not find WorldServerConnection");
        }
    }

    void Update()
    {

    }

    public void TEST_SetWorldServerTarget(string username)
    {
        wsConnection.SetTarget(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3000), username, 111);
    }
}

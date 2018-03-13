using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InterfaceAccessor : MonoBehaviour 
{
    [SerializeField]
    private Text gameTime;
    [SerializeField]
    private Text zoneName;

    void Start()
    {
        if (gameTime == null)
            Debug.LogError("InterfaceAccessor has no reference to GameTime.");
        if (zoneName == null)
            Debug.LogError("InterfaceAccessor has no reference to ZoneName.");

        GameTime = 0;
        ZoneName = Application.loadedLevelName;
    }

    public long GameTime
    {
        set
        {
            gameTime.text = "Time: " + ((float)value / 1000.0f).ToString("F");
        }
    }

    public string ZoneName
    {
        set
        {
            gameTime.text = "Zone: " + value;
        }
    }
}

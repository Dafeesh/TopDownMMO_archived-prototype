using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InterfaceController : MonoBehaviour 
{
    [SerializeField]
    private Text gameTime;
    [SerializeField]
    private Text zoneName;
    [SerializeField]
    private GameObject item_Console;
    [SerializeField]
    private GameObject item_Debug;
    [SerializeField]
    private GameObject item_ConnectMenu;

    private UIState state = UIState.ConnectMenu;

    void Start()
    {
        if (gameTime == null)
            Debug.LogError("InterfaceAccessor has no reference to GameTime.");
        if (zoneName == null)
            Debug.LogError("InterfaceAccessor has no reference to ZoneName.");
        if (item_Console == null)
            Debug.LogError("InterfaceAccessor has no reference to item_Console.");
        if (item_Debug == null)
            Debug.LogError("InterfaceAccessor has no reference to item_Debug.");
        if (item_ConnectMenu == null)
            Debug.LogError("InterfaceAccessor has no reference to item_ConnectMenu.");

        GameTime = 0;
        ZoneName = Application.loadedLevelName;

        SetState(UIState.ConnectMenu);
    }

    public void SetState(UIState state)
    {
        //Undo old state items
        switch (this.state)
        {
            case (UIState.ConnectMenu):
                {
                    item_ConnectMenu.SetActive(false);
                }
                break;

            case (UIState.Play):
                {

                }
                break;
        }

        //Enable new state items
        switch (state)
        {
            case (UIState.ConnectMenu):
                {
                    item_ConnectMenu.SetActive(true);
                }
                break;

            case (UIState.Play):
                {

                }
                break;
        }
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

    public enum UIState
    {
        ConnectMenu,
        Play
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using SharedComponents.Global.GameProperties;

public class CharListMenuController : MonoComponent
{
    //
    public static CharListMenuController Main = null;
    //

    List<CharListMenuItemButton> charListItems = new List<CharListMenuItemButton>();
    MasterServerConnection msConnection;

    void Awake()
    {
        Main = this;
    }

    void Start()
    {
        msConnection = MasterServerConnection.Main;
        if (msConnection == null)
            Debug.LogError("CharacterListController could not find MasterServerConnection.");
        msConnection.Received_AddCharacterListItem += OnReceive_AddCharacterListItem;

        Log.MessageLogged += Debug.Log;
    }

    void Update()
    {

    }

    public void OnButton_SelectCharacter(CharListMenuItemButton sender)
    {
        msConnection.OnAction_SelectCharacter(sender.Name);
    }

    public void OnReceive_AddCharacterListItem(string name, CharacterLayout layout, int level)
    {
        Log.Log("Create character: " + name);

        GameObject obj = (GameObject)Instantiate(Resources.Load(ResourceList.UI.CharacterListItem));
        obj.transform.parent = GameObject.Find("CharList").transform;
        CharListMenuItemButton button = obj.GetComponent<CharListMenuItemButton>();

        button.Name = name;
        button.Type = layout.Type;
        button.Level = level;

        charListItems.Add(button);
    }
}

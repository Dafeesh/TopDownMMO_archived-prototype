﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using SharedComponents.Global.GameProperties;

public class CharListMenuController : MonoComponent
{
	//
	public static CharListMenuController Main = null;
	//

	[SerializeField]
	GameObject CharListPanel = null;
	[SerializeField]
	GameObject WaitingPanel = null;

	List<CharListMenuItemButton> charListItems = new List<CharListMenuItemButton>();
	MasterServerConnection msConnection = null;

	void Awake()
	{
		Main = this;
	}

	void Start()
	{
		msConnection = MasterServerConnection.Main;
		if (msConnection == null)
			Debug.LogError("CharacterListController could not find MasterServerConnection.");
		msConnection.StateChanged += OnStateChanged_MSConnection;

		Log.MessageLogged += Debug.Log;
	}

	void OnDestroy()
	{
		Main = null;
		msConnection.StateChanged -= OnStateChanged_MSConnection;
	}

	void Update()
	{

	}

	public void OnStateChanged_MSConnection(ConnectionState state)
	{
		if (state == ConnectionState.NoConnection)
		{
			Log.Log("Lost connection while in Character Selection.");

			Application.LoadLevel(ResourceList.Scenes.LoginPage);
		}
	}

	public void OnButton_SelectCharacter(CharListMenuItemButton sender)
	{
		msConnection.OnAction_SelectCharacter(sender.Name, 1);

		CharListPanel.SetActive(false);
		WaitingPanel.SetActive(true);
	}

	public void AddCharacterListItem(string name, CharacterVisualLayout layout, int level)
	{
		//Log.Log("Create selection: " + name);

		GameObject obj = (GameObject)Instantiate(Resources.Load(ResourceList.UI.CharacterListItem));
		obj.transform.SetParent(GameObject.Find("CharList").transform);
		CharListMenuItemButton button = obj.GetComponent<CharListMenuItemButton>();

		button.Name = name;
		button.Type = layout.Type;
		button.Level = level;

		charListItems.Add(button);
	}
}

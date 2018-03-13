using UnityEngine;
using UnityEngine.UI;
using System.Collections;


using SharedComponents.Global.GameProperties;

public class CharListMenuItemButton : MonoBehaviour
{
    CharListMenuController charList = null;

    [SerializeField]
    Text Name_text = null;
    string Name_value;
    [SerializeField]
    Text VisualType_text = null;
    CharacterLayout.VisualType VisualType_value;
    [SerializeField]
    Text Level_text = null;
    int Level_value;

    public string Name
    {
        get
        {
            return Name_value;
        }
        set
        {
            Name_text.text = value;
            Name_value = value;
        }
    }

    public CharacterLayout.VisualType Type
    {
        get
        {
            return VisualType_value;
        }
        set
        {
            VisualType_text.text = value.ToString();
            VisualType_value = value;
        }
    }

    public int Level
    {
        get
        {
            return Level_value;
        }
        set
        {
            Level_text.text = value.ToString();
            Level_value = value;
        }
    }

    void Start()
    {
        charList = CharListMenuController.Main;
        if (charList == null)
            Debug.LogError("CharListItemButton could not find CharListMenuController.");

        if (Name_text == null ||
            VisualType_text == null ||
            Level_text == null)
            Debug.LogError("CharListItemButton is missing a reference to a SerializeField.");
    }

    public void OnClick()
    {
        charList.OnButton_SelectCharacter(this);
    }
}

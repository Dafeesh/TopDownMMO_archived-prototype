using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using Extant;
using SharedComponents.Global.GameProperties;

public class CharacterListController : MonoComponent
{
    //
    public static CharacterListController Main = null;
    //

    Dictionary<int, GameCharacterController> characters = new Dictionary<int, GameCharacterController>();

    void Awake()
    {
        Main = this;
    }

    void Start()
    {
        Log.MessageLogged += Debug.Log;
    }

    void Update()
    {

    }

    public void ClearAll()
    {
        foreach (var kvp in characters)
        {
            GameObject.Destroy(kvp.Value.gameObject);
        }
        characters.Clear();
    }

    public void AddCharacter(int id, CharacterVisualLayout layout)
    {
        if (characters.ContainsKey(id))
        {
            RemoveCharacter(id);
            Debug.LogWarning("CharacterListController added character that already existed.");
        }

        GameObject newChar = (GameObject)GameObject.Instantiate(Resources.Load(ResourceList.Characters.Robot));
        newChar.transform.parent = this.transform;

        characters.Add(id, newChar.GetComponent<GameCharacterController>());

        Log.Log("CharacterListController added character: " + id);
    }

    public void RemoveCharacter(int id)
    {
        if (characters.ContainsKey(id))
        {
            GameObject.Destroy(characters[id]);
            characters.Remove(id);

            Log.Log("CharacterListController removed character: " + id);
        }
        else
        {
            Debug.LogError("CharacterListController failed to remove character. Does not exist: " + id);
        }
    }

    public void SetCharacter_Position(int id, float x, float y)
    {
        if (characters.ContainsKey(id))
        {
            characters[id].SetPosition(x, y);

            //log.Log("CharacterListController moved character: " + id + " -> (" + x + "," + y + ")");
        }
        else
        {
            Debug.LogError("CharacterListController failed to move character. Does not exist: " + id);
        }
    }

    public void SetCharacter_Stats(int id, CharacterStats stats)
    {
        if (characters.ContainsKey(id))
        {
            characters[id].Stats = stats;
        }
        else
        {
            Debug.LogError("CharacterListController failed to update stats for character. Does not exist: " + id);
        }
    }

    public void SetCharacter_MovePoint(int id, MovePoint mp)
    {
        if (characters.ContainsKey(id))
        {
            characters[id].SetMovePoint(mp);
        }
        else
        {
            Debug.LogError("CharacterListController failed to update stats for character. Does not exist: " + id);
        }
    }

    public GameCharacterController GetControllerFromId(int id)
    {
        if (characters.ContainsKey(id))
        {
            return characters[id];
        }
        else
        {
            Log.Log("CharListController could not get controller from id: " + id);
            return null;
        }
    }
}

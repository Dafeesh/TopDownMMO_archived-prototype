using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using Extant;
using SharedComponents.GameProperties;

public class CharacterListController : MonoBehaviour
{
    Dictionary<int, GameCharacterController> characters = new Dictionary<int, GameCharacterController>();

    DebugLogger log;

    void Start()
    {
        log = new DebugLogger("CharListController");
        log.MessageLogged += Debug.Log;
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

    public void AddCharacter(int id, CharacterType charType, int modelNum)
    {
        if (characters.ContainsKey(id))
        {
            RemoveCharacter(id);
            Debug.LogWarning("CharacterListController added character that already existed.");
        }

        GameObject newChar;
        if (modelNum == 1)
            newChar = (GameObject)GameObject.Instantiate(Resources.Load("Character"));
        else
            newChar = (GameObject)GameObject.Instantiate(Resources.Load("Character"));
        newChar.transform.parent = this.transform;

        characters.Add(id, newChar.GetComponent<GameCharacterController>());

        log.Log("CharacterListController added character: " + id);
    }

    public void RemoveCharacter(int id)
    {
        if (characters.ContainsKey(id))
        {
            GameObject.Destroy(characters[id]);
            characters.Remove(id);
            
            log.Log("CharacterListController removed character: " + id);
        }
        else
        {
            Debug.LogError("CharacterListController failed to remove character. Does not exist: " + id);
        }
    }

    public void SetCharacterPosition(int id, float x, float y)
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

    public GameCharacterController GetControllerFromId(int id)
    {
        if (characters.ContainsKey(id))
        {
            return characters[id];
        }
        else
        {
            log.Log("CharListController could not get controller from id: " + id);
            return null;
        }
    }
}

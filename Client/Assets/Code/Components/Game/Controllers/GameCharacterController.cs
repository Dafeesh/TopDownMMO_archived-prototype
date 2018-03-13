using UnityEngine;

using Extant;
using SharedComponents.GameProperties;

public class GameCharacterController : MonoBehaviour, ILogging
{
    [SerializeField]
    GameObject meshObject = null;

    string name;
    CharacterType type;
    CharacterThreat threat;
    CharacterStats stats;

    DebugLogger log = new DebugLogger("CharObjContr");

    void Start()
    {
        if (meshObject == null)
            Debug.LogError("GameCharacterController does not have reference to mesh object.");

        Name = "NULL";
        Type = CharacterType.Npc;
        Threat = CharacterThreat.Neutral;
        Stats = new CharacterStats();
    }

    void Update()
    {

    }

    public void SetPosition(float x, float y)
    {
        this.gameObject.transform.position = new Vector3(x, 0, y);
    }

    public string Name
    {
        set
        {
            name = value;
        }
        get
        {
            return name;
        }
    }

    public CharacterType Type
    {
        set
        {
            type = value;
        }
        get
        {
            return type;
        }
    }

    public CharacterThreat Threat
    {
        set
        {
            threat = value;

            switch (threat)
            {
                case (CharacterThreat.Neutral):
                    {
                        meshObject.GetComponent<Renderer>().material.SetColor(1, Color.blue);
                    }
                    break;

                case (CharacterThreat.Friendly):
                    {
                        meshObject.GetComponent<Renderer>().material.SetColor(1, Color.green);
                    }
                    break;

                case (CharacterThreat.Enemy):
                    {
                        meshObject.GetComponent<Renderer>().material.SetColor(1, Color.red);
                    }
                    break;
                default:
                    Debug.LogWarning("CharacterObjectController does not support CharacterThread: " + threat.ToString());
                    break;
            }
        }
        get
        {
            return threat;
        }
    }

    public CharacterStats Stats
    {
        get
        {
            return stats;
        }

        private set
        {
            stats = value;
        }
    }

    public DebugLogger Log
    {
        get
        {
            return log;
        }
    }
}

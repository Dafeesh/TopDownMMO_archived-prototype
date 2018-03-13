using UnityEngine;

using Extant;
using SharedComponents.Global.GameProperties;

public class GameCharacterController : MonoComponent
{
    string _name;
    CharacterLayout _layout;
    CharacterThreat _threat;
    CharacterStats _stats;

    Position2D position = new Position2D();
    Position2D moveToPoint = null;

    [SerializeField]
    Animator animationController = null;
    AnimationState animState = AnimationState.Null;

    void Start()
    {
        if (animationController == null)
            Debug.LogError("GameCharacterController does not have reference to animationController.");

        this.Name = "NULL";
        this.Layout = new CharacterLayout(CharacterVisualType.Null);
        this.Threat = CharacterThreat.Neutral;
        this.Stats = new CharacterStats();

        SetAnimState(AnimationState.Idle);
    }

    void Update()
    {
        if (moveToPoint != null)
        {
            Quaternion lookAtAngle = Quaternion.LookRotation(new Vector3(moveToPoint.x - position.x, 0, moveToPoint.y - position.y));
            this.gameObject.transform.rotation = Quaternion.Lerp(this.transform.rotation, lookAtAngle, 5.0f * Time.deltaTime);

            SetPosition(CalculateMove(position, moveToPoint, Stats.MoveSpeed * Time.deltaTime));
        }
    }

    Position2D CalculateMove(Position2D old, Position2D target, float unitsToMove)
    {
        float x = (target.x - old.x);
        float y = (target.y - old.y);
        float angle = Mathf.Atan2(y, x);
        float d = Mathf.Sqrt(x * x + y * y);
        float deltaX = unitsToMove * Mathf.Cos(angle);
        float deltaY = unitsToMove * Mathf.Sin(angle);

        if (unitsToMove > d)
        {
            SetAnimState(AnimationState.Idle);
            moveToPoint = null;
            return target;
        }
        else
            return new Position2D(old.x + deltaX, old.y + deltaY);
    }

    void StepTowardPoint(ref Position2D moveToPoint, float travelDistance)
    {
        float distanceX = (moveToPoint.x - position.x);
        float distanceY = (moveToPoint.y - position.y);
        float angle = Mathf.Atan2(distanceY, distanceX);
        float distance = Mathf.Sqrt((distanceX * distanceX) + (distanceY * distanceY));

        if (travelDistance > distance)
        {
            moveToPoint = null;
            SetPosition(moveToPoint.x, moveToPoint.y);
        }
        else
        {
            float newx = position.x + (travelDistance * Mathf.Cos(distance));
            float newy = position.y + (travelDistance * Mathf.Sin(distance));
            Debug.Log("(" + newx + "," + newy + ")");
            SetPosition(newx,
                        newy);
        }
    }

    void SetAnimState(AnimationState state)
    {
        if (animState != state)
        {
            animState = state;

            switch (animState)
            {
                case (AnimationState.Idle):
                    {
                        animationController.SetFloat("RunningSpeed", 0.0f);
                    }
                    break;

                case (AnimationState.WalkForward):
                    {
                        animationController.SetFloat("RunningSpeed", Stats.MoveSpeed);
                    }
                    break;
            }
        }
    }

    public void SetPosition(Position2D pos)
    {
        SetPosition(pos.x, pos.y);
    }

    public void SetPosition(float x, float y)
    {
        position.x = x;
        position.y = y;
        this.gameObject.transform.position = new Vector3(x, 0, y);
    }

    public void SetMovePoint(MovePoint mp)
    {
        SetPosition(mp.start.x, mp.start.y);
        moveToPoint = mp.end;

        SetAnimState(AnimationState.WalkForward);
    }

    public string Name
    {
        set
        {
            _name = value;
        }
        get
        {
            return name;
        }
    }

    public CharacterLayout Layout
    {
        set
        {
            _layout = value;
        }
        get
        {
            return _layout;
        }
    }

    public CharacterThreat Threat
    {
        set
        {
            _threat = value;

            switch (_threat)
            {
                case (CharacterThreat.Neutral):
                    {
                        //meshObject.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
                    }
                    break;

                case (CharacterThreat.Friendly):
                    {
                        //meshObject.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                    }
                    break;

                case (CharacterThreat.Enemy):
                    {
                        //meshObject.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                    }
                    break;
                default:
                    Debug.LogWarning("CharacterObjectController does not support CharacterThread: " + _threat.ToString());
                    break;
            }
        }
        get
        {
            return _threat;
        }
    }

    public CharacterStats Stats
    {
        get
        {
            return _stats;
        }

        set
        {
            _stats = value;
        }
    }

    public enum AnimationState
    {
        Null,
        Idle,
        WalkForward
    }
}

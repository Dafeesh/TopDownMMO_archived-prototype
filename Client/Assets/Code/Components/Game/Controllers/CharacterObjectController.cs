using UnityEngine;

public class CharacterObjectController : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {

    }

    public void MoveTo(float x, float y)
    {
        this.gameObject.transform.position = new Vector3(x, 0, y);
    }
}

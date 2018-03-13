using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour
{
    private GameController game;

    void Start()
    {
        game = GameObject.Find("_MasterController").GetComponent<GameController>();
    }

    void Update()
    {
        
    }
}

using UnityEngine;
using System.Collections;

using Extant.GameServerShared;

public class CharacterController : MonoBehaviour 
{
    string name;
    TeamColor teamColor;

    CharacterMovement movement;

	void Start () 
    {
        movement = this.GetComponent<CharacterMovement>();
	}
	
	void Update () 
    {
	    
	}

    /// <summary>
    /// Updates the movement of the character.
    /// </summary>
    /// <param name="x">X position.</param>
    /// <param name="y">Y position.</param>
    /// <param name="sx">X speed.</param>
    /// <param name="sy">Y speed.</param>
    public void UpdateMovement(double x, double y, double sx, double sy)
    {

    }
}

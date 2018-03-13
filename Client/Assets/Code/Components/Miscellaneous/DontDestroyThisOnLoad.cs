using UnityEngine;
using System.Collections;

public class DontDestroyThisOnLoad : MonoBehaviour 
{
	void Start () 
    {
        DontDestroyOnLoad(this);
	}
}

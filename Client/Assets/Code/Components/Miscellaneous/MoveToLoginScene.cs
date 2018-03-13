using UnityEngine;
using System.Collections;

public class MoveToLoginScene : MonoBehaviour 
{
    void Update () 
    {
        if (Application.loadedLevelName == ResourceList.Scenes._Start)
        {
            Application.LoadLevel(ResourceList.Scenes.LoginPage);
        }
    }
}

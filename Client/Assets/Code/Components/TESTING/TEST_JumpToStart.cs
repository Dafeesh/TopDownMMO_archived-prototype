using UnityEngine;
using System.Collections;

public class TEST_JumpToStart : MonoBehaviour
{
    private static bool hasBeenToStart = false;

    private bool firstUpdate = true;

    void Start()
    {
        if (!hasBeenToStart)
        {
            hasBeenToStart = true;

            if (Application.loadedLevelName != SceneList._Start)
            {
                //Debug.ClearDeveloperConsole();
                Application.LoadLevel(SceneList._Start);
            }
            else
            {
                Debug.Log("Already here");
            }
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
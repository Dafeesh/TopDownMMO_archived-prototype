using UnityEngine;
using System.Collections;

public class TEST_JumpToStart : MonoBehaviour
{
    private static bool hasBeenToStart = false;
    private static bool consoleVisibile = false;

    void Update()
    {
        if (!hasBeenToStart)
        {
            hasBeenToStart = true;

            if (Application.loadedLevelName != SceneList._Start)
                Application.LoadLevel(SceneList._Start);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}

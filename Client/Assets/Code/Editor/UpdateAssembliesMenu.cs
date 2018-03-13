using System;
using UnityEditor;
using UnityEngine;
public class UpdateAssembliesMenu : MonoBehaviour
{
    [MenuItem("Assemblies/Update")]
    static void UpdateAssemblies()
    {
        try
        {
            string from = @"C:\Users\Blake\Code and Source\_Game\SharedComponents\_Build_Debug\";
            string to = @"C:\Users\Blake\Code and Source\_Game\Client\Assets\Assemblies\";

            string[] files = {"ExtantLibrary.dll",
                          "ClientToServers.dll"};

            foreach (string f in files)
            {
                FileUtil.ReplaceFile(from + f, to + f);
            }
        }
        catch (Exception e)
        {
            Debug.Log("Failed to copy over assemblies: " + e.ToString());
        }

        Debug.Log("Successfully copied over assemblies.");
    }
}

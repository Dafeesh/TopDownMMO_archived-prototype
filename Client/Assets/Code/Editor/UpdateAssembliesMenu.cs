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
            string from = @"C:\Users\Blake\Code & Source\_Game\SharedComponents\_Build_Debug\";
            string to = @"C:\Users\Blake\Code & Source\_Game\Client\Assets\Assemblies\";

            string[] files = {"Extant__Base.dll",
                              "Extant_Networking.dll",
                              "SharedComponents_Global.dll"};

            foreach (string f in files)
            {
                FileUtil.ReplaceFile(from + f, to + f);
            }
        }
        catch (Exception e)
        {
            Debug.Log("Failed to copy over assemblies: " + e.ToString());
            return;
        }

        Debug.Log("Successfully copied over assemblies.");
    }
}

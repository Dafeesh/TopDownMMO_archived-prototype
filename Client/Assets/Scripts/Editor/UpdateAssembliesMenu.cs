using UnityEditor;
using UnityEngine;
public class UpdateAssembliesMenu : MonoBehaviour
{
    [MenuItem("Assemblies/Update")]
    static void UpdateAssemblies()
    {
        string from = @"C:\Users\Blake\Code and Source\_Game\SharedComponents\_Build_Debug\";
        string to   = @"C:\Users\Blake\Code and Source\_Game\Client\Assets\Assemblies\";

        string[] files = {"ExtantLibrary.dll",
                          "ClientToServers.dll"};

        foreach (string f in files)
        {
            FileUtil.ReplaceFile(from + f, to + f);
        }
    }
}

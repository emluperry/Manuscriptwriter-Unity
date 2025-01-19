#if UNITY_EDITOR
using System.IO;
using UnityEditor;

namespace Scripting.Editor.Utility
{
    /// <summary>
    /// Code by spiney199 on the Unity Forums. Adds a menu item to create a new text file in the asset menu.
    /// </summary>
    public static class CreateTextFile
    {
        [MenuItem("Assets/Create/Text File", priority = 100)]
        private static void CreateNewTextFile()
        {
            string folderGUID = Selection.assetGUIDs[0];
            string projectFolderPath = AssetDatabase.GUIDToAssetPath(folderGUID);
            string folderDirectory = Path.GetFullPath(projectFolderPath);

            using (StreamWriter sw = File.CreateText(folderDirectory + "/NewTextFile.txt"))
            {
                sw.WriteLine("This is a new text file!");
            }
       
            AssetDatabase.Refresh();
        }
    }
}
#endif
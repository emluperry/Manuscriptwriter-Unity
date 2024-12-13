using UnityEngine;
using UnityEngine.Search;

namespace MSW.Scripting.Unity
{
    public class Storyteller : MonoBehaviour
    {
        //private void Awake()
        //{
        //    this.RegisterSceneObjects();
        //}

        //private void RegisterSceneObjects()
        //{
        //    // Get all Action components within the scene.
        //    var actionComponents = Object.FindObjectsByType<Actions>(FindObjectsSortMode.None);

        //    foreach (var actionComponent in actionComponents)
        //    {
        //        actionComponent.RunScript = this.RunScript;
        //    }
        //}

        //private void RunScript(string script)
        //{
            
        //}

        // DEBUG ONLY !!!
        [SerializeField]
        [SearchContext("ext:txt dir:Resources")] // QOL: Limit the files to ONLY project text files within Resources. 
        private TextAsset testScript;

        private MSWRunner runner;

        private void Start()
        {
            if(!testScript)
            {
                return;
            }

            runner = new MSWRunner() { ErrorLogger = this.LogError };
            runner.Run(testScript.text);
        }

        private void LogError(string message)
        {
            Debug.LogError(message); 
        }
    }
}

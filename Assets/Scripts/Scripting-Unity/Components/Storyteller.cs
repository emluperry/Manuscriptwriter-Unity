using System;
using UnityEngine;
using UnityEngine.Search;

using MSW.Compiler;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace MSW.Unity
{
    public class Storyteller : MonoBehaviour
    {
        [Header("Run on Start")]
        [SerializeField] [SearchContext("ext:txt dir:Resources")] // QOL: Limit the files to ONLY project text files within Resources. 
        private TextAsset startupScript;
        
        [Header("Storyteller Commands")]
        [SerializeField] private MSWUnityLibrary[] libraries;
        
        private Compiler.Compiler compiler;
        private Runner runner;
        
        private void Awake()
        {
            compiler = new Compiler.Compiler()
            {
                ErrorLogger = Logger,
                FunctionLibrary = libraries,
            };
            
            this.RegisterSceneObjects();
        }

        private void Start()
        {
            if(!startupScript)
            {
                return;
            }
            
            this.RunScript(startupScript.text);
        }

        private void OnDestroy()
        {
            this.CleanupOnFinish();
        }

        private void RegisterSceneObjects()
        {
            // Get all Action components within the scene.
            var actionComponents = Object.FindObjectsByType<Actions>(FindObjectsSortMode.None);

            foreach (var actionComponent in actionComponents)
            {
                this.RunScript(actionComponent.ActionScript.text);
            }
        }

        private void RunScript(string script)
        {
            var manuscript = compiler.Compile(script);

            runner = new Runner(manuscript)
            {
                Logger = Logger,
                OnFinish = CleanupOnFinish,
            };
            
            runner.Run();
        }
        
        private void CleanupOnFinish()
        {
            foreach (var library in this.libraries)
            {
                library.Cleanup();
            }
        }

        #region DEBUGGING
        
        private void Logger(string message)
        {
            Debug.LogError(message); 
        }

        #endregion
    }
}

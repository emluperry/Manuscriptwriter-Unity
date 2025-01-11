using System;
using UnityEngine;
using UnityEngine.Search;

using MSW.Compiler;
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
            
            this.GetRunScript(startupScript.text)?.Invoke();
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
                this.GetRunScript(actionComponent.ActionScript.text)?.Invoke();
            }
        }

        private Action GetRunScript(string script)
        {
            var manuscript = compiler.Compile(script);

            var runner = new Runner(manuscript)
            {
                Logger = Logger,
                OnFinish = CleanupOnFinish,
            };

            return () => this.RunScript(runner);
        }

        private void RunScript(Runner runner)
        {
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

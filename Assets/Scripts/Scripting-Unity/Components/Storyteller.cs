using System;
using System.Collections.Generic;
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

        private Dictionary<string, Descriptor> actors;
        
        private Compiler.Compiler compiler;
        
        private void Awake()
        {
            compiler = new Compiler.Compiler()
            {
                ErrorLogger = Logger,
                FunctionLibrary = libraries,
            };
            
            this.SetupLibraries();
            this.SetupSceneObjects();
        }

        private void Start()
        {
            if(!startupScript)
            {
                return;
            }
            
            this.GetRunScript(startupScript.text, this.gameObject.name)?.Invoke();
        }

        private void OnDestroy()
        {
            this.CleanupOnFinish();
        }

        private void SetupLibraries()
        {
            foreach (var library in libraries)
            {
                library.GetObjectWithName = this.GetObjectWithName;
            }
        }

        private void SetupSceneObjects()
        {
            this.actors = new Dictionary<string, Descriptor>();
            
            var objects = Object.FindObjectsByType<Descriptor>(FindObjectsSortMode.None);
            foreach (var obj in objects)
            {
                this.actors[obj.ObjectName] = obj;
            }
            
            // Get all Action components within the scene.
            var actionComponents = Object.FindObjectsByType<Actions>(FindObjectsSortMode.None);
            foreach (var actionComponent in actionComponents)
            {
                this.GetRunScript(actionComponent.ActionScript.text, actionComponent.gameObject.name)?.Invoke();
            }
        }

        #region ManuscriptWriter

        private Action GetRunScript(string script, string scriptName)
        {
            var manuscript = compiler.Compile(script);

            if (manuscript == null)
            {
                Debug.LogError($"Found an error in the actions for {scriptName} - see the log for more details");
                return () => { };
            }

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

        #endregion

        #region Utilities

        private Descriptor GetObjectWithName(string name)
        {
            return this.actors.GetValueOrDefault(name);
        }

        #endregion

        #region DEBUGGING
        
        private void Logger(string message)
        {
            Debug.LogError(message); 
        }

        #endregion
    }
}

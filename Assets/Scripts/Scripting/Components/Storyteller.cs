using UnityEngine;

namespace MSW.Scripting
{
    public class Storyteller : MonoBehaviour
    {
        private void Awake()
        {
            this.RegisterSceneObjects();
        }

        private void RegisterSceneObjects()
        {
            // Get all Action components within the scene.
            var actionComponents = Object.FindObjectsByType<Actions>(FindObjectsSortMode.None);

            foreach (var actionComponent in actionComponents)
            {
                actionComponent.RunScript = this.RunScript;
            }
        }

        private void RunScript(string script)
        {
            
        }
    }
}

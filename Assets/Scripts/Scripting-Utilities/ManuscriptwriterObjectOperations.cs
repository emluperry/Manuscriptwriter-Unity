#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Scripting.Editor.Utility
{
    public static class ManuscriptwriterObjectOperations
    {
        [MenuItem("GameObject/ManuScriptwriter/Make Selection Interactable", false, 10)]
        private static void ConvertToInteractableObject()
        {
            // Make new gameobject from prefab.
            Object prefab = AssetDatabase.LoadAssetAtPath("Assets/Design/ManuScriptwriter/InteractableObject.prefab", typeof(GameObject));

            var fullSelection = Selection.gameObjects;

            foreach (var selectionContext in fullSelection)
            {
                GameObject newInteractable = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                
                // Setup creation undo
                Undo.RegisterCreatedObjectUndo(newInteractable, "Create " + newInteractable.name);
            
                Undo.RecordObject(selectionContext, $"Reparent {selectionContext.name} to {newInteractable.name}");
            
                // Reparent selection to the interactable object
                var currentPosition = selectionContext.transform.position;
                var currentRotation = selectionContext.transform.rotation;
                var currentScale = selectionContext.transform.localScale;
            
                GameObjectUtility.SetParentAndAlign(selectionContext, newInteractable);
            
                // Set the transform of the interactable object to the original position
                newInteractable.transform.position = currentPosition;
                newInteractable.transform.rotation = currentRotation;
                newInteractable.transform.localScale = currentScale;
            
                // Make sure the original model has a solid collider
                var solidCollider = selectionContext.GetComponent<Collider>();
                if (!solidCollider)
                {
                    solidCollider = selectionContext.AddComponent<BoxCollider>();
                }
            
                // resize the interaction range collider based on the collider
                if (solidCollider)
                {
                    var collider = newInteractable.GetComponent<BoxCollider>();
                    collider.center = solidCollider.bounds.center - selectionContext.transform.position;
                    collider.size = solidCollider.bounds.size * 2;
                
                    // Change the position of the UI Elements
                    var uiElements = newInteractable.transform.Find("UI");
                    if (uiElements)
                    {
                        uiElements.transform.position = solidCollider.bounds.center;
                        uiElements.transform.rotation = new Quaternion(0, 0, 0, 1);
                    }
                }

                Selection.activeGameObject = newInteractable;
            }
        }

        [MenuItem("GameObject/ManuScriptwriter/Make Selection Interactable", true)]
        private static bool ValidateConvertToInteractableObject()
        {
            return Selection.activeGameObject != null;
        }
        
        
        [MenuItem("GameObject/ManuScriptwriter/Add Collision", false, 10)]
        private static void AddCollision()
        {
            var fullSelection = Selection.gameObjects;

            foreach (var selectionContext in fullSelection)
            {
                // Make sure the original model has a solid collider
                var solidCollider = selectionContext.GetComponent<Collider>();
                if (!solidCollider)
                {
                    Undo.RecordObject(selectionContext, $"Add box collider to {selectionContext.name}");
                    selectionContext.AddComponent<BoxCollider>();
                }
            }
        }

        [MenuItem("GameObject/ManuScriptwriter/Add Collision", true)]
        private static bool ValidateAddCollision()
        {
            return Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Collider>() == null;
        }
        
        [MenuItem("GameObject/ManuScriptwriter/Remove Collision", false, 10)]
        private static void RemoveCollision()
        {
            var fullSelection = Selection.gameObjects;

            foreach (var selectionContext in fullSelection)
            {
                // Make sure the original model has a solid collider
                var solidCollider = selectionContext.GetComponent<Collider>();
                if (!solidCollider)
                {
                    Undo.RecordObject(selectionContext, $"Add box collider to {selectionContext.name}");
                    selectionContext.AddComponent<BoxCollider>();
                }
            }
        }

        [MenuItem("GameObject/ManuScriptwriter/Remove Collision", true)]
        private static bool ValidateRemoveCollision()
        {
            return Selection.gameObjects.Any(obj => obj.GetComponent<Collider>() != null);
        }
    }
}
#endif
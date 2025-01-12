using System;
using MSW.Events;
using MSW.Reflection;
using UnityEngine;
using UnityEngine.Serialization;

namespace MSW.Unity.Camera
{
    public class UnityCameras : MSWUnityLibrary
    {
        [SerializeField] private UnityEngine.Camera currentCamera;
        private ICameraCommands currentCameraCommands;

        private void Awake()
        {
            this.currentCameraCommands = this.currentCamera?.GetComponent<ICameraCommands>();
        }

        #region MSW Events
        
        //private RunnerEvent continueAction;

        #endregion

        #region MSW Functions
        
        [MSWFunction("The camera stops moving.")]
        public object StopMovement(Context context)
        {
            //context.WaitForEvent(continueAction);
            return null;
        }
        
        [MSWFunction("The camera focuses on {0}.")]
        public object FocusOnTarget(Context context, string target)
        {
            // get the target from the string id
            var targetObject = this.GetObjectWithName(target);
            if (targetObject == null)
            {
                return null;
            }
            
            currentCameraCommands?.SetTarget(targetObject.transform);
            
            return null;
        }
        
        [MSWFunction("The camera shakes from side to side.")]
        public object ShakeHorizontal(Context context)
        {
            //context.WaitForEvent(continueAction);
            return null;
        }

        #endregion

        public override void Cleanup()
        {
            
        }
    }
}

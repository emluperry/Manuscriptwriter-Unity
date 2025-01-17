using MSW.Reflection;
using UnityEngine;

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
        
        [MSWFunction("The camera freezes.")]
        public object StopMovement(Context context)
        {
            currentCameraCommands?.FreezeCamera();
            return null;
        }
        
        [MSWFunction("The camera stops shaking.")]
        public object StopEffects(Context context)
        {
            currentCameraCommands?.StopAdditionalEffects();
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
            currentCameraCommands.StartCameraShake();
            return null;
        }

        #endregion

        public override void Cleanup()
        {
            currentCameraCommands?.StopAdditionalEffects();
        }
    }
}

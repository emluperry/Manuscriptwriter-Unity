using UnityEngine;

namespace MSW.Unity.Camera
{
    public interface ICameraCommands
    {
        void SetTarget(Transform target);
        void SetTargets(Transform[] targets);
        void FreezeCamera();

        void StartCameraShake();
        void StopAdditionalEffects();
    }
}

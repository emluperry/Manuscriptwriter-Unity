using UnityEngine;

namespace Demo.Camera
{
    public interface ICameraModifier
    {
        Vector3 ModifyCameraPosition(Vector3 position);
        
        float ModifyCameraZoom(float position);
    }
}

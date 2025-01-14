using UnityEngine;

namespace Demo.Camera
{
    public class HorizontalCameraShake : ICameraModifier
    {
        public float MaxOffset = 0.3f;
        private Vector3 targetOffset;
        private Vector3 currentOffset = Vector3.zero;
        
        public float ShakeSpeed = 0.1f;

        public HorizontalCameraShake()
        {
            targetOffset = new Vector3(MaxOffset, 0f, 0f);
        }
        
        public Vector3 ModifyCameraPosition(Vector3 position)
        {
            currentOffset = Vector3.MoveTowards(currentOffset, targetOffset, ShakeSpeed);

            // update the target offset if necessary
            if (targetOffset == currentOffset)
            {
                targetOffset *= -1;
            }
            
            // return the new position
            return position + currentOffset;
        }

        public float ModifyCameraZoom(float position)
        {
            // no op
            return position;
        }
    }
}

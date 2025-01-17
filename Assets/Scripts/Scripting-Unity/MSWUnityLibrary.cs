using System;
using UnityEngine;

namespace MSW.Unity
{
    public abstract class MSWUnityLibrary : MonoBehaviour
    {
        public Func<string, Descriptor> GetObjectWithName;
        
        public abstract void Cleanup();
    }
}
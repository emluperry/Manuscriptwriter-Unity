using System;
using Demo.Camera;
using Demo.Entity;
using Demo.Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Demo.Scene
{
    public class SceneManager : MonoBehaviour
    {
        [SerializeField] private EntityInitialiser player;
        [SerializeField] private PlayerController playerController;
        [FormerlySerializedAs("Camera")] [SerializeField] private CameraMovement cameraMovement;

        private void Awake()
        {
            playerController.SetupController();
            playerController.SetupPlayer(player);
            
            cameraMovement.SetTarget(player.transform);
        }
    }
}

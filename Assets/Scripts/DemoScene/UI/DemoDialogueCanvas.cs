using System;
using System.Collections.Generic;
using Demo.Input;
using MSW.Events;
using MSW.Unity.Dialogue;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Demo.UI
{
    public class DemoDialogueCanvas : DialogueCanvas, IInput
    {
        public override void UpdateCanvas(string speaker, string line)
        {
            if (!this.gameObject.activeSelf)
            {
                this.SwitchControlMap?.Invoke("UI");
            }
            
            base.UpdateCanvas(speaker, line);
            this.EnableInput();
        }

        public override void CleanupCanvas()
        {
            this.SwitchControlMap?.Invoke("Player");
            
            base.CleanupCanvas();
        }

        private InputAction continueInput;
        
        #region INPUT
        
        public Action<string> SwitchControlMap { get; set; }

        public void SetupInput(InputSystem_Actions inputs)
        {
            this.continueInput = inputs.UI.Submit;
            
            this.ContinueAction = new RunnerEvent();
        }

        public void EnableInput()
        {
            this.continueInput.started += HandleInput_Submit;
        }

        public void DisableInput()
        {
            this.continueInput.started -= HandleInput_Submit;
        }
        
        private void HandleInput_Submit(InputAction.CallbackContext obj)
        {
            this.DisableInput();
            this.ContinueAction.FireEvent(this, new RunnerEventArgs(new List<object>() {obj.action.name}));
        }
        
        #endregion
    }
}

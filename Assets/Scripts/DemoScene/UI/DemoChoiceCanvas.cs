using Demo.Input;
using MSW.Unity.Dialogue;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Demo.UI
{
    public class DemoChoiceCanvas : ChoiceCanvas, IInput
    {
        private InputAction continueInput;
        private InputAction movementInput;
        public Action<string> SwitchControlMap { get; set; }

        public override void SetupChoices(IEnumerable<string> choices)
        {
            if (!this.gameObject.activeSelf)
            {
                this.gameObject.SetActive(true);
                this.EnableInput();
            }

            base.SetupChoices(choices);
        }

        public override void CleanupCanvas()
        {
            this.DisableInput();
            base.CleanupCanvas();
        }

        public void SetupInput(InputSystem_Actions inputs)
        {
            this.continueInput = inputs.UI.Submit;
            this.movementInput = inputs.UI.Navigate;
        }

        public void EnableInput()
        {
            this.continueInput.started += HandleInput_Submit;
            this.movementInput.performed += HandleInput_Navigate;
        }

        private void HandleInput_Navigate(InputAction.CallbackContext ctx)
        {
            if(this.gameObject.activeSelf)
            {
                Vector2 obj = ctx.ReadValue<Vector2>();

                this.ChangeSelection(obj.y);
            }
        }

        public void DisableInput()
        {
            this.continueInput.started -= HandleInput_Submit;
            this.movementInput.performed -= HandleInput_Navigate;
        }

        private void HandleInput_Submit(InputAction.CallbackContext ctx)
        {
            if(this.gameObject.activeSelf)
            {
                int index = this.SelectChoice();

                this.OnChoiceSelected?.Invoke(index);
            }
        }
    }
}

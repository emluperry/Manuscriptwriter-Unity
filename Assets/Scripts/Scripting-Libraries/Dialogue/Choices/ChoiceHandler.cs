using UnityEngine;
using MSW.Input;
using System;
using MSW.Reflection;
using System.Collections.Generic;

namespace MSW.Unity.Dialogue
{
    public class ChoiceHandler : MonoBehaviour, IChoiceHandler
    {
        [SerializeField] private ChoiceCanvas canvas;
        public Action<object, int> OnChoiceSet { get; set; }

        public void Setup()
        {

        }

        public void ShowChoices(Context ctx, List<string> choices)
        {
            this.canvas.SetupChoices(choices);
            this.canvas.OnChoiceSelected += Handle_ChoiceSelected;
        }

        private void Handle_ChoiceSelected(int index)
        {
            this.canvas.OnChoiceSelected -= Handle_ChoiceSelected;
            this.canvas.CleanupCanvas();

            this.OnChoiceSet?.Invoke(this, index);
        }
    }
}

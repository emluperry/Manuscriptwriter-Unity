using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MSW.Unity.Dialogue
{
    public class ChoiceCanvas : MonoBehaviour
    {
        [SerializeField] private GameObject choicePrefab;

        [Header("UI References")]
        [SerializeField] private ListControl listControl;

        private int choiceCount = 0;

        private void Start()
        {
            this.gameObject.SetActive(false);
        }

        public virtual void SetupChoices(IEnumerable<string> choices)
        {
            if (!this.listControl)
            {
                return;
            }

            choiceCount = 0;

            var choiceObjects = new List<ChoiceItemViewModel>();
            foreach (var choice in choices)
            {
                choiceObjects.Add(new ChoiceItemViewModel(choice));
                choiceCount++;
            }

            this.listControl.LoadData(choiceObjects);

            if(!this.gameObject.activeSelf)
            {
                this.gameObject.SetActive(true);
            }
        }

        public void ChangeSelection(float direction)
        {
            if (direction < 0)
            {
                this.listControl.MoveToNextItem();
            }
            else if (direction > 0)
            {
                this.listControl.MoveToPrevItem();
            }
        }

        public Action<int> OnChoiceSelected;
        public int SelectChoice()
        {
            int index = this.listControl.GetCurrentItemIndex();
            this.OnChoiceSelected?.Invoke(index);
            return index;
        }

        public virtual void CleanupCanvas()
        {
            this.StartCoroutine(Exit());
        }

        public IEnumerator Exit()
        {
            yield return this.listControl.c_DisableControl();
            this.gameObject.SetActive(false);
        }
    }
}

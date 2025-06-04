using System.Collections;
using TMPro;
using UnityEngine;

namespace MSW.Unity.Dialogue
{
    public class ListItem : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] public RectTransform SelectionPivot;

        [SerializeField] private TextMeshProUGUI nameControl;

        public IItemViewModel ItemViewModel { get; private set; }
        public bool CanUnload;

        public virtual void SetViewModel(IItemViewModel itemViewModel)
        {
            this.CanUnload = false;

            if (this.ItemViewModel == itemViewModel)
            {
                return;
            }

            this.ItemViewModel = itemViewModel;

            if (this.ItemViewModel == null)
            {
                return;
            }

            if (this.nameControl)
            {
                this.nameControl.text = this.ItemViewModel.Name;
            }
        }

        public virtual void Select()
        {

        }

        public virtual void Deselect()
        {

        }

        public virtual IEnumerator Enter()
        {
            yield break;
        }

        public virtual IEnumerator Exit()
        {
            this.CanUnload = true;
            yield break;
        }
    }
}

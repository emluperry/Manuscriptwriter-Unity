using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MSW.Unity.Dialogue
{
    public class ListControl : MonoBehaviour
    {
        protected int totalMaxItems = 0;
        protected int currentIndex = 0;
        protected int currentMax = 0;

        protected IItemViewModel[] viewModels;

        [SerializeField] private bool startAtInvalidIndex = false;
        [Header("References")]
        [SerializeField] protected LayoutGroup itemLayoutGroup;
        protected List<ListItem> items;
        [SerializeField] protected Image cursor;

        private void Awake()
        {
            this.items = this.itemLayoutGroup.GetComponentsInChildren<ListItem>().ToList();
            this.totalMaxItems = this.items.Count;

            foreach (ListItem child in this.items)
            {
                child.gameObject.SetActive(false);
            }
        }

        public virtual void LoadData(IReadOnlyList<IItemViewModel> data)
        {
            currentIndex = startAtInvalidIndex ? -1 : 0;

            var dataCount = data.Count;
            if (dataCount <= 0)
            {
                return;
            }

            viewModels = new IItemViewModel[dataCount];
            viewModels = data.ToArray();
            this.SetupItems();

            if (startAtInvalidIndex)
            {
                this.cursor?.gameObject.SetActive(false);
            }

            this.StartCoroutine(c_FinalizeLoad());
        }

        protected virtual void SetupItems()
        {
            // update items to reflect new items

            this.currentMax = 0;
            for (int i = 0; i < this.totalMaxItems; i++)
            {
                var vm = this.viewModels[i];
                this.items[i].SetViewModel(vm);

                if (vm != null)
                {
                    currentMax++;
                    this.items[i].gameObject.SetActive(true);
                }
                else
                {
                    this.items[i].gameObject.SetActive(false);
                }
            }
        }

        protected IEnumerator c_FinalizeLoad()
        {
            yield return new WaitForEndOfFrame();

            if (!startAtInvalidIndex)
            {
                this.MoveCursorToIndex(currentIndex);
            }

            foreach (var listItem in this.items)
            {
                StartCoroutine(listItem.Enter());
            }
        }

        public IEnumerator c_DisableControl()
        {
            foreach (var listItem in this.items)
            {
                if (!listItem.enabled)
                {
                    continue;
                }

                StartCoroutine(listItem.Exit());
            }

            var itemsInUse = this.items.Where(i => i.gameObject.activeSelf);
            yield return new WaitUntil(() => itemsInUse.All(i => i.CanUnload));
        }

        protected void MoveCursorToIndex(int index)
        {
            if (this.currentIndex != -1)
            {
                var currentItem = this.items[this.currentIndex];
                currentItem.Deselect();
            }

            this.currentIndex = index;

            var indexItem = this.items[this.currentIndex];
            
            // could call Select on a ListItem to get it to handle any local animation/highlighted action?
            if (this.cursor)
            {
                this.cursor.transform.position = indexItem.SelectionPivot.position;
            }

            indexItem.Select();
        }

        public void MoveToPrevItem()
        {
            var newIndex = currentIndex - 1;
            if (newIndex < 0)
            {
                newIndex = currentMax - 1;
            }

            this.MoveCursorToIndex(newIndex);
        }

        public void MoveToNextItem()
        {
            var newIndex = currentIndex + 1;
            if (newIndex >= currentMax)
            {
                newIndex = 0;
            }

            this.MoveCursorToIndex(newIndex);
        }

        public virtual IItemViewModel GetCurrentItem()
        {
            if (this.currentIndex == -1)
            {
                return null;
            }

            return this.viewModels.Length == 0 ? null : this.viewModels[this.currentIndex];
        }

        public virtual int GetCurrentItemIndex()
        {
            return this.viewModels.Length == 0 ? -1 : this.currentIndex;
        }
    }
}

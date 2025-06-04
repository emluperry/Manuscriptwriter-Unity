using UnityEngine;

namespace MSW.Unity.Dialogue
{
    public class DynamicListControl : ListControl
    {
        [Header("References")]
        [SerializeField] private GameObject listPrefab;

        protected override void SetupItems()
        {
            this.currentMax = this.viewModels.Length;

            int numToCreate = this.currentMax - this.totalMaxItems;
            // update items to reflect new items
            for (int i = 0; i < numToCreate; i++)
            {
                var newChoice = Instantiate(listPrefab, new Vector3(0, 0, 0), Quaternion.identity, this.itemLayoutGroup.transform);

                this.items.Add(newChoice.GetComponent<ListItem>());
                this.totalMaxItems++;
            }

            for (int i = 0; i < this.currentMax; i++)
            {
                var vm = this.viewModels[i];
                this.items[i].SetViewModel(vm);

                this.items[i].gameObject.SetActive(vm != null);
            }
        }
    }
}

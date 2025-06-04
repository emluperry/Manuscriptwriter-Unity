using UnityEngine;

namespace MSW.Unity.Dialogue
{
    public class ChoiceItemViewModel : IItemViewModel
    {
        public ChoiceItemViewModel(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public Sprite Thumbnail { get; set; }
        public Sprite Icon { get; set; }
        public int Count { get; set; }
    }
}

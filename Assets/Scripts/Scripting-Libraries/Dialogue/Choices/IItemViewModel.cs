using UnityEngine;

namespace MSW.Unity.Dialogue
{
    public interface IItemViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Sprite Thumbnail { get; set; }
        public Sprite Icon { get; set; }
        public int Count { get; set; }
    }
}
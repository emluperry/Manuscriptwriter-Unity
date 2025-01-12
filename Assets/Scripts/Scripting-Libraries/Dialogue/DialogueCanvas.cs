using MSW.Events;
using TMPro;
using UnityEngine;

namespace MSW.Unity.Dialogue
{
    public class DialogueCanvas : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI speakerTextBox;
        [SerializeField] private TextMeshProUGUI dialogueTextBox;
        
        public RunnerEvent ContinueAction;

        public virtual void UpdateCanvas(string speaker, string line)
        {
            if (!this.gameObject.activeSelf)
            {
                this.gameObject.SetActive(true);
            }
            
            this.speakerTextBox.text = speaker;
            this.dialogueTextBox.text = line;
        }
        
        public virtual void CleanupCanvas()
        {
            this.gameObject.SetActive(false);
        }
    }
}

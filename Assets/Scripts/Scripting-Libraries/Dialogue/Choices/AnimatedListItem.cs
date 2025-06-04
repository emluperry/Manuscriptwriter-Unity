using System.Collections;
using UnityEngine;

namespace MSW.Unity.Dialogue
{
    public class AnimatedListItem : ListItem
    {
        private Animator itemAnimator;
        private static readonly int onSelect = Animator.StringToHash("OnSelect");
        private static readonly int onDeselect = Animator.StringToHash("OnDeselect");
        private static readonly int onExit = Animator.StringToHash("OnExit");
        private static readonly int onEntry = Animator.StringToHash("OnEntry");

        private void Awake()
        {
            this.itemAnimator = this.GetComponent<Animator>();
        }

        public override void Select()
        {
            base.Select();

            this.itemAnimator.ResetTrigger(onDeselect);
            this.itemAnimator.SetTrigger(onSelect);
        }

        public override void Deselect()
        {
            base.Deselect();

            this.itemAnimator.ResetTrigger(onSelect);
            this.itemAnimator.SetTrigger(onDeselect);
        }

        public override IEnumerator Enter()
        {
            this.itemAnimator.SetTrigger(onEntry);

            yield return base.Enter();
        }

        public override IEnumerator Exit()
        {
            this.itemAnimator.ResetTrigger(onEntry);
            this.itemAnimator.SetTrigger(onExit);

            yield return new WaitUntil(() => this.itemAnimator.GetCurrentAnimatorStateInfo(0).IsName("ChoiceExit-Animation"));
            yield return new WaitUntil(() => this.itemAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);

            yield return base.Exit();
        }
    }
}

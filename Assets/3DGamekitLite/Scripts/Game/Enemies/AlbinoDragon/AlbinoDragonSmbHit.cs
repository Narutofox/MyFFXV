using UnityEngine;

namespace Gamekit3D
{
    public class AlbinoDragonSmbHit : SceneLinkedSMB<AlbinoDragonBehavior>
    {
        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.ResetTrigger(AlbinoDragonBehavior.BasicAttack);
            animator.ResetTrigger(AlbinoDragonBehavior.ClawAttack);
            animator.ResetTrigger(AlbinoDragonBehavior.FlameAttack);
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.controller.ClearForce();
        }
    }
}
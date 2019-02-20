using UnityEngine;

namespace Gamekit3D
{
    public class AlbinoDragonSmbAttack : SceneLinkedSMB<AlbinoDragonBehavior>
    {
        protected Vector3 AttackPosition;

        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnSLStateEnter(animator, stateInfo, layerIndex);

            m_MonoBehaviour.controller.SetFollowNavmeshAgent(false);

            AttackPosition = m_MonoBehaviour.target.transform.position;
            Vector3 toTarget = AttackPosition - m_MonoBehaviour.transform.position;
            toTarget.y = 0;

            m_MonoBehaviour.transform.forward = toTarget.normalized;
            m_MonoBehaviour.controller.SetForward(m_MonoBehaviour.transform.forward);


            if (m_MonoBehaviour.CurrentAttackAudio != null)
                m_MonoBehaviour.CurrentAttackAudio.PlayRandomClip();
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnSLStateExit(animator, stateInfo, layerIndex);

            if (m_MonoBehaviour.CurrentAttackAudio != null)
                m_MonoBehaviour.CurrentAttackAudio.audioSource.Stop();

            m_MonoBehaviour.controller.SetFollowNavmeshAgent(true);
        }
    }
}
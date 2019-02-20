using UnityEngine;

namespace Gamekit3D
{
    public class AlbinoDragonSmbIdle : SceneLinkedSMB<AlbinoDragonBehavior>
    {
        public float MinimumIdleGruntTime = 2.0f;
        public float MaximumIdleGruntTime = 5.0f;

        protected float RemainingToNextGrunt = 0.0f;

        public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            if (MinimumIdleGruntTime > MaximumIdleGruntTime)
                MinimumIdleGruntTime = MaximumIdleGruntTime;

            RemainingToNextGrunt = Random.Range(MinimumIdleGruntTime, MaximumIdleGruntTime);
        }

        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex);
            if (m_MonoBehaviour == null)
            {
                return;
            }
            RemainingToNextGrunt -= Time.deltaTime;

            if (RemainingToNextGrunt < 0)
            {
                RemainingToNextGrunt = Random.Range(MinimumIdleGruntTime, MaximumIdleGruntTime);
                m_MonoBehaviour.Grunt();
            }

            m_MonoBehaviour.FindTarget();
            if (m_MonoBehaviour.target != null)
            {
                m_MonoBehaviour.StartPursuit();
            }
        }
    }
}
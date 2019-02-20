using UnityEngine;
using UnityEngine.AI;

namespace Gamekit3D
{
    public class AlbinoDragonSmbPursuit : SceneLinkedSMB<AlbinoDragonBehavior>
    {
        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex);

            m_MonoBehaviour.FindTarget();

            if (m_MonoBehaviour.controller.navmeshAgent.pathStatus == NavMeshPathStatus.PathPartial 
                || m_MonoBehaviour.controller.navmeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
            {
                m_MonoBehaviour.StopPursuit();
                return;
            }

            if (m_MonoBehaviour.target == null || m_MonoBehaviour.target.respawning)
            {//if the target was lost or is respawning, we stop the pursit
                m_MonoBehaviour.StopPursuit();
            }
            else
            {
                m_MonoBehaviour.RequestTargetPosition();

                Vector3 toTarget = m_MonoBehaviour.target.transform.position - m_MonoBehaviour.transform.position;

                if (toTarget.sqrMagnitude < m_MonoBehaviour.AttackDistance * m_MonoBehaviour.AttackDistance)
                {
                    m_MonoBehaviour.controller.navmeshAgent.updatePosition = false;
                    m_MonoBehaviour.controller.navmeshAgent.updateRotation = false;
                    m_MonoBehaviour.TriggerAttack();
                }
                else if (m_MonoBehaviour.followerData.assignedSlot != -1)
                {
                    m_MonoBehaviour.AttackEnd();
                    Vector3 targetPoint = m_MonoBehaviour.target.transform.position + 
                        m_MonoBehaviour.followerData.distributor.GetDirection(m_MonoBehaviour.followerData
                            .assignedSlot) * m_MonoBehaviour.AttackDistance * 0.9f;                  
                    m_MonoBehaviour.controller.SetTarget(targetPoint);
                    m_MonoBehaviour.controller.SetRotation(targetPoint);
                   
                    m_MonoBehaviour.controller.SetMove(toTarget * Time.deltaTime);
                }
                else
                {
                    m_MonoBehaviour.StopPursuit();
                }
            }
        }
    }
}
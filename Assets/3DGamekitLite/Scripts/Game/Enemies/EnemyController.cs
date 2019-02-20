using System;
using System.Collections;
using Assets._3DGamekitLite.Scripts.Game.Enemies;
using Assets._3DGamekitLite.Scripts.Game.MagicSystem;
using Assets._3DGamekitLite.Scripts.Game.TargetSystem;
using UnityEngine;
using UnityEngine.AI;

namespace Gamekit3D
{
//this assure it's runned before any behaviour that may use it, as the animator need to be fecthed
    [DefaultExecutionOrder(-1)]
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyController : MonoBehaviour, IMagicEffects
    {
        public bool interpolateTurning = false;
        public bool applyAnimationRotation = false;

        public Animator animator { get { return m_Animator; } }
        public Vector3 externalForce { get { return m_ExternalForce; } }
        public NavMeshAgent navmeshAgent { get { return m_NavMeshAgent; } }
        public bool followNavmeshAgent { get { return m_FollowNavmeshAgent; } }
        public bool grounded { get { return m_Grounded; } }

        public bool Burning { get; private set; }
        public bool Freezing { get; private set; }
        public bool Electrocuted { get; private set; }

        protected NavMeshAgent m_NavMeshAgent;
        protected bool m_FollowNavmeshAgent;
        protected Animator m_Animator;
        protected bool m_UnderExternalForce;
        protected bool m_ExternalForceAddGravity = true;
        protected Vector3 m_ExternalForce;
        protected bool m_Grounded;

        protected Rigidbody m_Rigidbody;

        const float k_GroundedRayDistance = .8f;
        private Camera _mainCamera;
        internal TargetController _targetController;
        private PlayerController _playerController;
        private IEnemyBehavior _behaviorScript;
        void Start()
        {
            _mainCamera = Camera.main;
            _targetController = FindObjectOfType<TargetController>();
            _playerController = FindObjectOfType<PlayerController>();
            _behaviorScript = GetComponent<IEnemyBehavior>();
        }
        void OnEnable()
        {
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
            m_Animator = GetComponent<Animator>();
            m_Animator.updateMode = AnimatorUpdateMode.AnimatePhysics;

            m_NavMeshAgent.updatePosition = false;

            m_Rigidbody = GetComponentInChildren<Rigidbody>();
            if (m_Rigidbody == null)
                m_Rigidbody = gameObject.AddComponent<Rigidbody>();

            m_Rigidbody.isKinematic = true;
            m_Rigidbody.useGravity = false;
            m_Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

            m_FollowNavmeshAgent = true;
        }

       

        private void Update()
        {
            Targeting();
        }

        private void Targeting()
        {
            //First Create A Vector3 With Dimensions Based On The Camera's Viewport
            Vector3 enemyPosition = _mainCamera.WorldToViewportPoint(gameObject.transform.position);
            Vector3 playerPosition = _mainCamera.WorldToViewportPoint(_playerController.transform.position);

            //If The X And Y Values Are Between 0 And 1, The Enemy Is On Screen
            bool onScreen = enemyPosition.z > 0 && enemyPosition.x > 0 && enemyPosition.x < 1 && enemyPosition.y > 0 && enemyPosition.y < 1;
            float distance = Vector3.Distance(enemyPosition, playerPosition);

            //If The Enemy Is On Screen and within specific distance add It To The List Of Nearby Enemies if not remove it from list
            if (onScreen && distance < 15)
            {
                _targetController.AddEnemyToList(this);
            }
            else
            {
                _targetController.RemoveEnemyToList(GetInstanceID());
            }
        }

        private void FixedUpdate()
        {
            animator.speed = PlayerInput.Instance != null && PlayerInput.Instance.HaveControl() ? 1.0f : 0.0f;

            CheckGrounded();

            if (m_UnderExternalForce)
                ForceMovement();
        }

        void CheckGrounded()
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position + Vector3.up * k_GroundedRayDistance * 0.5f, -Vector3.up);
            m_Grounded = Physics.Raycast(ray, out hit, k_GroundedRayDistance, Physics.AllLayers,
                QueryTriggerInteraction.Ignore);
        }

        void ForceMovement()
        {
            if(m_ExternalForceAddGravity)
                m_ExternalForce += Physics.gravity * Time.deltaTime;

            RaycastHit hit;
            Vector3 movement = m_ExternalForce * Time.deltaTime;
            if (!m_Rigidbody.SweepTest(movement.normalized, out hit, movement.sqrMagnitude))
            {
                m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
            }

            m_NavMeshAgent.Warp(m_Rigidbody.position);
        }

        private void OnAnimatorMove()
        {
            if (m_UnderExternalForce)
                return;

            if (m_FollowNavmeshAgent)
            {
                m_NavMeshAgent.speed = (m_Animator.deltaPosition / Time.deltaTime).magnitude;
                transform.position = m_NavMeshAgent.nextPosition;
            }
            else
            {
                RaycastHit hit;
                if (!m_Rigidbody.SweepTest(m_Animator.deltaPosition.normalized, out hit,
                    m_Animator.deltaPosition.sqrMagnitude))
                {
                    m_Rigidbody.MovePosition(m_Rigidbody.position + m_Animator.deltaPosition);
                }
            }

            if (applyAnimationRotation)
            {
                transform.forward = m_Animator.deltaRotation * transform.forward;
            }
        }

        // used to disable position being set by the navmesh agent, for case where we want the animation to move the enemy instead (e.g. Chomper attack)
        public void SetFollowNavmeshAgent(bool follow)
        {
            if (!follow && m_NavMeshAgent.enabled)
            {
                m_NavMeshAgent.ResetPath();
            }
            else if(follow && !m_NavMeshAgent.enabled)
            {
                m_NavMeshAgent.Warp(transform.position);
            }

            m_FollowNavmeshAgent = follow;
            m_NavMeshAgent.enabled = follow;
        }

        public void AddForce(Vector3 force, bool useGravity = true)
        {
            if (m_NavMeshAgent.enabled)
                m_NavMeshAgent.ResetPath();

            m_ExternalForce = force;
            m_NavMeshAgent.enabled = false;
            m_UnderExternalForce = true;
            m_ExternalForceAddGravity = useGravity;
        }

        public void ClearForce()
        {
            m_UnderExternalForce = false;
            m_NavMeshAgent.enabled = true;
        }

        public void SetForward(Vector3 forward)
        {
            Quaternion targetRotation = Quaternion.LookRotation(forward);

            if (interpolateTurning)
            {
                targetRotation = Quaternion.RotateTowards(transform.rotation, targetRotation,
                    m_NavMeshAgent.angularSpeed * Time.deltaTime);
            }

            transform.rotation = targetRotation;
        }

        public void SetTarget(Vector3 position)
        {
            m_NavMeshAgent.destination = position;
        }

        public void SetMove(Vector3 offset)
        {
            m_NavMeshAgent.Move(offset);
        }

        internal void SetRotation(Vector3 targetPoint)
        {
            Vector3 direction = (targetPoint - transform.position).normalized;
            Quaternion qDir = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, qDir, Time.deltaTime * 5f);
        }

        void OnDestroy()
        {
            _targetController.RemoveEnemyToList(GetInstanceID());
        }

        public IEnumerator OnBurn(float duration, int damageCount, int damageAmount)
        {
            if (Burning)
            {
                yield break;
            }

            int currentCount = 0;
            Burning = true;

            if (_behaviorScript != null)
            {
                while (currentCount < damageCount)
                {
                    Damageable.DamageMessage damageMessage = new Damageable.DamageMessage
                    {
                        amount = damageAmount,
                        damageSource = transform.position
                    };

                    _behaviorScript.ApplyDamage(damageMessage);
                    yield return new WaitForSeconds(duration);
                    currentCount++;
                }
            }
            

            Burning = false;
        }

        public IEnumerator OnSlow(float duration, float slowRate)
        {
            if (Freezing)
            {
                yield break;
            }

            Freezing = true;

            float originalDrag = m_Rigidbody.drag;
            if (m_Rigidbody.drag < 1f)
            {
                m_Rigidbody.drag = 1;
            }
            m_Rigidbody.drag *= slowRate;

            yield return new WaitForSeconds(duration);

            m_Rigidbody.drag = originalDrag;
            Freezing = false;
        }

        public IEnumerator OnParalyze(float duration)
        {
            if (Electrocuted)
            {
                yield break;
            }

            Electrocuted = true;
            float originalDrag = m_Rigidbody.drag;
            m_Rigidbody.drag = 50f;

            yield return new WaitForSeconds(duration);

            m_Rigidbody.drag = originalDrag;
            Electrocuted = false;
        }
    }
}
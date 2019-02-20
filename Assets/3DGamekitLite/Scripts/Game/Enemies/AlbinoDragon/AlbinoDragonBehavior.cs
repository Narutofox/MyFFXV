using Assets._3DGamekitLite.Scripts.Game.Enemies;
using Assets._3DGamekitLite.Scripts.Game.MagicSystem;
using Gamekit3D.Message;
using UnityEngine;

namespace Gamekit3D
{
    [DefaultExecutionOrder(100)]
    public class AlbinoDragonBehavior : MonoBehaviour, IMessageReceiver, IEnemyBehavior
    {
        public static readonly int Die = Animator.StringToHash("Die");
        public static readonly int GetHit = Animator.StringToHash("Get Hit");
        public static readonly int FlameAttack = Animator.StringToHash("Flame Attack");
        public static readonly int ClawAttack = Animator.StringToHash("Claw Attack");
        public static readonly int Sleep = Animator.StringToHash("Sleep");
        public static readonly int Walk = Animator.StringToHash("Walk");
        public static readonly int Run = Animator.StringToHash("Run");
        public static readonly int BasicAttack = Animator.StringToHash("Basic Attack");
        public static readonly int SpottedTrigger = Animator.StringToHash("Spotted");

        public EnemyController controller { get { return Controller; } }

        public PlayerController target { get { return Target; } }
        public TargetDistributor.TargetFollower followerData { get { return FollowerInstance; } }

        protected PlayerController Target = null;
        protected EnemyController Controller;
        protected TargetDistributor.TargetFollower FollowerInstance = null;

        public Vector3 OriginalPosition { get; protected set; }
        [System.NonSerialized]
        public float AttackDistance = 5;

        public MeleeWeapon BasicWeapon;
        public MeleeWeapon ClawWeapon1;
        public MeleeWeapon ClawWeapon2;

        private int _currentAttack;
        internal RandomAudioPlayer CurrentAttackAudio;

        public TargetScanner PlayerScanner;
        [Tooltip("Time in seconde before the Dragon stop pursuing the player when the player is out of sight")]
        public float TimeToStopPursuit;

        [Header("Audio")]
        public RandomAudioPlayer AttackAudio;
        public RandomAudioPlayer ClawAttackAudio;
        public RandomAudioPlayer HitAudio;
        public RandomAudioPlayer GruntAudio;
        public RandomAudioPlayer DeathAudio;
        public RandomAudioPlayer SpottedAudio;
        public bool HealOnDeath;

        protected float TimerSinceLostTarget = 0.0f;
        protected void OnEnable()
        {
            Controller = GetComponentInChildren<EnemyController>();

            OriginalPosition = transform.position;

            BasicWeapon.SetOwner(gameObject);
            ClawWeapon1.SetOwner(gameObject);
            ClawWeapon2.SetOwner(gameObject);

            Controller.animator.Play(Sleep, 0, Random.value);

            SceneLinkedSMB<AlbinoDragonBehavior>.Initialise(Controller.animator, this);
        }

        /// <summary>
        /// Called by animation events.
        /// </summary>
        public void Grunt()
        {
            if (GruntAudio != null)
                GruntAudio.PlayRandomClip();
        }

        public void Spotted()
        {
            if (SpottedAudio != null)
                SpottedAudio.PlayRandomClip();
        }

        protected void OnDisable()
        {
            if (FollowerInstance != null)
                FollowerInstance.distributor.UnregisterFollower(FollowerInstance);
        }

        private void FixedUpdate()
        {
            Vector3 toBase = OriginalPosition - transform.position;
            toBase.y = 0;
            if (toBase.sqrMagnitude < 0.1 * 0.1f && FollowerInstance != null && !FollowerInstance.requireSlot)
            {
                Controller.animator.SetBool(Walk, false);
            }
        }

        public void FindTarget()
        {
            //we ignore height difference if the target was already seen
            PlayerController detectedTarget = PlayerScanner.Detect(transform, Target == null);

            if (Target == null)
            {
                //we just saw the player for the first time, pick an empty spot to target around them
                if (detectedTarget != null)
                {
                    Controller.animator.SetTrigger(SpottedTrigger);
                    Target = detectedTarget;
                    TargetDistributor distributor = detectedTarget.GetComponentInChildren<TargetDistributor>();
                    if (distributor != null)
                        FollowerInstance = distributor.RegisterNewFollower();
                }
            }
            else
            {
                //we lost the target. But chomper have a special behaviour : they only loose the player scent if they move past their detection range
                //and they didn't see the player for a given time. Not if they move out of their detectionAngle. So we check that this is the case before removing the target
                if (detectedTarget == null)
                {
                    TimerSinceLostTarget += Time.deltaTime;

                    if (TimerSinceLostTarget >= TimeToStopPursuit)
                    {
                        Vector3 toTarget = Target.transform.position - transform.position;

                        if (toTarget.sqrMagnitude > PlayerScanner.detectionRadius * PlayerScanner.detectionRadius)
                        {
                            if (FollowerInstance != null)
                                FollowerInstance.distributor.UnregisterFollower(FollowerInstance);

                            //the target move out of range, reset the target
                            Target = null;
                        }
                    }
                }
                else
                {
                    if (detectedTarget != Target)
                    {
                        if (FollowerInstance != null)
                            FollowerInstance.distributor.UnregisterFollower(FollowerInstance);

                        Target = detectedTarget;

                        TargetDistributor distributor = detectedTarget.GetComponentInChildren<TargetDistributor>();
                        if (distributor != null)
                            FollowerInstance = distributor.RegisterNewFollower();
                    }

                    TimerSinceLostTarget = 0.0f;
                }
            }
        }

        public void StartPursuit()
        {
            
            if (FollowerInstance != null)
            {
                FollowerInstance.requireSlot = true;
                RequestTargetPosition();
            }
            Controller.animator.SetBool(Walk, false);
            Controller.animator.SetBool(Run,true);
        }

        public void StopPursuit()
        {
            Controller.animator.ResetTrigger(SpottedTrigger);
            if (FollowerInstance != null)
            {
                FollowerInstance.requireSlot = false;
            }
            Controller.animator.SetBool(Run, false);
        }

        public void RequestTargetPosition()
        {
            Vector3 fromTarget = transform.position - Target.transform.position;
            fromTarget.y = 0;

            FollowerInstance.requiredPoint = Target.transform.position + fromTarget.normalized * AttackDistance * 0.9f;
        }

        public void WalkBackToBase()
        {
            if (FollowerInstance != null)
                FollowerInstance.distributor.UnregisterFollower(FollowerInstance);
            Target = null;
            StopPursuit();
            Controller.SetTarget(OriginalPosition);
            Controller.SetMove(OriginalPosition);
            Controller.SetFollowNavmeshAgent(true);
            Controller.animator.SetBool(Walk, true);
        }

        public void TriggerAttack()
        {
           _currentAttack = Random.Range(1, 3) == 1 ? BasicAttack : ClawAttack;

            CurrentAttackAudio = _currentAttack == BasicAttack ? AttackAudio : ClawAttackAudio;

            Controller.animator.SetTrigger(_currentAttack);
            AttackBegin();
        }

        public void AttackBegin()
        {
            if (_currentAttack == BasicAttack)
            {
                BasicWeapon.BeginAttack(false);
            }
            else if (_currentAttack == ClawAttack)
            {
                ClawWeapon1.BeginAttack(true);
                ClawWeapon2.BeginAttack(true);
            }
        }

        public void AttackEnd()
        {
            if (_currentAttack == BasicAttack)
            {
                BasicWeapon.EndAttack();
            }
            else if (_currentAttack == ClawAttack)
            {
                ClawWeapon1.EndAttack();
                ClawWeapon2.EndAttack();
            }

            _currentAttack = 0;
        }

        public void OnReceiveMessage(Gamekit3D.Message.MessageType type, object sender, object msg)
        {
            switch (type)
            {
                case Gamekit3D.Message.MessageType.DEAD:
                    Death((Damageable.DamageMessage)msg);
                    break;
                case Gamekit3D.Message.MessageType.DAMAGED:
                    ApplyDamage((Damageable.DamageMessage)msg);
                    break;
            }
        }

        public void Death(Damageable.DamageMessage msg)
        {

            //Vector3 pushForce = transform.position - msg.damageSource;

            //pushForce.y = 0;

            //transform.forward = -pushForce.normalized;
            //controller.AddForce(pushForce.normalized * 7.0f - Physics.gravity * 0.6f);

            controller.animator.SetTrigger(Die);
            if (HealOnDeath && Target != null)
            {
                Target.gameObject.GetComponent<Damageable>().Heal(1,1);
            }

            //We unparent the hit source, as it would destroy it with the gameobject when it get replaced by the ragdol otherwise
            if (DeathAudio != null)
            {
                DeathAudio.transform.SetParent(null, true);
                DeathAudio.PlayRandomClip();
                Destroy(DeathAudio, DeathAudio.clip == null ? 0.0f : DeathAudio.clip.length + 0.5f);
            }                  
        }

        public void ApplyDamage(Damageable.DamageMessage msg)
        {
            //float verticalDot = Vector3.Dot(Vector3.up, msg.direction);
            //float horizontalDot = Vector3.Dot(transform.right, msg.direction);

            Vector3 pushForce = transform.position - msg.damageSource;

            pushForce.y = 0;

            transform.forward = -pushForce.normalized;
            controller.AddForce(pushForce.normalized * 5.5f, false);

            controller.animator.SetTrigger(GetHit);
           
            HitAudio.PlayRandomClip();
            MagicDamage(msg);
        }

        private void MagicDamage(Damageable.DamageMessage msg)
        {
            if (msg.effect != null)
            {
                switch (msg.effect)
                {
                    case Magic.MagicEffect.Burn:
                    {
                        StartCoroutine(controller.OnBurn(msg.magicEffectLasting, 1, msg.amount));
                        break;
                    }
                    case Magic.MagicEffect.Slow:
                    {
                        StartCoroutine(controller.OnSlow(msg.magicEffectLasting, 0.50f));
                        break;
                    }
                    case Magic.MagicEffect.Paralyze:
                    {
                        StartCoroutine(controller.OnParalyze(msg.magicEffectLasting));
                        break;
                    }
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            PlayerScanner.EditorGizmo(transform);
        }
#endif
    }
}

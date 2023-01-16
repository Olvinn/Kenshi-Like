using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Units.Views.IK;
using Units.Views.Ragdolls;
using Units.Weapons;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Units.Views
{
    public class UnitView : MonoBehaviour
    {
        public Unit Model { get; private set; }
        public List<UnitView> Sensed => sense.Views;
        public Vector3 Position { get; private set; }
        public Transform Target;
        public Action<List<UnitView>> OnHit;
        public GroundType GroundType { get; private set; }

        public Transform ViewTransform => transform;

        [field:SerializeField] public MovementStatus MovementStatus { get; private set; }
        
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private GameObject selection;
        [SerializeField] private UnitAttack attack;
        [SerializeField] private TriggerDetector sense;
        [SerializeField] private UnitAppearance appearance;
        [SerializeField] private Ragdoll ragdoll;
        [SerializeField] private IKController ik;
        [SerializeField] private AnimationController anim;
        [SerializeField] private ParticleSystem sparks, leak, splash;
        
        private Transform _destinationTransform;
        private Vector3 _destinationPos;
        private float _stopDistance;
        private float _cameraDistance;

        private void Awake()
        {
            selection.SetActive(false);
        }

        private void Start()
        {
            anim.OnHitBasic += HitBasic;
        }

        private void Update()
        {
            if (Model == null || !agent.enabled)
                return;

            var constants = GameContext.Instance.Constants;

            //Processing moving logic
            Position = transform.position;
            _cameraDistance = Vector3.Distance(Camera.main.transform.position, Position);
            
            bool stay = false;
            if (anim.CanMove && !agent.pathPending && MovementStatus == MovementStatus.Moving)
            {
                agent.SetDestination(_destinationTransform != null ? _destinationTransform.position : _destinationPos);

                if (agent.remainingDistance <= _stopDistance && MovementStatus == MovementStatus.Moving)
                {
                    MovementStatus = MovementStatus.Waiting;
                    _destinationTransform = null;
                    stay = true;
                }
            }
            else
            {
                stay = true;
            }
            agent.isStopped = stay;
            agent.stoppingDistance = _stopDistance;

            //Rotating unit on specific target
            if (Target != null)
            {
                Vector3 dir = Target.transform.position - Position;
                dir.y = 0;
                transform.rotation = Quaternion.RotateTowards(transform.rotation,Quaternion.LookRotation(dir),
                    Time.deltaTime * constants.AgentsAngularSpeed);
                ik.target = Target.transform;
            }
            else if (MovementStatus == MovementStatus.Moving)
            {
                ik.target = null;
                Vector3 dir = agent.velocity;
                dir.y = 0;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir),
                    Time.deltaTime * constants.AgentsAngularSpeed);
            }

            if (anim.State == AnimationControllerState.Attacking)
                attack.enabled = true;
            else
                attack.enabled = false;
            
            //Detecting ground type
            Ray ray = new Ray(Position, Vector3.down);
            RaycastHit[] hit = Physics.RaycastAll(ray, constants.DetectingGroundRayLength, constants.DetectingGroundLayerMask);
            if (hit.Length == 1 && hit[0].collider.gameObject.layer == 4)
            {
                GroundType = GroundType.Water;
            }
            else
            {
                GroundType = GroundType.Land;
            }

            //Update animations
            var v = transform.worldToLocalMatrix * agent.velocity;
            anim.UpdateMovingAnimation(v);

            agent.nextPosition = anim.AnimatorTransform.position;
            anim.AnimatorTransform.localPosition = Vector3.zero;

            if (_cameraDistance < constants.CameraDistanceCulling)
            {
                ik.enabled = true;
            }
            else
            {
                ik.enabled = false;
            }
        }

        private void OnDisable()
        {
            ik.enabled = false;
            ragdoll.StartRagdoll();
            if (agent != null)
            {
                agent.enabled = false;
                sense.enabled = false;
                attack.enabled = false;
            }

            var c = GetComponent<CapsuleCollider>();
            if (c == null)
                return;
            c.radius = 0f;
            c.enabled = false;
        }

        private void OnEnable()
        {
            ik.enabled = true;
            ragdoll.StopRagdoll();
            if (agent != null)
            {
                agent.enabled = true;
                sense.enabled = true;
                attack.enabled = true;
            }
            var c = GetComponent<CapsuleCollider>();
            if (c == null)
                return;
            c.radius = .5f;
            c.enabled = true;
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!GameContext.Instance.Constants.DebugGizmos)
                return;
            
            GUI.color = Color.black;
            GUI.backgroundColor = Color.white;
            if (Model != null)
            {
                Handles.Label(transform.position, $"{name}: {Model.HPPercentage}");
                var cs = Model.GetListOfCommands();
                float offset = .1f;
                foreach (var c in cs)
                {
                    Handles.Label(transform.position + Vector3.down * offset, $"Command: {c}");
                    offset += .1f;
                }
            }
        }
        #endif

        public void InjectModel(Unit model)
        {
            Model = model;
            agent.enabled = true;
        }

        /// <summary>
        /// Set the units appearance
        /// </summary>
        /// <param name="appearance"></param>
        public void SetAppearance(Appearance appearance)
        {
            var arr = Enum.GetValues(typeof(UnitColorType)).Cast<UnitColorType>().ToList();
            foreach (var color in arr)
            {
                Color metallic = appearance.GetColor(color) == appearance.GetColor(UnitColorType.Skin)
                    ? Color.white * .2f
                    : Color.black;
                this.appearance.SetColor(color, appearance.GetColor(color), color == UnitColorType.Eyes ? Color.white * .5f : metallic);
            }
        }

        /// <summary>
        /// Turns off most UnitView logic and plays dying animation
        /// </summary>
        public void Die()
        {
            StopAllCoroutines();
            anim.PerformDyingAnimation();
            Deselect();
            this.enabled = false;
        }

        /// <summary>
        /// Set destination for move to
        /// </summary>
        /// <param name="destination"></param>
        public void MoveTo(Vector3 destination, float stopDistance)
        {
            _destinationPos = destination;
            _destinationTransform = null;
            _stopDistance = stopDistance;
            MovementStatus = MovementStatus.Moving;
        }

        /// <summary>
        /// Set destination for move to
        /// </summary>
        /// <param name="destination"></param>
        public void MoveTo(Transform destination, float stopDistance)
        {
            _destinationTransform = destination;
            _stopDistance = stopDistance;
            MovementStatus = MovementStatus.Moving;
        }

        /// <summary>
        /// Select unit
        /// </summary>
        public void Select()
        {
            selection.SetActive(true);
            // appearance.gameObject.layer = 9;
        }

        /// <summary>
        /// Deselect unit
        /// </summary>
        public void Deselect()
        {
            selection.SetActive(false);
            // appearance.gameObject.layer = 3;
        }

        /// <summary>
        /// Play fight-ready animation
        /// </summary>
        public void PerformFightReadyAnimation()
        {
            anim.PerformFightReadyAnimation();
        }

        /// <summary>
        /// Play Idle animation
        /// </summary>
        public void PerformIdleAnimation()
        {
            anim.PerformIdleAnimation();
        }

        /// <summary>
        /// Checking can this unit attack target due their Views
        /// Multithreading safe
        /// </summary>
        /// <param name="target">Target which want to attack</param>
        /// <returns></returns>
        public bool CanAttack(Transform target)
        {
            return Vector3.Distance(target.position, Position) < GameContext.Instance.Constants.AttackDistance && 
                   anim.State is AnimationControllerState.Idle or AnimationControllerState.Blocking;
        }

        /// <summary>
        /// Checking animation state
        /// </summary>
        /// <returns></returns>
        public bool IsDodging()
        {
            return anim.State == AnimationControllerState.Dodging;
        }
        
        /// <summary>
        /// Checking animation state
        /// </summary>
        /// <returns></returns>
        public bool IsBlocking()
        {
            return anim.State == AnimationControllerState.Blocking;
        }
        
        /// <summary>
        /// Checking animation state
        /// </summary>
        /// <returns></returns>
        public bool IsAttacking()
        {
            return anim.State == AnimationControllerState.Attacking;
        }

        /// <summary>
        /// Play animation to hit enemy. If hit will successful, the callback will called.
        /// Multithreading safe
        /// </summary>
        /// <param name="callback">UnitViews that has been hit</param>
        public void StartHit(float attackRate, Action callback)
        {
            anim.PerformAttackAnimation(callback, attackRate);
        }

        /// <summary>
        /// Play animation blocking hits
        /// </summary>
        public void Block()
        {
            anim.PerformBlockAnimation();
        }

        /// <summary>
        /// Set unit max speed
        /// </summary>
        /// <param name="speed"></param>
        public void SetMaxSpeed(float speed)
        {
            agent.speed = speed;
        }

        /// <summary>
        /// Play animation representing taking damage. It will interrupt any other animation
        /// Multithreading safe
        /// </summary>
        public void GetDamage()
        {
            anim.PerformGetDamageAnimation(null);
            if (_cameraDistance < GameContext.Instance.Constants.CameraDistanceCulling)
            {
                splash.Play();
                leak.Play();
            }
        }

        /// <summary>
        /// Play dodging animation
        /// </summary>
        public void Dodge()
        {
            anim.PerformDodgeAnimation(null);
        }

        /// <summary>
        /// Rotate UnitView on target
        /// Multithreading safe
        /// </summary>
        /// <param name="target"></param>
        public void RotateOn(Transform target)
        {
            this.Target = target;
        }

        /// <summary>
        /// Teleports View on selected position
        /// </summary>
        /// <param name="pos"></param>
        public void Warp(Vector3 pos)
        {
            agent.Warp(pos);
        }

        /// <summary>
        /// Indicates of successful hit block
        /// Plays special effects and animations
        /// </summary>
        public void BlockComplete()
        {
            if (_cameraDistance < GameContext.Instance.Constants.CameraDistanceCulling)
                sparks.Play();
        }

        public void Swim()
        {
            anim.Swim();
        }

        public void Run()
        {
            anim.Run();
        }

        private void HitBasic()
        {
            OnHit?.Invoke(attack.GetUnitViewsFromBasicAttack());
        }
    }

    public enum MovementStatus
    {
        Waiting,
        Moving,
    }

    public enum GroundType
    {
        Land,
        Water
    }
}

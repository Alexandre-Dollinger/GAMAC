using System;
using System.Collections.Generic;
using _Scripts.GameManager;
using _Scripts.Health;
using _Scripts.Multiplayer;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Animations;

namespace _Scripts.Projectiles
{
    public class Projectile : UnitLocalHp
    {
        private bool _isServerProj;
        private readonly float _speedCoefficient = 100;
        private float Speed
        {
            get => Proj.Speed * _speedCoefficient;
        }

        private Rigidbody2D _rb;
        
        private string _objectTag;
        
        public Collider2D hitBoxCollider;
        public Collider2D findTargetCollider;

        private float _distanceToSpawn;

        private CircleCollider2D _findSenderCollider;
        private bool _searchingSender;
        private bool _finishedSearchingSender = false;
        private string _senderTag;
        
        private GameObject _senderGameObject = null;
        private Transform SenderTransform => _senderGameObject == null ? null : _senderGameObject.transform;
        
        private GameObject _fakeSenderGameObject = null;
        private Transform FakeSenderTransform => _fakeSenderGameObject == null ? null : _fakeSenderGameObject.transform;

        private bool _attachedConstraint = false;
        
        
        private List<Collider2D> _targetsFound = new List<Collider2D>();
        private bool _searchingTarget = false;
        
        private GameObject _targetGameObject = null;
        private Transform TargetTransform => _targetGameObject == null ? null : _targetGameObject.transform;
        
        private bool _initialised = false;
        public ProjectileStruct Proj;
        
        // still need sound, animation, sprite, collider
        public void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            findTargetCollider.enabled = false;
        }
        
        public void InitProjectile(ProjectileStruct projectileStruct, bool isServerProj = false)
        {
            Proj = projectileStruct;
            _initialised = true;

            MaxHp = Proj.GetMaxHpPrefab();
            CurrentHp = MaxHp;
            
            transform.localScale *= Proj.Scale;
            
            _objectTag = gameObject.tag;
            
            _isServerProj = isServerProj;

            _senderTag = GM.ProjM.GetSenderTag(Proj.SenderTag);

            if (Proj.IsBehindProjectile)
                GetComponent<SpriteRenderer>().sortingLayerID = GM.BehindProjectileSortingLayer;
            
            if (Proj.AttackType != ProjectileAttackTypes.AroundSender && Proj.AttackType != ProjectileAttackTypes.OnSender)
                UpdateProjRotation();

            _distanceToSpawn = Vector3.Distance(Proj.SpawnPos, Proj.CasterPos);
        }
        
        public void Update()
        {
            if (!_initialised) return;
            
            if (CurrentHp <= 0)
                Die();
            
            CountTimers();
        }
        
        public void FixedUpdate()
        {
            if (!_initialised) return;
            
            UpdateSpeed();
            
            if (Proj.AttackType == ProjectileAttackTypes.Tracking)
                UpdateProjRotation(); // because we rotate the rigid body rotation (angular velocity)
            
            if (!Mathf.Approximately(Proj.DestroyedTime, -1f) && Proj.DestroyedTime <= 0)
                DieSilent();
            
            ManageProjectileBehaviour();
        }
        
        private void UpdateProjRotation()
        {
            transform.eulerAngles = new Vector3(0, 0, GM.GetAngleFromVectorFloat(Proj.Direction));
        }
        
        private void UpdateSpeed()
        {       // only testing if maxSpeed is set to -1f (need it because of float type)
            if (Mathf.Approximately(Proj.MaxSpeed, -1f) || Proj.Speed < Proj.MaxSpeed)
            {
                Proj.Speed += Proj.Acceleration * Time.fixedDeltaTime;
            }
            else if (!Mathf.Approximately(Proj.MaxSpeed, -1f) && Proj.Speed > Proj.MaxSpeed)
            {
                Proj.Speed = Proj.MaxSpeed;
            }
        }

        /*public void OnDestroy()
        {
            if (Proj.AttackType == ProjectileAttackTypes.AroundSender && _fakeSenderGameObject is not null)
                Destroy(transform.parent.gameObject); // maybe error because it destroys both
        }*/

        #region AttackTypeManaging
        private void ManageProjectileBehaviour()
        {
            switch (Proj.AttackType)
            {
                case ProjectileAttackTypes.Linear:
                    LinearProjectile();
                    break;
                case ProjectileAttackTypes.Tracking:
                    if (TargetTransform is not null)
                        TrackingProjectile();
                    else
                        FindTrackingTarget();
                    break;
                case ProjectileAttackTypes.OnSender:
                    if (SenderTransform is not null)
                        OnSenderProjectile();
                    else
                        NoSenderManagement();
                    break;
                case ProjectileAttackTypes.AroundSender:
                    if (SenderTransform is not null && FakeSenderTransform is not null)
                        AroundSenderProjectile();
                    else if (SenderTransform is not null && FakeSenderTransform is null)
                        CreateFakeSender();
                    else
                        NoSenderManagement();
                    break;
                case ProjectileAttackTypes.Fix:
                    FixProjectile();
                    break;
            }
        }
        
        private void LinearProjectile()
        {
            _rb.linearVelocity = Proj.Direction * (Speed * Time.fixedDeltaTime);
        }
        
        private void TrackingProjectile()
            // Don't ask me the formula : https://www.youtube.com/watch?v=Srn0K6iqLOs / https://www.youtube.com/watch?v=0v_H3oOR0aU
        {
            _rb.linearVelocity = Proj.Direction * (Speed * Time.fixedDeltaTime);
            
            Proj.Direction = (TargetTransform.position - transform.position).normalized;
            
            // to understand Cross : https://www.youtube.com/watch?v=kz92vvioeng
            float rotateAmount = Vector3.Cross(Proj.Direction, transform.up).z;

            _rb.angularVelocity = -rotateAmount * Proj.RotateSpeed;
        }
        
        private void OnSenderProjectile()
        {
            UpdateRotationSelf();

            if (!_attachedConstraint)
            {
                PositionConstraint positionConstraint = gameObject.AddComponent<PositionConstraint>();

                ConstraintSource constraint = new ConstraintSource();
                constraint.sourceTransform = SenderTransform;
                constraint.weight = 1f;

                positionConstraint.AddSource(constraint);
                positionConstraint.constraintActive = true;

                _attachedConstraint = true;
            }
            
            //transform.position = SenderTransform.position;
        }
        
        private void AroundSenderProjectile()
        {
            UpdateRotationSelf();

            FakeSenderTransform.position = SenderTransform.position;
            
            transform.RotateAround(FakeSenderTransform.position, Vector3.forward, 
            Proj.RotateAroundSpeed * Time.fixedDeltaTime * (Proj.RotateAroundRight ? -1 : 1));
        }

        private void ParabolaProjectileRotation() // https://www.youtube.com/watch?v=tNwLaGUJTK4
        {
            float angle = Mathf.Atan2(_rb.linearVelocity.x, _rb.linearVelocity.y) * Mathf.Rad2Deg + 90f;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        private void CreateFakeSender()
        {
            _fakeSenderGameObject = new GameObject("RotateAroundPlayerGameObject");
            FakeSenderTransform.position = SenderTransform.position;
            
            transform.parent = _fakeSenderGameObject.transform;

            PositionConstraint constraintPosition = _fakeSenderGameObject.AddComponent<PositionConstraint>();
            ConstraintSource constraint = new ConstraintSource();
            constraint.sourceTransform = SenderTransform;
            constraint.weight = 1f;
            constraintPosition.AddSource(constraint);
        }
        
        private void FixProjectile()
        {
            UpdateRotationSelf();
            
            transform.position = Proj.SpawnPos;
        }

        private void UpdateRotationSelf()
        {
            transform.Rotate(Vector3.forward, Proj.RotateSpeed * Time.fixedDeltaTime * (Proj.RotateSelfRight ? -1 : 1));
        }
        
        private Vector3 GetParabolaPos(Vector3 targetPos)
        {
            // should return the ground under that pos
            
            return Vector3.zero;
        }

        [CanBeNull]
        private GameObject GetClosestTarget(Vector3 ourPos)
        {
            float? closestDis = null;
            GameObject closestGameObject = null;
            
            foreach (Collider2D target in _targetsFound)
            {
                float curDis = Vector2.Distance(target.transform.position, ourPos);
                if (closestDis is null || curDis < closestDis)
                {
                    closestDis = curDis;
                    closestGameObject = target.gameObject;
                }
            }

            return closestGameObject;
        }
        
        private void FindTrackingTarget() // Only run when the timer got finished 
        {
            if (Proj.FoundTrackingTarget)
                Proj.FoundTrackingTarget = false;
            
            if (_searchingTarget)
            {
                Proj.TrackingTargetTime = Proj.TrackingTargetCooldown;
                hitBoxCollider.enabled = true;
                findTargetCollider.enabled = false;
                _targetGameObject = GetClosestTarget(transform.position);
                _targetsFound = new List<Collider2D>();

                if (TargetTransform is not null)
                    Proj.FoundTrackingTarget = true;
                _searchingTarget = false;
                gameObject.tag = _objectTag;
            }
            
            if (Proj.TrackingTargetTime <= 0)
            {
                _searchingTarget = true;
                gameObject.tag = "Untagged";
                hitBoxCollider.enabled = false;
                findTargetCollider.enabled = true;
            }

            if (!Proj.FoundTrackingTarget) // When we are looking for a target it's a linear projectile
                LinearProjectile();
        }

        private void NoSenderManagement()
        {
            if (_finishedSearchingSender)
            {
                if (Proj.AttackType == ProjectileAttackTypes.AroundSender)
                    Proj.CasterPos = Proj.SpawnPos;
                
                FixProjectile();
            }
            else
                FindSenderTarget();
        }

        private void FindSenderTarget()
        {
            if (!_searchingSender)
            {
                hitBoxCollider.enabled = false;
                findTargetCollider.enabled = false;

                _searchingSender = true;
            
                _findSenderCollider = gameObject.AddComponent<CircleCollider2D>();
                _findSenderCollider.radius = 1f / Proj.Scale;
                _findSenderCollider.offset = GetCasterOffsetPos();
                _findSenderCollider.isTrigger = true;
            }
            else
            {
                _senderGameObject = GetClosestTarget(Proj.CasterPos);
                _targetsFound = new List<Collider2D>();

                /*_findSenderCollider.enabled = false;
                Destroy(_findSenderCollider);*/
                
                hitBoxCollider.enabled = true;

                _finishedSearchingSender = true;
                _searchingSender = false;
            }
        }

        private Vector2 GetCasterOffsetPos()
        {
            return (Proj.CasterPos - Proj.SpawnPos) / transform.localScale.x;
        }

        #endregion

        #region Die
        public override void Die()
        {
            // play sound + particle
            GM.ProjM.projListSpawned.Remove(this);
            
            if (Proj.AttackType == ProjectileAttackTypes.AroundSender && _fakeSenderGameObject is not null)
                Destroy(transform.parent.gameObject);
            
            Destroy(gameObject);
        }
        
        private void DieSilent()
        {
            GM.ProjM.projListSpawned.Remove(this);
            
            if (Proj.AttackType == ProjectileAttackTypes.AroundSender && _fakeSenderGameObject is not null)
                Destroy(transform.parent.gameObject);
            
            Destroy(gameObject);
        }
        #endregion

        #region Can Attack
        private bool SelfTargetingPlayerBehaviour(Collider2D other)
        {
            if (!Proj.CanTargetSelf)
            {
                return Proj.SenderId != other.transform.parent.parent.GetComponent<PlayerId>().GetPlayerId();
            }

            return true;
        }
        
        private bool SelfTargetingProjectileBehaviour(Collider2D other)
        {
            if (!Proj.CanTargetSelf)
            {
                return Proj.SenderId != other.GetComponent<Projectile>().Proj.SenderId;
            }

            return true;
        }

        private bool CanAttackThat(Collider2D other)
        {
            return (Proj.TargetingPlayer && GM.IsPlayer(other) && SelfTargetingPlayerBehaviour(other)) ||
                   (Proj.TargetingEnemy && GM.IsEnemy(other)) ||
                   (Proj.TargetingPlayerProjectile && GM.IsPlayerProjectile(other) && SelfTargetingProjectileBehaviour(other)) ||
                   (Proj.TargetingEnemyProjectile && GM.IsEnemyProjectile(other));
        }
        #endregion

        private void DoDamageOrHeal(IUnitHp otherHp)
        {
            if (Proj.Healing == 0)
                otherHp.TakeDamage(Proj.Damage);
            else
                otherHp.GainHealth(Proj.Healing);
        }

        private bool MayBeSender(Collider2D other)
        {
            return other.CompareTag(_senderTag);
        }
        
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (_searchingTarget)
            {
                if (CanAttackThat(other))
                {
                    _targetsFound.Add(other);
                }
                return;
            }

            if (_searchingSender)
            {
                if (MayBeSender(other))
                {
                    _targetsFound.Add(other);
                }
                return;
            }

            if (CanAttackThat(other) && other.TryGetComponent<IUnitHp>(out IUnitHp otherHp)) // found target to attack
            {
                if (otherHp.IsNetwork) // if the other hp is a NetworkVariable
                {
                    if (_isServerProj) // then only the server can do it damage
                        DoDamageOrHeal(otherHp);
                }
                else // if the enemy is local hp variable, all the projectiles need to do it damage
                    DoDamageOrHeal(otherHp);

                Die();
            }
            else if (!Proj.CanCrossWalls && GM.CrossedWall(other)) // got destroyed by wall
            {
                Die();
            }
        }
        
        #region Timer
        private void CountTimers()
        {
            float deltaTime = Time.deltaTime;
            
            if (!Mathf.Approximately(Proj.DestroyedTime, -1f) && Proj.DestroyedTime >= 0)
                Proj.DestroyedTime -= deltaTime;
            if (Proj.AttackType == ProjectileAttackTypes.Tracking && !Proj.FoundTrackingTarget && 
                Proj.TrackingTargetTime > 0)
                Proj.TrackingTargetTime -= deltaTime;
        }
        #endregion
    }
}

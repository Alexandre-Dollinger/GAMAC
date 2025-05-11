using System;
using System.Collections.Generic;
using _Scripts.GameManager;
using _Scripts.Health;
using JetBrains.Annotations;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class Projectile : UnitHp
    {
        private readonly float _speedCoefficient = 100;
        private float Speed
        {
            get => Proj.Speed * _speedCoefficient;
        }

        private Rigidbody2D _rb;

        private CircleCollider2D _hitBoxCollider;
        private PolygonCollider2D _findTargetCollider;
        private List<Collider2D> _targetsFound = new List<Collider2D>();
        private bool _searchingTarget = false;
        private string _objectTag;
        
        [CanBeNull] private Transform _targetTransform = null;
        
        private bool _initialised = false;
        public ProjectileStruct Proj;
        
        // still need sound, animation, sprite, collider
        
        public void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _hitBoxCollider = GetComponent<CircleCollider2D>();
            _findTargetCollider = GetComponent<PolygonCollider2D>();
            _findTargetCollider.enabled = false;
        }
        
        public void InitProjectile(ProjectileStruct projectileStruct)
        {
            Proj = projectileStruct;
            _initialised = true;

            MaxHp = Proj.GetMaxHpPrefab();
            CurrentHp = MaxHp;
            
            transform.localScale *= Proj.Scale;

            _objectTag = gameObject.tag;
            
            UpdateProjRotation();
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

        #region AttackTypeManaging
        private void ManageProjectileBehaviour()
        {
            switch (Proj.AttackType)
            {
                case ProjectileAttackType.Linear:
                    LinearProjectile();
                    break;
                case ProjectileAttackType.Tracking:
                    if ((UnityEngine.Object)_targetTransform is not null)
                        TrackingProjectile();
                    else
                        FindTrackingTarget();
                    break;
                case ProjectileAttackType.Parabola:
                    ParabolaProjectile();
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
            
            Proj.Direction = (_targetTransform!.position - transform.position).normalized;
            
            // to understand Cross : https://www.youtube.com/watch?v=kz92vvioeng
            float rotateAmount = Vector3.Cross(Proj.Direction, transform.up).z;

            _rb.angularVelocity = -rotateAmount * Proj.RotateSpeed;
        }

        private void ParabolaProjectile()
        {
            
        }
        
        private Vector3 GetParabolaPos(Vector3 targetPos)
        {
            // should return the ground under that pos
            
            return Vector3.zero;
        }

        [CanBeNull]
        private Transform GetClosestTarget()
        {
            float? closestDis = null;
            Transform closestTransform = null;

            foreach (Collider2D target in _targetsFound)
            {
                float curDis = Vector2.Distance(target.transform.position, transform.position);
                if (closestDis is null || curDis < closestDis)
                {
                    closestDis = curDis;
                    closestTransform = target.transform;
                }
            }

            return closestTransform;
        }
        
        private void FindTrackingTarget() // Only run when the timer got finished 
        {
            if (Proj.FoundTrackingTarget)
                Proj.FoundTrackingTarget = false;
            
            if (_searchingTarget)
            {
                Proj.TrackingTargetTime = Proj.TrackingTargetCooldown;
                _hitBoxCollider.enabled = true;
                _findTargetCollider.enabled = false;
                _targetTransform = GetClosestTarget();
                _targetsFound = new List<Collider2D>();
                if ((UnityEngine.Object)_targetTransform is not null)
                    Proj.FoundTrackingTarget = true;
                _searchingTarget = false;
                gameObject.tag = _objectTag;
            }
            
            if (Proj.TrackingTargetTime <= 0)
            {
                _searchingTarget = true;
                gameObject.tag = "Untagged";
                _hitBoxCollider.enabled = false;
                _findTargetCollider.enabled = true;
            }

            if (!Proj.FoundTrackingTarget) // When we are looking for a target it's a linear projectile
                LinearProjectile();
        }
        #endregion

        #region Die
        public override void Die()
        {
            // play sound + particle
            GM.ProjM.projList.Remove(this);
            Destroy(gameObject);
        }
        
        private void DieSilent()
        {
            GM.ProjM.projList.Remove(this);
            Destroy(gameObject);
        }
        #endregion

        private bool CanAttackThat(Collider2D other)
        {
            return (Proj.TargetingPlayer && GM.IsPlayer(other)) ||
                   (Proj.TargetingEnemy && GM.IsEnemy(other)) ||
                   (Proj.TargetingPlayerProjectile && GM.IsPlayerProjectile(other)) ||
                   (Proj.TargetingEnemyProjectile && GM.IsEnemyProjectile(other));
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
            
            if (CanAttackThat(other)) // found target to attack
            {
                IUnitHp otherHp = other.GetComponent<IUnitHp>();
                otherHp.TakeDamage(Proj.Damage);
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
            if (Proj.AttackType == ProjectileAttackType.Tracking && !Proj.FoundTrackingTarget && 
                Proj.TrackingTargetTime > 0)
                Proj.TrackingTargetTime -= deltaTime;
        }
        #endregion
    }
}

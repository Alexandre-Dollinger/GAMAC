using System;
using _Scripts.GameManager;
using _Scripts.Health;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

namespace _Scripts.Projectiles
{
    public class Projectile : UnitHp
    {
        private float _speed;
        private float? _maxSpeed;
        private float? _acceleration;

        private readonly float _speedCoefficient = 100;
        private float Speed
        {
            get => _speed * _speedCoefficient;
        }

        private Rigidbody2D _rb;

        private int _damage;
        private ProjectileAttackType _projectileAttackType;
        
        private Vector3 _spawnPos;
        private Vector3 _direction; // _direction is normalized
        private Vector3? _targetPos;
        [CanBeNull] private Transform _targetTransform;
        //private bool _foundTrackingTarget;
        private float _rotateSpeed;
        
        private GM.FilterType _filterTarget;
        private bool _canCrossWalls = false;
        private bool _canBeDestroyedByPlayer = false;
        private float? _destroyedTime;
        
        // still need sound, animation, sprite, collider

        
        
        public void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        /*[ServerRpc]
        public void BasicInitServerRpc(Vector3 spawnPos, Vector3 shootDir, float scale)
        {
            BasicInitClientRpc(spawnPos, shootDir, scale);
        }

        [ClientRpc]
        public void BasicInitClientRpc(Vector3 spawnPos, Vector3 shootDir, float scale)
        {
            _direction = shootDir.normalized;
            _spawnPos = spawnPos;
            transform.position = _spawnPos;
            
            transform.localScale *= scale;
            
            UpdateProjRotation();
        }*/
        
        public void BasicInit(Vector3 spawnPos, Vector3 shootDir, float scale)
        {
            _direction = shootDir.normalized;
            _spawnPos = spawnPos;
            transform.position = _spawnPos;
            
            transform.localScale *= scale;
            
            UpdateProjRotation();
        }


        public void InitDestroyCondition(GM.FilterType filterTarget,
            bool canBeDestroyedByPlayer = true, int health = 50,
            bool canCrossWalls = false, float? destroyedTime = null)
        {
            MaxHp = health;
            CurrentHp = MaxHp;
            
            _filterTarget = filterTarget;
            _canBeDestroyedByPlayer = canBeDestroyedByPlayer;
            _canCrossWalls = canCrossWalls;
            _destroyedTime = destroyedTime;
        }

        public void InitSpeed(float speed = 50, float? acceleration = null, float? maxSpeed = null) 
                                                    // if null then no acceleration but constant speed
        {
            _speed = speed;
            _maxSpeed = maxSpeed;
            _acceleration = acceleration;
        }

        public void InitAttackLinear(int damage)
        {
            _damage = damage;
            _projectileAttackType = ProjectileAttackType.Linear;
        }
        
        public void InitAttackTracking(int damage, float rotateSpeed, Transform targetTransform)
        {
            _damage = damage;
            _projectileAttackType = ProjectileAttackType.Tracking;
            _targetTransform = targetTransform;
        }
        
        public void InitAttackParabola(int damage, Vector3 targetPos)
        {
            _damage = damage;
            _projectileAttackType = ProjectileAttackType.Parabola;
            _targetPos = targetPos;
        }

        private Vector3 GetParabolaPos(Vector3 targetPos)
        {
            // should return the ground under that pos
            
            return Vector3.zero;
        }

        private Transform FindTrackingTarget()
        {
            // should return the Vector3 of transform.position of target
            // if not found just keep shootDirection

            throw new NotImplementedException();
        }

        private void UpdateProjRotation()
        {
            transform.eulerAngles = new Vector3(0, 0, GM.GetAngleFromVectorFloat(_direction));
        }
        
        public void Update()
        {
            if (CurrentHp <= 0)
                Die();
            
            ManageDestroyWithTime();
        }

        public void FixedUpdate()
        {
            if (_acceleration is not null)
                UpdateSpeed();
            switch (_projectileAttackType)
            {
                case ProjectileAttackType.Linear:
                    LinearProjectile();
                    break;
                case ProjectileAttackType.Tracking:
                    if ((UnityEngine.Object)_targetTransform is not null)
                        TrackingProjectile();
                    break;
                case ProjectileAttackType.Parabola:
                    ParabolaProjectile();
                    break;
            }
        }

        private void UpdateSpeed()
        {
            if (_maxSpeed is null || _speed < _maxSpeed)
            {
                _speed += _acceleration!.Value * Time.fixedDeltaTime;
            }
            else if (_maxSpeed is not null && _speed > _maxSpeed)
            {
                _speed = _maxSpeed.Value;
            }
        }

        private void LinearProjectile()
        {
            _rb.linearVelocity = _direction * (Speed * Time.fixedDeltaTime);
        }

        private void TrackingProjectile()
        // Don't ask me the formula : https://www.youtube.com/watch?v=Srn0K6iqLOs / https://www.youtube.com/watch?v=0v_H3oOR0aU
        {
            _rb.linearVelocity = _direction * (Speed * Time.fixedDeltaTime);
            
            _direction = (_targetTransform.position - transform.position).normalized;
            
            // to understand Cross : https://www.youtube.com/watch?v=kz92vvioeng
            float rotateAmount = Vector3.Cross(_direction, transform.up).z;

            _rb.angularVelocity = -rotateAmount * _rotateSpeed;
        }

        private void ParabolaProjectile()
        {
            
        }

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

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (_filterTarget(other)) // found target to attack
            {
                IUnitHp otherHp = other.GetComponent<IUnitHp>();
                otherHp.TakeDamage(_damage);
                Die();
            }
            else if (!_canCrossWalls && GM.CrossedWall(other)) // got destroyed by wall
            {
                Die();
            }
        }

        private void ManageDestroyWithTime()
        {
            if (_destroyedTime is not null)
            {
                float deltaTime = Time.deltaTime;

                if (_destroyedTime >= 0)
                    _destroyedTime -= deltaTime;
                else
                {
                    DieSilent();
                    // not Die since we don't want noise or others;
                }
            }
        }
    }
}

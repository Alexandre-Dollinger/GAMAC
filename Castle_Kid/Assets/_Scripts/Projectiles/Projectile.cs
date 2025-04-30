using System;
using _Scripts.GameManager;
using _Scripts.Health;
using JetBrains.Annotations;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class Projectile : UnitHp
    {
        private int _damage;
        private float _speed;
        //private float _maxSpeed;
        private bool _canDie;
        private bool _tracking;
        
        private Vector3 _startPos;
        private Vector3 _direction;
        private Vector3? _targetPos;
        
        private GM.FilterType _filterTarget;
        private GM.FilterType _filterDestroy;

        private float? _destroyedTime = null;
        
        // still need sound, animation, sprite, collider

        public void InitProjectile(int damage, float speed, Vector3 startPos, Vector3 shootDir,
            GM.FilterType filterTarget, [CanBeNull] GM.FilterType filterDestroy,
            float scale, bool tracking, Vector3? targetPos)
        {
            _damage = damage;
            _speed = speed;
            _startPos = startPos;
            _targetPos = targetPos;
            _direction = shootDir;

            _filterTarget = filterTarget;
            _filterDestroy = filterDestroy;
            
            _tracking = tracking;

            transform.localScale *= scale;

            transform.position = startPos;
            UpdateProjRotation();
        }

        private void UpdateProjRotation()
        {
            transform.eulerAngles = new Vector3(0, 0, GM.GetAngleFromVectorFloat(_direction));
        }
        
        private void Update()
        {
            ManageDestroyWithTime();
            transform.position += _direction * (_speed * Time.deltaTime);
        }

        public override void Die()
        {
            // play sound + particle
            Destroy(gameObject);
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (_filterTarget(other))
            {
                IUnitHp otherHp = other.GetComponent<IUnitHp>();
                otherHp.TakeDamage(_damage);
                Die();
            }
            else if (_filterDestroy(other))
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
                    Destroy(gameObject);
                    // not Die since we don't want noise or others;
                }
            }
        }
    }
}

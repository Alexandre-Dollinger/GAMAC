using System;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public enum ProjectilePrefabType
    {
        Spark,
    }
    
    public enum ProjectileAnimation
    {
        Spark,
    }

    public enum ProjectileBasicCollider
    {
        Spark,
        
    }
    
    public enum ProjectileSearchCollider
    {
        Cone,
        FindSender,
    }

    public enum ProjectileAttackType
    {
        Linear,
        Tracking,
        Parabola,
        OnSender, // spawn a projectile on the sender, example : a shield
        RotateAroundSender,
    }
    
    public struct ProjectileStruct : INetworkSerializable // https://www.youtube.com/watch?v=j-fYT4MHNDk&list=PLyWwWSgyfz-XMvcmT6T5F1JGYUHCJyXBJ&index=33
    {
        public float Speed;
        public float Acceleration; // null value is 0f
        public float MaxSpeed; // null value if -1f
        
        public int Damage;
        public int Healing;
        public ProjectileAttackType AttackType;
        
        public Vector3 SpawnPos;
        public Vector3 Direction; // _direction is normalized
        
        public bool CanCrossWalls;
        public bool CanBeDestroyedByPlayer;
        public bool CanBeDestroyedBySelf;
        public float DestroyedTime; // null value is -1f

        //public GM.FilterType FilterTarget; // if only but no because of NetworkVariable instead a lot of boolean
        public bool TargetingPlayer;
        public bool TargetingEnemy;
        public bool TargetingPlayerProjectile;
        public bool TargetingEnemyProjectile;
        public bool CanTargetSelf;

        public bool RotateSelfRight;
        public bool RotateAroundRight;
        
        public int SenderId; // null value is -1
        
        public int Scale;
        
        public float TrackingTargetCooldown; // maxTime // null value is -1f
        public float TrackingTargetTime; // Time updating continuously // null value is -1f
        public bool FoundTrackingTarget;
        public float RotateSpeed;
        
        private int _maxHpPrefab; // just to init the health of our object
        public int GetMaxHpPrefab()
        {
            return _maxHpPrefab;
        }

        public ProjectileStruct(Vector3 spawnPos, Vector3 direction, int scale = 1)
        {
            Speed = 0f;
            Acceleration = 0f;
            MaxSpeed = -1f;
            
            Damage = 0;
            Healing = 0;
            AttackType = ProjectileAttackType.Linear;
            
            SpawnPos = spawnPos;
            Direction = direction.normalized;

            TargetingPlayer = false;
            TargetingEnemy = false;
            TargetingPlayerProjectile = false;
            TargetingEnemyProjectile = false;
            CanTargetSelf = true;

            RotateSelfRight = true;
            RotateAroundRight = true;

            SenderId = -1;
            
            CanCrossWalls = false;
            CanBeDestroyedByPlayer = false;
            CanBeDestroyedBySelf = false; // takes the priority compared to CanBeDestroyedByPlayer
            DestroyedTime = -1f;

            Scale = scale;
            
            TrackingTargetCooldown = -1f;
            TrackingTargetTime = -1f;
            FoundTrackingTarget = false;
            RotateSpeed = 0f;
            
            _maxHpPrefab = 1;
            
            SenderId = -1;
        }
        
        public void InitDestroyCondition(bool canBeDestroyedByPlayer = true, 
            int health = 50, bool canCrossWalls = false, bool canBeDestroyedBySelf = true, float destroyedTime = -1f)
        {
            _maxHpPrefab = health;
            
            CanBeDestroyedByPlayer = canBeDestroyedByPlayer;
            CanCrossWalls = canCrossWalls;
            DestroyedTime = destroyedTime;
            CanBeDestroyedBySelf = canBeDestroyedBySelf;
        }

        public void InitTargeting(bool targetPlayer = false, bool targetEnemy = false, 
            bool targetPlayerProjectile = false, bool targetEnemyProjectile = false, bool canTargetSelf = true)
        {
            TargetingPlayer = targetPlayer;
            TargetingEnemy = targetEnemy;
            TargetingPlayerProjectile = targetPlayerProjectile;
            TargetingEnemyProjectile = targetEnemyProjectile;
            CanTargetSelf = TargetingPlayer && canTargetSelf; // enemy has no Id so only work for player
        }
        
        public void InitSpeed(float speed = 50, float acceleration = 0f, float maxSpeed = -1f) 
            // if acceleration null then no acceleration but constant speed
        {
            Speed = speed;
            Acceleration = acceleration;
            MaxSpeed = maxSpeed;
        }

        public void InitHealing(int healing = 50)
        {
            Damage = 0;
            Healing = healing;
        }
        
        public void InitAttackLinear(int damage)
        {
            Damage = damage;
            AttackType = ProjectileAttackType.Linear;
        }
        
        public void InitAttackTracking(int damage, float rotateSpeed, 
            float trackingTargetCooldown = 0.5f) // maybe need a bool to say if it need to track projectile or only others
        {
            Damage = damage;
            RotateSpeed = rotateSpeed;
            AttackType = ProjectileAttackType.Tracking;

            TrackingTargetCooldown = trackingTargetCooldown; // To not search for target each millisecond for optimisation
            TrackingTargetTime = 0f;
        }
        
        public void InitAttackParabola(int damage)
        {
            Damage = damage;
            AttackType = ProjectileAttackType.Parabola;
            throw new NotImplementedException("Git Gud. Not yet available");
        }
        
        public void InitAttackOnSender(int damage, float rotateSpeed = 0f, bool rotateRight = true)
        {
            Damage = damage;
            AttackType = ProjectileAttackType.OnSender;
            RotateSpeed = rotateSpeed;
            RotateSelfRight = rotateRight;
        }
        
        // Don't look at that it's to make NetworkVariable work
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        { 
            serializer.SerializeValue(ref Speed);
            serializer.SerializeValue(ref Acceleration);
            serializer.SerializeValue(ref MaxSpeed);
            
            serializer.SerializeValue(ref Damage);
            serializer.SerializeValue(ref Healing);
            serializer.SerializeValue(ref AttackType);
            
            serializer.SerializeValue(ref TargetingPlayer);
            serializer.SerializeValue(ref TargetingEnemy);
            serializer.SerializeValue(ref TargetingPlayerProjectile);
            serializer.SerializeValue(ref TargetingEnemyProjectile);
            serializer.SerializeValue(ref CanTargetSelf);
            
            serializer.SerializeValue(ref RotateSelfRight);
            serializer.SerializeValue(ref RotateAroundRight);
            
            serializer.SerializeValue(ref SenderId);
            
            serializer.SerializeValue(ref SpawnPos);
            serializer.SerializeValue(ref Direction);
            
            serializer.SerializeValue(ref CanCrossWalls);
            serializer.SerializeValue(ref CanBeDestroyedByPlayer);
            serializer.SerializeValue(ref CanBeDestroyedBySelf);
            serializer.SerializeValue(ref DestroyedTime);
            
            serializer.SerializeValue(ref Scale);
            
            serializer.SerializeValue(ref TrackingTargetCooldown);
            serializer.SerializeValue(ref TrackingTargetTime);
            serializer.SerializeValue(ref FoundTrackingTarget);
            serializer.SerializeValue(ref RotateSpeed);
            
            serializer.SerializeValue(ref _maxHpPrefab);
        }
    }
}

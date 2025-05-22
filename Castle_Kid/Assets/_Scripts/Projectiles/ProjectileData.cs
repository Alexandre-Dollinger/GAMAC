using System;
using _Scripts.GameManager;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public enum ProjectileStructEnum
    {
        LinearDamage,
        LinearHealing,
        LinearAcceleratingDamage,
        LinearAcceleratingHealing,
        TrackingDamage,
        TrackingHealing,
        TrackingAcceleratingDamage,
        TrackingAcceleratingHealing,
        OnSenderRotating,
        OnSender,
        AroundSenderRotating,
        AroundSenderRotatingSelf,
        AroundSender,
        Fix,
        FixHealing,
    }
    
    public enum ProjectilePrefabs
    {
        SparkCone,
        SparkCircle,
        ShieldBlueCircle,
        FireCone,
        FireBallCone,
        FireBall2Cone,
        TornadoCone,
        BlackHoleCone,
        BubblesCone,
        MagicArrowCone,
        MagicBallCone,
        MagicBulletCone,
        RocksCone,
        ShieldPlusCircle,
        WindCone
    }

    public enum ProjectileAttackTypes
    {
        Linear,
        Tracking,
        OnSender, // spawn a projectile on the sender, example : a shield
        AroundSender,
        Fix,
    }
    
    public enum SenderTags
    {
        Player,
        Enemy,
        PlayerProjectile,
        EnemyProjectile,
    }

    public struct
        ProjectileStruct : INetworkSerializable // https://www.youtube.com/watch?v=j-fYT4MHNDk&list=PLyWwWSgyfz-XMvcmT6T5F1JGYUHCJyXBJ&index=33
    {
        public float Speed;
        public float Acceleration; // null value is 0f
        public float MaxSpeed; // null value if -1f

        public int Damage;
        public int Healing;
        public ProjectileAttackTypes AttackType;

        public Vector3 SpawnPos;
        public Vector3 CasterPos; // not updated, only the caster pos at the start
        public Vector3 Direction; // Direction is normalized

        public float Offset;

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
        public float RotateAroundSpeed;

        public int SenderId; // null value is -1
        public SenderTags SenderTag;

        public float Scale;
        public bool IsBehindProjectile;

        public float TrackingTargetCooldown; // maxTime // null value is -1f
        public float TrackingTargetTime; // Time updating continuously // null value is -1f
        public bool FoundTrackingTarget;
        public float RotateSpeed;

        private int _maxHpPrefab; // just to init the health of our object

        public int GetMaxHpPrefab()
        {
            return _maxHpPrefab;
        }

        public ProjectileStruct(Vector3 casterPos, Vector3 direction, float offset = 50f, float scale = 1f)
        {
            Speed = 0f;
            Acceleration = 0f;
            MaxSpeed = -1f;

            Damage = 0;
            Healing = 0;
            AttackType = ProjectileAttackTypes.Linear;

            Direction = direction.normalized;
            SpawnPos = casterPos + offset * Direction;
            CasterPos = casterPos;

            Offset = offset;

            TargetingPlayer = false;
            TargetingEnemy = false;
            TargetingPlayerProjectile = false;
            TargetingEnemyProjectile = false;
            CanTargetSelf = true;

            RotateSelfRight = true;
            RotateAroundRight = true;
            RotateAroundSpeed = 0f;

            SenderId = -1;
            SenderTag = SenderTags.Player;

            CanCrossWalls = false;
            CanBeDestroyedByPlayer = false;
            CanBeDestroyedBySelf = false; // takes the priority compared to CanBeDestroyedByPlayer
            DestroyedTime = -1f;

            Scale = scale;
            IsBehindProjectile = false;

            TrackingTargetCooldown = -1f;
            TrackingTargetTime = -1f;
            FoundTrackingTarget = false;
            RotateSpeed = 0f;

            _maxHpPrefab = 1;

            SenderId = -1;
        }

        public void UpdatePosAndDir(Vector3 casterPos, Vector3 direction, float offset)
        {
            Direction = direction.normalized;
            SpawnPos = casterPos + offset * Direction;
            CasterPos = casterPos;
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
            AttackType = ProjectileAttackTypes.Linear;
        }

        public void InitAttackTracking(int damage, float rotateSpeed,
            float trackingTargetCooldown =
                0.5f) // maybe need a bool to say if it need to track projectile or only others
        {
            Damage = damage;
            RotateSpeed = rotateSpeed;
            AttackType = ProjectileAttackTypes.Tracking;

            TrackingTargetCooldown =
                trackingTargetCooldown; // To not search for target each millisecond for optimisation
            TrackingTargetTime = 0f;
        }

        public void InitAttackOnSender(int damage, SenderTags senderTag, float rotateSpeed = 0f,
            bool rotateRight = true)
        {
            Damage = damage;
            SenderTag = senderTag;
            AttackType = ProjectileAttackTypes.OnSender;
            RotateSpeed = rotateSpeed;
            RotateSelfRight = rotateRight;
        }

        public void InitAttackAroundSender(int damage, SenderTags senderTag, float rotateSpeed = 0f,
            bool rotateRight = true, float rotateAroundSpeed = 0f, bool rotateAroundRight = true)
        {
            Damage = damage;
            SenderTag = senderTag;
            AttackType = ProjectileAttackTypes.AroundSender;
            RotateSpeed = rotateSpeed;
            RotateSelfRight = rotateRight;
            RotateAroundSpeed = rotateAroundSpeed;
            RotateAroundRight = rotateAroundRight;
        }


        public void InitAttackFix(int damage, float rotateSpeed = 0f, bool rotateRight = true)
        {
            Damage = damage;
            AttackType = ProjectileAttackTypes.Fix;
            RotateSpeed = rotateSpeed;
            RotateSelfRight = rotateRight;
        }

        public void BecomeBehindPlayerProjectile() // change the filtering layer of the projectile
        {
            IsBehindProjectile = true;
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
            serializer.SerializeValue(ref RotateAroundSpeed);

            serializer.SerializeValue(ref SenderId);
            serializer.SerializeValue(ref SenderTag);

            serializer.SerializeValue(ref Direction);
            serializer.SerializeValue(ref SpawnPos);
            serializer.SerializeValue(ref CasterPos);

            serializer.SerializeValue(ref Offset);

            serializer.SerializeValue(ref CanCrossWalls);
            serializer.SerializeValue(ref CanBeDestroyedByPlayer);
            serializer.SerializeValue(ref CanBeDestroyedBySelf);
            serializer.SerializeValue(ref DestroyedTime);

            serializer.SerializeValue(ref Scale);
            serializer.SerializeValue(ref IsBehindProjectile);

            serializer.SerializeValue(ref TrackingTargetCooldown);
            serializer.SerializeValue(ref TrackingTargetTime);
            serializer.SerializeValue(ref FoundTrackingTarget);
            serializer.SerializeValue(ref RotateSpeed);

            serializer.SerializeValue(ref _maxHpPrefab);
        }
    }
}

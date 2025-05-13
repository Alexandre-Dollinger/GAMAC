using System;
using _Scripts.GameManager;
using _Scripts.Health;
using _Scripts.Inputs;
using _Scripts.Multiplayer;
using _Scripts.Projectiles;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace _Scripts.Player.Weapon
{
    public class WeaponScript : NetworkBehaviour
    {
        public int playerAttack = 50;
        
        public float offset;

        public float attackDelay = 1.3f;
        private float _curAttDelay;
        
        public float comboTimer = 0.6f;
        private float _curComboTimer;

        public float bufferAttackTimer = 0.2f;
        private float _curBufferTimer;
        
        private PolygonCollider2D _polygonCollider2D;
        private Camera _playerCamera;

        private Animator _animator;
        
        private float _fixedYRotation;
        private Quaternion _slashRotation;

        private int _playerId;
        
        // https://discussions.unity.com/t/mouse-movements-for-client-side-becoming-server-side-mouse-movements/938064/2
        private NetworkVariable<Vector2> _dirToMouse = new NetworkVariable<Vector2>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        
        public override void OnNetworkSpawn()
        {
            /*if (!IsOwner)
            {
                enabled = false;
                return;
            }*/
            
            _playerId = transform.parent.GetComponent<PlayerId>().GetPlayerId();
            
            if (IsOwner)
                UpdateDirToMouse();
        }
        
        private void Awake()
        {
            _polygonCollider2D = GetComponent<PolygonCollider2D>();
            _polygonCollider2D.enabled = false;
            _playerCamera = transform.parent.Find("Main Camera").GetComponent<Camera>();
            _animator = transform.Find("SlashAnimation").GetComponent<Animator>();
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            _fixedYRotation = transform.eulerAngles.y;
        }

        private void DebugAttack()
        {
            if (InputManager.AttackWasPressed)
            {
                if (_curComboTimer > 0)
                {
                    Debug.Log("Combo Attack");
                }
                else if (_curAttDelay <= 0)
                {
                    Debug.Log("Normal Attack");
                }
                else
                {
                    Debug.Log($"Couldn't attack because : \nCurrent combo Timer : {_curComboTimer} \nCurrent attack delay {_curAttDelay}");
                }
            }
        }

        // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
        void Update()
        {
            //DebugAttack();
            
            FixRotatingParent();

            FacingToMouse();
            
            UpdateTimer();
            
            CheckAttack();
        }

        private void FixRotatingParent()
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, _fixedYRotation, transform.eulerAngles.z);

            if (_polygonCollider2D.enabled)
            {
                transform.rotation = _slashRotation;
            }
        }

        private void FacingToMouse() // So that it face to the mouse (may need to change it so that it works in multiplayer)
        {
            if (!_polygonCollider2D.enabled) // We only face to the mouse when the player is not attacking (else he could turn fast and attack everywhere)
            {
                // Don't ask me, I don't know : https://youtu.be/bY4Hr2x05p8?t=133
                //Vector3 difference = _playerCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                if (IsOwner)
                    UpdateDirToMouse();
                //float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                float rotZ = Mathf.Atan2(_dirToMouse.Value.y, _dirToMouse.Value.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, rotZ + offset);
                _slashRotation = transform.rotation;
            }
        }
        
        private void UpdateDirToMouse()
        {
            Vector3 mouseWorld = _playerCamera.ScreenToWorldPoint(Input.mousePosition);
            _dirToMouse.Value = (mouseWorld - transform.position).normalized;
        }

        private void CheckAttack()
        {
            if (InputManager.AttackWasPressed && IsOwner) // init the buffer
            {
                _curBufferTimer = bufferAttackTimer;
            }
            
            if (_curBufferTimer > 0 && !_polygonCollider2D.enabled && (_curAttDelay <= 0 || _curComboTimer > 0)) // check if attack init and if either in a combo or starting a combo
            {
                _curBufferTimer = 0;
                Attack();
                /*Debug.Log($"Mouse pos : {(Input.mousePosition - transform.parent.transform.position).normalized}, posWithCam : {_playerCamera.ScreenToWorldPoint(Input.mousePosition).normalized}");
                Debug.Log($"Player pos : {transform.position}");*/
            }
        }

        private void Attack()
        {
            _curAttDelay = attackDelay;

            if (_curComboTimer > 0) // basically : if he was doing a combo he can't anymore else he can combo // is in a combo
            {
                _curComboTimer = 0;
                _animator.Play("SlashDown");
            }
            else // is not in a combo
            {
                _curComboTimer = comboTimer;
                _animator.Play("SlashUp");
            }
            
            _polygonCollider2D.enabled = true;
        }

        public void DisableCollider() // to disable the collider when the attack is finished
        {
            _polygonCollider2D.enabled = false;
            _animator.Play("Passive");
        }

        private void UpdateTimer()
        {
            float deltaTime = Time.deltaTime;

            if (_curAttDelay > 0)
            {
                _curAttDelay -= deltaTime;
            }
            if (_curComboTimer > 0)
            {
                _curComboTimer -= deltaTime;
            }
            if (_curBufferTimer > 0)
            {
                _curBufferTimer -= deltaTime;
            }
        }

        private bool TargetingPlayerProjectileBehaviour(Collider2D other)
        {
            Projectile proj = other.GetComponent<Projectile>();

            if (proj.Proj.CanBeDestroyedByPlayer)
            {
                if (proj.Proj.CanBeDestroyedBySelf)
                    return true;
                else
                    return proj.Proj.SenderId != _playerId;
            }

            return proj.Proj.CanBeDestroyedBySelf && proj.Proj.SenderId == _playerId;
        }

        private bool CanAttackThat(Collider2D other)
        {
            //return GM.IsTargetForPlayer(other) || GM.IsTargetForEnemy(other);
            return GM.IsPlayer(other) ||
                   GM.IsEnemy(other) ||
                   (GM.IsPlayerProjectile(other) && TargetingPlayerProjectileBehaviour(other)) ||
                   GM.IsEnemyProjectile(other);
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (CanAttackThat(other))
            {
                IUnitHp otherHp = other.GetComponent<IUnitHp>();
                
                if (otherHp.IsNetwork)
                    otherHp.TakeDamage(playerAttack);
                else if (other.TryGetComponent<Projectile>(out Projectile projectile)) // if the attacked object is local
                    GM.ProjM.DoDamageToProjManager(GM.ProjM.GetProjLstId(projectile), playerAttack);
                else
                    Debug.Log($"Couldn't locally remove health to : {other.gameObject.name}");
            }
        }
    }
}
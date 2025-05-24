using System;
using _Scripts.GameManager;
using _Scripts.Health;
using _Scripts.Inputs;
using _Scripts.Multiplayer;
using _Scripts.Projectiles;
using _Scripts.UI_scripts;
using Unity.Netcode;
using UnityEditor.Experimental;
using UnityEngine;
using static _Scripts.Player.PowerUp.PowerUp;

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

        private bool _controllerAttack;
        
        // https://discussions.unity.com/t/mouse-movements-for-client-side-becoming-server-side-mouse-movements/938064/2
        private NetworkVariable<Vector2> _dirToAttack = new NetworkVariable<Vector2>(default, 
            NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        private bool _attackedLocally = false;

        [SerializeField] private CooldownUI weaponCooldown; 
        
        public override void OnNetworkSpawn()
        {
            _playerId = transform.parent.GetComponent<PlayerId>().GetPlayerId();
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
                    Debug.Log($"Couldn't attack because : Current combo Timer : {_curComboTimer} Current attack delay {_curAttDelay}");
                }
            }
        }

        // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
        void Update()
        {
            //DebugAttack();
            
            FixRotatingParent();

            //FacingToMouse();
            
            UpdateTimer();
            
            CheckAttack();
            
            if (IsOwner)
                if (_curComboTimer <= 0) // we don't want it to show if we can still do a combo
                    weaponCooldown.CooldownManagement(_curAttDelay, attackDelay);
        }

        private void FixRotatingParent()
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, _fixedYRotation, transform.eulerAngles.z);

            if (_polygonCollider2D.enabled)
            {
                transform.rotation = _slashRotation;
            }
        }

        private void FacingToAttack() // So that it face to the mouse (may need to change it so that it works in multiplayer)
        {
            if (!_polygonCollider2D.enabled) // We only face to the mouse when the player is not attacking (else he could turn fast and attack everywhere)
            {
                float rotZ = Mathf.Atan2(_dirToAttack.Value.y, _dirToAttack.Value.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, rotZ + offset);
                _slashRotation = transform.rotation;
            }
        }

        private void UpdateDirToAttack(bool isController = false)
        {
            if (isController)
            {
                _dirToAttack.Value = InputManager.AimController.normalized;
            }
            else
            {
                // Don't ask me, I don't know : https://youtu.be/bY4Hr2x05p8?t=133
                //Vector3 difference = _playerCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                //float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                Vector3 mouseWorld = _playerCamera.ScreenToWorldPoint(Input.mousePosition);
                _dirToAttack.Value = (mouseWorld - transform.position).normalized;
            }
        }
        
        private void CheckAttack()
        {
            if (InputManager.AttackWasPressed && IsOwner) // init the buffer
            {
                _controllerAttack = false;
                _curBufferTimer = bufferAttackTimer;
            }

            if (InputManager.AttackControllerWasPressed && InputManager.AimController != Vector2.zero)
            {
                _controllerAttack = true;
                _curBufferTimer = bufferAttackTimer;
                UpdateDirToAttack(true);
            }
            
            if (_curBufferTimer > 0 && !_polygonCollider2D.enabled && (_curAttDelay <= 0 || _curComboTimer > 0)) // check if attack init and if either in a combo or starting a combo
            {
                if (_controllerAttack)
                {
                    _controllerAttack = false;
                    if (InputManager.AimController != Vector2.zero) // not sure 
                        UpdateDirToAttack(true);
                }
                else
                    UpdateDirToAttack();
                
                FacingToAttack();
                
                _curBufferTimer = 0;
                AttackLocally();
                AttackServerRpc();
            }
        }

        private void AttackLocally()
        {
            Attack();
            _attackedLocally = true;
        }

        [ServerRpc]
        private void AttackServerRpc()
        {
            if (_attackedLocally)
            {
                _attackedLocally = false;
                return;
            }
            
            Attack();
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
            if (CanAttackThat(other) && other.TryGetComponent<IUnitHp>(out IUnitHp otherHp))
            {
                if (otherHp.IsNetwork)
                {
                    if (IsServer)
                        otherHp.TakeDamage(playerAttack);
                }
                else // if the attacked object is local
                {
                    //GM.ProjM.DoDamageToProjManager(GM.ProjM.GetProjLstId(projectile), playerAttack);
                    otherHp.TakeDamage(playerAttack);
                }
            }
        }
    }
}
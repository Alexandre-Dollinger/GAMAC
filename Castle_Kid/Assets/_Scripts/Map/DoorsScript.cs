using System;
using _Scripts.GameManager;
using _Scripts.Inputs;
using _Scripts.Multiplayer;
using JetBrains.Annotations;
using UnityEngine;

namespace _Scripts.Map
{
    public enum DoorType
    {
        Stone,
        Portal,
        Temple,
        TempleUnique,
    }

    public class DoorsScript : MonoBehaviour
    {
        [SerializeField] private DoorType doorType;
        [SerializeField] private DoorsScript destinationDoor;
        private Animator _doorAnimator;
        [CanBeNull] private GameObject _player;

        public Vector3 GetDoorPos()
        {
            return transform.position;
        }
    
        public void Awake()
        {
            _doorAnimator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (_player != null && InputManager.InteractWasPressed)
                MakePlayerEnterDoor();
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (GM.IsPlayer(other) && other.transform.parent.parent.GetComponent<PlayerId>().IsItMyPlayer())
            {
                _player = other.transform.parent.parent.gameObject;
                if (doorType != DoorType.Portal)
                    _doorAnimator.Play(GetAnimationToPlay());
            }
        }

        public void OnTriggerExit2D(Collider2D other)
        {
            if (GM.IsPlayer(other) && other.transform.parent.parent.GetComponent<PlayerId>().IsItMyPlayer())
            {
                _player = null;
                if (doorType != DoorType.Portal)
                    _doorAnimator.Play(GetAnimationToPlay(false));
            }
        }

        private void MakePlayerEnterDoor()
        {
            _player!.transform.position = destinationDoor.GetDoorPos();
        }

        private string GetAnimationToPlay(bool toOpen = true)
        {
            switch (doorType)
            {
                case DoorType.Stone:
                    return toOpen ? "StoneDoorAnimation" : "StoneDoorClosedAnimation";
                case DoorType.Temple:
                    return toOpen ? "TempleDoorAnimation" : "TempleDoorClosedAnimation";
                case DoorType.TempleUnique:
                    return toOpen ? "TempleDoorUniqueAnimation" : "TempleDoorUniqueClosedAnimation";
            }

            throw new ArgumentException("Don't know that DoorType : " + doorType);
        }
    }
    
    
}
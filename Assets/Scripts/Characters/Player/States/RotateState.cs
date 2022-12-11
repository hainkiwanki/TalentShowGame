using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ERotationDirection
{
    MOVEMENT,
    MOUSE,
}
namespace PlayerStates
{
    [CreateAssetMenu(fileName = "New state", menuName = "Binki/Player/States/Rotate")]
    public class RotateState : PlayerStateData
    {
        public float rotationSpeed = 10.0f;
        public ERotationDirection direction = ERotationDirection.MOVEMENT;

        public override void OnEnter(PlayerState _state, AnimatorStateInfo _animInfo, Animator animator)
        {
        }

        public override void OnExit(PlayerState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
        }

        public override void OnUpdate(PlayerState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
            CharacterControl control = _state.GetCharControl(_animator);
            Vector3 lookDir = Vector3.zero;
            switch (direction)
            {
                case ERotationDirection.MOVEMENT:
                    lookDir = control.moveDir;
                    break;
                case ERotationDirection.MOUSE:
                    lookDir = (control.mousePos.NewY(0.0f) - control.transform.position.NewY(0.0f)).normalized;
                    break;
                default:
                    break;
            }

            control.Rotate(lookDir, rotationSpeed);
        }
    }
}
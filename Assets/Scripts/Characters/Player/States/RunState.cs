using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates
{
    [CreateAssetMenu(fileName = "New state", menuName = "Binki/Player/States/Run")]
    public class RunState : PlayerStateData
    {
        public float multiplier = 1.0f;
        public override void OnEnter(PlayerState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
        }

        public override void OnExit(PlayerState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
        }

        public override void OnUpdate(PlayerState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
            CharacterControl control = _state.GetCharControl(_animator);
            float speed = GAMESTATS.moveSpeed * multiplier;
            speed *= 1.0f - Mathf.Clamp(control.slowStacks * 0.1f, 0.0f, 0.9f);
            control.Move(control.moveDir, speed);

            _animator.SetBool(EPlayerTransitionParams.isHome.ToString(), SceneLoader.Inst.currentSceneIndex == 3);

            if (control.usedDodge)
                _animator.SetBool(EPlayerTransitionParams.usedDodge.ToString(), true);

            if (control.input == Vector3.zero)
                _animator.SetBool(EPlayerTransitionParams.isRunning.ToString(), false);

            if (control.CanAttack())
            {
                if (control.usedPrimary)
                {
                    _animator.SetBool(EPlayerTransitionParams.usedPrimary.ToString(), true);
                }

                if (control.isChargingPrimary)
                    _animator.SetBool(EPlayerTransitionParams.isChargingPrimary.ToString(), true);
            }
        }
    }
}
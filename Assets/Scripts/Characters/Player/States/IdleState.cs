using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates
{
    [CreateAssetMenu(fileName = "New state", menuName = "Binki/Player/States/Idle")]
    public class IdleState : PlayerStateData
    {
        public override void OnEnter(PlayerState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
            _animator.SetBool(EPlayerTransitionParams.usedPrimary.ToString(), false);
            _animator.SetBool(EPlayerTransitionParams.usedSecondary.ToString(), false);
        }

        public override void OnExit(PlayerState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
        }

        public override void OnUpdate(PlayerState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
            CharacterControl control = _state.GetCharControl(_animator);

            _animator.SetBool(EPlayerTransitionParams.isHome.ToString(), SceneLoader.Inst.currentSceneIndex == 3);

            if (control.usedDodge)
                _animator.SetBool(EPlayerTransitionParams.usedDodge.ToString(), true);

            if (control.input != Vector3.zero)
                _animator.SetBool(EPlayerTransitionParams.isRunning.ToString(), true);

            if (control.CanAttack())
            {
                if (control.usedPrimary)
                {
                    _animator.SetBool(EPlayerTransitionParams.usedPrimary.ToString(), true);
                }

                if (control.isChargingPrimary)
                {
                    _animator.SetBool(EPlayerTransitionParams.isChargingPrimary.ToString(), true);
                }
            }
        }
    }
}
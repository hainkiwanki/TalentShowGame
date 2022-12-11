using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates
{
    [CreateAssetMenu(fileName = "New state", menuName = "Binki/Player/States/Dodge")]
    public class DodgeState : PlayerStateData
    {
        [MinMaxSlider(0.0f, 1.0f, true)]
        public Vector2 immuneWindow = new Vector2(0.2f, 0.8f);

        public override void OnEnter(PlayerState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
            _animator.SetBool(EPlayerTransitionParams.usedDodge.ToString(), false);
        }

        public override void OnExit(PlayerState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
            CharacterControl control = _state.GetCharControl(_animator);
            control.isImmune = false;
        }

        public override void OnUpdate(PlayerState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
            CharacterControl control = _state.GetCharControl(_animator);
            control.isImmune = _animInfo.normalizedTime >= immuneWindow.x && _animInfo.normalizedTime <= immuneWindow.y;

            if(_animInfo.normalizedTime >= 0.90f)
            {
                if(control.usedPrimary)
                {
                    _animator.SetBool(EPlayerTransitionParams.usedPrimary.ToString(), true);
                }
                if(control.isChargingPrimary)
                {
                    _animator.SetBool(EPlayerTransitionParams.isChargingPrimary.ToString(), true);
                }
                if(control.input != Vector3.zero)
                {
                    _animator.SetBool(EPlayerTransitionParams.isRunning.ToString(), true);
                }
            }
        }
    }
}
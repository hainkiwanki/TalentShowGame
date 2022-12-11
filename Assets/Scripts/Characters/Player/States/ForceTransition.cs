using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates
{
    [CreateAssetMenu(fileName = "New state", menuName = "Binki/Player/States/Force Transition")]
    public class ForceTransition : PlayerStateData
    {
        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float transitionTime;

        public override void OnEnter(PlayerState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
        }

        public override void OnExit(PlayerState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
            _animator.SetBool(EPlayerTransitionParams.forceTransition.ToString(), false);
        }

        public override void OnUpdate(PlayerState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
            if (_animInfo.normalizedTime >= transitionTime)
                _animator.SetBool(EPlayerTransitionParams.forceTransition.ToString(), true);
        }
    }
}
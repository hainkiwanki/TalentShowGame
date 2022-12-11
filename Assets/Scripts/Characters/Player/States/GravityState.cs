using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates
{
    [CreateAssetMenu(fileName = "New state", menuName = "Binki/Player/States/Gravity")]
    public class GravityState : PlayerStateData
    {
        public override void OnEnter(PlayerState _state, AnimatorStateInfo _animInfo, Animator animator)
        {
        }

        public override void OnExit(PlayerState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
        }

        public override void OnUpdate(PlayerState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
            CharacterControl control = _state.GetCharControl(_animator);
            if(!control.IsGrounded() && SceneLoader.Inst.currentSceneIndex >= 4)
                control.Move(Vector3.down, 9.81f);
        }
    }
}
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates
{
    [CreateAssetMenu(fileName = "New state", menuName = "Binki/Player/States/Force Move")]
    public class ForcedMoveState : PlayerStateData
    {
        public AnimationCurve speedCurve;
        public float speed;
        public bool usePlayerForward = false;
        [ShowIf("@!usePlayerForward")]
        public Vector3 moveDir;

        public override void OnEnter(PlayerState _state, AnimatorStateInfo _animInfo, Animator animator)
        {
        }

        public override void OnExit(PlayerState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
        }

        public override void OnUpdate(PlayerState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
            CharacterControl control = _state.GetCharControl(_animator);
            Vector3 dir = control.transform.forward;
            if(!usePlayerForward)
            {
                dir = control.transform.forward * moveDir.z + control.transform.right * moveDir.x;
            }
            control.ForceMove(dir, speed * speedCurve.Evaluate(_animInfo.normalizedTime));
        }
    }
}
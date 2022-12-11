using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyStates
{
    [CreateAssetMenu(fileName = "New state", menuName = "Binki/Simp/States/Float")]
    public class FloatingState : EnemyStateData
    {
        public float arriveDistance = 1.0f;
        public float speed = 4.5f;

        public override void OnEnter(EnemyState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
            Simp simp = _state.GetCharControl(_animator);
            simp.navAgent.speed = speed * GAMESTATS.simpMoveSpeedMulti;
            if (simp.isMod)
                simp.navAgent.speed = speed * GAMESTATS.modMoveSpeedMulti;
            if (simp.isGnome)
                simp.navAgent.speed *= 1.7f;

            if (simp.isHuge)
            {
                simp.navAgent.speed *= 0.3f;
            }
            if (simp.isTransparent)
            {
                simp.navAgent.speed *= 1.2f;
            }

            simp.navAgent.stoppingDistance = arriveDistance;
        }
        
        public override void OnExit(EnemyState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
        }

        public override void OnUpdate(EnemyState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
            Simp simp = _state.GetCharControl(_animator);
            if (!simp.hasTarget)
                return;
            simp.navAgent.destination = simp.target.position;
        }
    }
}
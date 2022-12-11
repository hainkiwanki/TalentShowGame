using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyStates
{
    [CreateAssetMenu(fileName = "New state", menuName = "Binki/Simp/States/Idle")]
    public class IdleState : EnemyStateData
    {
        public float attackDistance = 1.1f;
        public float banHammerDistance = 5.5f;


        public override void OnEnter(EnemyState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
            Simp simp = _state.GetCharControl(_animator);
            simp.navAgent.speed = 0;
            simp.navAgent.velocity = Vector3.zero;
            simp.navAgent.destination = simp.transform.position;
        }

        public override void OnExit(EnemyState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
        }

        public override void OnUpdate(EnemyState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
            Simp simp = _state.GetCharControl(_animator);
            if (simp.hasTarget && simp.hasSpawned)
            {

                float stopDistance = (simp.isFriendly) ? banHammerDistance : attackDistance;
                if (simp.isBoss)
                    stopDistance *= 2.0f;

                float distance = Vector3.Distance(simp.transform.position, simp.target.position);
                bool isTargetInFront = _state.IsTargetInFront(_animator, stopDistance, 0.5f);
                if(isTargetInFront)
                {
                    int attack = Random.Range(1, 2);
                    if (simp.isFriendly && simp.canHammerAttack)
                    {
                        _animator.SetBool(EEnemyTransitionParams.banAttack.ToString(), true);
                    }
                    if(!simp.isFriendly)
                    {
                        _animator.SetInteger(EEnemyTransitionParams.attack.ToString(), attack);
                    }
                }
                if(distance > stopDistance || !isTargetInFront)
                {
                    _animator.SetBool(EEnemyTransitionParams.isRunning.ToString(), true);
                }
            }
        }
    }
}
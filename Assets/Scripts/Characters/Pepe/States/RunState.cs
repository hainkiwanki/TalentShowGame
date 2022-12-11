using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyStates
{
    [CreateAssetMenu(fileName = "New state", menuName = "Binki/Simp/States/Run")]
    public class RunState : EnemyStateData
    {
        public float arriveDistance = 1.0f;
        public float banHammerDistance = 5.5f;
        public float speed = 3.5f;

        public override void OnEnter(EnemyState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
            Simp simp = _state.GetCharControl(_animator);
            simp.navAgent.speed = speed * GAMESTATS.simpMoveSpeedMulti;
            if(simp.isMod)
                simp.navAgent.speed = speed * GAMESTATS.modMoveSpeedMulti;
            if (simp.isGnome)
                simp.navAgent.speed *= 1.7f;

            if(simp.isHuge)
            {
                simp.navAgent.speed *= 0.3f;
            }

            if(simp.isTransparent)
            {
                simp.navAgent.speed *= 1.4f;
            }
        }

        public override void OnExit(EnemyState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
        }

        public override void OnUpdate(EnemyState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
            Simp simp = _state.GetCharControl(_animator);
            if (!simp.hasTarget)
                return;

            float stopDistance = (simp.isFriendly) ? banHammerDistance : arriveDistance;
            if (simp.isBoss)
                stopDistance *= 2.0f;

            float distance = Vector3.Distance(simp.transform.position, simp.target.position);
            bool isTargetInFront = _state.IsTargetInFront(_animator, stopDistance, 0.5f);
            if (isTargetInFront)
            {
                if (simp.isFriendly && simp.canHammerAttack)
                    _animator.SetBool(EEnemyTransitionParams.banAttack.ToString(), true);
                if(!simp.isFriendly)
                    _animator.SetInteger(EEnemyTransitionParams.attack.ToString(), Random.Range(1, 2));
            }

            simp.navAgent.destination = simp.target.position;
            if (distance <= stopDistance)
            {
                Vector3 lookDir = (simp.target.position - simp.transform.position).normalized;
                Quaternion targetRot = Quaternion.LookRotation(lookDir);
                simp.transform.rotation = Quaternion.Slerp(simp.transform.rotation, targetRot, 4.0f * Time.deltaTime);
            }
        }
    }
}
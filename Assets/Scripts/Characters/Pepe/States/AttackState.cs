using UnityEngine;

namespace EnemyStates
{
    [CreateAssetMenu(fileName = "New state", menuName = "Binki/Simp/States/Attack")]
    public class AttackState : EnemyStateData
    {
        [Range(0.0f, 1.0f)]
        public float releaseAttackTime;
        private bool hasAttacked = false;
        public int attackIndex = 0;

        public override void OnEnter(EnemyState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
            _animator.SetBool(EEnemyTransitionParams.banAttack.ToString(), false);
            _animator.SetInteger(EEnemyTransitionParams.attack.ToString(), 0);
            Simp simp = _state.GetCharControl(_animator);
            simp.navAgent.speed = 0;
            simp.navAgent.velocity = Vector3.zero;
            simp.navAgent.destination = simp.transform.position;
            if (simp.isFriendly && simp.hasTarget)
            {
                simp.transform.rotation = Quaternion.LookRotation(simp.target.transform.position - simp.transform.position);
            }
        }

        public override void OnExit(EnemyState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
            _animator.SetBool(EEnemyTransitionParams.banAttack.ToString(), false);
            hasAttacked = false;
        }

        public override void OnUpdate(EnemyState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
            Simp simp = _state.GetCharControl(_animator);
            if(_animInfo.normalizedTime >= releaseAttackTime && !hasAttacked && simp.hasTarget)
            {
                hasAttacked = true;

                if (simp.isFriendly)
                {
                    simp.PlayBanParticle();
                }
                else
                {
                    _state.GetCharControl(_animator).PlayAttackParticle(attackIndex);
                }
            }
        }
    }
}
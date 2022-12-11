using Sirenix.OdinInspector;
using UnityEngine;

namespace PlayerStates
{
    [CreateAssetMenu(fileName = "New state", menuName = "Binki/Player/States/Attack")]
    public class AttackState : PlayerStateData
    {
        public float timeToReleaseAttack;
        private bool hasAttacked = false;
        public int tier = 0;
        public float damageMultiplier = 1.0f;

        [MinMaxSlider(0.0f, 1.0f, true)]
        public Vector2 comboWindow;

        private bool usedPrimaryCombo = false, usedSecondaryCombo = false;

        public override void OnEnter(PlayerState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
            _animator.SetBool(EPlayerTransitionParams.usedPrimary.ToString(), false);
            _animator.SetBool(EPlayerTransitionParams.usedSecondary.ToString(), false);
            usedPrimaryCombo = false;
            usedSecondaryCombo = false;
            hasAttacked = false;
        }

        public override void OnExit(PlayerState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
            _animator.SetBool(EPlayerTransitionParams.usePrimaryCombo.ToString(), false);
            _animator.SetBool(EPlayerTransitionParams.useSecondaryCombo.ToString(), false);
        }

        public override void OnUpdate(PlayerState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
            CharacterControl control = _state.GetCharControl(_animator);

            if (control.usedDodge)
                _animator.SetBool(EPlayerTransitionParams.usedDodge.ToString(), true);

            if (!hasAttacked && _animInfo.normalizedTime >= timeToReleaseAttack)
            {
                hasAttacked = true;
                var foodObj = PoolManager.Inst.GetObject(control.currentNormalAmmo);
                foodObj.SetActive(true);
                Food food = foodObj.GetComponent<Food>();
                food.transform.position = control.transform.position + Vector3.up * 0.8f + control.transform.forward * 0.2f;
                food.transform.rotation = control.transform.rotation;
                food.damage = GAMESTATS.damage * damageMultiplier;
                food.AddForce(control.transform.forward * GAMESTATS.throwForce, tier);
                control.OnAttack();
            }
            if (hasAttacked)
            {
                if (control.usedPrimary)
                {
                    usedSecondaryCombo = false;
                    usedPrimaryCombo = true;
                }
                if (control.usedSecondary)
                {
                    usedSecondaryCombo = true;
                    usedPrimaryCombo = false;
                }
            }
            CheckCombo(_state, _animInfo, _animator);
        }

        public void CheckCombo(PlayerState _state, AnimatorStateInfo _stateInfo, Animator _animator)
        {
            CharacterControl control = _state.GetCharControl(_animator);
            if (_stateInfo.normalizedTime >= comboWindow.x && _stateInfo.normalizedTime < comboWindow.y)
            {
                if (usedPrimaryCombo)
                {
                    _animator.SetBool(EPlayerTransitionParams.usePrimaryCombo.ToString(), true);
                }
                else if (usedSecondaryCombo)
                {
                    _animator.SetBool(EPlayerTransitionParams.useSecondaryCombo.ToString(), true);
                }
            }
        }
    }
}
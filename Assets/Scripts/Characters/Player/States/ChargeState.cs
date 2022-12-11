using Sirenix.OdinInspector;
using UnityEngine;

namespace PlayerStates
{
    [CreateAssetMenu(fileName = "New state", menuName = "Binki/Player/States/Charge Attack")]
    public class ChargeState : PlayerStateData
    {
        private float chargeTime = 0.0f;
        private float maxChargeTime;
        private bool hasAttacked = false;

        [MinMaxSlider(1.0f, 10.0f, true)]
        public Vector2 bonusMultiplier = new Vector2(1.0f, 2.0f);

        [MinMaxSlider(0.0f, 100.0f, true)]
        public Vector2 forceRange = new Vector2(10.0f, 30.0f);

        public override void OnEnter(PlayerState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
            CharacterControl control = _state.GetCharControl(_animator);

            _animator.SetBool(EPlayerTransitionParams.usedPrimary.ToString(), false);
            _animator.SetBool(EPlayerTransitionParams.usedSecondary.ToString(), false);

            control.chargeParticle.SetPower(0.0f);
            maxChargeTime = GAMESTATS.chargeTime;
            Debug.Log(maxChargeTime);
            chargeTime = 0.0f;
            hasAttacked = false;
        }

        public override void OnExit(PlayerState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
            _animator.SetBool(EPlayerTransitionParams.usedPrimary.ToString(), false);
            _animator.SetBool(EPlayerTransitionParams.usedSecondary.ToString(), false);
        }

        public override void OnUpdate(PlayerState _state, AnimatorStateInfo _animInfo, Animator _animator)
        {
            CharacterControl control = _state.GetCharControl(_animator);

            if (control.usedDodge)
            {
                control.chargeParticle.ForceStop();
                _animator.SetBool(EPlayerTransitionParams.usedDodge.ToString(), true);
            }

            if (!control.isChargingPrimary)
            {
                hasAttacked = true;
                control.chargeParticle.Stop(control.camera.transform);
                Attack(control, chargeTime);
                _animator.SetBool(EPlayerTransitionParams.isChargingPrimary.ToString(), false);
                return;
            }

            if (!hasAttacked)
            {
                if (chargeTime > maxChargeTime)
                    chargeTime = maxChargeTime;
                control.chargeParticle.SetPower(chargeTime / maxChargeTime);
                chargeTime += Time.deltaTime;
            }
        }

        private void Attack(CharacterControl _control, float _chargeTimeNormalized)
        {
            var foodObj = PoolManager.Inst.GetObject(_control.currentChargedAmmo);
            foodObj.SetActive(true);
            Food food = foodObj.GetComponent<Food>();
            food.transform.position = _control.transform.position + Vector3.up * 0.8f + _control.transform.forward * 0.2f;
            food.transform.rotation = Quaternion.LookRotation(_control.transform.forward) * Quaternion.Euler(food.correctionRotation);
            float bonusMulti = Mathf.Lerp(bonusMultiplier.x, bonusMultiplier.y, _chargeTimeNormalized);
            food.damage = GAMESTATS.damage * bonusMulti;
            float force = Mathf.Lerp(forceRange.x, forceRange.y, _chargeTimeNormalized);
            food.AddForce(_control.transform.forward * force, 0);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EEnemyTransitionParams
{
    isRunning, attack, banAttack, usedSpecial, 
}

public class EnemyState : StateMachineBehaviour
{
    public List<EnemyStateData> transitionList = new List<EnemyStateData>();
    private Simp simpControl;

    public bool IsTargetInFront(Animator _anim, float _distance = 1.111f, float _sphereRadius = 0.1f)
    {
        Simp simp = GetCharControl(_anim);
        Ray ray = new Ray(simp.transform.position + simp.transform.up * 0.5f, simp.transform.forward);
        int layerMask = 1 << 9;
        if (simp.isFriendly)
            layerMask = 1 << 10;
        bool isTargetInFront = Physics.SphereCast(ray, _sphereRadius, out RaycastHit hit, _distance, layerMask);
        if(isTargetInFront)
        {
            if (hit.transform == simp.target)
            {
                return true;
            }
        }

        return false;
    }

    public Simp GetCharControl(Animator _anim)
    {
        if (simpControl == null)
            simpControl = _anim.GetComponentInParent<Simp>();

        return simpControl;
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (EnemyStateData s in transitionList)
            s.OnEnter(this, stateInfo, animator);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (EnemyStateData s in transitionList)
            s.OnExit(this, stateInfo, animator);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (EnemyStateData s in transitionList)
            s.OnUpdate(this, stateInfo, animator);
    }
}

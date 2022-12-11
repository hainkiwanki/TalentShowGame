using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class EnemyStateData : SerializedScriptableObject
{
    public abstract void OnEnter(EnemyState _state, AnimatorStateInfo _animInfo, Animator _animator);
    public abstract void OnUpdate(EnemyState _state, AnimatorStateInfo _animInfo, Animator _animator);
    public abstract void OnExit(EnemyState _state, AnimatorStateInfo _animInfo, Animator _animator);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class PlayerStateData : SerializedScriptableObject
{
    public abstract void OnEnter(PlayerState _state, AnimatorStateInfo _animInfo, Animator _animator);
    public abstract void OnUpdate(PlayerState _state, AnimatorStateInfo _animInfo, Animator _animator);
    public abstract void OnExit(PlayerState _state, AnimatorStateInfo _animInfo, Animator _animator);
}

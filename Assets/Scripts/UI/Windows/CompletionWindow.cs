using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class CompletionWindow : MonoBehaviour
{
    private Animator m_animator;
    [SerializeField]
    private GameObject m_animationContainer;
    [SerializeField]
    private TextMeshProUGUI m_messageText;
    public bool isAnimating = false;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_animator.speed = 0;
        m_animationContainer.SetActive(false);
    }

    public void Hide()
    {
        if (isAnimating)
            StopAllCoroutines();

        isAnimating = false;
        m_animationContainer.SetActive(false);
        if (m_animator == null)
            m_animator = GetComponent<Animator>();
        m_animator.enabled = false;
        m_animator.enabled = true;
    }

    public void ShowMessage(string _message)
    {
        if(isAnimating)
        {
            Debug.Log("Is animating already");
            return;
        }
        isAnimating = true;
        m_messageText.text = _message;
        m_animationContainer.SetActive(true);
        m_animator.speed = 1.0f;
        StartCoroutine(CO_DelayFunction(2.0f, () =>
        {
            m_animator.speed = -1.0f;
            StartCoroutine(CO_DelayFunction(2.0f, () =>
            {
                m_animationContainer.SetActive(false);
                m_animator.speed = 0.0f;
                isAnimating = false;
            }));
        }));
    }

    IEnumerator CO_DelayFunction(float _delay, UnityAction _function)
    {
        yield return new WaitForSecondsRealtime(_delay);
        _function.Invoke();
    }
}

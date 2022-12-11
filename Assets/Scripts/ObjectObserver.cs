using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectObserver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Transform targetObject;
    public float rotationSpeed = 10.0f;
    private bool m_isHoldingLMB = false;
    private bool m_canDrag = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_canDrag = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_canDrag = false;
    }

    private void Awake()
    {
        GameManager.Inst.controls.Player.MouseDelta.performed += _ => OnMouseMove(_.ReadValue<Vector2>());
        GameManager.Inst.controls.Player.HoldPrimary.performed += _ => m_isHoldingLMB = true;
        GameManager.Inst.controls.Player.HoldPrimary.canceled += _ => m_isHoldingLMB = false;
    }

    private void OnDestroy()
    {
        GameManager.Inst.controls.Player.MouseDelta.performed -= _ => OnMouseMove(_.ReadValue<Vector2>());
        GameManager.Inst.controls.Player.HoldPrimary.performed -= _ => m_isHoldingLMB = true;
        GameManager.Inst.controls.Player.HoldPrimary.canceled -= _ => m_isHoldingLMB = false;
    }

    private void OnMouseMove(Vector2 _screenMousePos)
    {
        if(m_isHoldingLMB && m_canDrag)
        {
            float yRot = -rotationSpeed * _screenMousePos.x * Time.deltaTime;
            targetObject.rotation = targetObject.rotation * Quaternion.Euler(0.0f, yRot, 0.0f);
        }
    }
}

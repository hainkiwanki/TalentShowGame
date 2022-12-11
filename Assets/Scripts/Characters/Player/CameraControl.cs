using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraControl : MonoBehaviour
{
    [Header("Camera view")]
    public float distanceFromTarget = 8.0f;
    public Vector3 defaultRotation = new Vector3(53.0f, 45.0f, 0.0f);
    public float smoothSpeed = 10.0f;
    public Camera mainCamera;
    [SerializeField]
    private Transform m_target;
    private Transform m_dummyTarget;

    [Header("Zoom settings")]
    public float zoomSpeed = 0.5f;
    public Vector2 zoomLimits = new Vector2(5.0f, 12.0f);
    public float offset = 0.9f;
    private float m_goalZoom = 0.0f;

    private bool m_isInitialized = false;
    [HideInInspector]
    public bool isMoving = false;

    private void Awake()
    {
        Initialize(m_target);
    }

    public void Initialize(Transform _player = null)
    {
        if (m_isInitialized)
            return;

        mainCamera.transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localRotation = Quaternion.Euler(defaultRotation);

        if(_player == null)
        {
            var go = new GameObject("Dummy Target");
            m_dummyTarget = go.transform;
            m_target = m_dummyTarget;
        }
        else
        {
            m_target = _player;
        }

        mainCamera.transform.position = m_target.position;
        mainCamera.transform.position = -mainCamera.transform.forward * distanceFromTarget;
        transform.position = m_target.position;
        m_goalZoom = distanceFromTarget;

        GameManager.Inst.controls.Camera.Zoom.performed += _ => Zoom(_.ReadValue<float>());

        m_isInitialized = true;
    }

    private void Update()
    {
        if (!m_isInitialized)
            return;

        transform.position = Vector3.Lerp(transform.position, m_target.position.AddY(offset), smoothSpeed * Time.deltaTime);

        isMoving = Vector3.Distance(m_target.position, transform.position) <= 0.05f;

        Vector3 goalPos = mainCamera.transform.InverseTransformDirection(-mainCamera.transform.forward * m_goalZoom);
        mainCamera.transform.localPosition = Vector3.Lerp(mainCamera.transform.localPosition, goalPos, smoothSpeed * Time.deltaTime);
    }

    private void Zoom(float _value)
    {
        if (_value == 0.0f)
            return;

        m_goalZoom -= Mathf.Sign(_value) * zoomSpeed;
        m_goalZoom = Mathf.Clamp(m_goalZoom, zoomLimits.x, zoomLimits.y);
    }
}

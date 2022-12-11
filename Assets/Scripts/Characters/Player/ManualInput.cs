using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterControl))]
public class ManualInput : MonoBehaviour
{
    private Camera cam;
    private CharacterControl control;
    private Dictionary<EPlayerTransitionParams, int> framesPassed = new Dictionary<EPlayerTransitionParams, int>();

    private void Awake()
    {
        cam = Camera.main;
        control = GetComponent<CharacterControl>();
        BindPlayerInput();
        framesPassed.Add(EPlayerTransitionParams.usedPrimary, 0);
        framesPassed.Add(EPlayerTransitionParams.usedDodge, 0);
    }

    public void SetCam(Camera _cam)
    {
        cam = _cam;
    }

    private void BindPlayerInput()
    {
        GameManager.Inst.controls.Player.MousePos.performed += _ => MouseWorldPos(_.ReadValue<Vector2>());
        GameManager.Inst.controls.Player.Move.performed += _ => UsedMovementKeys(_.ReadValue<Vector2>());
        GameManager.Inst.controls.Player.Move.canceled += _ => ReleaseMovementKeys();

        GameManager.Inst.controls.Player.TapPrimary.performed += _ => control.usedPrimary = true;
        GameManager.Inst.controls.Player.HoldPrimary.performed += _ => control.isChargingPrimary = true;
        GameManager.Inst.controls.Player.HoldPrimary.canceled += _ => control.isChargingPrimary = false;

        GameManager.Inst.controls.Player.Dodge.performed += _ => control.usedDodge = SceneLoader.Inst.currentSceneIndex >= 4 && control.DodgeWhenSlowed();

        GameManager.Inst.controls.Player.Interact.performed += _ => control.Interact();
    }

    private void Update()
    {
        if(control.usedPrimary)
        {
            framesPassed[EPlayerTransitionParams.usedPrimary]++;
            if (framesPassed[EPlayerTransitionParams.usedPrimary] >= 5)
            {
                framesPassed[EPlayerTransitionParams.usedPrimary] = 0;
                control.usedPrimary = false;
            }
        }

        if(control.usedDodge)
        {
            framesPassed[EPlayerTransitionParams.usedDodge]++;
            if (framesPassed[EPlayerTransitionParams.usedDodge] >= 5)
            {
                framesPassed[EPlayerTransitionParams.usedDodge] = 0;
                control.usedDodge = false;
            }
        }
    }

    private void MouseWorldPos(Vector2 _mouseScreenPos)
    {
        Ray ray = cam.ScreenPointToRay(_mouseScreenPos);
        if(Physics.Raycast(ray, out RaycastHit hit, cam.farClipPlane, 1 << 8))
        {
            control.mousePos = hit.point;
        }
    }

    private void UsedMovementKeys(Vector2 _input)
    {
        control.input = new Vector3(_input.x, 0.0f, _input.y);
    }

    private void ReleaseMovementKeys()
    {
        control.input = Vector3.zero;
    }
}

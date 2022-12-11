using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class ActionInteractable : Interactable
{
    public EInteractableActions actionType = EInteractableActions.None;

    [ShowIf("actionType", EInteractableActions.None)]
    public UnityAction onUseAction; 

    [ShowIf("actionType", EInteractableActions.LoadScene)]
    public ESceneIndices sceneToLoad = ESceneIndices.Game;

    [ShowIf("actionType", EInteractableActions.ShowUI)]
    public EUiWindows uiWindow = EUiWindows.RunSettings;

    protected override void OnUse()
    {
        switch (actionType)
        {
            case EInteractableActions.None:
                onUseAction?.Invoke();
                break;
            case EInteractableActions.LoadScene:
                UIManager.Inst.questionBox.AddButtons("Ready to leave?",
                    new AnswerButton("Yes", () => { SceneLoader.Inst.LoadScene(sceneToLoad); }),
                    new AnswerButton("No", () => { UIManager.Inst.questionBox.Hide(); })
                    );
                break;
            case EInteractableActions.ShowUI:
                if (!UIManager.Inst.uiWindows.ContainsKey(uiWindow))
                    Debug.LogError("Trying to call window that is not set: " + uiWindow.ToString());
                else
                    UIManager.Inst.uiWindows[uiWindow].Show();
                break;
            default:
                break;
        }
    }
}

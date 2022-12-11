using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseWindow : UIElement
{
    [SerializeField]
    private Button m_returnHome;
    [SerializeField]
    private Button m_exitGame;

    protected override void OnHide()
    {
        Time.timeScale = 1.0f;
    }

    protected override void OnShow()
    {
        Time.timeScale = 0.0f;
    }

    public void OnPreShow()
    {
        m_returnHome.gameObject.SetActive(SceneLoader.Inst.currentSceneIndex != (int)ESceneIndices.Home);
    }

    public void OnReturnHome()
    {
        Hide();
        if(SceneLoader.Inst.currentLevelScene > 0)
        {
            SceneLoader.Inst.UnloadGameScene();
        }
        SceneLoader.Inst.LoadScene(ESceneIndices.Home);
    }

    public void OnExitGame()
    {
        Hide();
        SerializationManager.Save(SaveData.current);
        Application.Quit();
    }
}

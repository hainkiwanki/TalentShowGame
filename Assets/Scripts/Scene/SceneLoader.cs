using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum ESceneIndices
{
    Persistent = 0,
    Menu = 1,
    PlayerCustom = 2,
    Home = 3,
    Game = 4,
}

public class SceneLoader : Singleton<SceneLoader>
{
    public int currentSceneIndex = 0;
    public int previousSceneIndex = 0;

    public int currentLevelScene = 0;

    private GameObject m_loadingScreen;
    private List<AsyncOperation> m_scenesLoading = new List<AsyncOperation>();

    private void Awake()
    {
        m_loadingScreen = GameObject.FindGameObjectWithTag("LoadingScreen");
        m_loadingScreen.SetActive(false);
    }

    public void UnloadGameScene()
    {
        UnloadGameScene(currentLevelScene);
    }

    public void UnloadGameScene(int _index)
    {
        if (_index == currentLevelScene)
            SceneManager.UnloadSceneAsync(currentLevelScene);
        currentLevelScene = -1;
    }

    public void LoadGameScene(int _index, UnityAction _onComplete)
    {
        StartCoroutine(CO_LoadLevelScene(_index, _onComplete));
    }

    IEnumerator CO_LoadLevelScene(int _index, UnityAction _onComplete = null)
    {
        if (currentLevelScene > 0)
            m_scenesLoading.Add(SceneManager.UnloadSceneAsync(currentLevelScene));
        if(_index != 0)
            m_scenesLoading.Add(SceneManager.LoadSceneAsync(_index, LoadSceneMode.Additive));

        GameManager.Inst.controls.Disable();
        GameManager.Inst.SetPlayerCamera(true);
        GameManager.Inst.SetPlayerControl(true);
        GameManager.Inst.ResetPlayer();

        for (int i = 0; i < m_scenesLoading.Count; i++)
            while (!m_scenesLoading[i].isDone)
                yield return null;

        GameManager.Inst.controls.Enable();
        currentLevelScene = _index;
        if (_index != 0)
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(currentLevelScene));

        _onComplete?.Invoke();
    }

    public void LoadScene(ESceneIndices _index, UnityAction _onComplete = null)
    {
        StartCoroutine(CO_LoadScene((int)_index, _onComplete));
    }

    IEnumerator CO_LoadScene(int _index, UnityAction _onComplete = null)
    {
        m_scenesLoading = new List<AsyncOperation>();
        m_loadingScreen.SetActive(true);
        UIManager.Inst.HideAll();
        if (currentSceneIndex > 0)
            m_scenesLoading.Add(SceneManager.UnloadSceneAsync(currentSceneIndex));
        m_scenesLoading.Add(SceneManager.LoadSceneAsync(_index, LoadSceneMode.Additive));

        GameManager.Inst.controls.Disable();
        GameManager.Inst.SetPlayerCamera(true);
        GameManager.Inst.SetPlayerControl(true);
        GameManager.Inst.ResetPlayer();

        for (int i = 0; i < m_scenesLoading.Count; i++)
        {
            while (!m_scenesLoading[i].isDone)
            {
                yield return null;
            }
        }

        var sceneInitialiazer = FindObjectOfType<SceneInitializer>();

        float time = 0.0f;
        while (time <= 2.5f)
        {
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        _onComplete?.Invoke();
        m_loadingScreen.SetActive(false);

        previousSceneIndex = currentSceneIndex;
        currentSceneIndex = _index;
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(currentSceneIndex));

        if(sceneInitialiazer != null)
            sceneInitialiazer.ManualAwake();

        EventManager.onSceneLoaded?.Invoke(currentSceneIndex);
        yield return null;

        GameManager.Inst.controls.Enable();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class SceneInitializer : MonoBehaviour
{
    public bool useCustomCamera = false;
    public bool usePlayer = true;
    public UnityEvent onSceneLoaded;

    public StoryGraph storyGraph;

    private SceneData sceneData;

    private void Awake()
    {
        EventManager.onSceneLoaded += OnSceneLoaded;
    }

    public void ManualAwake()
    {
        if (!SaveData.current.sceneProgressionData.ContainsKey(SceneLoader.Inst.currentSceneIndex))
        {
            SaveData.current.sceneProgressionData.Add(SceneLoader.Inst.currentSceneIndex, new SceneData());
        }

        sceneData = SaveData.current.sceneProgressionData[SceneLoader.Inst.currentSceneIndex];

        if (storyGraph != null)
        {
            storyGraph.Initialize(sceneData.progress);
        }

        sceneData.sceneVisited++;
        if (storyGraph != null)
        {
            if (sceneData.sceneVisited == 1 && sceneData.progress == 0)
            {
                sceneData.progress++;
                storyGraph.Progress(sceneData.progress);
            }
        }

        SaveData.current.sceneProgressionData[SceneLoader.Inst.currentSceneIndex] = sceneData;
        SerializationManager.Save(SaveData.current);
    }

    private void OnDestroy()
    {
        EventManager.onSceneLoaded -= OnSceneLoaded;        
    }

    private void OnSceneLoaded(int _index)
    {
        GameManager.Inst.SetPlayerCamera(!useCustomCamera);
        GameManager.Inst.SetPlayerControl(usePlayer);
        onSceneLoaded?.Invoke();
    }
}

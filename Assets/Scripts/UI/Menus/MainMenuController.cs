using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    private TwitchClient client;
    public GameObject continueButton;

    private void Awake()
    {
        client = FindObjectOfType<TwitchClient>();
        continueButton.SetActive(SerializationManager.HasSave());
    }

    public void OnNew()
    {
        if (!client.isConnected)
        {
            UIManager.Inst.warningMessage.ShowMessage("Please connect to twitch. Go to options to connect to twitch", 2.0f);
        }
        else
        {
            SerializationManager.DeleteSave();
            transform.GetChild(0).gameObject.SetActive(false);
            SceneLoader.Inst.LoadScene(ESceneIndices.PlayerCustom);
        }
    }

    public void OnContinue()
    {
        if (!client.isConnected)
        {
            UIManager.Inst.warningMessage.ShowMessage("Please connect to twitch. Go to options to connect to twitch", 2.0f);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
            SceneLoader.Inst.LoadScene(ESceneIndices.Home);
        }
    }

    public void OnExit()
    {
        SerializationManager.Save(SaveData.current);
        Application.Quit();
    }
}

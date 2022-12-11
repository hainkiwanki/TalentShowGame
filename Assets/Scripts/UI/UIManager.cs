using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public InteractPopUp interactMessage => m_interactPopUp;
    public WarningPopUp warningMessage => m_warningPopUp;
    public TextBox textBox => m_textBoxPopUp;
    public CompletionWindow completionMessage => m_completionWindow;
    public NotificationManager notifications => m_notificationManager;
    public QuestionBox questionBox => m_questionBox;

    public Dictionary<EUiWindows, UIElement> uiWindows => m_uiElementWindows;


    [SerializeField]
    private InteractPopUp m_interactPopUp;
    [SerializeField]
    private WarningPopUp m_warningPopUp;
    [SerializeField]
    private TextBox m_textBoxPopUp;
    [SerializeField]
    private CompletionWindow m_completionWindow;
    [SerializeField]
    private NotificationManager m_notificationManager;
    [SerializeField]
    private QuestionBox m_questionBox;
    [SerializeField]
    private Dictionary<EUiWindows, UIElement> m_uiElementWindows = new Dictionary<EUiWindows, UIElement>();

    public void HideAll()
    {
        m_interactPopUp.Hide();
        m_warningPopUp.Hide();
        m_textBoxPopUp.Hide();
        m_questionBox.Hide();
        m_completionWindow.Hide();        // HACK: Might not work 100%

        foreach(var element in m_uiElementWindows)
        {
            element.Value.Hide();
        }
    }
}

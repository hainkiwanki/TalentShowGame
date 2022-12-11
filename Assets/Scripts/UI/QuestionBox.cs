using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class AnswerButton
{
    public string text;
    public UnityAction onClick;

    public AnswerButton(string _text, UnityAction _onClickCallback)
    {
        text = _text;
        onClick = _onClickCallback;
    }
}

public class QuestionBox : UIElement
{
    [Header("QuestionBox Variables")]
    public RectTransform buttonContainer;

    [SerializeField]
    private TextMeshProUGUI m_questionText;

    private List<Button> m_buttonList;
    private List<TextMeshProUGUI> m_buttonTextList;

    protected override void Awake()
    {
        base.Awake();

        m_buttonList = new List<Button>();
        m_buttonTextList = new List<TextMeshProUGUI>();

        foreach(RectTransform rt in buttonContainer)
        {
            Button bt = rt.GetComponent<Button>();
            m_buttonList.Add(bt);
            TextMeshProUGUI btText = bt.GetComponentInChildren<TextMeshProUGUI>();
            m_buttonTextList.Add(btText);
            bt.gameObject.SetActive(false);
        }
    }

    public void SetQuestion(string _question)
    {
        m_questionText.text = _question;
        foreach(var button in m_buttonList)
        {
            button.gameObject.SetActive(false);
        }
    }

    public void AddButton(string _buttonText, UnityAction _onClickCallback)
    {
        for(int i = 0; i < m_buttonList.Count; i++)
        {
            if(!m_buttonList[i].gameObject.activeSelf)
            {
                m_buttonList[i].gameObject.SetActive(true);
                m_buttonTextList[i].text = _buttonText;
                m_buttonList[i].onClick.RemoveAllListeners();
                m_buttonList[i].onClick.AddListener(_onClickCallback);
                break;
            }
        }
    }

    public void AddButtons(string _question, params AnswerButton[] _buttons)
    {
        SetQuestion(_question);
        AddButtons(_buttons);
    }

    public void AddButtons(params AnswerButton[] _buttons)
    {
        foreach(var answerButton in _buttons)
        {
            AddButton(answerButton.text, answerButton.onClick);
        }
    }
}

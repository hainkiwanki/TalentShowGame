using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class QuestionNode : BaseNode
{
    [TextArea]
    public string question;
    [Output(dynamicPortList = true)]
    public string[] answers;
    private int maxAnswers = 6;

    protected override void OnInit()
    {
        type = ENodeType.Question;
    }

    protected override void OnEnter()
    {
        UIManager.Inst.questionBox.SetQuestion(question);
        for(int i = 0; i < maxAnswers; i++)
        {
            if (i >= answers.Length)
                break;

            int index = i;
            string portStr = "answers " + index;
            BaseNode node = GetOutputPort(portStr).Connection.node as BaseNode;
            UIManager.Inst.questionBox.AddButton(answers[index], () => 
            { 
                if(node != null)
                {
                    next = node;
                }
                isdone = true;
            });
        }
        UIManager.Inst.questionBox.Show();
    }

    protected override void OnExecute()
    {
    }

    protected override void OnExit()
    {
        UIManager.Inst.questionBox.Hide();        
    }
}

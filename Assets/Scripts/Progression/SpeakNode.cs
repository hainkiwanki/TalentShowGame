using FMODUnity;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class SpeakNode : BaseNode
{
    public string speaker = "";
    [OnValueChanged("OnNodeTypeChanged")]
    public ESpeakNodeType nodeType = ESpeakNodeType.AudioAndText;
    public float delayBefore = 0.2f;
    [VerticalGroup("Text to say"), HideLabel, TextArea]
    [HideIf("nodeType", ESpeakNodeType.Audio)]
    public string text;
    [VerticalGroup("Text to say")]
    public float delayAfter = 0.0f;
    [ShowIf("nodeType", ESpeakNodeType.Text)]
    public float time;

    public bool waitForContinue = false;

    private void OnNodeTypeChanged()
    {
        showSounds = !(nodeType == ESpeakNodeType.Text);
    }

    protected override void OnInit()
    {
        type = ENodeType.Speak;
        OnNodeTypeChanged();
    }

    protected override void OnEnter()
    {
        GameManager.Inst.StartCoroutine(CO_Speak());
    }

    protected override void OnExit() // Called when interrupted or normally exited
    {
        if (next != null && next is SpeakNode && next.progress == progress)
            return;
        UIManager.Inst.textBox.Hide();
    }

    private IEnumerator CO_Speak()
    {
        GameManager.Inst.controls.Disable();
        GameManager.Inst.ChangeMusicVolume(0.2f);

        if (delayBefore > 0f)
            yield return new WaitForSeconds(delayBefore);
        GameManager.Inst.controls.Disable();

        UIManager.Inst.textBox.ShowText(speaker, text, waitForContinue);
        if (nodeType != ESpeakNodeType.Text)
        {
            if(soundClip == null)
            {
                Debug.LogError("Please attach sound effect");
            }
            soundClip.Play();
            yield return new WaitForSeconds(soundClip.duration);
        }
        else
        {
            yield return new WaitForSeconds(time);
        }
        GameManager.Inst.controls.Disable();

        if (delayAfter > 0f)
            yield return new WaitForSeconds(delayAfter);
        GameManager.Inst.controls.Disable();
        if (!waitForContinue)
        {
            isdone = true;
        }
        GameManager.Inst.ChangeMusicVolume(1.0f);
        GameManager.Inst.controls.Enable();
    }

    protected override void OnExecute()
    {
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;

public class ViewerVoterManager : MonoBehaviour
{
    [SerializeField]
    private UIElement m_uielement;

    [SerializeField]
    private List<ViewerVoteElement> m_viewerVoteElements = new List<ViewerVoteElement>();
    private TwitchClient m_twitchClient;
    [SerializeField]
    private List<Sprite> m_sprites = new List<Sprite>();
    [SerializeField]
    private TextMeshProUGUI m_effectDescription;

    public bool isVoting = false;

    public void Initialize(TwitchClient _client)
    {
        m_twitchClient = _client;
    }

    [Button]
    public void StartVoting()
    {
        if (m_twitchClient.isVotingInGame) return;
        m_twitchClient.isVotingInGame = true;

        m_effectDescription.text = "";
        List<int> indexList = new List<int>() { 0, 1, 2 };
        for (int i = 0; i < m_viewerVoteElements.Count; i++)
        {
            int index = indexList.RemoveRandom();
            m_viewerVoteElements[i].Initialize(i+1, m_sprites[index], () => 
            {
                DoVoteEffect(index);
            });
        }
        m_uielement.Show();
        isVoting = true;
    }

    public void ForceStop()
    {
        m_twitchClient.isVotingInGame = false;
        m_uielement.Hide();
        isVoting = false;
    }

    public void CountVote(string _v)
    {
        foreach (var element in m_viewerVoteElements)
            element.SendVote(_v);
    }

    private void DoVoteEffect(int _i)
    {
        m_twitchClient.isVotingInGame = false;
        switch (_i)
        {
            case 0:
                m_effectDescription.text = "Chat picked EZ and caused some effect.";
                PostProcessingEffect(Random.Range(0, 10));
                break;
            case 1:
                m_effectDescription.text = "Chat picked Sadge and nothing happened. PogO chat. PogO.";
                break;
            case 2:
                m_effectDescription.text = "Chat picked Poggies and caused a simp to spawn.";
                LevelManager.Inst.SpawnRandomSimp();
                break;
            default:
                break;
        }

        GameManager.Inst.ExecuteFunctionWithDelay(2.5f, () =>
        {
            m_uielement.Hide();
            m_effectDescription.text = "";
            isVoting = false;
        });

        GameManager.Inst.ExecuteFunctionWithDelay(4.0f, () => 
        {
            if(LevelManager.Inst.CanStartNewVote())
                StartVoting();
        });
    }

    public void PostProcessingEffect(int _effect)
    {
        if(_effect == 0)
        {
            PostProcessingManager.Inst.SetPosterize(0);
        }
        else if(_effect == 1)
        {
            PostProcessingManager.Inst.SetOverlay(0.05f);
        }
        else if (_effect == 2)
        {
            PostProcessingManager.Inst.SetHueShift3D(0.2f);
        }
        else if (_effect == 3)
        {
            PostProcessingManager.Inst.SetTitlShift(1.0f);
        }
        else if (_effect == 4)
        {
            PostProcessingManager.Inst.SetDoubleVision(10.0f, 7.0f);
        }
        else if (_effect == 5)
        {
            PostProcessingManager.Inst.SetChromaticAberration(0.5f);
        }
        else if (_effect == 6)
        {
            PostProcessingManager.Inst.SetFilmGrain(0.9f);
        }
        else if (_effect == 7)
        {
            PostProcessingManager.Inst.SetPaniniProjection(1.0f);
        }
        else if (_effect == 8)
        {
            PostProcessingManager.Inst.SetLensDistortion(0.6f);
        }
        else if (_effect == 9)
        {
            PostProcessingManager.Inst.SetVignette(1.0f);
        }
        else if(_effect == 10)
        {
            PostProcessingManager.Inst.SetPixelize(0.04f);
        }
        else if(_effect == 11)
        {
            PostProcessingManager.Inst.SetRipples(5.0f);
        }
        else if(_effect == 12)
        {
            PostProcessingManager.Inst.SetRefraction(0.2f);
        }
    }
}

using System.Collections.Generic;
using TwitchLib.Client.Models;
using TwitchLib.Unity;
using UnityEngine;

public class TwitchClient : MonoBehaviour
{
    public Client client;

    private string m_channelName, m_oauthCode;
    public bool canJoin = false;
    public bool isPlaying = false;
    public bool isConnected = false;
    public bool isVotingInGame = false;

    private void Awake()
    {
        EventManager.onSimpBeaten += OnSimpBeaten;        
    }

    private void Update()
    {
        if(client != null && isConnected != client.IsConnected)
        {
            isConnected = client.IsConnected;
            EventManager.onTwitchConnection?.Invoke(isConnected);
        }
    }

    private void OnSimpBeaten(Simp _s)
    {
        if(GAMESTATS.shouldTimeout && !_s.isMod)
        {
            string msg = "/timeout " + _s.username + " " + Mathf.CeilToInt(GAMESTATS.timeoutTime);
            client.SendMessage(m_channelName, msg);
        }
    }

    public void ManualStart(string _channelName, string _oauthCode)
    {
        m_channelName = _channelName;
        m_oauthCode = _oauthCode;

        ConnectionCredentials credentials = new ConnectionCredentials(m_channelName, m_oauthCode);
        client = new Client();
        client.Initialize(credentials, m_channelName);

        client.OnMessageReceived += Client_OnMessageReceived;

        client.Connect();
    }

    public void ManualDisable()
    {
        PlayerPrefs.DeleteAll();
        client.Disconnect();
    }

    private void Client_OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
    {
        string message = e.ChatMessage.Message;
        string[] msgSplit = message.Split(' ');
        string user = e.ChatMessage.DisplayName;
        if (user == "hainkiwanki")
        {
            UniqueUserPowers(msgSplit);
        }
        if (msgSplit.Length > 0)
        {
            if(msgSplit[0] == "!vote" && isVotingInGame)
            {
                LevelManager.Inst.VoteForInGameEffect(msgSplit[1]);
            }
            if (msgSplit[0] == "!vote" && SaveData.current.chatCanVoteUpgrade)
            {
                LevelManager.Inst.VoteForUpgrade(user, msgSplit[1]);
            }
            else if (msgSplit[0] == "!coomer")
            {
                if (canJoin)
                    JoinMessageParser(user, msgSplit, e.ChatMessage.IsModerator);
                else if (isPlaying)
                    IngameCommandParser(user, msgSplit, e.ChatMessage.IsModerator);
            }
        }
    }

    private void JoinMessageParser(string _user, string[] _message, bool _isMod = false)
    {
        SimpSpawnData simp = new SimpSpawnData();
        simp.userName = _user;
        simp.isMod = _isMod;
        simp.powers = new HashSet<ETwitchSettings>();

        if (_message.Length > 1)
        {
            string cmd1 = _message[1];
            if (WantsClass(cmd1, "0", "attacker", "damage", "dmg") && GAMESTATS.simpCanBeAttacker)
            {
                simp.powers.Add(ETwitchSettings.SIMP_ATTACK);
            }
            else if (WantsClass(cmd1, "1", "defense", "tank") && GAMESTATS.simpCanBeDefender)
            {
                simp.powers.Add(ETwitchSettings.SIMP_TANK);
            }
            else if (WantsClass(cmd1, "2", "transparant", "sneak") && GAMESTATS.simpCanBeTransparent)
            {
                simp.powers.Add(ETwitchSettings.SIMP_TRANSPARENT);
            }
            else if (WantsClass(cmd1, "3", "huge", "chad") && GAMESTATS.simpCanBeChad)
            {
                simp.powers.Add(ETwitchSettings.SIMP_HUGE);
            }

            if (_isMod)
            {
                if (WantsClass(cmd1, "4", "friendly", "anti") && GAMESTATS.modFriendlyEnabled)
                {
                    simp.powers.Add(ETwitchSettings.MOD_FRIENDLY);
                }
                else if (WantsClass(cmd1, "5", "boss") && GAMESTATS.modCanBeBoss)
                {
                    simp.powers.Add(ETwitchSettings.MOD_BOSS);
                }
            }
        }

        LevelManager.Inst.JoinQueue(simp);
    }

    private void IngameCommandParser(string _user, string[] _message, bool _isMod = false)
    {
        if (_message.Length >= 2)
        {
            if (_isMod)
            {
                if (GAMESTATS.modCanDespawn && _message[1] == "despawn")
                {
                    LevelManager.Inst.CommandFromSimps(_user, ETwitchSettings.MOD_DESPAWN);
                }
                else if (GAMESTATS.modCanSpawnMinis && _message[1] == "minime")
                {
                    LevelManager.Inst.CommandFromSimps(_user, ETwitchSettings.MOD_MINIVERSION);
                }
                else if (GAMESTATS.modCanShield && _message[1] == "shield")
                { 
                    LevelManager.Inst.CommandFromSimps(_user, ETwitchSettings.MOD_SHIELD);
                }
                else if(GAMESTATS.modCanShoot && _message[1] == "shoot")
                {
                    LevelManager.Inst.CommandFromSimps(_user, ETwitchSettings.MOD_SHOOT);
                }
                else if(_message[1] == "special")
                {
                    string amount = "1";
                    if (_message.Length > 2)
                        amount = _message[2];
                    LevelManager.Inst.CommandFromSimps(_user, ETwitchSettings.MOD_SPECIAL, amount);
                }
            }
        }
    }

    private bool WantsClass(string _input, params string[] _keywords)
    {
        foreach(string _keyword in _keywords)
        {
            if(_input == _keyword)
            {
                return true;
            }
        }
        return false;
    }

    private void UniqueUserPowers(string[] _msg)
    {
        if (_msg[0] == "!say") // !say message
        {
            if (_msg.Length <= 1)
                return;

            string msg = "";
            for (int i = 1; i < _msg.Length; i++)
            {
                msg += _msg[i] + " ";
            }
            Hainkiwanki.Inst.Say(msg, Color.white);
        }
        else if (SceneLoader.Inst.currentSceneIndex > 3)
        {
            if (_msg[0] == "!special")// !special name amount
            {
                string amount = "1";
                if (_msg.Length > 2)
                    amount = _msg[2];
                LevelManager.Inst.CommandFromSimps(_msg[1], ETwitchSettings.MOD_SPECIAL, amount, true);
            }
            else if(_msg[0] == "!rain")// !rain amount interval
            {
                int amount = 10;
                float interval = 0.2f;
                if (_msg.Length > 1)
                {
                    System.Int32.TryParse(_msg[1], out amount);
                }
                if (_msg.Length > 2)
                {
                    float.TryParse(_msg[2], out interval);
                }
                LevelManager.Inst.SimpRain(amount, interval);
            }
            else if(_msg[0] == "!spawn") // !spawn name ismod
            {
                string name = "Pepe";
                if (_msg.Length > 1)
                    name = _msg[1];
                bool isMod = false;
                if (_msg.Length > 2)
                    isMod = _msg[2] == "1";

                if (name.ToLower() == "random")
                {
                    LevelManager.Inst.SpawnRandomSimp();
                }
                else
                {
                    SimpSpawnData spawnData = new SimpSpawnData() { isMod = isMod, userName = name, powers = new HashSet<ETwitchSettings>() };
                    LevelManager.Inst.SpawnCustomSimp(spawnData);
                }
            }
            else if(_msg[0] == "!swarm") // !swarm amt
            {
                int amount = 5;
                if (_msg.Length > 1)
                    System.Int32.TryParse(_msg[1], out amount);

                for(int i = 0; i < amount; i++)
                {
                    LevelManager.Inst.SpawnRandomSimp();
                }
            }
            else if(_msg[0] == "!effect") // !effect number
            {
                int effect = 0;
                if (_msg.Length > 1)
                    System.Int32.TryParse(_msg[1], out effect);

                LevelManager.Inst.ScreenEffect(effect);
            }
        }
    }
}
using GameSparks.Api.Messages;
using GameSparks.Api.Responses;
using GameSparks.Core;
using GameSparks.RT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSparksManager : MonoBehaviour
{
    private static GameSparksManager instance = null;

    public static GameSparksManager Instance()
    {
        if (instance != null)
            return instance;

        Debug.LogError("GSM| GameSparksManager not initialized...");
        return null;
    }

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public delegate void AuthCallback(AuthenticationResponse _authResp);
    public delegate void MatchingErrorCallback(MatchmakingResponse _resp);

    public void AuthenticateUser(string displayName, AuthCallback authCallback)
    {
        if (displayName == "")
        {
            Debug.LogError("Fields cannot be empy!");
            return;
        }

        Debug.Log("Authenticating device...");
        new GameSparks.Api.Requests.DeviceAuthenticationRequest()
            .SetDisplayName(displayName)
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    authCallback(response);
                    Debug.Log("Device authentiated...");
                }

                else
                    Debug.Log("Error authenticating device...");
            });
    }

    public void FindPlayers(MatchingErrorCallback mec)
    {
        Debug.Log("GSM | Attempting matchmaking...");
        new GameSparks.Api.Requests.MatchmakingRequest()
            .SetMatchShortCode("SPACE_BATTLE")
            .SetSkill(0)
            .Send((response) =>
            {
                if (response.HasErrors)
                {
                    Debug.LogError("GSM | Matchmaking error \n" + response.Errors.JSON.ToString());
                    mec(response);
                }
            });
    }

    private GameSparksRTUnity gameSparksRTUnity;
    public GameSparksRTUnity GetRTSession() { return this.gameSparksRTUnity; }

    private RTSessionInfo sessionInfo;
    public RTSessionInfo GetSessionInf() { return this.sessionInfo; }

    public void StartNewRTSession(RTSessionInfo _info)
    {
        Debug.Log("GSM | Creating new RT Session instance...");
        sessionInfo = _info;
        gameSparksRTUnity = this.gameObject.AddComponent<GameSparksRTUnity>();

        GSRequestData mockedResponse = new GSRequestData()
            .AddNumber("port", (double)_info.GetPortID())
            .AddString("host", _info.GetHostURL())
            .AddString("accessToken", _info.GetAccessToken());

        FindMatchResponse response = new FindMatchResponse(mockedResponse);

        gameSparksRTUnity.Configure(response,
            (peerId) => { OnPlayerConnectedToGame(peerId); },
            (peerId) => { OnPlayerDisconnected(peerId); },
            (ready) => { OnRTReady(ready); },
            (packet) => { OnPacketReceived(packet); });
        gameSparksRTUnity.Connect(); // finally connect to the game
    }

    private void OnPlayerConnectedToGame(int _peerId)
    {
        Debug.Log("GSM | Player connected, " + _peerId);
    }

    private void OnPlayerDisconnected(int _peerId)
    {
        Debug.Log("GSM | Player disconnected, " + _peerId);
    }

    private void OnRTReady(bool _isReady)
    {
        if(_isReady)
        {
            Debug.Log("GSM | RT Session connected....");
            SceneManager.LoadScene("GameScene");
        }
    }

    private void OnPacketReceived(RTPacket _packet)
    {

    }
}

public class RTSessionInfo
{
    private string hostURL;
    public string GetHostURL() { return this.hostURL;  }
    private string accessToken;
    public string GetAccessToken() { return this.accessToken; }
    private int portID;
    public int GetPortID() { return this.portID; }
    private string matchID;
    public string GetMatchID() { return this.matchID; }

    private List<RTPlayer> playerList = new List<RTPlayer>();
    public List<RTPlayer> GetPlayerList() { return this.playerList; }
    

    public RTSessionInfo(MatchFoundMessage _message)
    {
        portID = (int)_message.Port;
        hostURL = _message.Host;
        accessToken = _message.AccessToken;
        matchID = _message.MatchId;

        foreach(MatchFoundMessage._Participant p in _message.Participants)
        {
            playerList.Add(new RTPlayer(p.DisplayName, p.Id, (int)p.PeerId));
        }

    }

}

public class RTPlayer
{
    public string displayName;
    public string id;
    public int peerId;
    public bool isOnline;

    public RTPlayer(string displayName, string id, int peerId)
    {
        this.displayName = displayName;
        this.id = id;
        this.peerId = peerId;
    }
}


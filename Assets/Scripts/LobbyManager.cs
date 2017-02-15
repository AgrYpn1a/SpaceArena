using GameSparks.Api.Messages;
using GameSparks.Api.Responses;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField]
    private Text nickName, status;
    [SerializeField]
    private Button btnLogin, btnFindMatch, btnStartGame;

    #region Panels
    [SerializeField]
    private GameObject loginPanel, matchMakingPanel;
    #endregion

    private RTSessionInfo tempRTSessionInfo;

    void Awake()
    {
        btnLogin.onClick.AddListener(Login);
        btnFindMatch.onClick.AddListener(FindMatch);

        GameSparks.Api.Messages.MatchNotFoundMessage.Listener = (message) =>
        {
            status.text = "No game found...";
        };

        GameSparks.Api.Messages.MatchFoundMessage.Listener += OnMatchFound;

        // start game
        btnStartGame.onClick.AddListener(() =>
        {
            GameSparksManager.Instance().StartNewRTSession(tempRTSessionInfo);
        });

    }

    private void Login()
    {
        GameSparksManager.Instance().AuthenticateUser(nickName.text, OnAuthentication);
    }

    private void FindMatch()
    {
        GameSparksManager.Instance().FindPlayers((response) => {
            status.text = response.Errors.JSON.ToString();
        });
        status.text = "Searching for games...";
    }

    private void OnAuthentication(AuthenticationResponse _authResp)
    {
        // set login panel inactive, and match making panel active
        loginPanel.SetActive(false);
        matchMakingPanel.SetActive(true);
    }

    private void OnMatchFound(MatchFoundMessage _message)
    {
        Debug.LogAssertion("Match found!");

        StringBuilder sBuilder = new StringBuilder();
        sBuilder.AppendLine("Match found");
        sBuilder.AppendLine("Host URL: " + _message.Host);
        sBuilder.AppendLine("Port: " + _message.Port);
        sBuilder.AppendLine("Opponents: " + _message.Participants);
        sBuilder.AppendLine("_______________");
        sBuilder.AppendLine();

        foreach(MatchFoundMessage._Participant player in _message.Participants)
        {
            sBuilder.AppendLine("Player: " + player.PeerId + "User name: " + player.DisplayName);
        }
        status.text = sBuilder.ToString();

        tempRTSessionInfo = new RTSessionInfo(_message);

        btnFindMatch.gameObject.SetActive(false);
        btnStartGame.gameObject.SetActive(true);
    }
}

using GameSparks.Api.Responses;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}


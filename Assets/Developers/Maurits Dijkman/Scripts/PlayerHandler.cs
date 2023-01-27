using Mirror;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHandler : NetworkBehaviour
{
    public static PlayerHandler singleton;

    [Header("PlayerValues")]
    public string playerName = "Player";
    private ClientInfo clientInfo;
    public bool isReady = false;

    private AudioListener audioListener = null;
    private UI_Game responseTimeUI = null;

    private void Awake()
    {
        audioListener = GetComponentInChildren<AudioListener>();
    }

    private void Start()
    {
        Debug.Log($"IsLocalPlayer: {isLocalPlayer}");

        if (isLocalPlayer)
        {
            singleton = this;

            if (PlayerPrefs.HasKey("PlayerName"))
            {
                playerName = PlayerPrefs.GetString("PlayerName");
                Debug.Log($"PlayerName was saved! || Name of player: {playerName}");
            }

            CMD_RequestClientInfo(playerName);
        }
        else
            audioListener.enabled = false;

        Debug.Log($"netId (unit): {netId} || netId (int): {(int)netId}");
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            if (SceneManager.GetActiveScene().name == "Lobby")
                HandleReadyState();
            else if (SceneManager.GetActiveScene().name == "Game")
                HandleGameUI();
        }
    }

    [Command]
    private void CMD_RequestClientInfo(string pPlayerName)
    {
        StartCoroutine(RequestClientInfo(pPlayerName));
    }

    private IEnumerator RequestClientInfo(string pPlayerName)
    {
        yield return new WaitForSeconds(0.0f);

        clientInfo = new ClientInfo();
        clientInfo.clientID = connectionToClient.connectionId;
        clientInfo.clientName = pPlayerName;

        if (clientInfo.clientID < 0)
        {
            Debug.Log($"ClientInfo is bellow 0! || clientId: {clientInfo.clientID} || Retrying...");
            clientInfo.clientID = connectionToClient.connectionId;
        }

        if (LobbyHandler.singleton != null)
            LobbyHandler.singleton.ConnectToServer(clientInfo, connectionToClient);

        Debug.Log($"Server\n ConnectionID: {clientInfo.clientID} || PlayerName: {clientInfo.clientName}");
    }

    private void OnDestroy()
    {
        if (LobbyHandler.singleton != null)
            LobbyHandler.singleton.DisconnectFromServer(netId);
    }

    public void StopClient()
    {
        if (isLocalPlayer)
        {
            if (NetworkManager.singleton != null)
            {
                if (!isServer)
                    NetworkManager.singleton.StopClient();
                else
                    NetworkManager.singleton.StopHost();

                //Destroy(NetworkManager.singleton.gameObject);
            }
        }
    }

    private void HandleReadyState()
    {
        if (Input.GetKeyDown(KeyCode.R))
            ChangeReadyState();
    }

    public void ChangeReadyState()
    {
        CMD_ChangeReadyState();
    }

    [Command]
    private void CMD_ChangeReadyState()
    {
        if (LobbyHandler.singleton != null)
        {
            if (isReady)
            {
                LobbyHandler.singleton.GetClientInfoList()[connectionToClient.identity.netId].isReady = false;
                isReady = false;
                Debug.Log($"Client with ID '{connectionToClient}' is not ready!");
            }
            else
            {
                LobbyHandler.singleton.GetClientInfoList()[connectionToClient.identity.netId].isReady = true;
                isReady = true;
                Debug.Log($"Client with ID '{connectionToClient}' is ready!");
            }
        }
    }

    private void HandleGameUI()
    {
        if (responseTimeUI == null)
            responseTimeUI = FindObjectOfType<UI_Game>();

        string msText = $"ms: {Math.Round(NetworkTime.rtt * 1000)}";

        if (responseTimeUI != null)
        {
            responseTimeUI.SetText(msText);
            Debug.Log($"Time is set! || Text: {msText}");
        }
    }
}

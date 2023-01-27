using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class ClientInfo
{
    [Header("Connection Info")]
    [SyncVar] public int clientID;
    [SyncVar] public string clientName = "StandardName";

    [Header("Stats")]
    [SyncVar] public bool isReady = false;
    [SyncVar] public uint killAmount = 0;
    [SyncVar] public uint deathAmount = 0;
}

public class LobbyHandler : NetworkBehaviour
{
    public static LobbyHandler singleton;
    public Dictionary<uint, ClientInfo> clientConnections = new Dictionary<uint, ClientInfo>();
    //public Dictionary<LobbyPlayerSetter, ClientInfo> playersUI = new Dictionary<LobbyPlayerSetter, ClientInfo>();

    [Header("Settings")]
    [SerializeField] private int minimumPlayerAmount = 2;
    [SerializeField] private float timeBeforeStart = 10;
    private float timer = 0;

    [Header("UI")]
    [SerializeField] private Button startGameButton = null;
    private LobbyPlayerList playerUIList = null;
    private List<LobbyPlayerSetter> playerUI = new List<LobbyPlayerSetter>();

    [Header("Debug")]
    [SerializeField] private bool printDebug = false;

    private bool timerStarted = false;
    private bool hasLoadedScene;

    private int selectedMap;

    private void Start()
    {
        singleton = this;
        timerStarted = false;

        playerUIList = FindObjectOfType<LobbyPlayerList>();
        playerUI = playerUIList.GetPlayerUIList();
    }

    public void ConnectToServer(ClientInfo pClientConnection, NetworkConnectionToClient connectionToClient)
    {
        if (pClientConnection == null)
            Debug.Log("NetworkConnectionToClient (pClientConnection) is null!");

        if (clientConnections == null)
            Debug.Log("Dictionary (clientConnections) is null!");

        if (!clientConnections.ContainsKey(connectionToClient.identity.netId))
        {
            clientConnections.Add(connectionToClient.identity.netId, pClientConnection);
            UpdatePlayerUI(pClientConnection);
        }
    }

    public void DisconnectFromServer(uint netID)
    {
        Debug.Log($"Player with ID '{netID}' left!");

        if (clientConnections.ContainsKey(netID))
        {
            RemovePlayerFromUI(netID);
            clientConnections.Remove(netID);
            Debug.Log($"Player with ID '{netID}' is removed from list!");
        }
    }

    private void Update()
    {
        if (CheckReady())
            Debug.Log($"Everyone is ready!");
        else
            Debug.Log($"Not everyone is ready!");

        if (CheckReady() && clientConnections.Count() >= minimumPlayerAmount)
            Debug.Log("Everyone is ready and the game has minimum players!");

        // Print info of all players
        if (Input.GetKeyDown(KeyCode.P))
        {
            foreach (ClientInfo client in clientConnections.Values)
            {
                Debug.Log($"ClientID: {client.clientID} || ClientName: {client.clientName}");
            }
        }

        if (clientConnections.Count() >= minimumPlayerAmount && CheckReady())
        {
            startGameButton.interactable = true;
        }
        else
            startGameButton.interactable = false;
    }

    private bool CheckReady()
    {
        ChangeReadyUI();

        for (int i = 0; i < clientConnections.Count; ++i)
        {
            List<ClientInfo> values = clientConnections.Values.ToList();

            if (values[i].isReady == false)
                return false;
        }

        return true;
    }

    [ServerCallback]
    public void LoadGameLevel()
    {
        if (!hasLoadedScene)
        {
            StartCoroutine(GoToSceneAfterHalfASecond());
            hasLoadedScene = true;
        }
    }

    private IEnumerator GoToSceneAfterHalfASecond()
    {
        yield return new WaitForSeconds(0.5f);
        NetworkManager.singleton.ServerChangeScene("Game");
    }

    [ServerCallback]
    private void Timer()
    {
        if (timer > 0)
            timer -= Time.deltaTime;
        else
        {
            timer = 0;

            UpdateServerTime(timer);
            UpdatePlayerTime(timer);

            LoadGameLevel();
        }

        UpdateServerTime(timer);
        UpdatePlayerTime(timer);
    }

    private void UpdateServerTime(float timeToDisplay)
    {
        if (timer < 0)
            timeToDisplay = 0;
        else if (timeToDisplay > 0)
            timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        Debug.Log($"Timer: {string.Format("{0:00}:{1:00}", minutes, seconds)}");
    }

    [ServerCallback]
    private void UpdatePlayerTime(float timeToDisplay)
    {
        if (timer < 0)
            timeToDisplay = 0;
        else if (timeToDisplay > 0)
            timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
    }

    public Dictionary<uint, ClientInfo> GetClientInfoList()
    {
        return clientConnections;
    }

    private void UpdatePlayerUI(ClientInfo clientInfo)
    {
        for (int i = 0; i < playerUI.Count(); i++)
        {
            if (!playerUI[i].isAssigned)
            {
                playerUI[i].SetPlayerName(clientInfo.clientName);
                playerUI[i].SetPlayerReadyState(clientInfo.isReady);
                break;
            }
        }

        CLIENT_UpdateUI();
    }

    private void RemovePlayerFromUI(uint pNetId)
    {
        ClientInfo value;
        bool hasValue = clientConnections.TryGetValue(pNetId, out value);

        if (hasValue)
        {
            Debug.Log($"Client with ID '{pNetId}' and name '{value.clientName}' left!");

            foreach (LobbyPlayerSetter playerLobby in playerUI)
            {
                if (playerLobby.GetPlayerName() == value.clientName)
                    playerLobby.ResetState();
            }
        }
        else
        {
            Debug.Log($"Key '{pNetId}' not present || ID's in list:");

            foreach (uint netID in clientConnections.Keys)
                Debug.Log($"{netID}");
        }

        CLIENT_UpdateUI();
    }

    private void ChangeReadyUI()
    {
        foreach (ClientInfo client in clientConnections.Values)
        {
            foreach (LobbyPlayerSetter playerLobby in playerUI)
            {
                if (playerLobby.GetPlayerName() == client.clientName)
                {
                    if (playerLobby.GetReadyState() != client.isReady)
                        playerLobby.SetPlayerReadyState(client.isReady);

                    break;
                }
            }
        }

        CLIENT_UpdateUI();
    }

    private void CLIENT_UpdateUI()
    {
        Debug.Log("Client UI updated");

        if (FindObjectOfType<ClientLobbyUI>() != null)
            FindObjectOfType<ClientLobbyUI>().SetUI();
    }
}

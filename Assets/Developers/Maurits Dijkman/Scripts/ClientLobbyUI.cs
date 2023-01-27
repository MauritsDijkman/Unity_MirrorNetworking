using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class ClientLobbyUI : NetworkBehaviour
{
    private LobbyPlayerList playerList = null;
    private List<LobbyPlayerSetter> playerUI = null;

    //[SyncVar] public List<LobbyPlayerSetter> serverInfo = new List<LobbyPlayerSetter>();
    //rivate List<LobbyPlayerSetter> serverInfo = new List<LobbyPlayerSetter>();

    private void Awake()
    {
        playerList = FindObjectOfType<LobbyPlayerList>();
        playerUI = playerList.GetPlayerUIList();
    }

    private void Update()
    {
        //SetUI(serverInfo);
    }

    public void SetUI()
    {
        Debug.Log($"RPC is called on server! \nplayerName 1: {playerUI[0].GetPlayerName()} || playerState 1: {playerUI[0].GetReadyState()}\nplayerName 2: {playerUI[1].GetPlayerName()} || playerState 2: {playerUI[1].GetReadyState()}");
        RPC_SyncUI(
            playerUI[0].GetPlayerName(), playerUI[0].GetReadyState(),
            playerUI[1].GetPlayerName(), playerUI[1].GetReadyState(),
            playerUI[2].GetPlayerName(), playerUI[2].GetReadyState(),
            playerUI[3].GetPlayerName(), playerUI[3].GetReadyState()
            );
    }

    [ClientRpc]
    private void RPC_SyncUI(
    string pName1, bool pState1,
    string pName2, bool pState2,
    string pName3, bool pState3,
    string pName4, bool pState4)
    {
        Debug.Log($"SyncUI is called!");

        if (isServer)
            return;

        Debug.Log($"RPC is called on server! \nplayerName 1: {pName1} || playerState 1: {pState1}\nplayerName 2: {pName2} || playerState 2: {pState2}");


        //if(!isLocalPlayer)    // Can be implemented later for safety (set UI for own player, then sync to server and other clients)
        playerUI[0].SetPlayerName(pName1);
        playerUI[0].SetPlayerReadyState(pState1);

        playerUI[1].SetPlayerName(pName2);
        playerUI[1].SetPlayerReadyState(pState2);

        playerUI[2].SetPlayerName(pName3);
        playerUI[2].SetPlayerReadyState(pState3);

        playerUI[3].SetPlayerName(pName4);
        playerUI[3].SetPlayerReadyState(pState4);
    }

    public void SetReady()
    {
        foreach (var player in FindObjectsOfType<PlayerHandler>())
        {
            if (player.isOwned)
            {
                player.ChangeReadyState();
                return;
            }
        }
    }

    public void LeaveLobby()
    {
        if (!isServer)
            NetworkManager.singleton.StopClient();
        else
            NetworkManager.singleton.StopHost();

        //Destroy(NetworkManager.singleton.gameObject);
    }
}

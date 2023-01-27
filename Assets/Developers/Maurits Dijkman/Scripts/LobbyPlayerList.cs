using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LobbyPlayerList : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private LobbyPlayerSetter[] playerUIList = null;
    [SerializeField] private GameObject startGameButton = null;

    private void Start()
    {
        if (!isServer)
            startGameButton.SetActive(false);
    }

    public List<LobbyPlayerSetter> GetPlayerUIList()
    {
        return playerUIList.ToList();
    }
}

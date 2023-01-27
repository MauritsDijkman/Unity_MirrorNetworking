using System;
using TMPro;
using UnityEngine;
using Mirror;

public class NetworkSearcher : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text serverName = null;
    [SerializeField] private TMP_Text serverPing = null;

    private ServerDiscoveryList networkDiscoveryUI = null;

    private void Awake()
    {
        networkDiscoveryUI = FindObjectOfType<ServerDiscoveryList>();
    }

    public void SetServerName(string pServerName)
    {
        serverName.text = pServerName;
        Debug.Log($"Server name is set to '{pServerName}'!");
    }

    public void Connect()
    {
        networkDiscoveryUI.ConnectToServer(serverName.text);
    }

    private void Update()
    {
        serverPing.text = $"{Math.Round(NetworkTime.rtt * 1000)}ms";

        if (transform.localScale != new Vector3(1.0f, 1.0f, 1.0f))
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }
}

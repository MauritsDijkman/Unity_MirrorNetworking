using Mirror.Discovery;
using UnityEngine;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
[RequireComponent(typeof(NetworkDiscovery))]
public class ServerDiscoveryList : MonoBehaviour
{
    [Header("Variables and objects")]
    private NetworkDiscovery networkDiscovery = null;

    [Header("UI")]
    [SerializeField] private TMP_Text noServersFoundText = null;
    [SerializeField] private GameObject serverButtonPrefab = null;
    [SerializeField] private GameObject buttonListContent = null;
    [SerializeField] private TMP_InputField serverName_InputField = null;

    [Header("Debug")]
    [SerializeField] private bool printDebug = false;

    private string selectedName = "";
    private string firstSceneName = "";
    private readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();

    private List<GameObject> serverButtons = new List<GameObject>();

    public void FindServers()
    {
        //foreach (GameObject button in serverButtons)
        //{
        //    Destroy(button);
        //    serverButtons.Remove(button);
        //}

        discoveredServers.Clear();          // Clear the list with servers
        networkDiscovery.StartDiscovery();  // Start the search for servers
    }

    private void Awake()
    {
        networkDiscovery = GetComponent<NetworkDiscovery>();
    }

    private void Start()
    {
        firstSceneName = SceneManager.GetActiveScene().name;
        ChangeServerName();
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name != firstSceneName)
            return;

        if (printDebug)
            Debug.Log($"Discovered server count: {discoveredServers.Count}");

        if (discoveredServers.Count <= 0)
        {
            // Turn on the text with 'No servers found' text
            if (noServersFoundText.gameObject != null)
            {
                if (!noServersFoundText.gameObject.activeSelf)
                    noServersFoundText.gameObject.SetActive(true);
            }

            if (printDebug)
                Debug.Log($"Server count is less then or 0!");
        }
        else
        {
            foreach (ServerResponse info in discoveredServers.Values)
            {
                // Turn off the text with 'No servers found' text
                if (noServersFoundText == null)
                    continue;

                if (noServersFoundText.gameObject.activeSelf)
                    noServersFoundText.gameObject.SetActive(false);

                // Destroy all the existing buttons
                foreach (Transform child in buttonListContent.transform)
                    Destroy(child.gameObject);

                // Add a button with the server IP to the UI (list)
                AddServerButton(info);
            }
        }
    }

    private void AddServerButton(ServerResponse pInfo)
    {
        // Create the buttton and set the text to the server name
        GameObject spawnedButton = Instantiate(serverButtonPrefab);
        spawnedButton.GetComponent<NetworkSearcher>().SetServerName(pInfo.serverName);
        Debug.Log($"Accessed the button script and changed name to '{pInfo.serverName}'!");

        // Set parent
        spawnedButton.transform.parent = buttonListContent.transform;

        // Reset values
        spawnedButton.transform.localPosition = new Vector3(spawnedButton.transform.position.x, spawnedButton.transform.position.y, 0);
        spawnedButton.transform.localEulerAngles = Vector3.zero;

        // Add button to the list
        serverButtons.Add(spawnedButton);
    }

    public void ConnectToServer(string pServerName)
    {
        // Check if the name is the same. If so, connect
        foreach (ServerResponse info in discoveredServers.Values)
        {
            //if (info.EndPoint.Address.ToString() == selectedIP)
            if (info.serverName == pServerName)
            {
                Connect(info);
                break;
            }
        }
    }

    private void Connect(ServerResponse info)
    {
        if (printDebug)
            Debug.Log($"IP: {selectedName} || Networkadress: {NetworkManager.singleton.networkAddress}");

        // Stop the search for servers and start the client with the chosen IP
        networkDiscovery.StopDiscovery();
        NetworkManager.singleton.StartClient(info.uri);
    }

    public void StartHost()
    {
        ChangeServerName();
        discoveredServers.Clear();              // Clear the list with servers
        NetworkManager.singleton.StartHost();   // Start the host
        networkDiscovery.AdvertiseServer();     // Advertise the server on the network
    }

    public void OnDiscoveredServer(ServerResponse info)
    {
        // Add the given server to the list with discovered servers
        discoveredServers[info.serverId] = info;

        if (printDebug)
            Debug.Log($"Server has been added to list! || Server ID: {info.serverId}");
    }

    public void ChangeServerName()
    {
        networkDiscovery.ServerName = serverName_InputField.text;
    }
}

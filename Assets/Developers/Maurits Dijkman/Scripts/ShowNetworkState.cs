using Mirror;
using TMPro;
using UnityEngine;

public class ShowNetworkState : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text networkState_Text = null;

    private void Start()
    {
        Debug.Log($"IsServer: {isServer} || IsClient: {isClient}");

        if (networkState_Text == null)
            return;

        if (!isLocalPlayer)
            networkState_Text.gameObject.SetActive(false);
        else
        {
            if (isServer)
                networkState_Text.text = $"NetworkState: Host";
            else if (isClient)
                networkState_Text.text = $"NetworkState: Client";
        }
    }
}

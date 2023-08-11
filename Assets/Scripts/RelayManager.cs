using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayManager : MonoBehaviour
{
    [SerializeField] int _maxPlayers = 4;

    private async void Start()
    {
        await SignInAnonymouslyAsync();
    }

    private async Task SignInAnonymouslyAsync()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            Debug.Log("[" + nameof(RelayManager) + "] " + nameof(SignInAnonymouslyAsync) + " | playerId: " + AuthenticationService.Instance.PlayerId);
        }
        catch (Exception ex)
        {
            Debug.LogError("[" + nameof(RelayManager) + "] " + nameof(SignInAnonymouslyAsync) + " | exception: " + ex);
        }
    }

    private async Task<Allocation> AllocateRelayServerAsync()
    {
        Allocation allocation = null;
        try
        {
            allocation = await RelayService.Instance.CreateAllocationAsync(_maxPlayers);
            Debug.Log("[" + nameof(RelayManager) + "] " + nameof(AllocateRelayServerAsync) + " | allocation: " + allocation.AllocationId);
        }
        catch (Exception ex)
        {
            Debug.LogError("[" + nameof(RelayManager) + "] " + nameof(AllocateRelayServerAsync) + " | Exception: " + ex);
        }

        return allocation;
    }

    private async Task<string> GetJoinCode(Allocation allocation)
    {
        string joinCode = "";
        try
        {
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("[" + nameof(RelayManager) + "] " + nameof(GetJoinCode) + " | " + joinCode);
        }
        catch (Exception ex)
        {
            Debug.LogError("[" + nameof(RelayManager) + "] " + nameof(GetJoinCode) + " | Exception: " + ex);
        }

        return joinCode;
    }

    public async void StartHostAsync()
    {
        var allocation = await AllocateRelayServerAsync();
        await GetJoinCode(allocation);

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
        NetworkManager.Singleton.StartHost();
    }

    private async Task<RelayServerData> JoinRelayServerAsync(string joinCode)
    {
        JoinAllocation joinAllocation = null;
        try
        {
            joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            Debug.Log("[" + nameof(RelayManager) + "] " + nameof(JoinRelayServerAsync) + " | Join code: " + joinCode);
            Debug.Log("[" + nameof(RelayManager) + "] " + nameof(JoinRelayServerAsync) + " | Allo Id: " + joinAllocation.AllocationId);
        }
        catch (Exception ex)
        {
            Debug.LogError("[" + nameof(RelayManager) + "] " + nameof(JoinRelayServerAsync) + " | Exception: " + ex);
        }

        return new RelayServerData(joinAllocation, "dtls");
    }

    public async void StartClientAsync(string joinCode)
    {
        var relayServerData = await JoinRelayServerAsync(joinCode);

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        NetworkManager.Singleton.StartClient();
    }
}

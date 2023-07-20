using Unity.Netcode;
using UnityEngine;
using TMPro;

public class NetcodeUIManager : MonoBehaviour
{
    [SerializeField] TMP_InputField joincodeInputField;

    public GameEvent<string> JoincodeEvent;

    public void StartHost()
    {
        Debug.Log("[" + nameof(NetcodeUIManager) + "] " + nameof(StartHost));
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        Debug.Log("[" + nameof(NetcodeUIManager) + "] " + nameof(StartClient));
        NetworkManager.Singleton.StartClient();
    }

    public void StartServer()
    {
        Debug.Log("[" + nameof(NetcodeUIManager) + "] " + nameof(StartServer));
        NetworkManager.Singleton.StartServer();
    }

    public void EnterJoincode()
    {
        if (string.IsNullOrEmpty(joincodeInputField.text))
            return;
        Debug.Log("[" + nameof(NetcodeUIManager) + "] " + nameof(EnterJoincode) + " | join code: " + joincodeInputField.text);
        JoincodeEvent.Raise(joincodeInputField.text);
    }
}

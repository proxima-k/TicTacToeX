using Unity.Netcode;

public class ConnectionHandler : NetworkBehaviour
{
    public static ConnectionHandler Instance { get; private set; }

    public bool StartHost() {
        if (NetworkManager.Singleton.StartHost())
            return true;
        return false;
    }

    public bool StartClient() {
        if (NetworkManager.Singleton.StartClient())
            return true;
        return false;
    }
}

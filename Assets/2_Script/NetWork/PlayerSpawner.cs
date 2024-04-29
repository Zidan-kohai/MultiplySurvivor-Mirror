using UnityEngine;
using Mirror;

public class PlayerSpawner : NetworkBehaviour
{
    public GameObject playerWithCameraPrefab;
    public GameObject playerWithoutCameraPrefab;

    public override void OnStartServer()
    {
        base.OnStartServer();
        if (isServer)
        {
            SpawnPlayerWithCamera();
        }
    }

    [Server]
    private void SpawnPlayerWithCamera()
    {
        GameObject player = Instantiate(playerWithCameraPrefab, transform.position, Quaternion.identity);

        NetworkServer.Spawn(player);
    }

    [Server]
    public void ReplacePlayerCamera(NetworkConnectionToClient conn)
    {
        foreach (NetworkIdentity identity in NetworkServer.connections[conn.connectionId].identity.gameObject.GetComponentsInChildren<NetworkIdentity>())
        {
            if (identity.gameObject.CompareTag("Player"))
            {
                Destroy(identity.gameObject.GetComponentInChildren<Camera>().gameObject);

                GameObject player = Instantiate(playerWithoutCameraPrefab, identity.gameObject.transform.position, Quaternion.identity);
                NetworkServer.ReplacePlayerForConnection(conn, player, true);
                break;
            }
        }
    }
}
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;

    void Start()
    {
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(16.8f, -17.66f, 49.15f), Quaternion.identity);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class Timer : MonoBehaviourPunCallbacks
{
    public static bool paused = false;
    public static bool disconnecting = false;

    private bool startTimer = false;
    private double timerIncrementValue = 0;
    private double startTime;
    public Text countdownText;
    public double timer = 20;
    ExitGames.Client.Photon.Hashtable CustomeValue;
    
    private void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            CustomeValue = new ExitGames.Client.Photon.Hashtable();
            startTime = PhotonNetwork.Time;
            startTimer = true;
            CustomeValue.Add("StartTime", startTime);
            PhotonNetwork.CurrentRoom.SetCustomProperties(CustomeValue);
        }
        else
        {
            startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties["StartTime"].ToString());
            startTimer = true;
        }
    }
 
    private void Update()
    {
        if (!startTimer) return;
        if (!paused) timerIncrementValue = PhotonNetwork.Time - startTime;
        countdownText.text = timerIncrementValue.ToString("0.##");
    }

    public void TogglePause()
    {
        if (disconnecting) return;
        paused = !paused;
        transform.GetChild(0).gameObject.SetActive(paused);
        Cursor.lockState = (paused) ? CursorLockMode.None : CursorLockMode.Confined;
        Cursor.visible = paused;
    }

    public void OnClickQuit()
    {
        disconnecting = true;
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        disconnecting = false;
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
    }
}

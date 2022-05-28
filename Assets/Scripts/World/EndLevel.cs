using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EndLevel : MonoBehaviour
{
    public List<GameObject> Levels;
    public int currLevel = 0;
    
    void OnTriggerEnter()
    {
        int nextLevel = currLevel + 1; 
        Levels[currLevel].SetActive(false);
        Levels[nextLevel].SetActive(true);
    }
}

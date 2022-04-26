using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController THIS;

    public int index;
    public bool isLevelDone;
    
    private void Awake()
    {
        if (THIS == null)
            THIS = this;
        else
        {
            if (THIS != this)
            {
                Destroy(this.gameObject);
            }
        }
        DontDestroyOnLoad(this);
    }
    
    [HideInInspector]public bool isMoved = false;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

    public GameObject helpMenu;
    public GameObject transtionImg;
    public GameObject rotRef;


    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);
        if (PlayerPrefs.GetInt("FIRST_TIME_OPENED", 0) == 0)
        {
            ShowHelp();
            PlayerPrefs.SetInt("FIRST_TIME_OPENED", 1);
        }        
    }

    public void StartNewGame()
    {        
        LeanTween.rotateLocal(rotRef,new Vector3(0,180f,0),1f);
    }
    public void ContinueGame()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
            SceneManager.LoadScene(1);       
    }
    public void Exit()
    {
        Application.Quit();
    }
    public void ShowHelp()
    {
        helpMenu.SetActive(true);       
        LeanTween.scale(helpMenu.transform.GetChild(0).GetComponent<RectTransform>(), Vector3.one, 0.5f).setEaseOutQuart();

    }

    public void CloseHelp()
    {
        LeanTween.scale(helpMenu.transform.GetChild(0).GetComponent<RectTransform>(), Vector3.zero, 0.5f).setEaseInBack().setOnComplete(() => { helpMenu.SetActive(false); });

    }
    public void ShowMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void StartLevel(int index)
    {
        SceneManager.LoadScene(index);
        GameController.THIS.index = index;
    }
    void StartTransition()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
            SceneManager.LoadScene(1);
        else if (SceneManager.GetActiveScene().buildIndex == 1)
            SceneManager.LoadScene(1);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveShow : MonoBehaviour
{

    public GameObject ObjectiveMenu;
    // Start is called before the first frame update
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);
        if (PlayerPrefs.GetInt("FIRST_TIME_Objective", 0) == 0)
        {
            ShowPause();
            PlayerPrefs.SetInt("FIRST_TIME_Objective", 1);
        }
    }

    public void ShowPause()
    {
        ObjectiveMenu.SetActive(true);
        LeanTween.scale(ObjectiveMenu.transform.GetChild(0).GetComponent<RectTransform>(), Vector3.one, 0.5f).setEaseOutQuart();
    }

    public void ResumeGame()
    {

        LeanTween.scale(ObjectiveMenu.transform.GetChild(0).GetComponent<RectTransform>(), Vector3.zero, 0.5f).setEaseInBack().setOnComplete(() => { ObjectiveMenu.SetActive(false); });

    }

}

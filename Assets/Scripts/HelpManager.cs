using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SubjectNerd.Utilities;

public class HelpManager : MonoBehaviour
{
    public Text InitialText;
    public TMP_Text tutorialText;
    public int i;
    int lowerThreshold = 1;
    public string[] tutorialTextData;
    public Button backButton;
    bool openHelp;
    public GameObject UI, navUI;
    private void Update()
    {
        if (openHelp)
        {
            UI.SetActive(false);
            navUI.SetActive(false);
        }
        if (openHelp)
            lowerThreshold = 2;
        else
            lowerThreshold = 0;
        if(i == lowerThreshold && tutorialText.gameObject.activeInHierarchy)
        {
            if (backButton.gameObject.activeInHierarchy)
                backButton.gameObject.SetActive(false);
        }
        else if(i > lowerThreshold && tutorialText.gameObject.activeInHierarchy)
        {
            if (!backButton.gameObject.activeInHierarchy)
                backButton.gameObject.SetActive(true);
        }
        if (i < 1)
        {
            if (tutorialText.gameObject.activeInHierarchy)
                tutorialText.gameObject.SetActive(false);
            if (!InitialText.gameObject.activeInHierarchy)
                InitialText.gameObject.SetActive(true);
        }
        else
        {
            if (InitialText.gameObject.activeInHierarchy)
                InitialText.gameObject.SetActive(false);
            if (!tutorialText.gameObject.activeInHierarchy)
                tutorialText.gameObject.SetActive(true);
            tutorialText.text = tutorialTextData[i];
        }
    }
    public void NextButton()
    {
        if (i < tutorialTextData.Length)
            i++;
        else
        {
            CloseHelp();
        }
    }
    public void BackButton()
    {
        if (!openHelp)
            i--;
        else
        {
            if (i > 2)
                i--;
        }
    }
    public void OpenHelp()
    {
        openHelp = true;
        this.gameObject.SetActive(true);
        i = 2;
    }
    public void CloseHelp()
    {
        openHelp = false;
        if (!FindObjectOfType<GameManager>().runTutorial)
        {
            FindObjectOfType<GameManager>().runTutorial = true;
            FindObjectOfType<GameManager>().Start();
        }
        UI.SetActive(true);
        navUI.SetActive(true);
        FindObjectOfType<NavigationManager>().ScrollToPos(FindObjectOfType<NavigationManager>().dayUI);
        this.gameObject.SetActive(false);
    }
}
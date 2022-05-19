using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBehaviour : MonoBehaviour
{

    //Tool Vars
    public GameObject searchBut;
    public GameObject sortBut;
    public GameObject propBut;
    public GameObject parentBut;
    public GameObject resetBut;
    public GameObject closeBut;
    bool isToolActive = false;

    //Search Vars
    public GameObject toSearch;
    public GameObject goBut;
    public GameObject keyBoard;
    public Text stringToSearch;
    bool isSearchActive = false;

    //Sort Vars
    public GameObject nameBut;
    public GameObject sizeBut;
    public GameObject timeCreatedBut;
    public GameObject typeBut;
    bool isSortActive = false;

    //Properties Vars
    public GameObject propertiesPanel;
    bool isPropertiesActive = false;

    //Other Vars
    FileBrowser fileBrowser;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PressTool()
    {
        if (fileBrowser == null)
        {
            fileBrowser = GetComponentInParent<FileBrowser>();
            fileBrowser.Properties(propertiesPanel.GetComponentInChildren<Text>());
        }
        isToolActive = !isToolActive;

        searchBut.SetActive(isToolActive);
        sortBut.SetActive(isToolActive);
        propBut.SetActive(isToolActive);
        resetBut.SetActive(isToolActive);
        parentBut.SetActive(isToolActive);
        closeBut.SetActive(isToolActive);

        if (!isToolActive)
        {
            isPropertiesActive = false;
            PressProperties();
            PressProperties();
        }
    }
    public void PressSearch()
    {
        isSearchActive = !isSearchActive;

        toSearch.SetActive(isSearchActive);
        goBut.SetActive(isSearchActive);
        keyBoard.SetActive(isSearchActive);

        if (isSearchActive)
        {
            isPropertiesActive = false;
            propertiesPanel.SetActive(isPropertiesActive);

            isSortActive = false;
            nameBut.SetActive(isSortActive);
            sizeBut.SetActive(isSortActive);
            timeCreatedBut.SetActive(isSortActive);
            typeBut.SetActive(isSortActive);
        }
    }

    public void PressSort()
    {
        isSortActive = !isSortActive;

        nameBut.SetActive(isSortActive);
        sizeBut.SetActive(isSortActive);
        timeCreatedBut.SetActive(isSortActive);
        typeBut.SetActive(isSortActive);

        if (isSortActive)
        {
            isPropertiesActive = false;
            propertiesPanel.SetActive(isPropertiesActive);

            isSearchActive = false;
            toSearch.SetActive(isSearchActive);
            goBut.SetActive(isSearchActive);
            keyBoard.SetActive(isSearchActive);
        }
    }

    public void PressProperties()
    {
        isPropertiesActive = !isPropertiesActive;

        propertiesPanel.SetActive(isPropertiesActive);

        if (isPropertiesActive)
        {
            isSortActive = false;
            isSearchActive = false;

            toSearch.SetActive(isSearchActive);
            goBut.SetActive(isSearchActive);
            nameBut.SetActive(isSortActive);
            sizeBut.SetActive(isSortActive);
            timeCreatedBut.SetActive(isSortActive);
            typeBut.SetActive(isSortActive);
        }
    }

    public void PressGo()
    {
        fileBrowser.Search(stringToSearch.text);
    }

    public void PressReset()
    {
        fileBrowser.ResetCab();
    }

    public void PressName()
    {
        fileBrowser.SortName();
    }

    public void PressTimeCreated()
    {
        fileBrowser.SortTimeCreated();
    }

    public void PressType()
    {
        fileBrowser.SortType();
    }

    public void PressSize()
    {
        fileBrowser.SortSize();
    }

    public void PressGoParent()
    {
        fileBrowser.GoParent();
        fileBrowser.Properties(propertiesPanel.GetComponentInChildren<Text>());
    }

    public void PressClose()
    {
        fileBrowser.CloseCabinet();
    }

}

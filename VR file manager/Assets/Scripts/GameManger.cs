using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManger : MonoBehaviour
{
    public GameObject controllerRight;
    public GameObject controllerLeft;
    public List<GameObject> cabinetList = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool InitCabinet(string path)
    {
        //Check if there is still a spot available
        if (AlreadyOpen(path))
                return false;
        

        for (int i = 0; i < cabinetList.Count; i++)
        {
            if (!cabinetList[i].activeInHierarchy)
            {
                cabinetList[i].GetComponent<FileBrowser>().pathToRead = path;
                cabinetList[i].SetActive(true);
                return true;
            }
        }   

        //if not return false
        return false;
    }

    public bool AlreadyOpen(string path)
    {
        foreach (var file in cabinetList)
        {
            if (file.GetComponent<FileBrowser>().pathToRead == path)
                return true;
        }
        return false;
    }

    public void ClearHands(GameObject toDelete)
    {
        controllerRight.GetComponent<ControllerGrabObject>().ReleaseObject(toDelete);
        controllerLeft.GetComponent<ControllerGrabObject>().ReleaseObject(toDelete);
    }
}

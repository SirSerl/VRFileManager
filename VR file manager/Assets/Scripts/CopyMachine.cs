using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine;

public class CopyMachine : MonoBehaviour
{
    public GameObject objectPos;

    // Properties Var
    public GameObject propertiesPanel;
    private bool isPropActive = true;

    private GameObject onMachine;
    private DirectoryInfo dir = new DirectoryInfo("././NewObjects");

    // Use this for initialization
    void Start()
    {

        if (!dir.Exists)
        {
            dir.Create();
        }

    }

    private void OnDestroy()
    {
        dir.Delete(true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PressProperties()
    {
        isPropActive = !isPropActive;
        propertiesPanel.SetActive(isPropActive);
    }

    public void PressCopy()
    {
        if (onMachine != null && onMachine.GetComponent<ObjectInfo>() != null && onMachine.GetComponent<ObjectInfo>().objectInfo.GetType() == typeof(FileInfo)) //Not able to copy directories yet
        {
            GameObject newObject = Instantiate(onMachine);

            FileSystemInfo fileSysInf = onMachine.GetComponent<ObjectInfo>().objectInfo;

            string nameStr = fileSysInf.Name.Substring(0);
            string extension = "";

            int name = fileSysInf.Name.LastIndexOf('.');
            if (name >= 0) //check for files withouth extension
            {
                nameStr = fileSysInf.Name.Substring(0, name);
                extension = fileSysInf.Name.Substring(name);
            }

            if (fileSysInf.GetType() == typeof(FileInfo))
            {
                string newFilePath = "././NewObjects/" + nameStr;

                bool exist = false;
                int id = 0;
                while (!exist)
                {
                    id++;
                    exist = Exist(1, newFilePath + id.ToString() + extension);
                }

               ((FileInfo)fileSysInf).CopyTo("././NewObjects/" + nameStr + id.ToString() + extension);
                newObject.GetComponent<ObjectInfo>().objectInfo = new FileInfo("././NewObjects/" + nameStr + id.ToString() + extension);
            }
            else
            {
                // string newFilePath = /*dir.FullName.Substring(0, pathLength)*/ "././NewObjects/" + nameStr;

                // bool exist = false;
                // int id = 0;
                // while (!exist)
                // {
                //     id++;
                //     exist = Exist(2, newFilePath + id.ToString() + extension);
                // }

                ////((DirectoryInfo)fileSysInf).CopyTo("././NewObjects/" + nameStr + id.ToString() + extension);
                // newObject.GetComponent<ObjectInfo>().objectInfo = new DirectoryInfo("././NewObjects/" + nameStr + id.ToString() + extension);
            }

            newObject.GetComponent<ObjectInfo>().cabinet = null;
            newObject.transform.position = objectPos.transform.position;
        }
    }

    private bool Exist(int type, string toCheck)
    {
        if (type == 1)
        {
            FileInfo check = new FileInfo(toCheck);
            return !check.Exists;
        }
        else
        {
            DirectoryInfo check = new DirectoryInfo(toCheck);
            return !check.Exists;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ObjectInfo>())
        {
            onMachine = other.gameObject;
            SetProperties();
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (onMachine == other.gameObject)
        {
            onMachine = null;
            propertiesPanel.GetComponentInChildren<Text>().text = "Put an Item on the copy machine to see it's properties!";
        }
    }

    private void SetProperties()
    {
        if (onMachine.GetComponent<ObjectInfo>() != null && onMachine.GetComponent<ObjectInfo>().objectInfo.GetType() != null)
        {
            Text textToChange = propertiesPanel.GetComponentInChildren<Text>();
            FileSystemInfo fileSysInf = onMachine.GetComponent<ObjectInfo>().objectInfo;
            if (fileSysInf.GetType() == typeof(FileInfo))
            {
                FileInfo file = (FileInfo)fileSysInf;
                textToChange.text = "Name : " + file.Name.ToString();
                textToChange.text += "\nFull name : " + file.FullName.ToString();
                textToChange.text += "\nExtension : " + file.Extension;
                textToChange.text += "\nSize : " + file.Length.ToString() + " bytes";
                textToChange.text += "\nCreation time : " + file.CreationTime.ToString();
                textToChange.text += "\nLast acces time : " + file.LastAccessTime.ToString();
                textToChange.text += "\nLast write time : " + file.LastWriteTime.ToString();
            }
            else
            {
                DirectoryInfo directory = (DirectoryInfo)fileSysInf;
                textToChange.text = "Name : " + directory.Name.ToString();
                textToChange.text += "\nFull name : " + directory.FullName.ToString();
                textToChange.text += "\nCreation time : " + directory.CreationTime.ToString();
                textToChange.text += "\nLast acces time : " + directory.LastAccessTime.ToString();
                textToChange.text += "\nLast write time : " + directory.LastWriteTime.ToString();
                if (directory.Parent != null)
                    textToChange.text += "\nParent : " + directory.Parent.ToString();
                textToChange.text += "\nRoot : " + directory.Root.ToString();
            }
        }
    }

}

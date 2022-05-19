using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Specialized;

public class FileBrowser : MonoBehaviour
{

    public string pathToRead;
    public Text path;
    DirectoryInfo directory;
    FileInfo[] filesDirectory;
    DirectoryInfo[] directoriesDirectory;
    public GameObject cabinet;

    //Item prefabs
    public GameObject audioPrefab;
    public GameObject picturePrefab;
    public GameObject textPrefab;
    public GameObject directoryPrefab;
    public GameObject genericPrefab;
    public GameObject videoPrefab;

    //Scroll button
    public GameObject leftButton;
    public GameObject rightButton;
    private int scrollOn = 0;
    private int maxScroll = 0;

    //GameManager
    private GameManger gameManager;
    public struct FileOnShelf
    {
        public FileOnShelf(GameObject gameObj, FileSystemInfo fileSysInf, Vector3 orgPos, Quaternion orgRot)
        {
            gameObject = gameObj;
            fileInfo = fileSysInf;
            originalPos = orgPos;
            originalRot = orgRot;
        }

        public GameObject gameObject;
        public FileSystemInfo fileInfo; //typecast to File/Directory Info
        public Vector3 originalPos;
        public Quaternion originalRot;

    }

    public List<FileOnShelf> filesList = new List<FileOnShelf>();


    //Use this for initialization
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManger>();

        if (filesList.Count <= 0)
            InitializeCabinet(pathToRead);
    }
    //Update is called once per frame
    void Update()
    {

    }

    void OnEnable()
    {
        ClearCabinet();
        InitializeCabinet(pathToRead);
    }

    private void InitializeCabinet(string pathDir)
    {
        //initialize directory and files
        if (pathDir == "")
            return;
        directory = new DirectoryInfo(pathDir);
        filesDirectory = directory.GetFiles();
        directoriesDirectory = directory.GetDirectories();

        //Set text above drawers
        path.text = pathDir;

        //load in the files on the 
        LoadInFiles();
        LoadInDirectories();

        //set buttons active when needed
        if (filesList.Count >= 50)
        {
            leftButton.SetActive(true);
            rightButton.SetActive(true);

            //calculate max scroll
            maxScroll = filesList.Count / 50;
        }
        else
        {
            leftButton.SetActive(false);
            rightButton.SetActive(false);
        }

        pathToRead = pathDir;
    }

    private void ClearCabinet()
    {
        foreach (var file in filesList)
        {
            Destroy(file.gameObject);
        }
        filesList.Clear();

        leftButton.SetActive(false);
        rightButton.SetActive(false);
    }
    private void LoadInFiles()
    {
        foreach (FileInfo file in filesDirectory)
        {
            //Check for what type
            string extension = file.Extension;
            GameObject toAdd = null;

            if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif")
            {
                toAdd = Instantiate(picturePrefab);
            }
            else if (extension == ".mp3" || extension == ".wav")
            {
                toAdd = Instantiate(audioPrefab);
            }
            else if (extension == ".mkv")
            {
                toAdd = Instantiate(videoPrefab);
            }
            else if (extension == ".txt" || extension == ".docx")
            {
                toAdd = Instantiate(textPrefab);
            }
            else
            {
                Debug.Log(file.Extension);
                toAdd = Instantiate(genericPrefab);
            }

            toAdd.GetComponentInChildren<Text>().text = file.Name;


            toAdd.GetComponent<ObjectInfo>().cabinet = this.gameObject;
            toAdd.GetComponent<ObjectInfo>().objectInfo = file;
            toAdd.GetComponent<ObjectInfo>().originalRot = toAdd.transform.rotation;

            //Set name for text and object
            toAdd.name = file.Name;

            SetPos(toAdd, 0, filesList.Count);

            //Add to list
            filesList.Add(new FileOnShelf(toAdd, file, toAdd.transform.position, toAdd.transform.rotation));

        }
    }

    public void LoadInDirectories()
    {
        foreach (DirectoryInfo direc in directoriesDirectory)
        {
            GameObject toAdd = Instantiate(directoryPrefab);

            //Set name for text and object
            toAdd.name = direc.Name;
            toAdd.GetComponentInChildren<Text>().text = direc.Name;

            toAdd.GetComponent<ObjectInfo>().cabinet = this.gameObject;
            toAdd.GetComponent<ObjectInfo>().objectInfo = direc;
            toAdd.GetComponent<ObjectInfo>().originalRot = toAdd.transform.rotation;

            //Set Position
            SetPos(toAdd, 0.05f, filesList.Count);

            filesList.Add(new FileOnShelf(toAdd, direc, toAdd.transform.position, toAdd.transform.rotation));
        }
    }

    private void SetPos(GameObject toSet, float offset, int posList)
    {
        Vector3 newPos;

        if (posList < 50)
        {
            newPos = new Vector3(-0.72f + (0.36f * ((posList / 2) % 5)), 1.68f - (0.36f * (posList / 10)), 0);

            //depth
            if (posList % 2 == 0)
            {
                newPos.z = -0.03f + offset;
                newPos.x -= 0.07f;
            }
            else
            {
                newPos.z = -0.12f + offset;
                newPos.x += 0.07f;
            }
        }
        else
        {
            int newPosList = posList - (50 * Mathf.FloorToInt(posList / 50)); //if above 50, start over from the top
            newPos = new Vector3(-0.72f + (0.36f * ((newPosList / 2) % 5)), 1.62f - (0.36f * (newPosList / 10)), 0);

            //depth
            if (newPosList % 2 == 0)
            {
                newPos.z = -0.03f + offset;
                newPos.x -= 0.07f;
            }
            else
            {
                newPos.z = -0.12f + offset;
                newPos.x += 0.07f;

            }

            toSet.SetActive(false);
        }
        toSet.transform.position = this.transform.position + newPos;
        toSet.transform.RotateAround(transform.position, new Vector3(0, 1, 0), transform.localEulerAngles.y + 90.0f);
    }

    #region CABINETUI
    //Searching
    public void Search(string toSearch)
    {
        int posInList = 0;
        //Go through objects on shelf
        foreach (FileOnShelf file in filesList)
        {
            //Check if it contains the string
            if (file.fileInfo.Name.Contains(toSearch, true))
            {
                file.gameObject.SetActive(true);
                SetPos(file.gameObject, 0.0f, posInList);
                posInList++;
            }
            else
            {
                file.gameObject.SetActive(false);
            }

        }
    }

    #region SORTING
    //Sorting
    public void SortName()
    {
        //SortedDictionary sorts on fileType
        SortedDictionary<string, FileOnShelf> nameSort = new SortedDictionary<string, FileOnShelf>();
        int number = 0;

        foreach (FileOnShelf file in filesList)
        {
            nameSort.Add(file.fileInfo.Name + number.ToString(), file); //Add Name to SortedDictionary
            number++;
            file.gameObject.transform.rotation = file.originalRot;
        }

        int listPos = 0;

        foreach (var entry in nameSort)
        {
            entry.Value.gameObject.SetActive(true); //Make sure gameObject is active
            SetPos(entry.Value.gameObject, 0.0f, listPos); //Set the new pos according to the sortedDictionary
            listPos++; //Increase the pos for next gameObject
        }
    }

    public void SortTimeCreated()
    {
        //SortedDictionary sorts on fileType
        SortedDictionary<string, FileOnShelf> nameSort = new SortedDictionary<string, FileOnShelf>();

        foreach (FileOnShelf file in filesList)
        {
            string date = file.fileInfo.CreationTimeUtc.ToString();
            string timeToAdd;
            int dateLastIn = date.LastIndexOf('/');
            int dateIn = date.IndexOf('/');

            //Set date to YYYY/MM/DD + HOURS + NAME
            timeToAdd = date.Substring(dateLastIn + 1, 4) + "/"; // Year
            timeToAdd += file.fileInfo.CreationTimeUtc.ToString().Substring(0, dateIn) + "/"; // Month
            timeToAdd += file.fileInfo.CreationTimeUtc.ToString().Substring(dateIn + 1, dateLastIn - dateIn - 1) + "/"; // Date
            timeToAdd += file.fileInfo.CreationTimeUtc.ToString().Substring(10) + file.fileInfo.Name; // Hour + Name for when files where created at same time

            nameSort.Add(timeToAdd, file); //Add Date to SortedDictionary
            file.gameObject.transform.rotation = file.originalRot;
        }

        int listPos = 0;

        foreach (var entry in nameSort)
        {
            entry.Value.gameObject.SetActive(true); //Make sure gameObject is active
            SetPos(entry.Value.gameObject, 0.0f, listPos); //Set the new pos according to the sortedDictionary
            listPos++; //Increase the pos for next gameObject

        }
    }

    public void SortType()
    {
        //SortedDictionary sorts on fileType
        SortedDictionary<string, FileOnShelf> nameSort = new SortedDictionary<string, FileOnShelf>();
        int number = 0;

        foreach (FileOnShelf file in filesList)
        {
            //Extension + Name for when files have the same extension
            string extension = file.fileInfo.Extension + file.fileInfo.Name + number.ToString();
            nameSort.Add(extension, file); //Add Name to SortedDictionary
            file.gameObject.transform.rotation = file.originalRot;
            number++;
        }

        int listPos = 0;

        foreach (var entry in nameSort)
        {
            entry.Value.gameObject.SetActive(true); //Make sure gameObject is active
            SetPos(entry.Value.gameObject, 0.0f, listPos); //Set the new pos according to the sortedDictionary
            listPos++; //Increase the pos for next gameObject

        }
    }

    public void SortSize()
    {
        // SortedDictionary sorts on fileType
        SortedDictionary<double, FileOnShelf> nameSort = new SortedDictionary<double, FileOnShelf>();
        double difId = 0;
        foreach (FileOnShelf file in filesList)
        {
            //Extension + Name for when files have the same extension
            double size;

            if (file.fileInfo.GetType() == typeof(FileInfo))
            {
                size = (double)((FileInfo)file.fileInfo).Length + difId;
            }
            else
            {
                size = 0 + difId;
            }
            nameSort.Add(size, file); //Add size to SortedDictionary
            difId += 0.1f;
            file.gameObject.transform.rotation = file.originalRot;
        }

        int listPos = 0;

        foreach (var entry in nameSort)
        {
            entry.Value.gameObject.SetActive(true); //Make sure gameObject is active
            SetPos(entry.Value.gameObject, 0.0f, listPos); //Set the new pos according to the sortedDictionary
            listPos++; //Increase the pos for next gameObject
        }
    }
    #endregion

    //Reset
    public void ResetCab()
    {
        //foreach (FileOnShelf file in filesList)
        //{
        //    file.gameObject.SetActive(true); //Make sure gameObject is active

        //    //Put the object back on it's initial position
        //    file.gameObject.transform.position = file.originalPos;
        //    file.gameObject.transform.rotation = file.originalRot;
        //    file.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        //    file.gameObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
        //}
        SetActive();
    }

    public void GoParent()
    {
        if (directory.Parent != null && !gameManager.AlreadyOpen(directory.Parent.ToString()))
        {
            ClearCabinet();
            InitializeCabinet(directory.Parent.ToString());
        }
    }

    public void Properties(Text textToChange)
    {
        textToChange.text = "Name : " + directory.Name.ToString();
        textToChange.text += "\nFull name : " + directory.FullName.ToString();
        textToChange.text += "\nCreation time : " + directory.CreationTime.ToString();
        textToChange.text += "\nLast acces time : " + directory.LastAccessTime.ToString();
        textToChange.text += "\nLast write time : " + directory.LastWriteTime.ToString();
        if (directory.Parent != null)
            textToChange.text += "\nParent : " + directory.Parent.ToString();
        textToChange.text += "\nRoot : " + directory.Root.ToString();
    }

    #region SCROLL
    public void PressLeft()
    {
        //move int
        scrollOn--;
        if (scrollOn < 0)
            scrollOn = maxScroll;

        //Set the next set active
        SetActive();
    }

    public void PressRight()
    {
        //move int
        scrollOn++;
        if (scrollOn > maxScroll)
            scrollOn = 0;

        //Set the next set active
        SetActive();
    }

    //Set Right files active
    private void SetActive()
    {
        //Set all on inactive
        foreach (var file in filesList)
        {
            file.gameObject.SetActive(false);
        }

        int amountOfObj = 50;
        if (scrollOn == maxScroll)
            amountOfObj = filesList.Count % 50;

        for (int posList = 0; posList < amountOfObj; posList++)
        {
            filesList[posList + (50 * scrollOn)].gameObject.SetActive(true);
            filesList[posList + (50 * scrollOn)].gameObject.transform.position = filesList[posList + (50 * scrollOn)].originalPos;
            filesList[posList + (50 * scrollOn)].gameObject.transform.rotation = filesList[posList + (50 * scrollOn)].originalRot;
        }
    }
    #endregion
    #endregion

    #region MOVING
    //Moving Files/Directories around
    void OnTriggerEnter(Collider other)
    {
        //check if object is a file/dir - if it isn't being moved in the same cabinet - if the dir isn't being moved in the dir
        if (other.GetComponent<ObjectInfo>() && other.GetComponent<ObjectInfo>().cabinet != cabinet && other.GetComponent<ObjectInfo>().objectInfo.FullName != directory.FullName)
        {
            ObjectInfo objInf = other.GetComponent<ObjectInfo>();
            Debug.Log("Passed");

            string nameStr = objInf.objectInfo.Name.Substring(0);
            string extension = "";
            //Move on disk
            int name = objInf.objectInfo.Name.LastIndexOf('.');
            if (name >= 0) //check for files withouth extension
            {
                nameStr = objInf.objectInfo.Name.Substring(0, name);
                extension = objInf.objectInfo.Name.Substring(name);
            }

            bool exist = false;
            int id = 1;

            //check if file already exist in new location, if so rename it
            if (objInf.objectInfo.GetType() == typeof(FileInfo))
            {
                if (!Exist(1, directory.FullName + "\\" + nameStr + extension))
                {
                    while (!exist)
                    {
                        exist = Exist(1, directory.FullName + "\\" + nameStr + id.ToString() + extension);
                        id++;
                    }
                    ((FileInfo)objInf.objectInfo).MoveTo(directory.FullName + "\\" + nameStr + id.ToString() + extension);
                    objInf.name = directory.FullName + "\\" + nameStr + id.ToString() + extension;
                    objInf.objectInfo = new FileInfo(directory.FullName + "\\" + nameStr + id.ToString() + extension);
                }
                else
                {
                    ((FileInfo)objInf.objectInfo).MoveTo(directory.FullName + "\\" + nameStr + extension);
                    objInf.objectInfo = new FileInfo(directory.FullName + "\\" + nameStr + extension);
                }
            }
            else //if Directory
            {
                if (!Exist(0, directory.FullName + "\\" + nameStr + extension))
                {
                    while (!exist)
                    {
                        exist = Exist(0, directory.FullName + "\\" + nameStr + id.ToString() + extension);
                        id++;
                    }

                    ((DirectoryInfo)objInf.objectInfo).MoveTo(directory.FullName + "\\" + nameStr + id.ToString() + extension);
                    objInf.name = directory.FullName + "\\" + nameStr + id.ToString() + extension;
                    objInf.objectInfo = new DirectoryInfo(directory.FullName + "\\" + nameStr + id.ToString() + extension);
                }
                else
                {
                    ((DirectoryInfo)objInf.objectInfo).MoveTo(directory.FullName + "\\" + nameStr + extension);
                    objInf.objectInfo = new DirectoryInfo(directory.FullName + "\\" + nameStr + extension);
                }
            }

            //remove from list in old cabinet
            if (objInf.cabinet != null)
                objInf.cabinet.GetComponent<FileBrowser>().RemoveFileFromList(other.gameObject);

            //add to list in new cabinet
            SetPos(other.gameObject, 0.00f, filesList.Count + 1);
            filesList.Add(new FileOnShelf(other.gameObject, other.GetComponent<ObjectInfo>().objectInfo, other.transform.position, other.GetComponent<ObjectInfo>().originalRot));
            objInf.cabinet = cabinet;
        }
    }

    //checks if the file/directory exists
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
    public void RemoveFileFromList(GameObject toRemove)
    {
        string toRemoveName = toRemove.GetComponent<ObjectInfo>().objectInfo.Name;

        //Loop through list to remove object that got moved
        foreach (var objToRemove in filesList)
        {
            if (objToRemove.fileInfo.Name == toRemoveName)
            {
                filesList.Remove(objToRemove);
                return;
            }
        }
    }
    #endregion

    //Close the cabinet
    public void CloseCabinet()
    {
        ClearCabinet();
        //Destroy(this.gameObject);
        pathToRead = "";
        this.gameObject.SetActive(false);
    }
}

//Contains Helper
public static class Helpers
{
    //Make it so the search function isn't case sensitive
    public static bool Contains(this string s, string other, bool lowerC)
    {
        if (lowerC)
        {
            s = s.ToLower();
            other = other.ToLower();
        }

        return s.Contains(other);
    }
}

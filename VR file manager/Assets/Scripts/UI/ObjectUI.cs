using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectUI : MonoBehaviour
{
    //General   
    public GameObject objCanPref;
    private GameObject objInst;
    private Canvas objCan;

    //Picture
    public GameObject imageObjPref;
    private GameObject imageObj;
    public enum TypeOfObj
    {
        picture,
        jar,
        cassette,
        video,
        generic,
        image
    }

    public TypeOfObj typeObj;

    // Use this for initialization
    void Start()
    {
        objInst = Instantiate(objCanPref);
        objCan = objInst.GetComponentInChildren<Canvas>();
        switch (typeObj)
        {
            case TypeOfObj.picture:
                objCan.GetComponentInChildren<Button>().onClick.AddListener(OpenPic);
                break;
            case TypeOfObj.jar:
                objCan.GetComponentInChildren<Button>().onClick.AddListener(OpenCabinet);
                break;
            case TypeOfObj.cassette:
                break;
            case TypeOfObj.video:
                break;
            case TypeOfObj.generic:
                break;
            case TypeOfObj.image:
                objCan.GetComponent<RectTransform>().sizeDelta = new Vector2 (this.GetComponent<BoxCollider>().size.x,this.GetComponent<BoxCollider>().size.y);
                objCan.GetComponent<RectTransform>().localScale = this.GetComponent<Transform>().localScale;
                objCan.GetComponentInChildren<Button>().GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                objCan.GetComponentInChildren<Button>().onClick.AddListener(CloseImage);
                break;
        }

    }

    // Update is called once per frame
    void Update()
    {
        objCan.GetComponent<RectTransform>().position = this.transform.position;
        objCan.GetComponent<RectTransform>().rotation = this.transform.rotation;

        switch (typeObj)
        {
            case TypeOfObj.picture:
                objCan.GetComponent<RectTransform>().Rotate(90, 0, -90);
                break;
            case TypeOfObj.jar:
                objCan.GetComponent<RectTransform>().Rotate(90, 0, 0);
                break;
            case TypeOfObj.cassette:
                break;
            case TypeOfObj.video:
                break;
            case TypeOfObj.generic:
                break;
            case TypeOfObj.image:
                objCan.GetComponent<RectTransform>().Rotate(0, 0, 0);
                break;
        }
    }

    private void OnDisable()
    {
        if (objCan)
            objCan.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (objCan)
            objCan.gameObject.SetActive(true);
    }
    private void OnDestroy()
    {
        if (objCan)
            Destroy(objCan.gameObject);
    }

    public void OpenCabinet()
    {
        GameObject manager = GameObject.Find("GameManager");

        manager.GetComponent<GameManger>().InitCabinet(this.GetComponent<ObjectInfo>().objectInfo.FullName);
    }

    public void OpenPic()
    {
        if (!imageObj)
        {
            imageObj = Instantiate(imageObjPref);
            imageObj.GetComponent<OpenPicture>().OpenThePicture(GetComponent<ObjectInfo>().objectInfo.FullName);
            imageObj.transform.position = transform.position;
        }
    }
    public void CloseImage()
    {
        GameObject.Find("GameManager").GetComponent<GameManger>().ClearHands(gameObject);
        Destroy(gameObject);
    }
}

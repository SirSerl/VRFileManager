using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
public class OpenPicture : MonoBehaviour
{

    public GameObject image;
    //private Rigidbody rigidBod;
    
    // Use this for initialization
    void Start()
    {
       // rigidBod = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
       // image.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
       // image.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
    }

    public void OpenThePicture(string path)
    {
        Texture2D tex = null;
        Rect rec;
        byte[] fileData;

        if (File.Exists(path))
        {
            fileData = File.ReadAllBytes(path);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
            rec = new Rect(0, 0, tex.width, tex.height);

        }
        else
            return;

        image.GetComponent<Image>().sprite = Sprite.Create(tex, rec, new Vector2(0, 0), 1);
        if (tex.width > 64)
        {
            image.GetComponent<RectTransform>().sizeDelta = new Vector2(tex.width / 64, tex.height / 64);
            this.GetComponent<BoxCollider>().size = new Vector3(tex.width / 64, tex.height / 64, 1);
        }
        else
        {
            image.GetComponent<RectTransform>().sizeDelta = new Vector2(tex.width, tex.height);
            this.GetComponent<BoxCollider>().size = new Vector3(tex.width, tex.height, 1);
        }
    }
}

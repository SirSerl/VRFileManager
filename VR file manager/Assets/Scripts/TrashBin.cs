using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBin : MonoBehaviour
{

    private List<GameObject> toDel = new List<GameObject>();
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        toDel.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        toDel.Remove(other.gameObject);
    }

    public void DeleteObj()
    {
        foreach (var obj in toDel)
        {
            if (obj.tag != "controller")
            {

                if (obj.GetComponent<ObjectInfo>())
                {
                    if (obj.GetComponent<ObjectInfo>().cabinet)
                        obj.GetComponent<ObjectInfo>().cabinet.GetComponent<FileBrowser>().RemoveFileFromList(obj.gameObject);
                    obj.GetComponent<ObjectInfo>().objectInfo.Delete();
                }
                GameObject.Find("GameManager").GetComponent<GameManger>().ClearHands(obj.gameObject);
                Destroy(obj.gameObject);
            }
        }
        toDel.Clear();
    }
}

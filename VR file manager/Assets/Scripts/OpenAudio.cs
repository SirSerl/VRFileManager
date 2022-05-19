using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenAudio : MonoBehaviour
{
    AudioClip audio;

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
        if (other.GetComponent<ObjectInfo>() && other.GetComponent<ObjectInfo>().objectInfo.Extension == ".wav")
            StartCoroutine(LoadAudio(other.GetComponent<ObjectInfo>().objectInfo.FullName));
    }
    IEnumerator LoadAudio(string path)
    {
        WWW loader = new WWW("file:///" + path);
        while (!loader.isDone) yield return 0;

        audio = loader.GetAudioClip();
        if (GetComponentInChildren<AudioSource>().isPlaying)
            GetComponentInChildren<AudioSource>().Stop();
        GetComponentInChildren<AudioSource>().PlayOneShot(audio);
    }
}

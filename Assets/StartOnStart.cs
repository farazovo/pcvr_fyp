using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViveSR.anipal.Eye;

public class StartOnStart : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ART_DataRecorder.Instance.Open();
    }

/*    private void OnDestroy()
    {
        ART_DataRecorder.Instance.Close();
    }*/
}

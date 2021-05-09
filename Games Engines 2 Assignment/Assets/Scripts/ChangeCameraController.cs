using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCameraController : MonoBehaviour
{
    [SerializeField] private GameObject[] cameraObjects;
    [SerializeField] private GameObject[] cameraCanvas;

    private bool activated;

    [SerializeField] private GameObject soundDiving;
    [SerializeField] private GameObject soundNature;

    private int tempRecordState;

    private bool isAuto;

    private void Start()
    {
        activated = false;
        Cursor.lockState = CursorLockMode.Locked;
        ActivateCanvas(activated);
        tempRecordState = 0;
        StartAuto();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            activated = !activated;
            if (!activated)
                Cursor.lockState = CursorLockMode.Locked;
            else
                Cursor.lockState = CursorLockMode.None;
            ActivateCanvas(activated);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha0))
            ChangeCamera(0);
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ChangeCamera(1);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            ChangeCamera(3);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            ChangeCamera(3);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            ChangeCamera(4);
        if (Input.GetKeyDown(KeyCode.Alpha5))
            ChangeCamera(5);
        if (Input.GetKeyDown(KeyCode.Alpha6))
            ChangeCamera(6);
        if (Input.GetKeyDown(KeyCode.Alpha7))
            ChangeCamera(7);
        if (Input.GetKeyDown(KeyCode.Alpha8))
            ChangeCamera(8);
        if (Input.GetKeyDown(KeyCode.Alpha9))
            ChangeCamera(9);
    }

    private void ActivateCanvas(bool turnOn)
    {
        foreach (var canvas in cameraCanvas)
        {
            canvas.SetActive(turnOn);
        }
    }

    public void ChangeCamera(int whichCamera)
    {
        tempRecordState = whichCamera;
        
        if (whichCamera == 1)
        {
            soundDiving.SetActive(false);
            soundNature.SetActive(true);
        }
        else
        {
            soundDiving.SetActive(true);
            soundNature.SetActive(false);
        }

        if (whichCamera == 9)
        {
            activated = false;
            ActivateCanvas(activated);
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        for (var i = 0; i < cameraObjects.Length; i++)
        {
            cameraObjects[i].SetActive(i == whichCamera);
        }
    }

    public void StartAuto()
    {
        Cursor.lockState = CursorLockMode.Locked;
        activated = false;
        ActivateCanvas(activated);
        isAuto = true;
        if (tempRecordState == 9)
        {
            tempRecordState = 0;
            ChangeCamera(tempRecordState);
        }
        StartCoroutine(VideoRecord());
    }

    public void StopAuto()
    {
        isAuto = false;
        StopCoroutine(VideoRecord());
    }

    private IEnumerator VideoRecord()
    {
        yield return new WaitForSeconds(15);
        tempRecordState++;
        if (tempRecordState >= 9)
            tempRecordState = 0;
        
        if (isAuto)
        {
            ChangeCamera(tempRecordState);
            StartCoroutine(VideoRecord());
        }
            
    }
}

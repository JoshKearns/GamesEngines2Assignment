using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishCamSetUp : MonoBehaviour
{
    [SerializeField] private GameObject cam;
    private void Start()
    {
        StartCoroutine(FindFishCamObject());
    }

    private IEnumerator FindFishCamObject()
    {
        yield return new WaitForSeconds(0.5f);
        transform.parent = GameObject.FindGameObjectWithTag("FishCam").transform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        
        cam.SetActive(true);
        gameObject.SetActive(false);
    }
}

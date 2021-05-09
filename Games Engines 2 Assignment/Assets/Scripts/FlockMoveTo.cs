using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class FlockMoveTo : MonoBehaviour
{
    [SerializeField] private float waitTime;
    
    [SerializeField] private float minBoundsX;
    [SerializeField] private float maxBoundsX;
    
    [SerializeField] private float minBoundsY;
    [SerializeField] private float maxBoundsY;
    
    [SerializeField] private float minBoundsZ;
    [SerializeField] private float maxBoundsZ;
    
    [SerializeField] private LayerMask obstacleMask;
    private void Start()
    {
        StartCoroutine(MoveFlock());
    }

    private IEnumerator MoveFlock()
    {
        LookForNewPosition();
        yield return new WaitForSeconds(waitTime);
        StartCoroutine(MoveFlock());
    }

    private void MoveFunction(float MoveToX, float MoveToY)
    {
        transform.position = new Vector3(UnityEngine.Random.Range(minBoundsX, maxBoundsX),
            UnityEngine.Random.Range(minBoundsY, maxBoundsY),
            UnityEngine.Random.Range(minBoundsX, maxBoundsX));
    }

    private void LookForNewPosition()
    {
        var randomX = UnityEngine.Random.Range(minBoundsX, maxBoundsX);
        var randomY = UnityEngine.Random.Range(minBoundsY, maxBoundsY);
        var randomZ = UnityEngine.Random.Range(minBoundsZ, maxBoundsZ);
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, new Vector3(randomX, randomY, randomX) - transform.position, out hit, 5,
            obstacleMask))
        {
            LookForNewPosition();
        }
        else
        {
            MoveFunction(randomX, randomY);
        }
    }
}

﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class FlockUnit : MonoBehaviour
{
    [SerializeField] private bool twoDMovement;
    
    [SerializeField] private float FOVAngle;
    [SerializeField] private float smoothDamp;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private Vector3[] directionToCheckWhenAvoidingObstacle;

    private List<FlockUnit> cohesionNeighbours = new List<FlockUnit>();
    private List<FlockUnit> avoidanceNeighbours = new List<FlockUnit>();
    private List<FlockUnit> alignmentNeighbours = new List<FlockUnit>();
    private Flock assignedFlock;
    private Vector3 currentVelocity;
    private Vector3 currentObstacleAvoidanceVector;
    private float speed;
    
    public Transform myTransform { get; set; }

    private void Awake()
    {
        myTransform = transform;
    }

    public void AssignFlock(Flock flock)
    {
        assignedFlock = flock;
    }

    public void InitializeSpeed(float speed)
    {
        this.speed = speed;
    }

    public void SetFishCamSmoothDamp()
    {
        smoothDamp = 5;
    }
    
    public void MoveUnit()
    {
        FindNeighbours();
        CalculateSpeed();
        
        var cohesionVector = CalculateCohesionVector() * assignedFlock.cohesionWeight;
        var avoidanceVector = CalculateAvoidanceVector() * assignedFlock.avoidanceWeight;
        var alignmentVector = CalculateAlignmentVector() * assignedFlock.alignmentWeight;
        var boundsVector = CalculateBoundsVector() * assignedFlock.boundsWeight;
        var obstacleVector = CalculateObstacleVector() * assignedFlock.obstacleWeight;

        var moveVector = Vector3.zero;
        
        if (twoDMovement)
        {
            moveVector = new Vector3(cohesionVector.x, 0, cohesionVector.z) + 
                new Vector3(avoidanceVector.x, 0, avoidanceVector.z) +
                new Vector3(alignmentVector.x, 0, alignmentVector.z) + 
                new Vector3(boundsVector.x, 0, boundsVector.z) + 
                new Vector3(obstacleVector.x, 0, obstacleVector.z);
        }
        else
        {
            moveVector = cohesionVector + avoidanceVector + alignmentVector + boundsVector + obstacleVector;
        }
        
        moveVector = Vector3.SmoothDamp(myTransform.forward, moveVector, ref currentVelocity, smoothDamp);
        moveVector = moveVector.normalized * speed;
        if (moveVector == Vector3.zero)
            moveVector = transform.forward;
        
        myTransform.forward = moveVector;
        myTransform.position += moveVector * Time.deltaTime;
    }

    private void FindNeighbours()
    {
        cohesionNeighbours.Clear();
        avoidanceNeighbours.Clear();
        alignmentNeighbours.Clear();
        
        var allUnits = assignedFlock.allUnits;
        for (var i = 0; i < allUnits.Length; i++)
        {
            var currentUnit = allUnits[i];
            if (currentUnit != this)
            {
                float currentNeighbourDistanceSqr =
                    Vector3.SqrMagnitude(currentUnit.myTransform.position - myTransform.position);
                if (currentNeighbourDistanceSqr <= assignedFlock.cohesionDistance * assignedFlock.cohesionDistance)
                {
                    cohesionNeighbours.Add(currentUnit);
                }
                if (currentNeighbourDistanceSqr <= assignedFlock.avoidanceDistance * assignedFlock.avoidanceDistance)
                {
                    avoidanceNeighbours.Add(currentUnit);
                }
                if (currentNeighbourDistanceSqr <= assignedFlock.alignmentDistance * assignedFlock.alignmentDistance)
                {
                    alignmentNeighbours.Add(currentUnit);
                }
            }
        }
    }

    private void CalculateSpeed()
    {
        if (cohesionNeighbours.Count == 0)
            return;
        speed = 0;
        for (int i = 0; i < cohesionNeighbours.Count; i++)
        {
            speed += cohesionNeighbours[i].speed;
        }

        speed /= cohesionNeighbours.Count;
        speed = Mathf.Clamp(speed, assignedFlock.minSpeed, assignedFlock.maxSpeed);
    }

    private Vector3 CalculateCohesionVector()
    {
        var cohesionVector = Vector3.zero;
        if (cohesionNeighbours.Count == 0)
            return cohesionVector;
        int neighboursInFOV = 0;
        for (int i = 0; i < cohesionNeighbours.Count; i++)
        {
            if (IsInFOV(cohesionNeighbours[i].myTransform.position))
            {
                neighboursInFOV++;
                cohesionVector += cohesionNeighbours[i].myTransform.position;
            }
        }

        cohesionVector /= neighboursInFOV;
        cohesionVector -= myTransform.position;
        cohesionVector = cohesionVector.normalized;
        return cohesionVector;
    }

    private Vector3 CalculateAlignmentVector()
    {
        var alignmentVector = myTransform.forward;
        if (alignmentNeighbours.Count == 0)
            return myTransform.forward;
        int neighboursInFOV = 0;
        for (int i = 0; i < alignmentNeighbours.Count; i++)
        {
            if (IsInFOV(alignmentNeighbours[i].myTransform.position))
            {
                neighboursInFOV++;
                alignmentVector += alignmentNeighbours[i].myTransform.forward;
            }
        }

        alignmentVector /= neighboursInFOV;
        alignmentVector = alignmentVector.normalized;
        return alignmentVector;
    }
    private Vector3 CalculateAvoidanceVector()
    {
        var avoidanceVector = Vector3.zero;
        if (alignmentNeighbours.Count == 0)
            return Vector3.zero;
        int neighboursInFOV = 0;
        for (int i = 0; i < avoidanceNeighbours.Count; i++)
        {
            if (IsInFOV(avoidanceNeighbours[i].myTransform.position))
            {
                neighboursInFOV++;
                avoidanceVector += (myTransform.position - avoidanceNeighbours[i].myTransform.position);
            }
        }

        avoidanceVector /= neighboursInFOV;
        avoidanceVector = avoidanceVector.normalized;
        return avoidanceVector;
    }

    private Vector3 CalculateBoundsVector()
    {
        var offsetToCenter = assignedFlock.transform.position - myTransform.position;
        bool isNearCenter = (offsetToCenter.magnitude >= assignedFlock.boundsDistance * 0.9f);
        return isNearCenter ? offsetToCenter.normalized : Vector3.zero;
    }

    private Vector3 CalculateObstacleVector()
    {
        var obstacleVector = Vector3.zero;
        RaycastHit hit;
        if (Physics.Raycast(myTransform.position, myTransform.forward, out hit, assignedFlock.obstacleDistance,
            obstacleMask))
        {
            obstacleVector = FindBestDirectionToAvoidObstacle();
        }
        else
        {
            currentObstacleAvoidanceVector = Vector3.zero;
        }

        return obstacleVector;
    }

    private Vector3 FindBestDirectionToAvoidObstacle()
    {
        if (currentObstacleAvoidanceVector != Vector3.zero)
        {
            RaycastHit hit;
            if (Physics.Raycast(myTransform.position, myTransform.forward, out hit, assignedFlock.obstacleDistance,
                obstacleMask))
            {
                return currentObstacleAvoidanceVector;
            }
        }
        float maxDistance = int.MinValue;
        var selectedDirection = Vector3.zero;
        for (int i = 0; i < directionToCheckWhenAvoidingObstacle.Length; i++)
        {
            RaycastHit hit;
            var currentDirection = myTransform.TransformDirection(directionToCheckWhenAvoidingObstacle[i].normalized);
            if (Physics.Raycast(myTransform.position, currentDirection, out hit, assignedFlock.obstacleDistance, obstacleMask))
            {
                float currentDistance = (hit.point - myTransform.position).sqrMagnitude;
                if (currentDistance > maxDistance)
                {
                    maxDistance = currentDistance;
                    selectedDirection = currentDirection;
                }
            }
            else
            {
                selectedDirection = currentDirection;
                currentObstacleAvoidanceVector = currentDirection.normalized;
                return selectedDirection.normalized;
            }
        }

        return selectedDirection.normalized;
    }

    private bool IsInFOV(Vector3 position)
    {
        return Vector3.Angle(myTransform.forward, position - myTransform.position) <= FOVAngle;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraClampAndFollow : MonoBehaviour
{
    [SerializeField] private Transform targetToFollow;
    [SerializeField] public Transform left, right, top, down;
    [SerializeField] public bool isActive;
    [SerializeField] public float cameraOffsetX;
    [SerializeField] public float cameraOffsetY;

    private void Update()
    {
        //ClampScene();

        if (isActive)
            TestFollow();
        else
            return;


    }

    private void Follow()
    {
        transform.position = new Vector3(
            Mathf.Clamp(targetToFollow.position.x + cameraOffsetX, left.position.x, right.position.x),
            Mathf.Clamp(targetToFollow.position.y + cameraOffsetY, down.position.y, top.position.y),
            transform.position.z);
    }    
    private void TestFollow()
    {
        transform.position = new Vector3(
           Mathf.Clamp(targetToFollow.position.x, -2.98f, 33.18f), Mathf.Clamp(targetToFollow.position.y, -0.2f, 0.48f),
            transform.position.z);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    public Transform moon;
    public float sensitivity = 1f;
    public Vector2 verticalRange = new Vector2(-90, 90);
    public Transform xRotationObject;
    public Transform yRotationObject;
    public Transform zPosition;
    public Transform camera;

    private Vector2 lookDegrees = new Vector2(0, 0);

    void Update()
    {
        lookDegrees += new Vector2(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X")) * sensitivity;
        lookDegrees.x = Mathf.Clamp(lookDegrees.x, verticalRange.x, verticalRange.y);

        xRotationObject.localRotation = Quaternion.AngleAxis(-lookDegrees.x, Vector3.right);
        yRotationObject.localRotation = Quaternion.AngleAxis(lookDegrees.y, Vector3.up);
    }

    public void Recenter(Transform target, float cameraDistance)
    {
        transform.SetParent(target);
        transform.localPosition = Vector3.zero;
        transform.LookAt(target, Vector3.up);
        zPosition.localPosition = new Vector3(0, 0, -cameraDistance);
    }
}

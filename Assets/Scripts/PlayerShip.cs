using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : MonoBehaviour
{
    [SerializeField] float shipSpeed = 5f;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float timer = 60f;
    private Quaternion originalRotation;

    private void Start()
    {
        originalRotation = transform.rotation;
    }

    void Update()
    {
        //위치변환
        if (Input.GetKey(KeySetting.Keys[KeyAction.UP]))
        {
            transform.position += Vector3.up * shipSpeed * Time.deltaTime;
            transform.Rotate(new Vector3(0,0,40) * Time.deltaTime); 
        }
        else if (Input.GetKey(KeySetting.Keys[KeyAction.DOWN]))
        {
            transform.position += Vector3.down * shipSpeed * Time.deltaTime;
            transform.Rotate(new Vector3(0, 0, -40) * Time.deltaTime);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, rotationSpeed * Time.deltaTime);
        }
    }
}

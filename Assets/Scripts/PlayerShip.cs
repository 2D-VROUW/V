using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : MonoBehaviour
{
    [SerializeField] float shipSpeed = 6f;
    [SerializeField] float rotationSpeed = 5f;
    [SerializeField] float timer = 60f;
    private Quaternion originalRotation;
    private float maxY = 3.3f;

    Vector2 currentPos;

    private void Start()
    {
        originalRotation = transform.rotation;
    }

    void Update()
    {
        currentPos = transform.position;
        //위치변환
        if (Input.GetKey(KeySetting.Keys[KeyAction.UP]))
        {
            if (currentPos.y <= maxY)
            {
                transform.position += Vector3.up * shipSpeed * Time.deltaTime;
                transform.Rotate(new Vector3(0, 0, 30) * Time.deltaTime);
            }
        }
        else if (Input.GetKey(KeySetting.Keys[KeyAction.DOWN]))
        {
            if (currentPos.y >= -maxY)
            {
                transform.position += Vector3.down * shipSpeed * Time.deltaTime;
                transform.Rotate(new Vector3(0, 0, -30) * Time.deltaTime);
            }
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, rotationSpeed * Time.deltaTime);
        }

        if (currentPos.y >= maxY || currentPos.y <= -maxY)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, rotationSpeed * Time.deltaTime);
        }
    }
}

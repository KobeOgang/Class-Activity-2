using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSystem : MonoBehaviour
{
    public float speed = 0;
    public float minSpeed = 0.0f;
    public float maxSpeed = 50.0f;
    public int accel;
    public int decel;
    public int rotSpeed;

    private void Start()
    {
        accel = Random.Range(4, 6);
        decel = Random.Range(5, 7);
        rotSpeed = Random.Range(10, 12);
    }

    public void ApplyAcceleration()
    {
        speed = Mathf.Clamp(speed + (accel * Time.deltaTime), minSpeed, maxSpeed);
    }

    public void ApplyDeceleration()
    {
        speed = Mathf.Clamp(speed - (decel * Time.deltaTime), minSpeed, maxSpeed);
    }

    public void ApplyTranslation()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}

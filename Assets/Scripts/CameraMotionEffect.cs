using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotionEffect : MonoBehaviour
{
    public float smoothness = 0.5f;
    public float motionScale = 0.5f;
    public float returnSpeed = 2f;
    public float maxTiltAngle = 10f;

    private Vector3 initialRotation;
    private Vector3 currentTilt;
    private bool isMoving = false;

    void Start()
    {
        Input.gyro.enabled = true;
        initialRotation = transform.localEulerAngles;
    }

    void Update()
    {
        Vector3 tilt = Vector3.zero;

        if (SystemInfo.supportsGyroscope)
        {
            // Sử dụng con quay hồi chuyển (gyroscope) nếu thiết bị hỗ trợ
            Quaternion deviceRotation = Input.gyro.attitude;
            deviceRotation = new Quaternion(deviceRotation.x, deviceRotation.y, -deviceRotation.z, -deviceRotation.w); // Chuyển đổi hệ tọa độ
            tilt = deviceRotation.eulerAngles;
        }
        else
        {
            // Nếu không có con quay, sử dụng gia tốc kế (accelerometer)
            Vector3 acceleration = Input.acceleration;
            tilt = new Vector3(-acceleration.y, acceleration.x, 0) * motionScale;
        }

        tilt.x = Mathf.Clamp(tilt.x, -maxTiltAngle, maxTiltAngle);
        tilt.y = Mathf.Clamp(tilt.y, -maxTiltAngle, maxTiltAngle);

        if (tilt.magnitude > 0.1f)
        {
            isMoving = true;
            currentTilt = tilt;
        }
        else
        {
            isMoving = false;
        }

        if (!isMoving)
        {
            // Khi không có chuyển động, quay lại góc quay ban đầu (khóa Z)
            transform.localEulerAngles = new Vector3(
                Mathf.Lerp(transform.localEulerAngles.x, initialRotation.x, Time.deltaTime * returnSpeed),
                Mathf.Lerp(transform.localEulerAngles.y, initialRotation.y, Time.deltaTime * returnSpeed),
                initialRotation.z); // Khóa góc Z
        }
        else
        {
            // Khi có chuyển động, tính toán góc quay mới (khóa Z)
            Vector3 targetRotation = initialRotation + currentTilt;
            transform.localEulerAngles = new Vector3(
                Mathf.Lerp(transform.localEulerAngles.x, targetRotation.x, Time.deltaTime * smoothness),
                Mathf.Lerp(transform.localEulerAngles.y, targetRotation.y, Time.deltaTime * smoothness),
                initialRotation.z); // Khóa góc Z
        }
    }
}

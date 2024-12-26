using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    public float smoothSpeed = 0.125f;
    CinemachineVirtualCamera vcam;

    private void Start()
    {
        vcam = gameObject.GetComponent<CinemachineVirtualCamera>();
        vcam.Follow = player;
        vcam.LookAt = player;
    }
    //void LateUpdate()
    //{
    //    if (player == null)
    //    {
    //        return;
    //    }
    //    if (onCamera)
    //    {
    //        Vector3 desiredPosition = player.position + offset;
    //        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);
    //        transform.position = smoothedPosition;
    //        transform.LookAt(player);
    //    }
    //}

    public void OnCamera(float x, float y, float z)
    {
        var framingTransposer = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
        framingTransposer.m_TrackedObjectOffset = new Vector3(x, y, z);
    }

    public void SetAim(float screenX)
    {
        var body = vcam.GetCinemachineComponent<CinemachineComposer>();
        body.m_ScreenX = screenX;
    }
}

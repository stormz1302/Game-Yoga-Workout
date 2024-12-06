using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RotateModel : MonoBehaviour
{
    private Vector3 previousMousePosition;
    private bool isDragging = false;
    [SerializeField] private GameObject SpawnCharacter;
    [SerializeField] private RawImage dragZone;
    bool shopOpening = false;

    private void Start()
    {
        dragZone = Shop.instance.character;
    }

    private void Update()
    {
        shopOpening = CanvasLv1.Instance.shopOpening;
        DragRotate();
        if (!isDragging)
        {
            SpawnCharacter.transform.Rotate(Vector3.up, 15f * Time.deltaTime, Space.World);
        }

    }

    private void DragRotate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Kiểm tra nếu raycast trúng mô hình
            if (IsPointerOverModel())
            {
                isDragging = true;
                previousMousePosition = Input.mousePosition;
            }
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector3 deltaMousePosition = Input.mousePosition - previousMousePosition;
            previousMousePosition = Input.mousePosition;

            Debug.Log("Drag Rotating");
            SpawnCharacter.transform.Rotate(Vector3.up, -deltaMousePosition.x * 0.5f, Space.World);
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    private bool IsPointerOverModel()
    {
        if (!shopOpening)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Camera.main == null) Debug.LogError("Main Camera is missing!");

            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green, 2f);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~0, QueryTriggerInteraction.Collide))
            {
                
                Debug.Log("Raycast hit: " + hit.collider.name);
                if (hit.collider.gameObject == SpawnCharacter || hit.collider.transform.IsChildOf(SpawnCharacter.transform))
                {
                    Debug.Log($"Raycast hit: {hit.collider.name}");
                    return true;
                }
            }
            return false;
        }
        else
        {
            // Tạo danh sách để lưu kết quả raycast UI
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition // Vị trí chuột
            };

            // Danh sách kết quả raycast
            var results = new System.Collections.Generic.List<RaycastResult>();

            // Lấy GraphicRaycaster từ Canvas
            GraphicRaycaster raycaster = dragZone.canvas.GetComponent<GraphicRaycaster>();

            // Gọi raycast trên UI
            raycaster.Raycast(pointerEventData, results);

            // Kiểm tra xem raycast có trúng RawImage (dragZone) hay không
            foreach (var result in results)
            {
                if (result.gameObject == dragZone.gameObject)
                {
                    return true; // Chuột đang trỏ vào RawImage
                }
            }
            return false; 
        }
    }
}

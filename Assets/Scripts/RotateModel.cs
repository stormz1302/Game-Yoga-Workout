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
        if (Input.GetMouseButtonDown(0) && Input.touchCount != 2)
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

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~0, QueryTriggerInteraction.Collide))
            {
                if (hit.collider.gameObject == SpawnCharacter || hit.collider.transform.IsChildOf(SpawnCharacter.transform))
                {
                    return true;
                }
            }
            return false;
        }
        else
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition // Vị trí chuột
            };
            var results = new System.Collections.Generic.List<RaycastResult>();
            GraphicRaycaster raycaster = dragZone.canvas.GetComponent<GraphicRaycaster>();
            raycaster.Raycast(pointerEventData, results);

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

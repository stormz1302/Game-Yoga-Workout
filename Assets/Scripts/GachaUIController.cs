using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GachaUIController : MonoBehaviour
{
    public RectTransform objectToMove; // Đối tượng UI cần di chuyển
    public RectTransform[] points; // Mảng chứa các RectTransform làm mục tiêu
    public List<int> selectedPoints = new List<int>();
    public float speed = 2.0f; // Tốc độ di chuyển
    public int maxLoops = 3; // Số vòng lặp tối đa trước khi dừng
    public TMP_Text rewardText; // Text hiển thị số tiền thưởng
    public GameObject AdsButton;

    private int loopCount = 0; // Biến đếm số vòng lặp
    private int currentTargetIndex = 0; // Chỉ số của RectTransform mục tiêu hiện tại

    public void OnLoadEndGamePopup()
    {
        AdsButton.gameObject.SetActive(false);
        // Kiểm tra xem mảng RectTransform có ít nhất 5 điểm không
        if (points.Length < 5)
        {
            Debug.LogError("Cần ít nhất 5 tọa độ (RectTransform).");
            return;
        }

        // Bắt đầu Coroutine di chuyển đối tượng
        StartCoroutine(MoveObjectCoroutine());
    }

    IEnumerator MoveObjectCoroutine()
    {
        while (loopCount < maxLoops)
        {
            currentTargetIndex = Random.Range(0, points.Length);
            
            // Di chuyển đối tượng đến tọa độ mục tiêu, chỉ thay đổi trục X
            yield return StartCoroutine(MoveToTarget(points[currentTargetIndex].localPosition.x));

            loopCount++;
            yield return null; // Thêm thời gian nghỉ giữa các vòng lặp
        }
    }

    // Cập nhật phương thức này để chỉ thay đổi trục X
    IEnumerator MoveToTarget(float targetX)
    {
        float startX = objectToMove.localPosition.x;
        float startY = objectToMove.localPosition.y; // Giữ nguyên giá trị Y
        float startZ = objectToMove.localPosition.z; // Giữ nguyên giá trị Z
        float lerpTime = 0f;

        // Di chuyển đối tượng đến tọa độ mục tiêu trên trục X
        while (lerpTime < 1f)
        {
            lerpTime += Time.deltaTime * speed; // Tăng lerpTime theo thời gian

            // Chỉ thay đổi giá trị X, giữ nguyên Y và Z
            objectToMove.localPosition = new Vector3(Mathf.Lerp(startX, targetX, lerpTime), startY, startZ);

            yield return null; // Đợi một frame tiếp theo
        }
        AdsButton.gameObject.SetActive(true);
        rewardText.text = (GameManager.Instance.bonusMoney * selectedPoints[currentTargetIndex]).ToString();
        // Đảm bảo đối tượng dừng tại vị trí chính xác trên trục X
        objectToMove.localPosition = new Vector3(targetX, startY, startZ);
    }
}

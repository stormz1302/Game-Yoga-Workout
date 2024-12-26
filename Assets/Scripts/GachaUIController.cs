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
    public float speed = 3.0f; // Tốc độ di chuyển
    public int maxLoops = 3; // Số vòng lặp tối đa trước khi dừng
    public TMP_Text rewardText; // Text hiển thị số tiền thưởng
    public GameObject AdsButton;
    private int currentTargetIndex = 0; // Chỉ số của RectTransform mục tiêu hiện tại
    private bool isSpinning = false; // Cờ kiểm tra xem kim có đang quay không
    private bool movingForward = true; // Cờ điều khiển hướng di chuyển (true: từ đầu đến cuối, false: từ cuối lên đầu)


    private void Start()
    {
        AdsButton.GetComponent<Button>().onClick.AddListener(StopSpinning);
    }
    public void OnLoadEndGamePopup()
    {
        //AdsButton.gameObject.SetActive(false);
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
        isSpinning = true; // Đánh dấu kim đang quay
        while (isSpinning)
        {
            if (movingForward)
            {
                // Di chuyển từ đầu đến cuối
                for (int i = 0; i < points.Length; i++)
                {
                    // Di chuyển đối tượng đến tọa độ mục tiêu (slot)
                    yield return StartCoroutine(MoveToTarget(points[i].localPosition.x));

                    // Khi đến điểm cuối, thay đổi hướng di chuyển
                    if (i == points.Length - 1)
                    {
                        movingForward = false;
                    }
                }
            }
            else
            {
                // Di chuyển từ cuối lên đầu
                for (int i = points.Length - 1; i >= 0; i--)
                {
                    // Di chuyển đối tượng đến tọa độ mục tiêu (slot)
                    yield return StartCoroutine(MoveToTarget(points[i].localPosition.x));

                    // Khi đến điểm đầu, thay đổi hướng di chuyển
                    if (i == 0)
                    {
                        movingForward = true;
                    }
                }
            }
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
        while (lerpTime < 1f && isSpinning)
        {
            lerpTime += Time.deltaTime * speed; // Tăng lerpTime theo thời gian

            // Chỉ thay đổi giá trị X, giữ nguyên Y và Z
            objectToMove.localPosition = new Vector3(Mathf.Lerp(startX, targetX, lerpTime), startY, startZ);
            currentTargetIndex = GetCurrentTargetIndex();
            rewardText.text = (GameManager.Instance.bonusMoney * selectedPoints[currentTargetIndex]).ToString();
            yield return null; // Đợi một frame tiếp theo
        }
    }

    // Hàm để dừng ngay lập tức khi người chơi bấm nút
    public void StopSpinning()
    {
        //show ads

        //dừng quay kim ngay lập tức
        isSpinning = false; // Dừng quay kim ngay lập tức
        currentTargetIndex = GetCurrentTargetIndex(); // Xác định chỉ số mục tiêu kim đang dừng tại
        Debug.Log("Bạn nhận được phần thưởng tại vị trí: " + selectedPoints[currentTargetIndex]);
    }

    // Hàm lấy vị trí mục tiêu hiện tại của kim
    int GetCurrentTargetIndex()
    {
        // Lấy chỉ số mục tiêu gần nhất
        float currentX = objectToMove.localPosition.x;
        float closestDistance = Mathf.Abs(currentX - points[0].localPosition.x);
        int index = 0;

        for (int i = 1; i < points.Length; i++)
        {
            float distance = Mathf.Abs(currentX - points[i].localPosition.x);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                index = i;
            }
        }

        return index;
    }
}

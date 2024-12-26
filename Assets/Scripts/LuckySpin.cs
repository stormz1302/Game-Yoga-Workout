using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LuckySpin : MonoBehaviour
{
    [SerializeField] RectTransform wheel; // Vòng quay (đối tượng UI)
    [SerializeField] Button spinButton;   // Nút bấm để quay
    [SerializeField] Button adsButton;
    [SerializeField] List<GiftScript> prizes = new List<GiftScript>(); // Danh sách phần thưởng
    [SerializeField] Image resultImage; // Hình ảnh hiển thị kết quả
    [SerializeField] GameObject resultPanel; // Panel hiển thị kết quả
    [SerializeField] Button closeButton; // Nút đóng kết quả
    [SerializeField] TMP_Text valueText;
    [SerializeField] Sprite moneySprite;
    private bool isSpinning = false; // Kiểm tra vòng quay có đang hoạt động không

    void Start()
    {
        spinButton.onClick.AddListener(SpinWheel);
        adsButton.onClick.AddListener(watchAds);
        resultPanel.SetActive(false);
    }

    public void watchAds()
    {
        // Show ad

        //wheel spin
        if (!isSpinning)
        {
            StartCoroutine(SpinTheWheel());
        }
    }

    public void SpinWheel()
    {
        if (!isSpinning)
        {
            StartCoroutine(SpinTheWheel());
        }
    }

    private IEnumerator SpinTheWheel()
    {
        isSpinning = true;
        spinButton.interactable = false; // Vô hiệu hóa nút trong khi quay

        float totalDuration = 5f; // Thời gian tổng cho toàn bộ vòng quay
        float slowDownDuration = 2f; // Thời gian giảm tốc
        float fastSpeed = Random.Range(1000f, 1360f); // Tốc độ ban đầu
        float angle = 0f;

        // Giai đoạn 1: Quay nhanh
        float elapsed = 0f;
        while (elapsed < totalDuration - slowDownDuration)
        {
            angle += fastSpeed * Time.deltaTime;
            wheel.localEulerAngles = new Vector3(0, 0, -angle);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Giai đoạn 2: Giảm tốc để dừng chính xác
        float targetAngle = (360f / prizes.Count) * GetRandomSegment();
        float currentSpeed = fastSpeed;
        float deltaAngle = Mathf.Infinity;
        while (currentSpeed > 180)
        {
            angle += currentSpeed * Time.deltaTime;
            currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * 2); // Giảm tốc mượt
            wheel.localEulerAngles = new Vector3(0, 0, -angle);
            yield return null;
        }
        float currentAngle = Mathf.Repeat(-wheel.localEulerAngles.z, 360);
        deltaAngle = Mathf.DeltaAngle(currentAngle, targetAngle);
        Debug.Log("deltaAngle: " + Mathf.Abs(deltaAngle));
        //// Điều chỉnh lại góc để dừng chính xác tại phần tử được chọn
        while (Mathf.Abs(deltaAngle) > 2f)
        {
            angle += currentSpeed * Time.deltaTime;
            currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * 0.12f);
            wheel.localEulerAngles = new Vector3(0, 0, -angle);
            currentAngle = Mathf.Repeat(-wheel.localEulerAngles.z, 360);
            deltaAngle = Mathf.DeltaAngle(currentAngle, targetAngle);
            yield return null;
        }
        wheel.localEulerAngles = new Vector3(0, 0, -targetAngle);
        //// Hiển thị kết quả
        int selectedSegment = Mathf.RoundToInt(targetAngle / (360f / prizes.Count)) % prizes.Count;
        selectedSegment = prizes.Count - selectedSegment;
        if (selectedSegment == prizes.Count)
            selectedSegment = 0;
        Debug.Log("Result: " + prizes[selectedSegment].value);
        LoadResult(prizes[selectedSegment]);
        // Kết thúc vòng quay
        isSpinning = false;
        spinButton.interactable = true; // Kích hoạt lại nút
    }


    private int GetRandomSegment()
    {
        // Tính tổng tỷ lệ
        float totalProbability = 0f;
        foreach (var prize in prizes)
        {
            totalProbability += prize.ratio;
        }

        // Tạo một giá trị ngẫu nhiên trong khoảng 0 đến tổng tỷ lệ
        float randomPoint = Random.Range(0f, totalProbability);

        // Tìm phần tử phù hợp với giá trị ngẫu nhiên
        float current = 0f;
        for (int i = 0; i < prizes.Count; i++)
        {
            current += prizes[i].ratio;
            if (randomPoint < current)
                return i;
        }

        return prizes.Count - 1; // Mặc định trả về phần tử cuối nếu lỗi
    }

    private void LoadResult(GiftScript gift)
    {
        AudioManager.Instance.PlaySound("SpinEnd");
        resultPanel.SetActive(true);
        if (gift.giftType == GiftScript.GiftType.Skin)
        {
            int skinID = gift.GetSkinID();
            foreach (Character skin in gift.skins)
            {
                if (skinID == skin.ID)
                    resultImage.sprite = skin.CharacterIcon;
            }
            valueText.gameObject.SetActive(false);
            bool isOwned = SkinsManager.instance.CheckOwnedCharacter(skinID);
            if (isOwned)
            {
                gift = prizes[1];
                closeButton.onClick.AddListener(() => LoadResult(gift));
                return;
            }
            SkinsManager.instance.UnlockCharacter(skinID);
        }
        if (gift.giftType == GiftScript.GiftType.Money)
        {
            resultImage.sprite = moneySprite;
            valueText.text = "+" + gift.value.ToString();
            GameManager.Instance.money += gift.value;
            SaveData saveData = new SaveData();
            saveData.Save();
        }
        closeButton.onClick.AddListener(() => resultPanel.SetActive(false));
        
    }
    private void closeResult()
    {
        resultPanel.SetActive(false);
        AudioManager.Instance.PlaySound("MenuClose");
    }
}

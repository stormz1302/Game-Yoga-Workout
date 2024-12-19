using UnityEngine;

public class IconRenderer : MonoBehaviour
{
    public Camera renderCamera; // Camera để render model
    public RenderTexture renderTexture; // RenderTexture để lưu hình ảnh
    public Transform modelPosition; // Vị trí đặt model khi render
    string savePath = Application.dataPath + "/Images/"; // Đường dẫn lưu icon
    public GameObject[] models; // Các đối tượng trong scene

    void Start()
    {
        // Kích hoạt camera trước khi bắt đầu
        renderCamera.gameObject.SetActive(true);

        foreach (GameObject model in models)
        {
            RenderModel(model, model.name);
        }

        // Tắt camera sau khi render xong
        renderCamera.gameObject.SetActive(false);
    }

    public void RenderModel(GameObject model, string iconName)
    {
        // Lưu trạng thái ban đầu
        Vector3 originalPosition = model.transform.position;
        Quaternion originalRotation = model.transform.rotation;
        bool originalActiveState = model.activeSelf;

        // Đặt model vào vị trí render và kích hoạt nó
        model.transform.position = modelPosition.position;
        model.transform.rotation = Quaternion.identity;
        model.SetActive(true);

        // Kích hoạt camera để render
        renderCamera.targetTexture = renderTexture;
        renderCamera.Render();

        // Lưu RenderTexture thành Texture2D
        SaveRenderTextureAsPNG(renderTexture, savePath + iconName + ".png");

        // Khôi phục trạng thái ban đầu của model
        model.transform.position = originalPosition;
        model.transform.rotation = originalRotation;
        model.SetActive(false);

        renderCamera.targetTexture = null;
    }

    private void SaveRenderTextureAsPNG(RenderTexture rt, string path)
    {
        // Kiểm tra và tạo thư mục nếu cần
        string directoryPath = System.IO.Path.GetDirectoryName(path);
        if (!System.IO.Directory.Exists(directoryPath))
        {
            System.IO.Directory.CreateDirectory(directoryPath);
        }

        // Chuyển RenderTexture thành Texture2D và lưu
        Texture2D texture = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);

        RenderTexture.active = rt;
        texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        texture.Apply();
        RenderTexture.active = null;

        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, bytes);
    }
}

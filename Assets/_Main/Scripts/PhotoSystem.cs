using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PhotoSystem : MonoBehaviour
{
    [Header("UI")]
    public GameObject gameUI;
    public GameObject cameraUI;
    public Image flashImage;

    [Header("Post Processing")]
    public Volume volume;
    private DepthOfField dof;

    [Header("Camera")]
    public CinemachineVirtualCamera virtualCamera;
    private Camera mainCam;

    [Header("FOV")]
    public float zoomSpeed = 30f;
    public float minFov = 20f;
    public float maxFov = 60f;

    bool isPhotoMode = false;
    bool isTakingPhoto = false;

    void Start()
    {
        cameraUI.SetActive(false);
        flashImage.color = new Color(1, 1, 1, 0);

        volume.profile.TryGet(out dof);

        mainCam = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TogglePhotoMode();
        }

        if (!isPhotoMode) return;

        if (Input.GetKeyDown(KeyCode.Space) && !isTakingPhoto)
        {
            StartCoroutine(TakePhoto());
        }

        FOVControl();
    }

    void TogglePhotoMode()
    {
        isPhotoMode = !isPhotoMode;

        gameUI.SetActive(!isPhotoMode);
        cameraUI.SetActive(isPhotoMode);

        if (isPhotoMode)
        {
            StartCoroutine(CameraSwitchFocus());
        }
    }

    IEnumerator CameraSwitchFocus()
    {
        if (dof == null) yield break;

        // 等待虚拟相机切换完成
        yield return new WaitForSeconds(2f);

        // 初始化焦距
        dof.focalLength.value = 300f;

        // 等待1.5秒
        yield return new WaitForSeconds(1.5f);

        // 恢复
        dof.focalLength.value = 1f;
    }

    IEnumerator TakePhoto()
    {
        isTakingPhoto = true;

        gameUI.SetActive(false);
        cameraUI.SetActive(false);

        yield return new WaitForEndOfFrame();

        Texture2D screen = ScreenCapture.CaptureScreenshotAsTexture();

        SavePhoto(screen);

        yield return StartCoroutine(FlashEffect());

        if (isPhotoMode)
            cameraUI.SetActive(true);

        isTakingPhoto = false;
    }

    void SavePhoto(Texture2D tex)
    {
        byte[] bytes = tex.EncodeToPNG();

        string folder = Application.dataPath + "/Photos/";

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string fileName = "photo_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";

        File.WriteAllBytes(folder + fileName, bytes);

        Debug.Log("照片保存: " + folder + fileName);
    }

    IEnumerator FlashEffect()
    {
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * 8;
            flashImage.color = new Color(1, 1, 1, t);
            yield return null;
        }

        t = 1;

        while (t > 0)
        {
            t -= Time.deltaTime * 4;
            flashImage.color = new Color(1, 1, 1, t);
            yield return null;
        }

        flashImage.color = new Color(1, 1, 1, 0);
    }

    void FOVControl()
    {
        float fov = virtualCamera.m_Lens.FieldOfView;

        if (Input.GetKey(KeyCode.A))
        {
            fov += zoomSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            fov -= zoomSpeed * Time.deltaTime;
        }

        fov = Mathf.Clamp(fov, minFov, maxFov);

        virtualCamera.m_Lens.FieldOfView = fov;
    }
}
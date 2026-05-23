using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraStuff : MonoBehaviour
{
    [Header("Displays")]
    [SerializeField] private RawImage cameraDisplay;
    [SerializeField] private RawImage thumbnailDisplay;
    
    [Header("UI")]
    [SerializeField] private Button captureButton;
    
    [Header("Phone options")]
    [SerializeField] private bool useFrontCamera;
    
    //This is the list of where it is taking the camera
    private WebCamTexture _webCamTexture;

    private IEnumerator Start()
    {
        captureButton.onClick.AddListener(CapturePhoto);
        yield return RequestCameraPermission();
    }

    private void CapturePhoto()
    {
        if (_webCamTexture == null || !_webCamTexture.isPlaying) return;
        
        Texture2D photo = new Texture2D(_webCamTexture.width, _webCamTexture.height, TextureFormat.RGBA32, false);
        
        //We can change here the relation with the pixels
        photo.SetPixels(_webCamTexture.GetPixels());
        photo.Apply();
        
        thumbnailDisplay.texture = photo;
        thumbnailDisplay.gameObject.SetActive(true);

        SaveToGallery(photo);
    }

    private void SaveToGallery(Texture2D photo)
    {
        //Establish the name of the file and the path
        string filename = $"capture_{System.DateTime.Now:yyyyMMMMdd_HHmmss}.png";
        string path = System.IO.Path.Combine(Application.persistentDataPath, filename);
        System.IO.File.WriteAllBytes(path, photo.EncodeToPNG());

#if UNITY_ANDROID
        //find the path though URI and calls the action to locate it there -> Note, this is based on java, but it is not Java itself, it is only for unity. 
        using var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        using var activity = player.GetStatic<AndroidJavaObject>("currentActivity");
        using var intent = new AndroidJavaObject("android.content.Intent", "android.intent.action.MEDIA_SCANNER_SCAN_FILE");
        using var uri = new AndroidJavaClass("android.net.Uri").CallStatic<AndroidJavaObject>("parse", "file://" + path);
        intent.Call<AndroidJavaObject>("setData", uri);
        activity.Call("sendBroadcast", intent);
#endif //TEST THIS STUFF DUDE, CMON

    }

    //Ask for permission for the camera
    private IEnumerator RequestCameraPermission()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);

        if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            Debug.LogWarning("WebCam is not authorized");
            yield break;
        }

        InitializeCamera();
    }
    
    
    // Starts and assign the camera
    private void InitializeCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.LogWarning("No camera devices found");
        }

        string cameraName = null;
        foreach (WebCamDevice device in devices)
        {
            if (device.isFrontFacing == useFrontCamera)
            {
                cameraName = device.name;
                break;
            }
        }
        
        cameraName ??= devices[0].name; // we use the ??= to avoid null references
        //Syntaxis (name, width, Height, requested FPS)
        _webCamTexture = new WebCamTexture(cameraName, 1920, 1080, 30);
        
        cameraDisplay.texture = _webCamTexture;
        _webCamTexture.Play();

        StartCoroutine(AdjustDisplayAfterFrame());
    }

    private IEnumerator AdjustDisplayAfterFrame()
    {
        yield return null;
        
        //cameraDisplay.rectTransform.localEulerAngles = new Vector3(0, 0, -_webCamTexture.videoRotationAngle);

        if (useFrontCamera)
        {
            cameraDisplay.rectTransform.localScale = new Vector3(-1, 1, 1);
        }
    }
}

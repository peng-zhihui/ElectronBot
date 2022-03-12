using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeCameraFullScreen : MonoBehaviour
{
    private const string TAG = "pzh::Unity::";

    public Camera NativeViewCamera;

    private DeviceOrientation currentOrientation;
    private int previewHeight;
    private int previewWidth;
    private int screenHeight;
    private int screenWidth;


    void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

#if (UNITY_ANDROID && !UNITY_EDITOR)
        previewHeight = this.GetComponent<NativeCameraController>().previewHeight;
        previewWidth = this.GetComponent<NativeCameraController>().previewWidth;
#else
        previewHeight = 480;
        previewWidth = 640;
#endif

        //设置为正交
        NativeViewCamera.orthographic = true;

#if (UNITY_ANDROID && !UNITY_EDITOR)
        //前置的话需要设置镜像
        if (GetComponent<NativeCameraController>().cameraID == 1)
            transform.Find("TexturePlane").transform.localScale = new Vector3(-1, 1, 1);

        if (Screen.orientation == ScreenOrientation.LandscapeLeft)
        {
            currentOrientation = DeviceOrientation.LandscapeLeft;

            screenWidth = Screen.width;
            screenHeight = Screen.height;
            gameObject.transform.rotation = Quaternion.AngleAxis(0, new Vector3(0, 0, -1));

            //设置大小为屏幕宽度的一半 
            NativeViewCamera.orthographicSize = screenHeight / 2 * 0.1f;

            if ((float)screenWidth / screenHeight > (float)previewWidth / previewHeight)
            {
                gameObject.transform.localScale = new Vector3(-screenWidth * 0.01f
                    , (float)screenWidth * previewHeight / previewWidth / screenHeight
                    * screenHeight * 0.01f, 1);
            }
            else
            {
                gameObject.transform.localScale = new Vector3(-screenWidth * 0.01f
                   * (float)screenHeight * previewWidth / previewHeight / screenWidth
                   , screenHeight * 0.01f, 1);
            }
        }
        else if (Screen.orientation == ScreenOrientation.Portrait)
        {
            currentOrientation = DeviceOrientation.Portrait;

            screenWidth = Screen.height;
            screenHeight = Screen.width;
            gameObject.transform.rotation = Quaternion.AngleAxis(90, new Vector3(0, 0, -1));

            //设置大小为屏幕宽度的一半 
            NativeViewCamera.orthographicSize = screenWidth / 2 * 0.1f;

            if ((float)screenWidth / screenHeight > (float)previewWidth / previewHeight)
            {
                gameObject.transform.localScale = new Vector3(-screenWidth * 0.01f
                    , (float)screenWidth * previewHeight / previewWidth / screenHeight
                    * screenHeight * 0.01f, 1);
            }
            else
            {
                gameObject.transform.localScale = new Vector3(-screenWidth * 0.01f
                   * (float)screenHeight * previewWidth / previewHeight / screenWidth
                   , screenHeight * 0.01f, 1);
            }
        }
#else
        {
            currentOrientation = DeviceOrientation.LandscapeLeft;

            screenWidth = previewWidth; // Screen.width;
            screenHeight = previewHeight; //Screen.height;
            gameObject.transform.rotation = Quaternion.AngleAxis(0, new Vector3(0, 0, -1));

            //设置大小为屏幕宽度的一半 
            NativeViewCamera.orthographicSize = screenHeight / 2 * 0.1f;

            if ((float) screenWidth / screenHeight > (float) previewWidth / previewHeight)
            {
                gameObject.transform.localScale = new Vector3(-screenWidth * 0.01f
                    , (float) screenWidth * previewHeight / previewWidth / screenHeight
                      * screenHeight * 0.01f, 1);
            }
            else
            {
                gameObject.transform.localScale = new Vector3(-screenWidth * 0.01f
                                                                           * (float) screenHeight * previewWidth /
                                                              previewHeight / screenWidth
                    , screenHeight * 0.01f, 1);
            }
        }
#endif

        Debug.Log(TAG + "Screen width:" + screenWidth + " Screen height:" + screenHeight);
    }


    void Start()
    {
#if (UNITY_ANDROID && !UNITY_EDITOR)
        if (Screen.orientation == ScreenOrientation.LandscapeLeft)
            AndroidBridgeManager.GetComponent<AndroidBridge>().CallAndroidForBool("LandscapeLeft");
        else if (Screen.orientation == ScreenOrientation.Portrait)
            AndroidBridgeManager.GetComponent<AndroidBridge>().CallAndroidForBool("Portrait");
#endif
    }

    void Update()
    {
        if (Input.deviceOrientation != currentOrientation)
        {
            if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft)
            {
                currentOrientation = DeviceOrientation.LandscapeLeft;

                gameObject.transform.rotation = Quaternion.AngleAxis(0, new Vector3(0, 0, -1));

                //设置大小为屏幕宽度的一半 
                NativeViewCamera.orthographicSize = screenHeight / 2 * 0.1f;

                if ((float) screenWidth / screenHeight > (float) previewWidth / previewHeight)
                {
                    gameObject.transform.localScale = new Vector3(-screenWidth * 0.01f
                        , (float) screenWidth * previewHeight / previewWidth / screenHeight
                          * screenHeight * 0.01f, 1);
                }
                else
                {
                    gameObject.transform.localScale = new Vector3(-screenWidth * 0.01f
                                                                               * (float) screenHeight * previewWidth /
                                                                  previewHeight / screenWidth
                        , screenHeight * 0.01f, 1);
                }

#if (UNITY_ANDROID && !UNITY_EDITOR)
                AndroidBridgeManager.GetComponent<AndroidBridge>().CallAndroidForBool("LandscapeLeft");
#endif
            }
            else if (Input.deviceOrientation == DeviceOrientation.Portrait)
            {
                currentOrientation = DeviceOrientation.Portrait;

                gameObject.transform.rotation = Quaternion.AngleAxis(90, new Vector3(0, 0, -1));

                //设置大小为屏幕宽度的一半 
                NativeViewCamera.orthographicSize = screenWidth / 2 * 0.1f;

                if ((float) screenWidth / screenHeight > (float) previewWidth / previewHeight)
                {
                    gameObject.transform.localScale = new Vector3(-screenWidth * 0.01f
                        , (float) screenWidth * previewHeight / previewWidth / screenHeight
                          * screenHeight * 0.01f, 1);
                }
                else
                {
                    gameObject.transform.localScale = new Vector3(-screenWidth * 0.01f
                                                                               * (float) screenHeight * previewWidth /
                                                                  previewHeight / screenWidth
                        , screenHeight * 0.01f, 1);
                }

#if (UNITY_ANDROID && !UNITY_EDITOR)
                AndroidBridgeManager.GetComponent<AndroidBridge>().CallAndroidForBool("Portrait");
#endif
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }
}
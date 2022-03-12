using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class ScrollMechanic : MonoBehaviour, IDropHandler, IDragHandler, IBeginDragHandler, IPointerExitHandler,
    IPointerEnterHandler
{
    public GameObject deltaTime;

    [Header("Test variables")] public bool initTest; //Test initialization
    public bool isInfinite; //Is infinite scrolling (Required initialization)
    public string[] testData; //Test data

    [Header("Text prefab")] public GameObject templateValues;

    [Header("Required objects")] public Camera camera; //Main camera
    public RectTransform targetCanvas; //Target canvas

    public RectTransform contentTarget; //Target content
    public AutoSizeLayout contentSize; //My own layout group script. You could use it instead of default layout group

    [Header("Settings")] [Space(20)] public float heightTemplate = 27; //Height of template rect texts

    public AnimationCurve curve; //Curve for controlling "Shape" of scroll
    public AnimationCurve curveShift; //Curve for controlling text offset

    public float speedLerp = 5; //Speed of concentrating
    public float minVelocity = 0.2f; //Minimun inertion value to start concentrating

    public float shiftUp = 32; //Offset of upper texts
    public float shiftDown = 32; //Offset of lower texts
    public float padding = 0; //Spacing from upper and lower borders
    [Range(0, 1)] public float colorPad = 0.115f; //Padding of text color
    public float maxFontSize = 48.2f; //Maximun font size

    public bool isElastic = true; //Is elastic movement
    public float maxElastic = 50; //Maximun elasity distance

    public float inertiaSense = 4; //Inertia sensibility

    [Header("Mouse Wheel and Touchpad scroll methods")]
    public bool isCanUseMouseWheel;

    public bool isInvertMouseWheel;
    public float mouseWheelSensibility = 0.5f;
    public float touchpadSensibility = 0.5f;

    bool isDragging;
    float inertia;

    float startPosContent;
    float startPosMouse;
    float middle;
    float heightText = 27;

    int countCheck = 4;

    int currentCenter;

    bool isInitialized;

    int countTotal;

    int padCount;

    float _padScroll;

    public float MouseScroll
    {
        get
        {
            float mouseScroll = Input.mouseScrollDelta.y;

            if (mouseScroll != 0)
                return mouseScroll;
            else
                return _padScroll;
        }
    }

    //Get TrackPad Scroll
    void OnGUI()
    {
        if (Event.current.type == EventType.ScrollWheel)
            _padScroll = (-Event.current.delta.y / 10) * touchpadSensibility;
        else
            _padScroll = 0;
    }

    private void Start()
    {
        heightText = heightTemplate / 2;
        middle = GetComponent<RectTransform>().sizeDelta.y / 2;
        contentSize.topPad = middle - heightText;
        contentSize.bottomPad = middle - heightText;
        countCheck = Mathf.CeilToInt((middle * 2) / heightTemplate);
    }

    /// <summary>
    /// Initialization method
    /// </summary>
    /// <param name="dataToInit"> List of texts to show </param>
    /// <param name="isInfinite"> Is scroll will be infinite </param>
    /// <param name="firstTarget"> Which text in list will be first </param>
    public void Initialize(List<string> dataToInit, bool isInfinite = false, int firstTarget = 0)
    {
        countTotal = dataToInit.Count;
        for (int i = 0; i < contentTarget.childCount; i++)
        {
            Destroy(contentTarget.GetChild(i).gameObject);
        }

        this.isInfinite = isInfinite;

        if (isInfinite)
        {
            int half = (int) (countCheck / 2) + 1;

            if (dataToInit.Count > half)
            {
                padCount = half;
                for (int i = dataToInit.Count - half; i < dataToInit.Count; i++)
                {
                    var textComponent = Instantiate(templateValues, contentTarget.transform).transform.GetChild(0)
                        .GetComponent<TextMeshProUGUI>();
                    textComponent.text = dataToInit[i];
                    textComponent.transform.parent.name = i + "";
                    textComponent.transform.parent.GetComponent<RectTransform>().sizeDelta =
                        new Vector2(GetComponent<RectTransform>().sizeDelta.x, heightTemplate);
                }
            }
            else
            {
                padCount = dataToInit.Count;
                for (int j = 0; j < Mathf.CeilToInt((float) half / (float) dataToInit.Count); j++)
                {
                    for (int i = 0; i < dataToInit.Count; i++)
                    {
                        var textComponent = Instantiate(templateValues, contentTarget.transform).transform.GetChild(0)
                            .GetComponent<TextMeshProUGUI>();
                        textComponent.text = dataToInit[i];
                        textComponent.transform.parent.name = i + "";
                        textComponent.transform.parent.GetComponent<RectTransform>().sizeDelta =
                            new Vector2(GetComponent<RectTransform>().sizeDelta.x, heightTemplate);
                    }
                }
            }

            isElastic = false;
            contentTarget.anchoredPosition = new Vector2(0, (firstTarget + padCount) * (heightText * 2));
        }
        else
        {
            padCount = (int) (countCheck / 2) + 1;
            contentTarget.anchoredPosition = new Vector2(0, firstTarget * (heightText * 2));
        }

        for (int i = 0; i < dataToInit.Count; i++)
        {
            var textComponent = Instantiate(templateValues, contentTarget.transform).transform.GetChild(0)
                .GetComponent<TextMeshProUGUI>();
            textComponent.text = dataToInit[i];
            textComponent.transform.parent.name = i + "";
            textComponent.transform.parent.GetComponent<RectTransform>().sizeDelta =
                new Vector2(GetComponent<RectTransform>().sizeDelta.x, heightTemplate);
        }

        if (isInfinite)
        {
            int half = (int) (countCheck / 2) + 1;
            if (dataToInit.Count > half)
            {
                for (int i = 0; i < half; i++)
                {
                    var textComponent = Instantiate(templateValues, contentTarget.transform).transform.GetChild(0)
                        .GetComponent<TextMeshProUGUI>();
                    textComponent.text = dataToInit[i];
                    textComponent.transform.parent.name = i + "";
                    textComponent.transform.parent.GetComponent<RectTransform>().sizeDelta =
                        new Vector2(GetComponent<RectTransform>().sizeDelta.x, heightTemplate);
                }
            }
            else
            {
                for (int j = 0; j < Mathf.CeilToInt((float) half / (float) dataToInit.Count); j++)
                {
                    for (int i = 0; i < dataToInit.Count; i++)
                    {
                        var textComponent = Instantiate(templateValues, contentTarget.transform).transform.GetChild(0)
                            .GetComponent<TextMeshProUGUI>();
                        textComponent.text = dataToInit[i];
                        textComponent.transform.parent.name = i + "";
                        textComponent.transform.parent.GetComponent<RectTransform>().sizeDelta =
                            new Vector2(GetComponent<RectTransform>().sizeDelta.x, heightTemplate);
                    }
                }
            }
        }

        contentSize.UpdateLayout();
        isInitialized = true;
    }

    /// <summary>
    /// Return list ID of current concentration
    /// </summary>
    /// <returns></returns>
    public int GetCurrentValue()
    {
        return int.Parse(contentTarget.GetChild(currentCenter).name);
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isCanUseMouseWheel && isInArea && Input.mouseScrollDelta.y != 0)
        {
            isDragging = true;
        }
        else if (!Input.GetMouseButton(0))
        {
            isDragging = false;
        }

        if (initTest)
        {
            initTest = false;
            var newList = new List<string>();
            for (int i = 0; i < testData.Length; i++)
            {
                newList.Add(testData[i]);
            }

            Initialize(newList, isInfinite);
        }

        if (isInitialized)
        {
            if (!isDragging)
            {
                if (contentTarget.anchoredPosition.y + inertia < 0)
                {
                    if (isElastic)
                    {
                        contentTarget.anchoredPosition = new Vector2(0, contentTarget.anchoredPosition.y + inertia);
                        inertia = inertia * Mathf.Clamp(1 - Mathf.Abs(contentTarget.anchoredPosition.y) /
                            maxElastic, 0, 1);
                    }
                    else
                    {
                        contentTarget.anchoredPosition = new Vector2(0, 0);
                        inertia = 0;
                    }
                }
                else if (contentTarget.anchoredPosition.y + inertia > contentTarget.sizeDelta.y - middle * 2)
                {
                    if (isElastic)
                    {
                        contentTarget.anchoredPosition = new Vector2(0, contentTarget.anchoredPosition.y + inertia);
                        inertia = inertia * Mathf.Clamp(1 - Mathf.Abs((contentTarget.sizeDelta.y - middle * 2) -
                                                                      contentTarget.anchoredPosition.y) /
                            maxElastic, 0, 1);
                    }
                    else
                    {
                        contentTarget.anchoredPosition = new Vector2(0, contentTarget.sizeDelta.y - middle * 2);
                        inertia = 0;
                    }
                }
                else
                {
                    contentTarget.anchoredPosition = new Vector2(0, contentTarget.anchoredPosition.y + inertia);
                    inertia = Mathf.Lerp(inertia, 0, inertiaSense * Time.deltaTime);
                }
            }
            else
            {
                if (isCanUseMouseWheel && isInArea && MouseScroll != 0)
                {
                    if (isElastic)
                    {
                        if (contentTarget.anchoredPosition.y < 0)
                        {
                            inertia = 0;
                            contentTarget.anchoredPosition = new Vector2(0,
                                contentTarget.anchoredPosition.y +
                                ((isInvertMouseWheel ? -1 : 1) * MouseScroll * mouseWheelSensibility)
                                * Mathf.Clamp(1 - Mathf.Abs(contentTarget.anchoredPosition.y) /
                                    maxElastic, 0, 1));
                        }
                        else if (contentTarget.anchoredPosition.y > contentTarget.sizeDelta.y - middle * 2)
                        {
                            inertia = 0;
                            contentTarget.anchoredPosition = new Vector2(0,
                                contentTarget.anchoredPosition.y +
                                ((isInvertMouseWheel ? -1 : 1) * MouseScroll * mouseWheelSensibility)
                                * Mathf.Clamp(1 - Mathf.Abs((contentTarget.sizeDelta.y - middle * 2) -
                                                            contentTarget.anchoredPosition.y) /
                                    maxElastic, 0, 1));
                        }
                        else
                        {
                            inertia += ((isInvertMouseWheel ? -1 : 1) * MouseScroll
                                                                      * mouseWheelSensibility);
                            contentTarget.anchoredPosition = new Vector2(0,
                                contentTarget.anchoredPosition.y +
                                ((isInvertMouseWheel ? -1 : 1) * MouseScroll * mouseWheelSensibility));
                        }
                    }
                    else
                    {
                        inertia += ((isInvertMouseWheel ? -1 : 1) * MouseScroll
                                                                  * mouseWheelSensibility);
                        contentTarget.anchoredPosition = new Vector2(0, Mathf.Clamp(
                            contentTarget.anchoredPosition.y +
                            ((isInvertMouseWheel ? -1 : 1) * MouseScroll * mouseWheelSensibility),
                            0, contentTarget.sizeDelta.y - middle * 2));
                    }
                }
                else
                {
                    if (isElastic)
                    {
                        if (contentTarget.anchoredPosition.y < 0)
                        {
                            inertia = 0;
                            contentTarget.anchoredPosition = new Vector2(0,
                                startPosContent + (-startPosMouse + (Input.mousePosition.y / camera.pixelHeight)
                                    * targetCanvas.sizeDelta.y) * Mathf.Clamp(1 -
                                                                              Mathf.Abs(
                                                                                  contentTarget.anchoredPosition.y) /
                                                                              maxElastic, 0, 1));
                        }
                        else if (contentTarget.anchoredPosition.y > contentTarget.sizeDelta.y - middle * 2)
                        {
                            inertia = 0;
                            contentTarget.anchoredPosition = new Vector2(0,
                                startPosContent + (-startPosMouse + (Input.mousePosition.y / camera.pixelHeight)
                                    * targetCanvas.sizeDelta.y) * Mathf.Clamp(1 - Mathf.Abs(
                                        (contentTarget.sizeDelta.y - middle * 2) -
                                        contentTarget.anchoredPosition.y) /
                                    maxElastic, 0, 1));
                        }
                        else
                        {
                            inertia = startPosContent + (-startPosMouse +
                                                         (Input.mousePosition.y / camera.pixelHeight) *
                                                         targetCanvas.sizeDelta.y) -
                                      contentTarget.anchoredPosition.y;
                            contentTarget.anchoredPosition = new Vector2(0,
                                startPosContent + (-startPosMouse + (Input.mousePosition.y /
                                                                     camera.pixelHeight) * targetCanvas.sizeDelta.y));
                        }

                        startPosMouse = (Input.mousePosition.y / camera.pixelHeight) * targetCanvas.sizeDelta.y;
                        startPosContent = contentTarget.anchoredPosition.y;
                    }
                    else
                    {
                        inertia = startPosContent + (-startPosMouse +
                                                     (Input.mousePosition.y / camera.pixelHeight) *
                                                     targetCanvas.sizeDelta.y) -
                                  contentTarget.anchoredPosition.y;
                        contentTarget.anchoredPosition = new Vector2(0, Mathf.Clamp(
                            startPosContent + (-startPosMouse + (Input.mousePosition.y /
                                                                 camera.pixelHeight) * targetCanvas.sizeDelta.y), 0,
                            contentTarget.sizeDelta.y - middle * 2));
                    }
                }
            }

            if (isInfinite)
            {
                if (contentTarget.anchoredPosition.y < middle)
                {
                    contentTarget.anchoredPosition = new Vector2(0, contentTarget.anchoredPosition.y +
                                                                    (padCount + (countTotal - padCount)) *
                                                                    (heightText * 2));
                    for (int i = 0; i < (padCount + (countTotal - padCount)); i++)
                    {
                        contentTarget.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().fontSize = 0;
                    }

                    startPosMouse = (Input.mousePosition.y / camera.pixelHeight) * targetCanvas.sizeDelta.y;
                    startPosContent = contentTarget.anchoredPosition.y;
                }
                else if (contentTarget.anchoredPosition.y > contentTarget.sizeDelta.y - middle * 3)
                {
                    contentTarget.anchoredPosition = new Vector2(0, contentTarget.anchoredPosition.y -
                                                                    (padCount + (countTotal - padCount)) *
                                                                    (heightText * 2));
                    for (int i = contentTarget.childCount - 1;
                         i >= contentTarget.childCount -
                         (padCount + (countTotal - padCount));
                         i--)
                    {
                        contentTarget.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().fontSize = 0;
                    }

                    startPosMouse = (Input.mousePosition.y / camera.pixelHeight) * targetCanvas.sizeDelta.y;
                    startPosContent = contentTarget.anchoredPosition.y;
                }
            }

            float contentPos = contentTarget.anchoredPosition.y;

            int startPoint = Mathf.CeilToInt((contentPos - (middle + heightText)) / (heightText * 2));
            int minID = Mathf.Max(0, startPoint);
            int maxID = Mathf.Min(contentTarget.transform.childCount, startPoint + countCheck + 1);
            minID = Mathf.Clamp(minID, 0, int.MaxValue);
            maxID = Mathf.Clamp(maxID, 0, int.MaxValue);
            /*currentCenter = Mathf.Clamp(Mathf.RoundToInt((contentPos - (middle + heightText)) / (heightText * 2)) +
                padCount, 0, contentTarget.childCount - 1);*/

            currentCenter = Mathf.Clamp(Mathf.RoundToInt(contentPos / (heightText * 2)), 0,
                contentTarget.childCount - 1);

            if (maxID > minID)
            {
                for (int i = minID; i < maxID; i++)
                {
                    var currentRect = contentTarget.transform.GetChild(i).GetComponent<RectTransform>();
                    var currentText = contentTarget.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>();
                    float ratio =
                        Mathf.Clamp(
                            1 - Mathf.Abs(contentPos + currentRect.anchoredPosition.y + middle) / (middle - padding), 0,
                            1);
                    if (contentPos + currentRect.anchoredPosition.y + middle > 0)
                    {
                        currentText.GetComponent<RectTransform>().anchoredPosition =
                            new Vector2(0, -curveShift.Evaluate(1 - ratio) * shiftUp);
                    }
                    else
                    {
                        currentText.GetComponent<RectTransform>().anchoredPosition =
                            new Vector2(0, curveShift.Evaluate(1 - ratio) * shiftDown);
                    }

                    currentText.fontSize = maxFontSize * curve.Evaluate(ratio);
                    currentText.color = new Vector4(currentText.color.r, currentText.color.g, currentText.color.b,
                        Mathf.Clamp((ratio - colorPad) / (1 - colorPad), 0, 1));
                }
            }

            if (Mathf.Abs(inertia) < minVelocity && !Input.GetMouseButton(0))
            {
                inertia = 0;
                contentTarget.anchoredPosition = new Vector2(0,
                    Mathf.Lerp(contentTarget.anchoredPosition.y,
                        -contentTarget.transform.GetChild(currentCenter).GetComponent<RectTransform>().anchoredPosition
                            .y - middle, speedLerp * Time.deltaTime));

                OnValueChanged(currentCenter);
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        isDragging = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        startPosMouse = (Input.mousePosition.y / camera.pixelHeight) * targetCanvas.sizeDelta.y;
        startPosContent = contentTarget.anchoredPosition.y;
    }

    bool isInArea;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isInArea = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isInArea = false;
    }

    public void OnValueChanged(int id)
    {
        float[] deltaTimes = new float[testData.Length];
        deltaTimes[0] = 0.1f;
        deltaTimes[1] = 0.2f;
        deltaTimes[2] = 0.5f;
        deltaTimes[3] = 1.0f;
        deltaTimes[4] = 2.0f;
        deltaTimes[5] = 5.0f;
        deltaTime.GetComponent<PlayButtonBehavior>().deltaTime = deltaTimes[id];
    }
}
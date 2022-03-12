using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

public class RobotController : MonoBehaviour
{
    public float slerpRatio = 0.5f;
    public int syncMode = 1;

    public Transform armRollLeft;
    public Transform armPitchLeft;
    public Transform armRollRight;
    public Transform armPitchRight;
    public Transform head;
    public Transform body;

    public float targetAngleArmRollLeft;
    public float targetAngleArmPitchLeft;
    public float targetAngleArmRollRight;
    public float targetAngleArmPitchRight;
    public float targetAngleHead;
    public float targetAngleBody;
    public float backLight;


    public CenteredSlider sliderAngleArmRollLeft;
    public CenteredSlider sliderAngleArmPitchLeft;
    public CenteredSlider sliderAngleArmRollRight;
    public CenteredSlider sliderAngleArmPitchRight;
    public CenteredSlider sliderAngleHead;
    public CenteredSlider sliderAngleBody;
    public Transform sliderCover;

    public int requestFrame = -1;
    public int currentFrame = -1;
    public bool isPlaying = false;

    public UnityGetImageFromCpp cvManager;


    // Start is called before the first frame update
    void Start()
    {
        targetAngleArmRollLeft = 0;
        targetAngleArmPitchLeft = 0;
        targetAngleArmRollRight = 0;
        targetAngleArmPitchRight = 0;
        targetAngleHead = 0;
        targetAngleBody = 0;
        backLight = 1;
    }

    // Update is called once per frame
    void Update()
    {
        armRollLeft.localRotation = Quaternion.Slerp(armRollLeft.localRotation,
            Quaternion.Euler(0, 0, targetAngleArmRollLeft), slerpRatio);
        armPitchLeft.localRotation = Quaternion.Slerp(armPitchLeft.localRotation,
            Quaternion.Euler(targetAngleArmPitchLeft, 0, 0), slerpRatio);
        armRollRight.localRotation = Quaternion.Slerp(armRollRight.localRotation,
            Quaternion.Euler(0, 0, -targetAngleArmRollRight), slerpRatio);
        armPitchRight.localRotation = Quaternion.Slerp(armPitchRight.localRotation,
            Quaternion.Euler(targetAngleArmPitchRight, 0, 0), slerpRatio);
        body.localRotation = Quaternion.Slerp(body.localRotation,
            Quaternion.Euler(0, targetAngleBody, 0), slerpRatio);
        head.localRotation = Quaternion.Slerp(head.localRotation,
            Quaternion.Euler(targetAngleHead, 0, 0), slerpRatio);

        if (isPlaying)
        {
            sliderAngleBody.Value = (int) targetAngleBody;
            sliderAngleHead.Value = (int) targetAngleHead;
            sliderAngleArmRollLeft.Value = (int) targetAngleArmRollLeft;
            sliderAngleArmPitchLeft.Value = (int) targetAngleArmPitchLeft;
            sliderAngleArmRollRight.Value = (int) targetAngleArmRollRight;
            sliderAngleArmPitchRight.Value = (int) targetAngleArmPitchRight;
        }
    }

    private void FixedUpdate()
    {
        // 20Hz
        cvManager.FixUpdate();

        // Up to timeline process
        if (isPlaying && requestFrame != currentFrame)
        {
            currentFrame = requestFrame;
            cvManager.KeyFrameChangeUpdate();

            Debug.Log(">>>> " + currentFrame);
        }
    }

    public void SetAngleArmRollLeft(int _val)
    {
        targetAngleArmRollLeft = _val;
    }

    public void SetAngleArmPitchLeft(int _val)
    {
        targetAngleArmPitchLeft = _val;
    }

    public void SetAngleArmRollRight(int _val)
    {
        targetAngleArmRollRight = _val;
    }

    public void SetAngleArmPitchRight(int _val)
    {
        targetAngleArmPitchRight = _val;
    }

    public void SetAngleBody(int _val)
    {
        targetAngleBody = _val;
    }

    public void SetAngleHead(int _val)
    {
        targetAngleHead = _val;
    }


    public void ResetPose()
    {
        targetAngleArmRollLeft = 0;
        targetAngleArmPitchLeft = 0;
        targetAngleArmRollRight = 0;
        targetAngleArmPitchRight = 0;
        targetAngleHead = 0;
        targetAngleBody = 0;


        sliderAngleArmRollLeft.Value = 0;
        sliderAngleArmPitchLeft.Value = 0;
        sliderAngleArmRollRight.Value = 0;
        sliderAngleArmPitchRight.Value = 0;
        sliderAngleHead.Value = 0;
        sliderAngleBody.Value = 0;
    }


    public void OnSyncModeChanged(Slider _slider)
    {
        syncMode = (int) _slider.value;
        switch (syncMode)
        {
            case 0:
                _slider.transform.Find("Text (TMP)").GetComponent<TMP_Text>().text = "模型优先";
                sliderCover.gameObject.SetActive(false);
                break;
            case 1:
                _slider.transform.Find("Text (TMP)").GetComponent<TMP_Text>().text = "禁用同步";
                sliderCover.gameObject.SetActive(false);
                break;
            case 2:
                _slider.transform.Find("Text (TMP)").GetComponent<TMP_Text>().text = "实体优先";
                sliderCover.gameObject.SetActive(true);
                break;
        }
    }
}
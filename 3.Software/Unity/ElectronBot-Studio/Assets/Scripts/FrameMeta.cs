using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameMeta : MonoBehaviour
{
    public int id;
    public RenderTexture renderTexture;

    public float targetAngleArmRollLeft;
    public float targetAngleArmPitchLeft;
    public float targetAngleArmRollRight;
    public float targetAngleArmPitchRight;
    public float targetAngleHead;
    public float targetAngleBody;
    public float backLight;

    public string filePath;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}
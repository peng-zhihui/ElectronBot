using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class PlayButtonBehavior : MonoBehaviour
{
    public Sprite imgPlay;
    public Sprite imgPause;
    public Scrollbar timelineSb;
    public GameObject timelineFrameManager;
    public GameObject robot;
    public float deltaTime = 0.1f;

    private bool isPlaying = false;
    
    public 


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaying && timelineSb.GetComponent<Scrollbar>().value < 1)
        {
            int frameCount = timelineFrameManager.GetComponent<PoseEditor>().timelineFrames.Count;
            float totalTime = frameCount * deltaTime;

            timelineSb.GetComponent<Scrollbar>().value += (Time.deltaTime / totalTime);


            RobotController rc = robot.GetComponent<RobotController>();
            rc.slerpRatio = 0.01f / deltaTime; // need tuning
            int index = (int) (timelineSb.GetComponent<Scrollbar>().value * (frameCount - 1));
            FrameMeta meta = timelineFrameManager.GetComponent<PoseEditor>().timelineFrames[index]
                .GetComponent<FrameMeta>();
            rc.requestFrame = index;
            rc.targetAngleBody = meta.targetAngleBody;
            rc.targetAngleHead = meta.targetAngleHead;
            rc.targetAngleArmRollLeft = meta.targetAngleArmRollLeft;
            rc.targetAngleArmPitchLeft = meta.targetAngleArmPitchLeft;
            rc.targetAngleArmRollRight = meta.targetAngleArmRollRight;
            rc.targetAngleArmPitchRight = meta.targetAngleArmPitchRight;
            
            rc.isPlaying = true;
        }
        else
        {
            isPlaying = false;
            transform.Find("Icon").GetComponent<Image>().sprite = imgPlay;
            RobotController rc = robot.GetComponent<RobotController>();
            rc.slerpRatio = 0.5f * Time.deltaTime / 0.03f; // need tuning
            rc.isPlaying = false;
        }
    }

    public void OnClick()
    {
        isPlaying = !isPlaying;
        RobotController rc = robot.GetComponent<RobotController>();
        rc.isPlaying = isPlaying;

        if (isPlaying)
        {
            transform.Find("Icon").GetComponent<Image>().sprite = imgPause;
            timelineSb.GetComponent<Scrollbar>().value = 0;
        }
        else
        {
            transform.Find("Icon").GetComponent<Image>().sprite = imgPlay;
        }
    }

    public void OnDeltaTimeChanged(float _val)
    {
        Debug.Log(_val);
    }
}
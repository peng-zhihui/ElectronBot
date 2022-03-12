using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class AutoSizeLayout : MonoBehaviour
{
    public bool isLoopUpdate; //Is need to update layout in void Update

    public bool isVertical = true; //Is vertical
    public bool isResizeSelf = true; //Is need to resize self
          
    public float topPad; //Top padding
    public float bottomPad; //Bottom padding
    public float leftPad; //Left padding
    public float rightPad; //Right padding

    public float spacing; //Spacing between objects

    public int repeatFrames = 2; //How many frames it should update layout after first update

    private void Update() {
        if (isLoopUpdate) {
            UpdateLayout(false);
        }
    }

    public void UpdateLayout(bool isRepeat = true, bool isRecursive = false) {
        UpdateAllRect(isRecursive);
        if (isRepeat) {
            if(unpateRoutine != null) {
                StopCoroutine(unpateRoutine);
            }
            if (gameObject.activeInHierarchy) {
                unpateRoutine = StartCoroutine(UpdateRepeate(isRecursive));
            }
        }
    }

    void UpdateAllRect(bool isRecursive) {
        if (isVertical) {
            float sizeTotal = topPad;
            for (int i = 0; i < transform.childCount; i++) {
                if (transform.GetChild(i).tag != "NotInLayout" && transform.GetChild(i).gameObject.activeSelf) {
                    var rect = transform.GetChild(i).GetComponent<RectTransform>();
                    if (isRecursive) {
                        if (rect.GetComponent<AutoSizeLayout>()) {
                            rect.GetComponent<AutoSizeLayout>().UpdateLayout(isRecursive: true);
                        }
                    }
                    rect.anchoredPosition = new Vector2(leftPad - rightPad, -rect.sizeDelta.y * (1 - rect.pivot.y) - sizeTotal);
                    sizeTotal += rect.sizeDelta.y + spacing;
                }
            }
            sizeTotal -= spacing;
            sizeTotal += bottomPad;
            if (isResizeSelf) {
                GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, sizeTotal);
            }
        } else {
            float sizeTotal = leftPad;
            for (int i = 0; i < transform.childCount; i++) {
                if (transform.GetChild(i).tag != "NotInLayout" && transform.GetChild(i).gameObject.activeSelf) {
                    var rect = transform.GetChild(i).GetComponent<RectTransform>();
                    if (isRecursive) {
                        if (rect.GetComponent<AutoSizeLayout>()) {
                            rect.GetComponent<AutoSizeLayout>().UpdateLayout(isRecursive: true);
                        }
                    }
                    rect.anchoredPosition = new Vector2(rect.sizeDelta.x * (1 - rect.pivot.x) + sizeTotal, topPad - bottomPad);
                    sizeTotal += rect.sizeDelta.x + spacing;
                }
            }
            sizeTotal -= spacing;
            sizeTotal += rightPad;
            if (isResizeSelf) {
                GetComponent<RectTransform>().sizeDelta = new Vector2(sizeTotal, GetComponent<RectTransform>().sizeDelta.y);
            }
        }
    }

    Coroutine unpateRoutine;
    IEnumerator UpdateRepeate(bool isRecursive) {
        for(int i = 0; i < repeatFrames; i++) {
            yield return new WaitForEndOfFrame();
            UpdateAllRect(isRecursive);
        }
    }
}

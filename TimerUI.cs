using BepInEx;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TimerUI
{
    // private GameObject timerCanvas;
    // private Text timerText;
    GameObject textHolder;
    GameObject otherTextThing;
    Canvas timerCanvas;
    Text timerText;

    public TimerUI()
    {
        textHolder = new GameObject();
        timerCanvas = textHolder.AddComponent<Canvas>();
        timerCanvas.renderMode = UnityEngine.RenderMode.ScreenSpaceOverlay;

        textHolder.AddComponent<CanvasScaler>();
        textHolder.AddComponent<GraphicRaycaster>();

        otherTextThing = new GameObject();
        otherTextThing.transform.parent = textHolder.transform;

        timerText = otherTextThing.AddComponent<Text>();
        timerText.font = (Font)Resources.Load("Trajan Pro Regular");
        timerText.text = "test hello?";
        timerText.fontSize = 40;

        RectTransform rectTransform = timerText.GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(240, 0, 40);
        rectTransform.sizeDelta = new Vector2(50, 50);

        UnityEngine.Object.DontDestroyOnLoad(textHolder);
        UnityEngine.Object.DontDestroyOnLoad(otherTextThing);

        textHolder.SetActive(true);
        otherTextThing.SetActive(true);
    }

    public void setTime(string time)
    {
        timerText.text = time;
    }
}

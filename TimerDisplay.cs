using BepInEx;
using UnityEngine;
using UnityEngine.UI;

public class TimerDisplay
{
    private GameObject canvas;

    private GameObject textObject;
    private Text text;

    public TimerDisplay()
    {
        canvas = new GameObject("TimerModCanvas");
        canvas.AddComponent<UnityEngine.Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        canvas.AddComponent<GraphicRaycaster>();

        textObject = new GameObject("TimerModTimerText");
        textObject.AddComponent<CanvasRenderer>();
        RectTransform textTransform = textObject.AddComponent<RectTransform>();
        textTransform.sizeDelta = new Vector2(300, 400);

        CanvasGroup group = textObject.AddComponent<CanvasGroup>();
        group.interactable = false;
        group.blocksRaycasts = false;

        text = textObject.AddComponent<Text>();
        text.text = "HIHIHI";
        // text.font = font;
        text.fontSize = 20;
        text.fontStyle = FontStyle.Normal;
        text.alignment = TextAnchor.UpperLeft;

        textObject.transform.SetParent(canvas.transform, false);

        Vector2 position = new Vector2(600, 900);
        textTransform.anchorMin = position;
        textTransform.anchorMax = position;

        Object.DontDestroyOnLoad(canvas);
        Object.DontDestroyOnLoad(textObject);
    }

    public void setTime(string time)
    {
        text.text = time;
    }
}

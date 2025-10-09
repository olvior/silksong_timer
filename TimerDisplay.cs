using BepInEx;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class TimerDisplay
{
    private GameObject canvas;

    // private GameObject textObject;
    private Text text;
    private Text pbText;
    private Font theFont;

    public TimerDisplay()
    {
        // find font
        var allFonts = Resources.FindObjectsOfTypeAll<Font>();
        theFont = allFonts.FirstOrDefault(x => x.name == "TrajanPro-Regular");

        Console.WriteLine($"{allFonts.Length}");
        Console.WriteLine($"DSADSA ADSDA DSADSA {theFont == null}");

        canvas = new GameObject("TimerModCanvas");
        var canvasC = canvas.AddComponent<UnityEngine.Canvas>();
        canvasC.renderMode = RenderMode.ScreenSpaceOverlay;
        // canvasC.worldCamera = GameCameras.SilentInstance.hudCamera;
        canvasC.planeDistance = 20;
        canvas.layer = LayerMask.NameToLayer("UI");

        CanvasScaler scaler = canvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        canvas.AddComponent<GraphicRaycaster>();

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        canvasRect.anchorMin = new Vector2(0, 0);
        canvasRect.anchorMax = new Vector2(1, 1);
        canvasRect.sizeDelta = new Vector2(0, 0);

        text = createText("TimerText", "0:00.00", 70, new Vector2(0.95f, 0.9f), new Vector2(0.8f, 0.8f));
        pbText = createText("TimerPBText", "PB: 0:00.00", 40, new Vector2(0.95f, 0.8f), new Vector2(0.7f, 0.7f));

        UnityEngine.Object.DontDestroyOnLoad(canvas);
        // UnityEngine.Object.DontDestroyOnLoad(textObject);
    }

    public Text createText(string name, string defaultText, int fontSize, Vector2 anchorMax, Vector2 anchorMin)
    {
        GameObject textObject = new GameObject(name);
        textObject.layer = LayerMask.NameToLayer("UI");
        textObject.AddComponent<CanvasRenderer>();
        RectTransform textTransform = textObject.AddComponent<RectTransform>();
        textTransform.localPosition = Vector3.zero;

        CanvasGroup group = textObject.AddComponent<CanvasGroup>();
        group.interactable = false;
        group.blocksRaycasts = false;

        Text text = textObject.AddComponent<Text>();
        text.text = defaultText;
        text.font = theFont;
        text.fontSize = fontSize;
        text.fontStyle = FontStyle.Normal;
        text.alignment = TextAnchor.UpperRight;

        textObject.transform.SetParent(canvas.transform, false);

        textTransform.anchorMax = anchorMax;
        textTransform.anchorMin = anchorMin;

        return text;
    }

    public void setTime(string time)
    {
        text.text = time;
    }

    public void setPbTime(string time)
    {
        pbText.text = "PB: " + time;
    }
}

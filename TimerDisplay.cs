using BepInEx;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class TimerDisplay
{
    private GameObject canvas;

    private GameObject textObject;
    private Text text;
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


        textObject = new GameObject("TimerModTimerText");
        textObject.layer = LayerMask.NameToLayer("UI");
        textObject.AddComponent<CanvasRenderer>();
        RectTransform textTransform = textObject.AddComponent<RectTransform>();
        textTransform.localPosition = Vector3.zero;

        CanvasGroup group = textObject.AddComponent<CanvasGroup>();
        group.interactable = false;
        group.blocksRaycasts = false;

        text = textObject.AddComponent<Text>();
        text.text = "1:01.21";
        text.font = theFont;
        text.fontSize = 70;
        text.fontStyle = FontStyle.Normal;
        text.alignment = TextAnchor.UpperRight;

        textObject.transform.SetParent(canvas.transform, false);

        // Vector2 position = new Vector2(1, 1);
        textTransform.anchorMax = new Vector2(0.95f, 0.9f);
        textTransform.anchorMin = new Vector2(0.8f, 0.8f);

        UnityEngine.Object.DontDestroyOnLoad(canvas);
        UnityEngine.Object.DontDestroyOnLoad(textObject);
    }

    public void setTime(string time)
    {
        text.text = time;
    }
}

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Evergreen;

public static class TextDrawing {
  internal static GameObject ConsoleCanvas;
  private static GameObject Console;
  private static GameObject VersionObj;
  private static List<string> ConsoleHistory = new List<string>();
  private static int MaxHistoryCount = 5;

  public static event EventHandler OnConsoleReady;

  public enum TextAlignmentOptions {
    TopLeft = TMPro.TextAlignmentOptions.TopLeft,
    Top = TMPro.TextAlignmentOptions.Top,
    TopRight = TMPro.TextAlignmentOptions.TopRight,
    Left = TMPro.TextAlignmentOptions.Left,
    Center = TMPro.TextAlignmentOptions.Center,
    Right = TMPro.TextAlignmentOptions.Right,
    BottomLeft = TMPro.TextAlignmentOptions.BottomLeft,
    Bottom = TMPro.TextAlignmentOptions.Bottom,
    BottomRight = TMPro.TextAlignmentOptions.BottomRight,
  }

  internal static void Init() {
    ConsoleCanvas = new GameObject("EvergreenConsole");
    var canvas = ConsoleCanvas.AddComponent<Canvas>();
    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    GameObject.DontDestroyOnLoad(ConsoleCanvas);

    Console = DrawText("", TextAlignmentOptions.TopRight);
    VersionObj = DrawText($"{Evergreen.Name} ver. {Evergreen.Version}", TextAlignmentOptions.TopLeft);
    VersionObj.SetActive(false);
    SceneManager.activeSceneChanged += ShowEvergreenVer;

    OnConsoleReady?.Invoke(ConsoleCanvas, EventArgs.Empty);
  }

  private static void ShowEvergreenVer(Scene current, Scene next) {
    if (next.name == "MainMenu") VersionObj.SetActive(true);
    else VersionObj.SetActive(false);
  }

  public static void DrawToConsole(string text) {
    if (ConsoleHistory.Count >= MaxHistoryCount) ConsoleHistory.RemoveAt(0);
    ConsoleHistory.Add(text);

    Console.GetComponent<TextMeshProUGUI>().text = string.Join("\n", ConsoleHistory.ToArray());
  }

  public static GameObject DrawText(string text, float x, float y, float fontSize = 36) { // screen is ~2000 units wide and ~1400 units tall
    var to = CreateTextObject();
    var tm = to.GetComponent<TextMeshProUGUI>();
    tm.alignment = TMPro.TextAlignmentOptions.Center;
    tm.text = text;
    tm.fontSize = fontSize;

    to.transform.localPosition = new Vector3(x, y, 0);

    return to;
  }

  public static GameObject DrawText(string text, TextAlignmentOptions alignment, float fontSize = 36) {
    var to = CreateTextObject();
    var tm = to.GetComponent<TextMeshProUGUI>();
    tm.alignment = (TMPro.TextAlignmentOptions)alignment;
    tm.text = text;
    tm.fontSize = fontSize;

    return to;
  }

  private static GameObject CreateTextObject() {
    var to = new GameObject("Custom Textbox");
    var tm = to.AddComponent<TextMeshProUGUI>();
    tm.font = CommonResources.pixelFont;
    to.transform.SetParent(ConsoleCanvas.transform);
    var rt = to.GetComponent<RectTransform>();
    rt.anchorMin = new Vector2(0, 0);
    rt.anchorMax = new Vector2(1f, 1f);
    rt.offsetMin = new Vector2(10f, 10f);
    rt.offsetMax = new Vector2(-10f, -10f);
    rt.localScale = new Vector3(1f, 1f, 1f);

    return to;
  }
}
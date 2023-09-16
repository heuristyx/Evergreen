using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Evergreen;

public static class Text {
  private static GameObject ConsoleCanvas;
  private static GameObject Console;
  private static GameObject VersionObj;
  private static List<string> ConsoleHistory = new List<string>();
  private static int MaxHistoryCount = 5;

  internal static void Init() {
    ConsoleCanvas = new GameObject("EvergreenConsole");
    var canvas = ConsoleCanvas.AddComponent<Canvas>();
    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    GameObject.DontDestroyOnLoad(ConsoleCanvas);

    Console = DrawText("Evergreen console initialized", TextAlignmentOptions.TopRight);
    VersionObj = DrawText($"{Evergreen.Name} ver. {Evergreen.Version}", TextAlignmentOptions.BottomRight);
    VersionObj.SetActive(false);
    SceneManager.activeSceneChanged += ShowEvergreenVer;
  }

  private static void ShowEvergreenVer(Scene current, Scene next) {
    if (next.name == "MainMenu") VersionObj.SetActive(true);
    else VersionObj.SetActive(false);
  }

  public static void DrawToConsole(string text) {
    if (ConsoleHistory.Count > MaxHistoryCount) ConsoleHistory.RemoveAt(ConsoleHistory.Count - 1);
    ConsoleHistory.Insert(0, text);

    Console.GetComponent<TextMeshProUGUI>().text = string.Join("\n", ConsoleHistory.ToArray());
  }

  public static GameObject DrawText(string text, float x, float y) {
    var to = CreateTextObject();
    var tm = to.GetComponent<TextMeshProUGUI>();
    tm.alignment = TextAlignmentOptions.Center;
    tm.text = text;

    to.transform.localPosition = new Vector3(x, y, 0);

    return to;
  }

  public static GameObject DrawText(string text, TextAlignmentOptions alignment) {
    var to = CreateTextObject();
    var tm = to.GetComponent<TextMeshProUGUI>();
    tm.alignment = alignment;
    tm.text = text;

    return to;
  }

  private static GameObject CreateTextObject() {
    var to = new GameObject("Custom Textbox");
    var tm = to.AddComponent<TextMeshProUGUI>();
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
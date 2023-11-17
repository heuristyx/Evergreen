using System.Collections.Generic;
using Doozy.Engine.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Evergreen;

public static class TextDrawing
{
  internal static GameObject EvergreenCanvas;
  public static TMP_InputField inputField;
  private static GameObject Console;
  private static GameObject ConsoleLog;
  private static GameObject VersionObj;
  private static List<string> ConsoleHistory = new List<string>();
  private static int MaxHistoryCount = 5;

  public enum TextAlignmentOptions
  {
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

  internal static void Init()
  {
    Evergreen.Log.LogInfo($"Loading Evergreen {nameof(TextDrawing)}");

    EvergreenCanvas = new GameObject("EvergreenCanvas");
    var canvas = EvergreenCanvas.AddComponent<Canvas>();
    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    GameObject.DontDestroyOnLoad(EvergreenCanvas);
    EvergreenCanvas.GetComponent<RectTransform>().FullScreen(true);

    InitConsole();
    Evergreen.OnToggleDebugMode += (self, state) => { Console.SetActive(state); };

    VersionObj = DrawText($"{Evergreen.Name} ver. {Evergreen.Version}", TextAlignmentOptions.TopLeft, 40);
    VersionObj.SetActive(false);
    SceneManager.activeSceneChanged += ShowEvergreenVer;
  }

  private static void InitConsole()
  {
    Console = new GameObject("Console");
    Console.transform.SetParent(EvergreenCanvas.transform);
    Console.AddComponent<RectTransform>().FullScreen(true);
    ConsoleLog = DrawText("", TextAlignmentOptions.TopRight);
    ConsoleLog.transform.SetParent(Console.transform);
    var consoleInput = new GameObject("Console input");
    consoleInput.transform.SetParent(Console.transform);
    var rect = consoleInput.AddComponent<RectTransform>();
    rect.anchorMin = new Vector2(.85f, .65f);
    rect.anchorMax = new Vector2(1f, 1f);
    var inputText = new GameObject("Console input text");
    inputText.transform.SetParent(consoleInput.transform);
    var inputPlaceholder = new GameObject("Console input placeholder");
    inputPlaceholder.transform.SetParent(consoleInput.transform);
    inputField = consoleInput.AddComponent<TMP_InputField>();
    inputField.onSubmit.AddListener(Commands.SendCommand);
    var text = inputText.AddComponent<TextMeshProUGUI>();
    text.font = Assets.pixelFont;
    text.fontSize = 30;
    text.color = Color.gray;
    inputField.textComponent = text;
    var placeholder = inputPlaceholder.AddComponent<TextMeshProUGUI>();
    placeholder.font = Assets.pixelFont;
    placeholder.color = Color.gray;
    placeholder.fontSize = 30;
    placeholder.text = "Type \"/\"...";
    placeholder.GetComponent<RectTransform>().offsetMax = new Vector2(1000f, placeholder.GetComponent<RectTransform>().offsetMax.y);
    inputField.placeholder = placeholder;

    Console.SetActive(false);
  }

  /// <summary>
  /// Gets the GameObject of the Evergreen canvas.
  /// </summary>
  public static GameObject GetCanvas()
  {
    return EvergreenCanvas;
  }

  private static void ShowEvergreenVer(Scene current, Scene next)
  {
    if (next.name == "MainMenu") VersionObj.SetActive(true);
    else VersionObj.SetActive(false);
  }

  /// <summary>
  /// Displays text in the Evergreen ingame console.
  /// </summary>
  public static void DrawToConsole(string text)
  {
    if (ConsoleHistory.Count >= MaxHistoryCount) ConsoleHistory.RemoveAt(0);
    ConsoleHistory.Add(text);

    ConsoleLog.GetComponent<TextMeshProUGUI>().text = string.Join("\n", ConsoleHistory.ToArray());
  }

  /// <summary>
  /// Draws text on-screen. Returns the parent <c>GameObject</c>
  /// </summary>
  public static GameObject DrawText(string text, float x, float y, float fontSize = 30)
  { // screen is ~2000 units wide and ~1400 units tall
    var to = CreateTextObject();
    var tm = to.GetComponent<TextMeshProUGUI>();
    tm.alignment = TMPro.TextAlignmentOptions.Center;
    tm.text = text;
    tm.fontSize = fontSize;

    to.transform.localPosition = new Vector3(x, y, 0);

    return to;
  }

  /// <summary>
  /// Draws text on-screen. Returns the parent <c>GameObject</c>
  /// </summary>
  public static GameObject DrawText(string text, TextAlignmentOptions alignment, float fontSize = 30)
  {
    var to = CreateTextObject();
    var tm = to.GetComponent<TextMeshProUGUI>();
    tm.alignment = (TMPro.TextAlignmentOptions)alignment;
    tm.text = text;
    tm.fontSize = fontSize;

    return to;
  }

  private static GameObject CreateTextObject()
  {
    var to = new GameObject("Custom Textbox");
    var tm = to.AddComponent<TextMeshProUGUI>();
    tm.font = Assets.pixelFont;
    to.transform.SetParent(EvergreenCanvas.transform);
    var rt = to.GetComponent<RectTransform>();
    rt.anchorMin = new Vector2(0, 0);
    rt.anchorMax = new Vector2(1f, 1f);
    rt.offsetMin = new Vector2(10f, 10f);
    rt.offsetMax = new Vector2(-10f, -10f);
    rt.localScale = new Vector3(1f, 1f, 1f);

    return to;
  }
}
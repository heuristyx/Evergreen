using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Evergreen;

public static class Text {
  private static GameObject console;

  internal static void Init() {
    console = DrawText("", TextAlignmentOptions.TopRight);
  }

  public static void DrawToConsole(string text) {
    console.GetComponent<TextMeshProUGUI>().text += "\n" + text;
  }

  public static GameObject DrawText(string text, float x, float y) {
    var to = CreateTextObject();
    var tm = to.GetComponent<TextMeshProUGUI>();
    tm.alignment = TextAlignmentOptions.Center;
    tm.text = text;
    tm.fontSize = 12;

    to.transform.localPosition = new Vector3(x, y, -19f);

    return to;
  }

  public static GameObject DrawText(string text, TextAlignmentOptions alignment) {
    var to = CreateTextObject();
    var tm = to.AddComponent<TextMeshProUGUI>();
    tm.alignment = alignment;
    tm.text = text;
    tm.fontSize = 12;

    return to;
  }

  private static GameObject CreateTextObject() {
    var to = new GameObject("Custom Textbox");
    var tm = to.AddComponent<TextMeshProUGUI>();
    to.transform.SetParent(GameObject.FindObjectOfType<Canvas>().transform);
    var rt = to.GetComponent<RectTransform>();
    rt.anchorMin = new Vector2(0, 0);
    rt.anchorMax = new Vector2(1f, 1f);
    rt.offsetMin = new Vector2(10f, 10f);
    rt.offsetMax = new Vector2(-10f, -10f);
    rt.localScale = new Vector3(1f, 1f, 1f);

    return to;
  }
}
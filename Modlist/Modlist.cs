using System.Collections.Generic;
using System.Linq;
using BepInEx.Bootstrap;
using Doozy.Engine.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Evergreen;

public static class Modlist {
  private static GameObject ModlistButton;
  private static ModlistUI UI = new ModlistUI();
  private static GameObject ModlistPanel;
  private static Button BackToMenuButton;

  internal class ModlistUI : MonoBehaviour {
    public void OpenModlistPanel() {
      ModlistPanel.transform.SetAsLastSibling();
      ModlistPanel.SetActive(true);
    }
  }

  internal static void Init() {
    On.HomeUI.Awake += InitModlistUI;
  }

  private static void InitModlistUI(On.HomeUI.orig_Awake orig, HomeUI self) {
    orig(self);

    CapturePixelFontAsset(self);
    MakeModPanel(self);
    InsertModButtonIntoMenu(self);

    TextDrawing.Init(); // Bad place to init this?
  }

  private static void CapturePixelFontAsset(HomeUI homeUI) {
    CommonResources.pixelFont = homeUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().font;
  }

  private static void MakeModPanel(HomeUI homeUI) {
    // Modlist panel container
    ModlistPanel = new GameObject("Modlist Panel", typeof(ModlistUI));
    ModlistPanel.transform.SetParent(homeUI.transform.parent);
    ModlistPanel.gameObject.SetActive(false);

    var mlimg = ModlistPanel.AddComponent<Image>();
    mlimg.sprite = homeUI.GetComponent<Image>().sprite;
    mlimg.type = Image.Type.Sliced;
    mlimg.color = Color.black;

    ModlistPanel.GetComponent<RectTransform>().FullScreen(true);

    // Mod list text object
    GameObject ModlistText = new GameObject("Text");
    ModlistText.transform.SetParent(ModlistPanel.transform);
    ModlistText.AddComponent<RectTransform>().FullScreen(true);
    var mlt = ModlistText.AddComponent<TextMeshProUGUI>();
    mlt.alignment = TextAlignmentOptions.Center;
    mlt.fontSize = 24;
    mlt.font = CommonResources.pixelFont;
    mlt.text = "===== Enabled mods =====\n" + string.Join("\n", GenerateModList());

    // Return to menu button
    GameObject BackToMenu = GameObject.Instantiate(homeUI.transform.Find("Panel").Find("Quit").gameObject);
    BackToMenu.transform.SetParent(ModlistPanel.transform);
    var btmrt = BackToMenu.GetComponent<RectTransform>();
    btmrt.FullScreen(true);
    btmrt.anchorMax = new Vector2(1f, 0.1f);
    BackToMenuButton = BackToMenu.GetComponent<Button>();
    BackToMenuButton.navigation = new Navigation() { mode = Navigation.Mode.None };
    var btmText = BackToMenu.GetComponent<TextMeshProUGUI>();
    btmText.alignment = TextAlignmentOptions.Center;
    btmText.text = "Back to menu";
    BackToMenuButton.onClick.AddListener(delegate() {
      homeUI.OpenHomeUI();
    });
    var cursor = BackToMenu.transform.GetChild(0).gameObject;
    cursor.transform.localPosition = new Vector3(-155f, -4f, 0f);
    cursor.SetActive(true);
  }

  private static void InsertModButtonIntoMenu(HomeUI homeUI) {
    var panel = homeUI.transform.Find("Panel");
    var quitButton = panel.Find("Quit");
    int i = quitButton.GetSiblingIndex() - 1;

    // Mod list button object
    ModlistButton = GameObject.Instantiate(quitButton.gameObject);
    ModlistButton.name = "Modlist";
    ModlistButton.transform.SetParent(panel);
    ModlistButton.transform.SetSiblingIndex(i);
    ModlistButton.transform.localPosition = quitButton.localPosition;
    ModlistButton.transform.localScale = quitButton.localScale;
    ModlistButton.GetComponent<TextMeshProUGUI>().text = "Mods";
    ModlistButton.GetComponent<Button>().onClick.AddListener(delegate() {
      UI.OpenModlistPanel();
      BackToMenuButton.Select();
    });

    // New menu navigation directions
    Navigation newAboveNav, newModlistNav, newBelowNav;
    newAboveNav = newModlistNav = newBelowNav = new Navigation() { mode = Navigation.Mode.Explicit };

    var buttons = GetButtons(panel.gameObject);

    newAboveNav.selectOnUp = buttons[i-2];
    newAboveNav.selectOnDown = buttons[i];

    newModlistNav.selectOnUp = buttons[i-1];
    newModlistNav.selectOnDown = buttons[i+1];

    newBelowNav.selectOnUp = buttons[i];
    newBelowNav.selectOnDown = buttons[i+2];

    buttons[i-1].navigation = newAboveNav;
    buttons[i].navigation = newModlistNav;
    buttons[i+1].navigation = newBelowNav;
  }

  private static List<Button> GetButtons(GameObject parent) {
    var l = new List<Button>();
    for (int i = 0; i < parent.transform.childCount; i++) {
      var btn = parent.transform.GetChild(i).GetComponent<Button>();
      if (btn != null) l.Add(btn);
    }
    return l;
  }

  private static List<string> GenerateModList() {
    var infos = Chainloader.PluginInfos;
    return infos.Select(i => $"{i.Value.Metadata.Name} ({i.Value.Metadata.Version})").ToList();
  }
}
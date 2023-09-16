using System.Collections.Generic;
using System.Linq;
using BepInEx.Bootstrap;
using Doozy.Engine.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Evergreen;

public static class Modlist {
  private static GameObject ModlistButtonGO;
  private static Button ModlistButton;
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
    On.HomeUI.SetButtonsListeners += AddModlistButtonListener;
  }

  private static void InitModlistUI(On.HomeUI.orig_Awake orig, HomeUI self) {
    Evergreen.Log.LogInfo("Initializing home UI");

    MakeModPanel(self);

    var quitButton = self.transform.Find("Panel").Find("Quit");

    Navigation newQuitNav = new Navigation();
    newQuitNav.mode = Navigation.Mode.Explicit;

    Navigation newModlistNav = new Navigation();
    newModlistNav.mode = Navigation.Mode.Explicit;

    Navigation newTwitterNav = new Navigation();
    newTwitterNav.mode = Navigation.Mode.Explicit;

    ModlistButtonGO = GameObject.Instantiate(quitButton.gameObject);
    ModlistButtonGO.name = "Modlist";
    ModlistButtonGO.transform.SetParent(quitButton.parent);
    ModlistButtonGO.transform.localPosition = quitButton.localPosition;
    ModlistButtonGO.transform.localScale = quitButton.localScale;
    ModlistButtonGO.GetComponent<TextMeshProUGUI>().text = "Mods";
    ModlistButton = ModlistButtonGO.GetComponent<Button>();

    newQuitNav.selectOnUp = self.transform.Find("Panel").Find("Settings").GetComponent<Button>();
    newQuitNav.selectOnDown = ModlistButton;

    newModlistNav.selectOnUp = quitButton.GetComponent<Button>();
    newModlistNav.selectOnDown = self.transform.Find("Panel").Find("Twitter").GetComponent<Button>();

    newTwitterNav.selectOnUp = ModlistButton;
    newTwitterNav.selectOnDown = self.transform.Find("Panel").Find("Discord").GetComponent<Button>();

    quitButton.GetComponent<Button>().navigation = newQuitNav;
    ModlistButton.navigation = newModlistNav;
    self.transform.Find("Panel").Find("Twitter").GetComponent<Button>().navigation = newTwitterNav;
  }

  private static void MakeModPanel(HomeUI homeUI) {
    ModlistPanel = new GameObject("Modlist Panel");
    ModlistPanel.transform.SetParent(homeUI.transform.parent);
    var mlui = ModlistPanel.AddComponent<ModlistUI>();
    ModlistPanel.gameObject.SetActive(false);
    ModlistPanel.AddComponent<ModlistUI>();
    var img = ModlistPanel.AddComponent<Image>();
    img.sprite = homeUI.GetComponent<Image>().sprite;
    img.type = Image.Type.Sliced;
    img.color = Color.black;
    var mlrt = ModlistPanel.GetComponent<RectTransform>();
    mlrt.FullScreen(true);

    GameObject ModlistText = new GameObject("Text");
    ModlistText.transform.SetParent(ModlistPanel.transform);
    var mltrt = ModlistText.AddComponent<RectTransform>();
    mltrt.FullScreen(true);
    var title = ModlistText.AddComponent<TextMeshProUGUI>();
    title.alignment = TextAlignmentOptions.Center;
    title.fontSize = 24;
    title.text = "===== Enabled mods =====\n" + string.Join("\n", GenerateModList());

    GameObject BackToMenuGO = GameObject.Instantiate(homeUI.transform.Find("Panel").Find("Quit").gameObject);
    BackToMenuGO.transform.SetParent(ModlistPanel.transform);
    var btmrt = BackToMenuGO.GetComponent<RectTransform>();
    btmrt.FullScreen(true);
    btmrt.anchorMax = new Vector2(1f, 0.1f);
    BackToMenuButton = BackToMenuGO.GetComponent<Button>();
    var btmNav = new Navigation();
    btmNav.mode = Navigation.Mode.None;
    BackToMenuButton.navigation = btmNav;
    var btmText = BackToMenuGO.GetComponent<TextMeshProUGUI>();
    btmText.text = "Back to menu";
    btmText.alignment = TextAlignmentOptions.Center;
    BackToMenuButton.onClick.AddListener(delegate() {
      homeUI.OpenHomeUI();
    });
    var cursor = BackToMenuGO.transform.GetChild(0).gameObject;
    cursor.transform.localPosition = new Vector3(-160f, 0f, 0f);
    cursor.SetActive(true);
  }

  private static List<string> GenerateModList() {
    var infos = Chainloader.PluginInfos;
    return infos.Select(i => i.Value.Metadata.Name).ToList();
  }

  private static void AddModlistButtonListener(On.HomeUI.orig_SetButtonsListeners orig, HomeUI self) {
    orig(self);

    ModlistButton.onClick.AddListener(delegate() {
      UI.OpenModlistPanel();
      BackToMenuButton.Select();
    });
  }
}
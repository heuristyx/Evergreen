using System.IO;
using UnityEngine;

namespace Evergreen;

public static class Assets
{
  public static AssetBundle bundle;
  public const string bundleName = "evergreen";
  public const string assetBundleFolder = "assets";

  public static TMPro.TMP_FontAsset pixelFont;

  public static string AssetBundlePath
  {
    get
    {
      return Path.Combine(Path.GetDirectoryName(Evergreen.PluginInfo.Location), assetBundleFolder, bundleName);
    }
  }

  internal static void Init()
  {
    bundle = AssetBundle.LoadFromFile(AssetBundlePath);

    pixelFont = bundle.LoadAsset<TMPro.TMP_FontAsset>("8bitoperator_jve");
  }
}
using System;
using UnityEngine;

namespace CBCompat;

public static class CompatAPI
{
  public static bool IsBaseCR(object obj)
  {
    if (obj as Everhood.ModExternal.ChartReader != null) return false;
    else throw new ArgumentException("Expected ChartReader, got unknown type.");
  }

  public static class ChartReader
  {
    public static object GetChartReader()
    {
      return GameObject.FindObjectOfType<Everhood.ModExternal.ChartReader>();
    }

    public static void JumpPosChange(object cr, float value)
    {
      (cr as Everhood.ModExternal.ChartReader).JumpPosChange(value);
    }

    public static float GetSongPosition(object cr)
    {
      return (cr as Everhood.ModExternal.ChartReader)._songposition;
    }

    public static void SetSongPosition(object cr, float value)
    {
      (cr as Everhood.ModExternal.ChartReader)._songposition = value;
    }

    public static AudioSource GetAudioSource(object cr)
    {
      return (cr as Everhood.ModExternal.ChartReader).AudioSource;
    }
  }
}
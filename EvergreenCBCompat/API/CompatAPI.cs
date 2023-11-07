using System;
using UnityEngine;

namespace CBCompat;

public static class CompatAPI
{
  public static bool IsBaseCR<T>(T obj)
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

    public static void JumpPosChange<T>(T cr, float value)
    {
      (cr as Everhood.ModExternal.ChartReader).JumpPosChange(value);
    }

    public static float GetSongPosition<T>(T cr)
    {
      return (cr as Everhood.ModExternal.ChartReader)._songposition;
    }

    public static void SetSongPosition<T>(T cr, float value)
    {
      (cr as Everhood.ModExternal.ChartReader)._songposition = value;
    }

    public static AudioSource GetAudioSource<T>(T cr)
    {
      return (cr as Everhood.ModExternal.ChartReader).AudioSource;
    }
  }
}
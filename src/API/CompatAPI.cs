using System;
using Everhood.Chart;
using UnityEngine;

namespace Evergreen;

public static class CompatAPI
{
  public static bool IsBaseCR(object obj)
  {
    if (obj as Everhood.Chart.ChartReader != null) return true;
    else return CBCompat.CompatAPI.IsBaseCR(obj);
  }

  public static class ChartReader
  {
    public static object GetChartReader()
    {
      if (Evergreen.IsBaseGame) return GameObject.FindObjectOfType<Everhood.Chart.ChartReader>();
      else return CBCompat.CompatAPI.ChartReader.GetChartReader();
    }

    public static void JumpPosChange(object cr, float value)
    {
      if (IsBaseCR(cr)) (cr as Everhood.Chart.ChartReader).JumpPosChange(value);
      else CBCompat.CompatAPI.ChartReader.JumpPosChange(cr, value);
    }

    public static float GetSongPosition(object cr)
    {
      if (IsBaseCR(cr)) return (cr as Everhood.Chart.ChartReader)._songposition;
      else return CBCompat.CompatAPI.ChartReader.GetSongPosition(cr);
    }

    public static void SetSongPosition(object cr, float value)
    {
      if (IsBaseCR(cr)) (cr as Everhood.Chart.ChartReader)._songposition = value;
      else CBCompat.CompatAPI.ChartReader.SetSongPosition(cr, value);
    }

    public static AudioSource GetAudioSource(object cr)
    {
      if (IsBaseCR(cr)) return (cr as Everhood.Chart.ChartReader).AudioSource;
      else return CBCompat.CompatAPI.ChartReader.GetAudioSource(cr);
    }
  }
}
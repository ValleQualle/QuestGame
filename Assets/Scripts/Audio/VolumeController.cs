using System;
using System.Collections;
using System.Collections.Generic;

using FMOD.Studio;

using FMODUnity;

using UnityEngine;

public class VolumeController : MonoBehaviour
{
    private VCA master;
    private VCA music;
    private VCA sfx;

    #region Unity Event Functions

    private void Awake()
    {
        master = RuntimeManager.GetVCA("vca:/Master");
        master = RuntimeManager.GetVCA("vca:/Music");
        master = RuntimeManager.GetVCA("vca:/SFX");
    }

    private void Update()
    {
        master.setVolume(PlayerPrefs.GetFloat(SettingsMenu.MasterVolumeKey, SettingsMenu.DefaultMasterVolume));
        music.setVolume(PlayerPrefs.GetFloat(SettingsMenu.MusicVolumeKey, SettingsMenu.DefaultMusicVolume));
        sfx.setVolume(PlayerPrefs.GetFloat(SettingsMenu.SFXVolumeKey, SettingsMenu.DefaultSFXVolume));
    }

    #endregion
}

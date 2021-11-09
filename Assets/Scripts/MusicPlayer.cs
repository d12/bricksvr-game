using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MusicPlayer : MonoBehaviour
{
   public AudioClip[] songs;

   private AudioSource _source;

   private float _currentClipLen;
   private bool _musicDisabled = true;

    void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    public void OnMusicVolumeSet(float volume)
    {
        _source.volume = volume;
    }

    public void OnMusicToggle(bool musicSettingEnabled)
    {
        if (musicSettingEnabled)
        {
            if(!_source.isPlaying) _source.Play();
            _musicDisabled = false;
        }
        else
        {
            _source.Pause();
            _musicDisabled = true;
        }
    }

    void Update()
    {
        if (!_musicDisabled && (_currentClipLen - _source.time) < 0.5f)
        {
            _source.clip = RandomSong();
            _currentClipLen = _source.clip.length;
            _source.Play();
        }
    }

    private AudioClip RandomSong()
    {
        while (true)
        {
            if (songs.Length == 1) return songs[0];

            AudioClip randomClip = songs[Random.Range(0, songs.Length)];
            if (randomClip == _source.clip) continue;

            return randomClip;
        }
    }

    public void Pause()
    {
        _source.Pause();
    }

    public void Resume()
    {
        if (_musicDisabled) return;
        _source.UnPause();
    }
}

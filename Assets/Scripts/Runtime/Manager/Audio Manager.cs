using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IdleClicker
{
    public class AudioManager : MonoBehaviour
    {
        AudioSource audio;
        [SerializeField] Toggle t_Music;
        private void Awake()
        {
            audio = GetComponent<AudioSource>();
            if (!PlayerPrefs.HasKey("musicmute")) PlayerPrefs.SetInt("musicmute", 1);
            t_Music.isOn = PlayerPrefs.GetInt("musicmute")==1?true:false;
            Setup();
        }
        public void Setup()
        {
            audio.enabled = t_Music.isOn;
            PlayerPrefs.SetInt("musicmute",t_Music.isOn?1:0);
        }
    }
}

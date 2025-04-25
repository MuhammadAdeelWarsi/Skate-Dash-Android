using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace CustomControllers 
{
    public class AudioManager : MonoBehaviour
    {
        private bool isMusicEnabled, isSoundEnabled;
        private Dictionary<AudioNames, AudioData> audiosDictionary;
        [SerializeField] AudioData[] audiosData;

        public bool IsMusicEnabled => isMusicEnabled;
        public bool IsSoundEnabled => isSoundEnabled;


        private void Awake()
        {
            audiosDictionary = new Dictionary<AudioNames, AudioData>();
            InitializeAudioData();
        }
        private void Start()
        {
            if(GameManager.Instance)
                GameManager.Instance.UiManager.onAudioToggleEvent += UpdateAudioSettings;
        }

        public void InitializeAudioSettings()   //Initializes audio settings based on saved data from settings menu
        {
            isMusicEnabled = PrefsDataManager.Instance.LoadData(PrefsDataManager.PrefsKeys.Music, 1) == 1;
            isSoundEnabled = PrefsDataManager.Instance.LoadData(PrefsDataManager.PrefsKeys.Sound, 1) == 1;
        }
        private void InitializeAudioData()  //Fills up dictionary based on current scene's audio data
        {
            foreach (AudioData audioData in audiosData)
            {
                if (!audiosDictionary.ContainsKey(audioData.Name))
                    audiosDictionary.Add(audioData.Name, audioData);
            }
        }

        private void UpdateAudioSettings(PrefsDataManager.PrefsKeys audioKey, bool value)
        {
            switch (audioKey)
            {
                case PrefsDataManager.PrefsKeys.Music:
                    isMusicEnabled = value;
                    break;

                case PrefsDataManager.PrefsKeys.Sound:
                    isSoundEnabled = value;
                    break;
            }
        }

        public void PlayAudio(AudioNames audioName)         //Plays the audio with provided name
        {
            AudioData audioData = audiosDictionary[audioName];

            if(!audioData.DoOverlap)    //If overlapping is not enabled
                StopAudio(audioData.Source);

            if (audioData.Source.clip != audioData.Clip && !audioData.DoOverlap)
            {
                audioData.Source.clip = audioData.Clip;
                if (audioData.HasCustomizedSettings)
                {
                    audioData.Source.loop = audioData.DoLoop;
                }
            }

            if (audioData.Source.mute)
                audioData.Source.mute = false;

            if (!audioData.DoOverlap)
                audioData.Source.Play();
            else
                audioData.Source.PlayOneShot(audioData.Clip);   //Plays audio without stopping the current and keep high priority
        }
        public void PlayAudio(AudioNames audioName, float delay)    //Plays the audio with provided name and delay
        {
            AudioData audioData = audiosDictionary[audioName];
            StartCoroutine(PlayAudioWithDelay(audioData, delay));
        }
        private IEnumerator PlayAudioWithDelay(AudioData audioData, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (!audioData.DoOverlap)
                StopAudio(audioData.Source);

            if (audioData.Source.clip != audioData.Clip && !audioData.DoOverlap)
            {
                audioData.Source.clip = audioData.Clip;
                if (audioData.HasCustomizedSettings)
                {
                    audioData.Source.loop = audioData.DoLoop;
                }
            }

            if (audioData.Source.mute)
                audioData.Source.mute = false;

            if (!audioData.DoOverlap)
                audioData.Source.Play();
            else
                audioData.Source.PlayOneShot(audioData.Clip);
        }

        private void StopAudio(AudioSource audioSource)     //Stops the audio on the target audio source. For internal use!
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
        }
        public void StopAudio(AudioNames audioName)         //Stops the audio with provided name
        {
            AudioData audioData = audiosDictionary[audioName];

            if (audioData.Source.isPlaying)
                audioData.Source.Stop();
        }

        public void PauseAudio(AudioNames audioName)        //Pauses the audio with provided name
        {
            AudioData audioData = audiosDictionary[audioName];
            audioData.Source.Pause();
        }
        public void UnPauseAudio(AudioNames audioName)      //Unpauses the audio with provided name
        {
            AudioData audioData = audiosDictionary[audioName];
            audioData.Source.UnPause();
        }

        public void MuteAudio(AudioNames audioName)        //Mutes the audio with provided name
        {
            AudioData audioData = audiosDictionary[audioName];
            audioData.Source.mute = true;
        }
        public void UnMuteAudio(AudioNames audioName, bool doPlay)  //Unmutes the audio with provided name
        {
            AudioData audioData = audiosDictionary[audioName];
            audioData.Source.mute = false;

            if(audioData.Source.clip != audioData.Clip)   //Clip is not correct
            {
                audioData.Source.clip = audioData.Clip;
                audioData.Source.Play();    //Clip change stops the audio source so manual play is applied
                audioData.Source.Pause();   //Instantly audio source is paused to let play check do its work below independently
            }

            if (doPlay)
            {
                if (!audioData.DoOverlap)
                    audioData.Source.Play();
                else
                    audioData.Source.PlayOneShot(audioData.Clip);
            }
        }


        #region Nested Class
        [Serializable]
        public class AudioData
        {
            [SerializeField] AudioNames name;
            [SerializeField] AudioClip clip;
            [SerializeField] AudioSource source;
            [Space(7), SerializeField] bool hasCustomizedSettings = false;
            [SerializeField] bool doLoop = false;
            [Space(7), Tooltip("If enabled, overlapped with currently played clip in the audio source without stopping it and keep high priority"), SerializeField] bool doOverlap = false;

            public AudioNames Name => name;
            public AudioClip Clip => clip;
            public AudioSource Source => source;
            public bool HasCustomizedSettings => hasCustomizedSettings;
            public bool DoLoop => doLoop;
            public bool DoOverlap => doOverlap;
        }
        #endregion

        #region Enum
        public enum AudioNames
        {
            LogoReveal,
            UiSwoosh,
            ButtonClick,
            ButtonPop,
            PanelPop,
            ButtonOffToggle,
            ButtonOnToggle,
            PointGranted,
            ScoreCounterFill,
            GameEndWithHighscore,
            GameEndWithLowscore,
            ClockTick,
            ObstacleCollision,
            SkateboardRiding,
            WindBlowing,
            DayMusic,
            NightMusic,
            Dragonfly,
            
            None    //For setting null value of AudioNames variable
        }
        #endregion
    }
}
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Audio;

namespace FTG.AudioController
{
    public class AudioController : MonoBehaviour
    {
        /* ======================================================================================================================== */
        /* VARIABLE DECLARATIONS                                                                                                    */
        /* ======================================================================================================================== */

        [Header("Music")]
        [SerializeField] private AudioMixer musicMixer;
        [SerializeField] private float musicVolume = -6f;
        [SerializeField] private Sound[] arrayMusic;
        [SerializeField] private AudioMixerSnapshot[] arraySnapshot;
        private bool[] arrayPausedMusic;

        [Header("Sound")]
        [SerializeField] private AudioMixer soundMixer;
        [SerializeField] private float soundVolume = -6f;
        [SerializeField] private Sound[] arraySound;
        private bool[] arrayPausedSound;

        // Singleton instance
        public static AudioController Instance;

        /* ======================================================================================================================== */
        /* UNITY CALLBACKS                                                                                                          */
        /* ======================================================================================================================== */

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this.gameObject);
                return;
            }

            foreach (Sound sound in arrayMusic)
            {
                sound.source = gameObject.AddComponent<AudioSource>();
                sound.source.clip = sound.audioClip;
                sound.source.outputAudioMixerGroup = sound.audioGroup;
                sound.source.volume = sound.volume;
                sound.source.pitch = sound.pitch;
                sound.source.playOnAwake = sound.playOnAwake;
                sound.source.loop = sound.loop;
            }

            foreach (Sound sound in arraySound)
            {
                sound.source = gameObject.AddComponent<AudioSource>();
                sound.source.clip = sound.audioClip;
                sound.source.outputAudioMixerGroup = sound.audioGroup;
                sound.source.volume = sound.volume;
                sound.source.pitch = sound.pitch;
                sound.source.playOnAwake = sound.playOnAwake;
                sound.source.loop = sound.loop;
            }
            
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", -6f);
            soundVolume = PlayerPrefs.GetFloat("SoundVolume", -6f);
        }

        private void Start()
        {
            arrayPausedMusic = new bool[arrayMusic.Length];
            arrayPausedSound = new bool[arraySound.Length];

            UpdateVolumes();
        }

        /* ======================================================================================================================== */
        /* MUSIC FUNCTIONS                                                                                                   */
        /* ======================================================================================================================== */

        /// <summary>
        /// Plays music with name = <paramref name="musicName"/>
        /// </summary>
        /// <param name="musicName"></param> Music to be played
        [UsedImplicitly]
        public void PlayMusic(string musicName, bool stopPlaying = false)
        {
            foreach (Sound sound in arrayMusic)
            {
                if (sound.name == musicName)
                {
                    if (stopPlaying == true)
                    {
                        sound.source.Stop();
                    }
                    
                    if (sound.source.isPlaying == false)
                    {
                        sound.source.Play();
                    }

                    return;
                }
            }
            
            
            Debug.Log("Music '" + musicName + "' not found.");
        }

        /// <summary>
        /// Stops music with name = <paramref name="musicName"/>
        /// </summary>
        /// <param name="musicName"></param> Music to be stopped
        [UsedImplicitly]
        public void StopMusic(string musicName, float delay = 0f)
        {
            foreach (Sound sound in arrayMusic)
            {
                if (sound.name == musicName)
                {
                    StartCoroutine(StopMusicCoroutine(sound, delay));
                }
            }
        }
        
        private IEnumerator StopMusicCoroutine(Sound sound, float delay)
        {
            yield return new WaitForSeconds(delay);
            sound.source.Stop();
        }

        /// <summary>
        /// Plays all music
        /// </summary>
        [UsedImplicitly]
        public void PlayAllMusic()
        {
            foreach (Sound item in arrayMusic)
            {
                if (item.source.isPlaying == false)
                {
                    item.source.Play();
                }
            }
        }

        /// <summary>
        /// Stops all music
        /// </summary>
        [UsedImplicitly]
        public void StopAllMusic()
        {
            foreach (Sound sound in arrayMusic)
            {
                sound.source.Stop();
            }
        }

        /// <summary>
        /// Pauses all music currently playing
        /// Saves isPlaying information into <seealso cref="AudioController.MusicIsPlaying(string)"/>
        /// </summary>
        [UsedImplicitly]
        public void PauseAllMusic()
        {
            for (int i = 0; i < arrayMusic.Length; i++)
            {
                arrayPausedMusic[i] = arrayMusic[i].source.isPlaying;
                arrayMusic[i].source.Pause();
            }
        }

        /// <summary>
        /// Continue all music which were paused by <seealso cref="AudioController.PauseAllMusic"/>
        /// </summary>
        [UsedImplicitly]
        public void ContinueAllMusic()
        {
            for (int i = 0; i < arrayMusic.Length; i++)
            {
                if (arrayPausedMusic[i] == true)
                {
                    arrayMusic[i].source.Play();
                }

                arrayPausedMusic[i] = false;
            }
        }

        /// <summary>
        /// Checks if selected music is currently playing
        /// </summary>
        /// <param name="musicName"></param> Name of music to be checked
        /// <returns></returns> Returns true if music with name = <param name="musicName"/> is currently playing, else otherwise
        [UsedImplicitly]
        public bool MusicIsPlaying(string musicName)
        {
            foreach (Sound sound in arrayMusic)
            {
                if (sound.name == musicName)
                {
                    return sound.source.isPlaying;
                }
            }
            return false;
        }

        /// <summary>
        /// Transitions to selected snapshot
        /// </summary>
        /// <param name="snapshotName"></param> Snapshot to which the transition is performed
        /// <param name="transitionTime"></param> Time for the transition
        [UsedImplicitly]
        public void TransitionToSnapshot(string snapshotName, float transitionTime = 0f)
        {
            foreach (AudioMixerSnapshot item in arraySnapshot)
            {
                if (item.name == snapshotName)
                {
                    item.TransitionTo(transitionTime);
                    return;
                }
            }
            
            Debug.Log("Snapshot '" + snapshotName + "' not found!");
        }

        /* ======================================================================================================================== */
        /* SOUND FUNCTIONS                                                                                                   */
        /* ======================================================================================================================== */

        /// <summary>
        /// Plays sound with name = <paramref name="soundName"/>
        /// </summary>
        /// <param name="soundName"></param> Sound to be played
        [UsedImplicitly]
        public void PlaySound(string soundName)
        {
            foreach (Sound sound in arraySound)
            {
                if (sound.name == soundName)
                {
                    sound.source.volume = sound.volume;
                    sound.source.pitch = sound.pitch;

                    sound.source.volume += Random.Range(-sound.volumeVariation, sound.volumeVariation);
                    sound.source.pitch += Random.Range(-sound.pitchVariation, sound.pitchVariation);

                    sound.source.Play();
                    return;
                }
            }
            
            Debug.Log("Sound '" + soundName + "' not found.");
        }
        
        /// <summary>
        /// Plays sound with name = <paramref name="soundName"/>
        /// </summary>
        /// <param name="soundName"></param> Sound to be played
        [UsedImplicitly]
        public void PlaySoundOneShot(string soundName)
        {
            foreach (Sound sound in arraySound)
            {
                if (sound.name == soundName)
                {
                    sound.source.volume = sound.volume;
                    sound.source.pitch = sound.pitch;

                    sound.source.volume += Random.Range(-sound.volumeVariation, sound.volumeVariation);
                    sound.source.pitch += Random.Range(-sound.pitchVariation, sound.pitchVariation);

                    sound.source.PlayOneShot(sound.source.clip);
                    return;
                }
            }
            
            Debug.Log("Sound '" + soundName + "' not found.");
        }
        

        /// <summary>
        /// Stops sound with name = <paramref name="soundName"/>
        /// </summary>
        /// <param name="soundName"></param> Sound to be stopped
        [UsedImplicitly]
        public void StopSound(string soundName)
        {
            foreach (Sound sound in arraySound)
            {
                if (sound.name == soundName)
                {
                    sound.source.Stop();
                    return;
                }
            }
            
            Debug.Log("Sound '" + soundName + "' not found.");
        }

        /// <summary>
        /// Stops all sounds
        /// </summary>
        [UsedImplicitly]
        public void StopAllSounds()
        {
            foreach (Sound sound in arraySound)
            {
                sound.source.Stop();
            }
        }

        /// <summary>
        /// Pauses all sounds currently playing
        /// Saves isPlaying information into <seealso cref="AudioController.SoundIsPlaying(string)"/>
        /// </summary>
        [UsedImplicitly]
        public void PauseAllSounds()
        {
            for (int i = 0; i < arraySound.Length; i++)
            {
                arrayPausedSound[i] = arraySound[i].source.isPlaying;
                arraySound[i].source.Pause();
            }
        }

        /// <summary>
        /// Continues all sounds which were paused by <seealso cref="AudioController.SoundIsPlaying(string)"/>
        /// </summary>
        [UsedImplicitly]
        public void ContinueAllSounds()
        {
            for (int i = 0; i < arraySound.Length; i++)
            {
                if (arrayPausedSound[i] == true)
                {
                    arraySound[i].source.Play();
                }

                arrayPausedSound[i] = false;
            }
        }

        /// <summary>
        /// Checks if selected sound is currently playing
        /// </summary>
        /// <param name="soundName"></param> Name of sound to be checked
        /// <returns></returns> Returns true if sound with name = <param name="soundName"/> is currently playing, else otherwise
        [UsedImplicitly]
        public bool SoundIsPlaying(string soundName)
        {
            foreach (Sound sound in arraySound)
            {
                if (sound.name == soundName)
                {
                    return sound.source.isPlaying;
                }
            }
            return false;
        }

        /* ======================================================================================================================== */
        /* VOLUME FUNCTIONS                                                                                                  */
        /* ======================================================================================================================== */

        /// <summary>
        /// Setter for <seealso cref="AudioController.musicVolume"/>
        /// </summary>
        /// <param name="newMusicVolume"></param> New music volume
        [UsedImplicitly]
        public void SetMusicVolume(float newMusicVolume)
        {
            musicVolume = Mathf.Clamp(Mathf.Log10(newMusicVolume) * 20f, -80f, 0f);
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
            UpdateVolumes();
        }

        /// <summary>
        /// Setter for <seealso cref="AudioController.soundVolume"/>
        /// </summary>
        /// <param name="newSoundVolume"></param> New sound volume
        [UsedImplicitly]
        public void SetSoundVolume(float newSoundVolume)
        {
            soundVolume = Mathf.Clamp(Mathf.Log10(newSoundVolume) * 20f, -80f, 0f);
            PlayerPrefs.SetFloat("SoundVolume", soundVolume);
            UpdateVolumes();
        }

        /// <summary>
        /// Updates volumes for all music and sounds
        /// </summary>
        private void UpdateVolumes()
        {
            musicMixer.SetFloat("musicVolume", musicVolume);
            soundMixer.SetFloat("soundVolume", soundVolume);
        }

        /// <summary>
        /// Getter for <seealso cref="AudioController.musicVolume"/>
        /// </summary>
        /// <returns></returns><seealso cref="AudioController.musicVolume"/>
        [UsedImplicitly]
        public float GetMusicVolume()
        {
            return musicVolume;
        }

        /// <summary>
        /// Getter for <seealso cref="AudioController.soundVolume"/>
        /// </summary>
        /// <returns></returns><seealso cref="AudioController.soundVolume"/>
        [UsedImplicitly]
        public float GetSoundVolume()
        {
            return soundVolume;
        }
    }
}

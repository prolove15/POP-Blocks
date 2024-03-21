// // ©2015 - 2023 Candy Smith
// // All rights reserved
// // Redistribution of this software is strictly not allowed.
// // Copy of this software can be obtained from unity asset store only.
// // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// // FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
// // AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// // LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// // OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// // THE SOFTWARE.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace POPBlocks.Scripts
{
    /// <summary>
    /// Sound manager
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class SoundBase : MonoBehaviour
    {
        public static SoundBase Instance;
        public AudioClip click;
        public AudioClip itemExplosion;
        public AudioClip bombExplosion;
        public AudioClip bonusAppear;
        public AudioClip ballExplosion;
        public AudioClip rocketSound;
        public AudioClip noMatch;
        public AudioClip[] pinWheelSound;
        public AudioClip cash;
        public AudioClip woosh;
        private AudioSource _audioSource;
        public AudioMixer audioMixer;
        public AudioMixer audioMixerMusic;
        List<AudioClip> clipsPlaying = new List<AudioClip>();

        ///SoundBase.Instance.audio.PlayOneShot( SoundBase.Instance.kreakWheel );

        // Use this for initialization
        void Awake()
        {

            _audioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(gameObject);

        }

        private void Start()
        {
            RestoreSound();
        }

        public void RestoreSound()
        {
            audioMixer.SetFloat("soundVolume", PlayerPrefs.GetInt("soundVolume"));
            audioMixer.SetFloat("musicVolume", PlayerPrefs.GetInt("musicVolume"));
        }

        public void MuteSoundTemporarily()
        {
            if(PlayerPrefs.GetInt("soundVolume")> -80)
                audioMixer.SetFloat("soundVolume", -80);
        }

        public void PlayOneShot(AudioClip audioClip)
        {
            if (audioClip != null) _audioSource.PlayOneShot(audioClip);
        }
    
        public void PlaySoundsRandom(AudioClip[] clip)
        {
            if (clip.Length > 0)
                PlayOneShot(clip[Random.Range(0, clip.Length)]);
        }

        public void PlayLimitSound(AudioClip clip)
        {
            if (clipsPlaying.IndexOf(clip) < 0)
            {
                clipsPlaying.Add(clip);
                PlayOneShot(clip);
                StartCoroutine(WaitForCompleteSound(clip));
            }
        }

        IEnumerator WaitForCompleteSound(AudioClip clip)
        {
            yield return new WaitForSeconds(0.1f);
            clipsPlaying.Remove(clipsPlaying.Find(x => clip));
        }

        public void MuteMusic(bool mute)
        {
            audioMixerMusic.SetFloat("musicVolume", mute?-80:0);
        }
    }
}

using UnityEngine;
using System;

namespace Framework
{
    /// <summary>
    /// 音效对象
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public sealed class SoundObject : MonoBehaviour
    {
        /// <summary>
        /// 停止事件
        /// </summary>
        public event Action<SoundObject> onStopEvent;

        /// <summary>
        /// 音效资源
        /// </summary>
        private AudioSource m_AudioSource;

        /// <summary>
        /// 唤醒函数
        /// </summary>
        private void Awake()
        {
            m_AudioSource = GetComponent<AudioSource>();
            m_AudioSource.playOnAwake = false;
        }

        /// <summary>
        /// 更新函数
        /// </summary>
        private void Update()
        {
            if (!m_AudioSource.isPlaying)
            {
                Stop();
            }
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="name">音效名</param>
        /// <param name="audioClip">音效剪辑</param>
        /// <param name="loop">是否循环播放</param>
        public void Play(/*AudioClip audioClip,float volume, bool loop = false*/)
        {
            
            gameObject.SetActive(true);

            //m_AudioSource.volume = volume;
            //m_AudioSource.clip = audioClip;
            //m_AudioSource.loop = loop;
            m_AudioSource.enabled = true;
            m_AudioSource.Play();
		}

        /// <summary>
        /// 重新播放
        /// </summary>
        public void Replay()
        {
            if (m_AudioSource.clip == null)
            {
                return;
            }

            gameObject.SetActive(true);

			m_AudioSource.enabled = true;
            m_AudioSource.time = 0;
            m_AudioSource.Play();
		}

        /// <summary>
        /// 停止音效
        /// </summary>
        /// <param name="destroy">是否销毁</param>
        public void Stop(bool destroy = false)
        {
            gameObject.SetActive(false);

			m_AudioSource.Stop();
            m_AudioSource.clip = null;
            m_AudioSource.enabled = false;

            if (onStopEvent != null)
            {
                onStopEvent.Invoke(this);
                onStopEvent = null;
            }

            if (destroy)
            {
                Destroy(gameObject);
            }
        }

		/// <summary>
		/// 暂停音效
		/// </summary>
		public void Pause()
		{
			gameObject.SetActive(false);

			m_AudioSource.Pause();
		}

		/// <summary>
		/// 取消暂停
		/// </summary>
		public void UnPause()
		{
			gameObject.SetActive(true);

			//m_AudioSource.UnPause();
            m_AudioSource.Play();

        }

        /// <summary>
        /// 设置音乐Clip
        /// </summary>
        /// <param name="audioClip">音乐Clip</param>
        public void SetAudioClip(AudioClip audioClip)
        {
            if (m_AudioSource != null)
            {
                gameObject.name = audioClip.name;
                m_AudioSource.clip = audioClip;
            }
        }

        /// <summary>
        /// 设置音乐是否循环
        /// </summary>
        /// <param name="loop">是否循环</param>
        public void SetLoop(bool loop)
        {
            if (m_AudioSource != null)
            {
                m_AudioSource.loop = loop;
            }
        }

        /// <summary>
        /// 设置音乐音量
        /// </summary>
        /// <param name="val">音量大小</param>
        public void SetVolume(float val)
        {
            if (m_AudioSource != null)
            {
                m_AudioSource.volume = val;
            }
        }

        /// <summary>
        /// 获取音乐音量
        /// </summary>
        /// <param name="val">音量大小</param>
        public float GetVolume(float val)
        {
            if (m_AudioSource != null)
            {
                return m_AudioSource.volume;
            }
            return 0;
        }
    }
}
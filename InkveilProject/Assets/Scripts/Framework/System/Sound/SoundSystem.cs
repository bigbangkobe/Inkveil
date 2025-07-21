using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Framework
{
    /// <summary>
    /// 音效系统
    /// </summary>
    public sealed class SoundSystem : MonoSingleton<SoundSystem>, IResLoadListener
    {

        /// <summary>
        /// 音效对象池
        /// </summary>
        private ObjectPool m_SoundPool = new ObjectPool(OnSoundConstruct, OnSoundDestroy, OnSoundEnabled, OnSoundDisabled);

        /// <summary>
        /// 音乐资源管理字典
        /// </summary>
        private Dictionary<string, AudioClip> soundDic = new Dictionary<string, AudioClip>();

        private SoundObject BgmSoundObject = null;
        private List<SoundObject> effectSoundObjectList = new List<SoundObject>();

        private float m_BgmVolume = 1;
        private float m_SfxVolume = 1;

        /// <summary>
        /// 初始化音乐
        /// </summary>
        public void OnInit()
        {
            //加载音乐
            ResMgr.Instance.Load("BOSSBG", this, typeof(AudioClip));
            ResMgr.Instance.Load("CombatBG", this, typeof(AudioClip));
            ResMgr.Instance.Load("Duojian", this, typeof(AudioClip));
            ResMgr.Instance.Load("God", this, typeof(AudioClip));
            ResMgr.Instance.Load("Gun", this, typeof(AudioClip));
            ResMgr.Instance.Load("Knife", this, typeof(AudioClip));
            ResMgr.Instance.Load("MainBG", this, typeof(AudioClip));
            ResMgr.Instance.Load("Stick", this, typeof(AudioClip));
            ResMgr.Instance.Load("Sword", this, typeof(AudioClip));
            ResMgr.Instance.Load("我这是在哪", this, typeof(AudioClip));
            ResMgr.Instance.Load("你醒来啦", this, typeof(AudioClip));
            ResMgr.Instance.Load("墨妖来了", this, typeof(AudioClip));
            ResMgr.Instance.Load("呼(语气词) 先把小怪清理掉就行啦", this, typeof(AudioClip));
            ResMgr.Instance.Load("妖王怎么就出现啦", this, typeof(AudioClip));
            ResMgr.Instance.Load("怪物太多的时候 可以使用祈愿令哦", this, typeof(AudioClip));
            ResMgr.Instance.Load("我们胜利啦", this, typeof(AudioClip));
            ResMgr.Instance.Load("可以点祈愿按钮去抽取神将哦", this, typeof(AudioClip));
            ResMgr.Instance.Load("快去看看商城吧", this, typeof(AudioClip));
            ResMgr.Instance.Load("按成长按钮可提升属性哦", this, typeof(AudioClip));
            ResMgr.Instance.Load("战斗一次花费五点体力哦", this, typeof(AudioClip));
            ResMgr.Instance.Load("战斗分为普通级 困难级 深渊级哦", this, typeof(AudioClip));
            ResMgr.Instance.Load("肃清墨妖", this, typeof(AudioClip));
            ResMgr.Instance.Load("我们继续努力吧", this, typeof(AudioClip));
            ResMgr.Instance.Load("战斗失败了 唉", this, typeof(AudioClip));
            ResMgr.Instance.Load("恭喜抽到了神将关羽", this, typeof(AudioClip));
            ResMgr.Instance.Load("恭喜抽到了神将悟空", this, typeof(AudioClip));
            ResMgr.Instance.Load("恭喜抽到了神将杨戬", this, typeof(AudioClip));
            ResMgr.Instance.Load("恭喜抽到了神将哪吒", this, typeof(AudioClip));
            ResMgr.Instance.Load("中坛元帅", this, typeof(AudioClip));
            ResMgr.Instance.Load("二郎真君", this, typeof(AudioClip));
            ResMgr.Instance.Load("武圣关羽", this, typeof(AudioClip));
            ResMgr.Instance.Load("齐天大圣", this, typeof(AudioClip));
            ResMgr.Instance.Load("修行者", this, typeof(AudioClip));
            ResMgr.Instance.Load("别打扰我 我在休息", this, typeof(AudioClip));
            ResMgr.Instance.Load("勇气让我们前行", this, typeof(AudioClip));
            ResMgr.Instance.Load("呜呜 嘟嘟", this, typeof(AudioClip));
            ResMgr.Instance.Load("想看我长大的样子吗", this, typeof(AudioClip));
            ResMgr.Instance.Load("战斗吧", this, typeof(AudioClip));
            ResMgr.Instance.Load("琴声真好听呀", this, typeof(AudioClip));
            ResMgr.Instance.Load("长辈跟我说 相逢便是缘", this, typeof(AudioClip));
        }

        protected override void Awake()
        {
            base.Awake();

            m_BgmVolume = PlayerPrefs.GetFloat("BgmVolume",1);
            m_SfxVolume = PlayerPrefs.GetFloat("SfxVolume",1);
        }

        /// <summary>
        /// 销毁回调
        /// </summary>
        private void OnDestroy()
        {
            Clear();
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="audioClipName">音效包名</param>
        /// <param name="loop">是否循环播放</param>
        /// <returns>返回音效对象</returns>
        public async Task<SoundObject> Play(string audioClipName, float volume = 1.0f, bool loop = false, bool isBG = false)
        {
            if (!soundDic.ContainsKey(audioClipName))
            {
                Debug.LogError("不存在该音效资源" + audioClipName);
                return null;
            }
            AudioClip resource = soundDic[audioClipName];
            SoundObject soundObject = await instance.m_SoundPool.GetAsync(audioClipName) as SoundObject;

            if (isBG) {
                BgmSoundObject?.Stop();
                BgmSoundObject = soundObject;
                soundObject.SetVolume(m_BgmVolume);
            }
            else
            {
                effectSoundObjectList.Add(soundObject);
                soundObject.SetVolume(m_SfxVolume);
            }

            soundObject.SetAudioClip(resource);
            soundObject.SetLoop(loop);
            soundObject.Play();

            return soundObject;
        }

        public void SetBgmSound(float value) 
        {
            m_BgmVolume = value;
            BgmSoundObject?.SetVolume(m_BgmVolume);
        }

        public void SetSfxSound(float value)
        {
            m_SfxVolume = value;
        }

        /// <summary>
		/// 音效停止回调
		/// </summary>
		/// <param name="soundObject">音效对象</param>
		private static void OnSoundStop(SoundObject soundObject)
        {
            if (soundObject.gameObject.name == "bgm")
            {
                instance.BgmSoundObject = null;
            }
            else
            {
                if (instance.effectSoundObjectList.Contains(soundObject))
                {
                    instance.effectSoundObjectList.Remove(soundObject);
                }
            }
            instance.m_SoundPool.Remove(soundObject);
        }

        public void Clear()
        {
            // 停止并回收 BGM
            if (BgmSoundObject != null)
            {
                BgmSoundObject.Stop(true); // true = 强制停止
                m_SoundPool.Remove(BgmSoundObject);
                BgmSoundObject = null;
            }

            // 停止并回收所有音效
            for (int i = effectSoundObjectList.Count - 1; i >= 0; i--)
            {
                SoundObject soundObject = effectSoundObjectList[i];
                if (soundObject != null)
                {
                    soundObject.Stop(true);
                    m_SoundPool.Remove(soundObject);
                }
            }
            effectSoundObjectList.Clear();

            // 清空对象池（彻底销毁或重置）
            m_SoundPool.Clear();
        }


        /// <summary>
        /// 音效构造回调
        /// </summary>
        /// <returns>返回音效对象</returns>
        private static Task<object> OnSoundConstruct(object obj)
        {
            string audioClipName = obj as string;

            GameObject gameObject = new GameObject(audioClipName);
            gameObject.transform.SetParent(instance.transform, false);

            SoundObject soundObject = gameObject.AddComponent<SoundObject>();
            return Task.FromResult<object>(soundObject); // ✅ 正确包裹，非 async 版本
        }


        /// <summary>
        /// 音效销毁回调
        /// </summary>
        /// <param name="obj">对象</param>
        private static void OnSoundDestroy(object obj, object o = null)
        {
            SoundObject soundObject = obj as SoundObject;
            soundObject.Stop(true);
        }

        /// <summary>
        /// 音效开启回调
        /// </summary>
        /// <param name="obj">对象</param>
        private static void OnSoundEnabled(object obj, object o = null)
        {
            SoundObject soundObject = obj as SoundObject;
            soundObject.onStopEvent += OnSoundStop;
        }

        /// <summary>
        /// 音效关闭回调
        /// </summary>
        /// <param name="obj">对象</param>
        private static void OnSoundDisabled(object obj, object o = null)
        {

        }

        public void Finish(object asset, string name)
        {
            Debug.Log("加载音乐对象成功" + name);
            AudioClip audioClip = asset as AudioClip;
            soundDic[name] = audioClip;
        }

        public void Failure()
        {
            Debug.LogError("加载音乐对象失败" + name);
        }

        /// <summary>
        /// 点击按钮音效
        /// </summary>
        public void OnButtonClickSound()
        {
            //SoundObject soundObject = Play(SoundDefine.BUTTON_CLICK_SOUND);
            //soundObject.SetVolume(0.5f);
        }

        /// <summary>
        /// 设置Bgm的音量大小
        /// </summary>
        /// <param name="soundName"></param>
        /// <param name="val"></param>
        public void SetBgmVolume(float val)
        {
            if (BgmSoundObject != null)
            {
                BgmSoundObject.SetVolume(val);
            }
        }

        /// <summary>
        /// 所有BGM开关
        /// </summary>
        /// <param name="isOn"></param>
        public void AllBgmToggle(bool isOn)
        {
            if (BgmSoundObject != null)
            {
                if (isOn)
                {
                    BgmSoundObject.UnPause();
                }
                else
                {
                    BgmSoundObject.Pause();
                }
            }
        }

        /// <summary>
        /// 所有音效开关
        /// </summary>
        /// <param name="isOn"></param>
        public void AllSoundEffectToggle(bool isOn)
        {
            if (effectSoundObjectList.Count > 0)
            {
                for (int i = 0; i < effectSoundObjectList.Count; i++)
                {
                    SoundObject soundObject = effectSoundObjectList[i];
                    //if(soundObject.gameObject.name == "fight_bgm" && GameConfig.gameState == GameState.Pause)
                    //{
                    //    continue;
                    //}
                    if (isOn)
                    {
                        soundObject.UnPause();
                    }
                    else
                    {
                        soundObject.Pause();
                    }
                }
            }
        }
    }
}
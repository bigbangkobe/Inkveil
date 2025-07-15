using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Framework
{
    /// <summary>
    /// 音效系统 （单例 + 对象池 + Addressables 预加载）
    /// </summary>
    public sealed class SoundSystem : MonoSingleton<SoundSystem>
    {
        // 对象池，用来复用 SoundObject
        private ObjectPool m_SoundPool;
        // 缓存所有预加载好的 AudioClip
        private Dictionary<string, AudioClip> soundDic = new Dictionary<string, AudioClip>();

        // 当前播放的 BGM 对象
        private SoundObject BgmSoundObject;
        // 正在播放的 SFX 列表
        private List<SoundObject> effectSoundObjectList = new List<SoundObject>();

        // 用户设置的音量
        private float m_BgmVolume;
        private float m_SfxVolume;

        // 需要预加载的所有 Addressables Key
        private readonly string[] audioKeys = new[]
        {
            "BOSSBG", "CombatBG", "Duojian", "God", "Gun", "Knife", "MainBG", "Stick", "Sword",
            "我这是在哪", "你醒来啦", "墨妖来了", "呼(语气词) 先把小怪清理掉就行啦", "妖王怎么就出现啦",
            "怪物太多的时候 可以使用祈愿令哦", "我们胜利啦", "可以点祈愿按钮去抽取神将哦", "快去看看商城吧",
            "按成长按钮可提升属性哦", "战斗一次花费五点体力哦", "战斗分为普通级 困难级 深渊级哦",
            "肃清墨妖", "我们继续努力吧", "战斗失败了 唉", "恭喜抽到了神将关羽", "恭喜抽到了神将悟空",
            "恭喜抽到了神将杨戬", "恭喜抽到了神将哪吒", "中坛元帅", "二郎真君", "武圣关羽", "齐天大圣",
            "修行者", "别打扰我 我在休息", "勇气让我们前行", "呜呜 嘟嘟", "想看我长大的样子吗", "战斗吧",
            "琴声真好听呀", "长辈跟我说 相逢便是缘"
        };

        public void OnInit()
        {

        }

        protected override void Awake()
        {
            base.Awake();
            // 读取用户设置的音量
            m_BgmVolume = PlayerPrefs.GetFloat("BgmVolume", 1f);
            m_SfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1f);

            // 初始化对象池
            m_SoundPool = new ObjectPool(
                OnSoundConstruct, OnSoundDestroy, OnSoundEnabled, OnSoundDisabled
            );
        }

        private void Start()
        {
            // 启动时预加载所有音频
            StartCoroutine(PreloadAudioCoroutine());
        }

        /// <summary>
        /// 批量异步预加载所有 Addressables 音频
        /// </summary>
        private IEnumerator PreloadAudioCoroutine()
        {
            // 1. 初始化 Addressables
            var initHandle = Addressables.InitializeAsync();
            yield return initHandle;
            if (initHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError("[SoundSystem] Addressables 初始化失败");
                yield break;
            }

            // 2. 发起所有加载请求
            var handles = new List<AsyncOperationHandle<AudioClip>>();
            foreach (var key in audioKeys)
            {
                handles.Add(Addressables.LoadAssetAsync<AudioClip>(key));
            }

            // 3. 等待全部完成
            foreach (var handle in handles)
                yield return handle;

            // 4. 缓存结果
            foreach (var handle in handles)
            {
                if (handle.Status == AsyncOperationStatus.Succeeded &&
                    handle.Result != null)
                {
                    soundDic[handle.Result.name] = handle.Result;
                }
                else
                {
                    Debug.LogError($"[SoundSystem] 预加载失败: {handle.DebugName} -> {handle.OperationException}");
                }
            }

            Debug.Log("[SoundSystem] 预加载完成，已缓存 " + soundDic.Count + " 个音频");
        }

        /// <summary>
        /// 播放音效／背景音乐
        /// </summary>
        /// <param name="audioClipName">Addressables Key</param>
        /// <param name="volume">播放音量（0-1）</param>
        /// <param name="loop">是否循环</param>
        /// <param name="isBG">是否作为 BGM</param>
        public SoundObject Play(string audioClipName,
                                float volume = 1f,
                                bool loop = false,
                                bool isBG = false)
        {
            if (!soundDic.TryGetValue(audioClipName, out var clip) || clip == null)
            {
                Debug.LogError($"[SoundSystem] 不存在音效资源: {audioClipName}");
                return null;
            }

            // 从池里获取一个 SoundObject
            var soundObject = m_SoundPool.Get(audioClipName) as SoundObject;

            if (isBG)
            {
                // 如果已有 BGM，先停掉
                BgmSoundObject?.Stop();
                BgmSoundObject = soundObject;
                // 使用全局 BGM 音量
                soundObject.SetVolume(m_BgmVolume * volume);
            }
            else
            {
                effectSoundObjectList.Add(soundObject);
                soundObject.SetVolume(m_SfxVolume * volume);
            }

            soundObject.SetAudioClip(clip);
            soundObject.SetLoop(loop);
            soundObject.Play();
            return soundObject;
        }

        #region 对象池回调

        private static object OnSoundConstruct(object key)
        {
            // 新建一个 GameObject 并挂上 SoundObject（会自动添加 AudioSource）
            var go = new GameObject(key as string);
            go.transform.SetParent(instance.transform, false);
            return go.AddComponent<SoundObject>();
        }

        private static void OnSoundDestroy(object obj, object _ = null)
        {
            // 强制销毁
            (obj as SoundObject)?.Stop(destroy: true);
        }

        private static void OnSoundEnabled(object obj, object _ = null)
        {
            // 注册播放结束回调
            (obj as SoundObject).onStopEvent += OnSoundStop;
        }

        private static void OnSoundDisabled(object obj, object _ = null)
        {
            // （目前无需额外操作）
        }

        private static void OnSoundStop(SoundObject so)
        {
            if (so == instance.BgmSoundObject)
                instance.BgmSoundObject = null;
            else
                instance.effectSoundObjectList.Remove(so);

            instance.m_SoundPool.Remove(so);
        }

        #endregion

        #region 额外控制接口

        /// <summary>停止并回收所有音效／BGM</summary>
        public void Clear()
        {
            BgmSoundObject?.Stop(destroy: true);
            foreach (var so in effectSoundObjectList)
                so.Stop(destroy: true);

            m_SoundPool.Clear();
            effectSoundObjectList.Clear();
            BgmSoundObject = null;
        }

        /// <summary>设置 BGM 音量</summary>
        public void SetBgmVolume(float value)
        {
            m_BgmVolume = value;
            PlayerPrefs.SetFloat("BgmVolume", value);
            if (BgmSoundObject != null)
                BgmSoundObject.SetVolume(m_BgmVolume);
        }

        /// <summary>设置 SFX 音量</summary>
        public void SetSfxVolume(float value)
        {
            m_SfxVolume = value;
            PlayerPrefs.SetFloat("SfxVolume", value);
        }

        /// <summary>全局 BGM 开关</summary>
        public void AllBgmToggle(bool isOn)
        {
            if (BgmSoundObject == null) return;
            if (isOn) BgmSoundObject.UnPause();
            else BgmSoundObject.Pause();
        }

        /// <summary>全局 SFX 开关</summary>
        public void AllSoundEffectToggle(bool isOn)
        {
            foreach (var so in effectSoundObjectList)
            {
                if (isOn) so.UnPause();
                else so.Pause();
            }
        }

        #endregion
    }
}

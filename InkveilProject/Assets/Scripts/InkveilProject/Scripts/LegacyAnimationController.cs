using UnityEngine;
using System.Collections.Generic;

public class LegacyAnimationController : MonoBehaviour
{
    [System.Serializable]
    public class AnimationClipData
    {
        public string name;            // 动画名称标识
        public AnimationClip clip;     // 动画片段
        public float fadeTime = 0.3f;  // 淡入淡出时间
        public bool loop = false;      // 是否循环
        [Range(0, 2)] public float speed = 1f; // 播放速度
    }

    [Header("动画设置")]
    public Animation animationComponent; // Animation组件
    public List<AnimationClipData> animations = new List<AnimationClipData>();

    private string currentAnimation;    // 当前播放的动画
    private Dictionary<string, AnimationClipData> animationDict;

    void Awake()
    {
        Initialize();
    }

    // 初始化动画系统
    private void Initialize()
    {
        if (animationComponent == null)
        {
            animationComponent = GetComponent<Animation>();
            if (animationComponent == null)
            {
                Debug.LogError("LegacyAnimationController: 没有找到Animation组件!");
                return;
            }
        }

        // 设置动画组件属性
        animationComponent.playAutomatically = false;
        animationComponent.Stop();

        animationDict = new Dictionary<string, AnimationClipData>();
        foreach (var animData in animations)
        {
            if (animData.clip == null)
            {
                Debug.Log($"LegacyAnimationController: 动画 {animData.name} 的Clip为空!");
                continue;
            }

            // 设置动画循环模式
            animData.clip.wrapMode = animData.loop ? WrapMode.Loop : WrapMode.Once;

            // 添加动画到Animation组件
            if (!animationComponent.GetClip(animData.clip.name))
            {
                animationComponent.AddClip(animData.clip, animData.name);
            }

            if (!animationDict.ContainsKey(animData.name))
            {
                animationDict.Add(animData.name, animData);
            }
            else
            {
                Debug.Log($"LegacyAnimationController: 重复的动画名称 {animData.name} 将被忽略.");
            }
        }
    }

    // 播放动画(带淡入淡出效果)
    public void PlayAnimation(string animationName,float speed = 1)
    {
        if (currentAnimation == animationName) return;
        if (animationDict.TryGetValue(animationName, out AnimationClipData animData))
        {
            currentAnimation = animationName;
            animationComponent[animationName].speed = animData.speed * speed;
            animationComponent.CrossFade(animationName, animData.fadeTime);
        }
        else
        {
            Debug.Log($"LegacyAnimationController: 未找到名为 {animationName} 的动画.");
        }
    }

    // 立即播放动画(无淡入淡出)
    public void PlayAnimationImmediate(string animationName,float speed = 1)
    {
        if (animationDict.TryGetValue(animationName, out AnimationClipData animData))
        {
            currentAnimation = animationName;
            animationComponent[animationName].speed = animData.speed * speed;
            animationComponent.Play(animationName);
        }
        else
        {
            Debug.Log($"LegacyAnimationController: 未找到名为 {animationName} 的动画.");
        }
    }

    // 混合播放动画(与其他动画混合)
    public void BlendAnimation(string animationName, float weight = 1f, float fadeTime = 0.3f)
    {
        if (animationDict.TryGetValue(animationName, out AnimationClipData animData))
        {
            animationComponent[animationName].speed = animData.speed;
            animationComponent.Blend(animationName, weight, fadeTime);
        }
        else
        {
            Debug.Log($"LegacyAnimationController: 未找到名为 {animationName} 的动画.");
        }
    }

    // 获取当前动画的播放进度(0-1)
    public float GetCurrentAnimationProgress()
    {
        if (string.IsNullOrEmpty(currentAnimation)) return 0;

        return animationComponent[currentAnimation].normalizedTime;
    }

    // 检查动画是否正在播放
    public bool IsAnimationPlaying(string animationName)
    {
        return animationComponent.IsPlaying(animationName);
    }

    // 停止所有动画
    public void StopAllAnimations()
    {
        animationComponent.Stop();
        currentAnimation = null;
    }

    // 停止指定动画
    public void StopAnimation(string animationName, float fadeTime = 0f)
    {
        if (fadeTime <= 0)
        {
            animationComponent.Stop(animationName);
        }
        else
        {
            animationComponent.Blend(animationName, 0, fadeTime);
        }

        if (currentAnimation == animationName)
        {
            currentAnimation = null;
        }
    }

    // 设置动画速度
    public void SetAnimationSpeed(string animationName, float speed)
    {
        if (animationDict.ContainsKey(animationName))
        {
            animationComponent[animationName].speed = speed;
            animationDict[animationName].speed = speed;
        }
    }

    // 重置动画速度到默认
    public void ResetAnimationSpeed(string animationName)
    {
        if (animationDict.TryGetValue(animationName, out AnimationClipData animData))
        {
            animationComponent[animationName].speed = animData.speed;
        }
    }
}
using UnityEngine;
using System.Collections.Generic;

public class LegacyAnimationController : MonoBehaviour
{
    [System.Serializable]
    public class AnimationClipData
    {
        public string name;            // �������Ʊ�ʶ
        public AnimationClip clip;     // ����Ƭ��
        public float fadeTime = 0.3f;  // ���뵭��ʱ��
        public bool loop = false;      // �Ƿ�ѭ��
        [Range(0, 2)] public float speed = 1f; // �����ٶ�
    }

    [Header("��������")]
    public Animation animationComponent; // Animation���
    public List<AnimationClipData> animations = new List<AnimationClipData>();

    private string currentAnimation;    // ��ǰ���ŵĶ���
    private Dictionary<string, AnimationClipData> animationDict;

    void Awake()
    {
        Initialize();
    }

    // ��ʼ������ϵͳ
    private void Initialize()
    {
        if (animationComponent == null)
        {
            animationComponent = GetComponent<Animation>();
            if (animationComponent == null)
            {
                Debug.LogError("LegacyAnimationController: û���ҵ�Animation���!");
                return;
            }
        }

        // ���ö����������
        animationComponent.playAutomatically = false;
        animationComponent.Stop();

        animationDict = new Dictionary<string, AnimationClipData>();
        foreach (var animData in animations)
        {
            if (animData.clip == null)
            {
                Debug.Log($"LegacyAnimationController: ���� {animData.name} ��ClipΪ��!");
                continue;
            }

            // ���ö���ѭ��ģʽ
            animData.clip.wrapMode = animData.loop ? WrapMode.Loop : WrapMode.Once;

            // ��Ӷ�����Animation���
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
                Debug.Log($"LegacyAnimationController: �ظ��Ķ������� {animData.name} ��������.");
            }
        }
    }

    // ���Ŷ���(�����뵭��Ч��)
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
            Debug.Log($"LegacyAnimationController: δ�ҵ���Ϊ {animationName} �Ķ���.");
        }
    }

    // �������Ŷ���(�޵��뵭��)
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
            Debug.Log($"LegacyAnimationController: δ�ҵ���Ϊ {animationName} �Ķ���.");
        }
    }

    // ��ϲ��Ŷ���(�������������)
    public void BlendAnimation(string animationName, float weight = 1f, float fadeTime = 0.3f)
    {
        if (animationDict.TryGetValue(animationName, out AnimationClipData animData))
        {
            animationComponent[animationName].speed = animData.speed;
            animationComponent.Blend(animationName, weight, fadeTime);
        }
        else
        {
            Debug.Log($"LegacyAnimationController: δ�ҵ���Ϊ {animationName} �Ķ���.");
        }
    }

    // ��ȡ��ǰ�����Ĳ��Ž���(0-1)
    public float GetCurrentAnimationProgress()
    {
        if (string.IsNullOrEmpty(currentAnimation)) return 0;

        return animationComponent[currentAnimation].normalizedTime;
    }

    // ��鶯���Ƿ����ڲ���
    public bool IsAnimationPlaying(string animationName)
    {
        return animationComponent.IsPlaying(animationName);
    }

    // ֹͣ���ж���
    public void StopAllAnimations()
    {
        animationComponent.Stop();
        currentAnimation = null;
    }

    // ָֹͣ������
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

    // ���ö����ٶ�
    public void SetAnimationSpeed(string animationName, float speed)
    {
        if (animationDict.ContainsKey(animationName))
        {
            animationComponent[animationName].speed = speed;
            animationDict[animationName].speed = speed;
        }
    }

    // ���ö����ٶȵ�Ĭ��
    public void ResetAnimationSpeed(string animationName)
    {
        if (animationDict.TryGetValue(animationName, out AnimationClipData animData))
        {
            animationComponent[animationName].speed = animData.speed;
        }
    }
}
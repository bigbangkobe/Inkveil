using Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum CriticalLevel
{
    None,
    Normal,     // 普通暴击（橙色）
    Super,      // 超暴击（紫色）
    Extreme     // 极限暴击（金色）
}


/// <summary>
/// 怪物UI
/// </summary>
public class EnemyUI : BaseUI
{
    public Image m_BloodFill;
    public Text m_DemageText;

    protected override void OnAwake()
    {
        base.OnAwake();
    }

    /// <summary>
    /// 更新函数
    /// </summary>
    public override void OnUpdate()
    {
        LookAtCamera();
    }

    private void LookAtCamera()
    {
        if (Camera.main != null)
        {
            transform.forward = Camera.main.transform.forward;
        }
    }


    public void OnReset()
    {
        m_BloodFill.fillAmount = 1;
        m_DemageText.text = "";
    }

    /// <summary>
    /// 怪物收到伤害
    /// </summary>
    public void OnDemage(float damage, float currentHP, float maxHP, bool isImmune, CriticalLevel criticalLevel)
    {
        m_BloodFill.fillAmount = currentHP / maxHP;

        if (isImmune)
        {
            m_DemageText.text = "MISS";
            m_DemageText.color = Color.gray;
        }
        else
        {
            switch (criticalLevel)
            {
                case CriticalLevel.Extreme:
                    m_DemageText.text = $"{damage:F0}";
                    m_DemageText.color = Color.red; // 金色
                    break;
                case CriticalLevel.Super:
                    m_DemageText.text = $"{damage:F0}";
                    m_DemageText.color = Color.red; // 紫色
                    break;
                case CriticalLevel.Normal:
                    m_DemageText.text = $"{damage:F0}";
                    m_DemageText.color = Color.red; // 橙色
                    break;
                default:
                    m_DemageText.text = $"{damage:F0}";
                    m_DemageText.color = Color.red;
                    break;
            }
        }

        StartCoroutine(DamageTextAnimation(criticalLevel, isImmune));
    }

    /// <summary>
    /// 文本动画
    /// </summary>
    /// <param name="criticalLevel">文本类型</param>
    /// <param name="isImmune">是否免疫伤害</param>
    /// <returns></returns>
    private IEnumerator DamageTextAnimation(CriticalLevel criticalLevel, bool isImmune)
    {
        float duration = 1f;
        float elapsed = 0f;
        Vector3 originalPos = m_DemageText.rectTransform.localPosition;
        Color originalColor = m_DemageText.color;
        float floatDistance = isImmune ? 0f : 50f;

        float startScale = criticalLevel switch
        {
            CriticalLevel.Extreme => 2.2f,
            CriticalLevel.Super => 1.8f,
            CriticalLevel.Normal => 1.5f,
            _ => 1f
        };

        m_DemageText.rectTransform.localScale = Vector3.one * startScale;

        // 轻微随机偏移
        Vector3 randomOffset = new Vector3(UnityEngine.Random.Range(-10f, 10f), 0, 0);
        Vector3 basePos = originalPos + randomOffset;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            if (!isImmune)
            {
                // 漂浮 + 微抖动
                float shakeX = Mathf.Sin(Time.time * 30f) * (criticalLevel != CriticalLevel.None ? 2f : 0.5f);
                m_DemageText.rectTransform.localPosition = basePos + Vector3.up * floatDistance * progress + new Vector3(shakeX, 0, 0);

                // 缩放（暴击特效更明显）
                float scale = criticalLevel != CriticalLevel.None ?
                    Mathf.Lerp(startScale, 1f, progress) :
                    Mathf.Lerp(1f, 1.2f, progress);
                m_DemageText.rectTransform.localScale = Vector3.one * scale;
            }

            // 渐隐
            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(1f, 0f, progress);
            m_DemageText.color = newColor;

            yield return null;
        }

        m_DemageText.rectTransform.localPosition = originalPos;
        m_DemageText.rectTransform.localScale = Vector3.one;
        m_DemageText.color = originalColor;
        m_DemageText.text = "";

        //自动回收自身
        EnemyUIManager.instance.RemoveEnemyUI(name, this);
    }
}

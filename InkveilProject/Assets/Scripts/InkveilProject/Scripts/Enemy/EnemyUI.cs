using Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum CriticalLevel
{
    None,
    Normal,     // ��ͨ��������ɫ��
    Super,      // ����������ɫ��
    Extreme     // ���ޱ�������ɫ��
}


/// <summary>
/// ����UI
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
    /// ���º���
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
    /// �����յ��˺�
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
                    m_DemageText.color = Color.red; // ��ɫ
                    break;
                case CriticalLevel.Super:
                    m_DemageText.text = $"{damage:F0}";
                    m_DemageText.color = Color.red; // ��ɫ
                    break;
                case CriticalLevel.Normal:
                    m_DemageText.text = $"{damage:F0}";
                    m_DemageText.color = Color.red; // ��ɫ
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
    /// �ı�����
    /// </summary>
    /// <param name="criticalLevel">�ı�����</param>
    /// <param name="isImmune">�Ƿ������˺�</param>
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

        // ��΢���ƫ��
        Vector3 randomOffset = new Vector3(UnityEngine.Random.Range(-10f, 10f), 0, 0);
        Vector3 basePos = originalPos + randomOffset;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            if (!isImmune)
            {
                // Ư�� + ΢����
                float shakeX = Mathf.Sin(Time.time * 30f) * (criticalLevel != CriticalLevel.None ? 2f : 0.5f);
                m_DemageText.rectTransform.localPosition = basePos + Vector3.up * floatDistance * progress + new Vector3(shakeX, 0, 0);

                // ���ţ�������Ч�����ԣ�
                float scale = criticalLevel != CriticalLevel.None ?
                    Mathf.Lerp(startScale, 1f, progress) :
                    Mathf.Lerp(1f, 1.2f, progress);
                m_DemageText.rectTransform.localScale = Vector3.one * scale;
            }

            // ����
            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(1f, 0f, progress);
            m_DemageText.color = newColor;

            yield return null;
        }

        m_DemageText.rectTransform.localPosition = originalPos;
        m_DemageText.rectTransform.localScale = Vector3.one;
        m_DemageText.color = originalColor;
        m_DemageText.text = "";

        //�Զ���������
        EnemyUIManager.instance.RemoveEnemyUI(name, this);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PalyerBloodUI : MonoBehaviour
{

    public Image m_ShieldHealthImagae;
    public Image m_HitImagae;

    // Start is called before the first frame update
    void Start()
    {
        PlayerController.instance.onSkillBar += OnSkillBarHandler;
        //PlayerController.instance.onDamage += OnDamageHandler;
        //PlayerController.instance.onInitial += OnDamageHandler;
    }

    private void OnSkillBarHandler(float obj)
    {
        m_ShieldHealthImagae.fillAmount = obj;
    }

    private void OnDestroy()
    {
        PlayerController.instance.onSkillBar -= OnSkillBarHandler;
        //PlayerController.instance.onDamage -= OnDamageHandler;
        //PlayerController.instance.onInitial -= OnDamageHandler;
    }

    private void OnDamageHandler()
    {
        m_ShieldHealthImagae.fillAmount = PlayerController.instance.ShieldHealth;
        m_HitImagae.fillAmount = PlayerController.instance.HitPoints;
    }
}

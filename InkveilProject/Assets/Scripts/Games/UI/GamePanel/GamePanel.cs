using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : BaseUI
{
   /// <summary>
   /// 玩家血量
   /// </summary>
   [Header("玩家血量")]
   public Image m_BloodVolumeFill;
   
   /// <summary>
   /// 敌人血量_1
   /// </summary>
   [Header("敌人血量_1")]
   public Image m_BloodVolumeEnemy_1Fill;
   
   /// <summary>
   /// 敌人血量_2
   /// </summary>
   [Header("玩家血量")]
   public Image m_BloodVolumeEnemy_2Fill;
   
   /// <summary>
   /// 敌人血量_3
   /// </summary>
   [Header("玩家血量")]
   public Image m_BloodVolumeEnemy_3Fill;

   /// <summary>
   /// 武器按键链表
   /// </summary>
   [Header("武器按键链表")]
   public List<Button> m_WeaponBtnlist;

   /// <summary>
   /// 向左按键
   /// </summary>
   [Header("向左按键")]
   public Button m_LeftBtn;
   
   /// <summary>
   /// 向右按键
   /// </summary>
   [Header("向右按键")]
   public Button m_RightBtn;

   /// <summary>
   /// 特效图标
   /// </summary>
   [Header("特效图标")]
   public Image m_EffectIamge;

   /// <summary>
   /// 请神按键
   /// </summary>
   [Header("请神按键")]
   public Button m_InviteGodBtn;

   /// <summary>
   /// 初始化函数
   /// </summary>
   protected override void OnInit()
   {
      
   }

   /// <summary>
   /// 显示界面
   /// </summary>
   protected override void OnShowEnable()
   {
      base.OnShowEnable();
      m_LeftBtn.onClick.AddListener(OnLeftBtnClick);
      m_InviteGodBtn.onClick.AddListener(OnInviteGodBtnClick);
      m_RightBtn.onClick.AddListener(OnRightBtnClick);
   }

   /// <summary>
   /// 点击向右按键
   /// </summary>
   private void OnRightBtnClick()
   {
      //发送向右移动事件
      EventSystem.Broadcast(UIEventDefine.OnRightBtnClickEven);
   }

   /// <summary>
   /// 点击请神按键
   /// </summary>
   private void OnInviteGodBtnClick()
   {
      //发送请神事件
      EventSystem.Broadcast(UIEventDefine.OnInviteGodBtnClickEven);
   }

   /// <summary>
   /// 点击向左按键
   /// </summary>
   private void OnLeftBtnClick()
   {
      //发送向左移动事件
      EventSystem.Broadcast(UIEventDefine.OnLeftBtnClickEven);
   }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WKSkillAttackCtrl : GodAttackCtrl
{
    [Header("��������")]
    [SerializeField] private float waitSkill = 1; // �ȴ�ʱ��

    private GodInfo _godInfo;
    private Transform _target;

    internal override void OnInitialFlag(GodInfo godInfo, Transform tf1, Transform tf2)
    {
        base.OnInitialFlag(godInfo, tf1, tf2);
        _godInfo = godInfo;
        _target = tf2;

        // �ƶ���Ŀ��λ��
        //transform.position = new Vector3(0,3, 5);

        // ����ִ�й������
        StartCoroutine(AttackWithRangeCheck());
    }

    private IEnumerator AttackWithRangeCheck()
    {
        yield return new WaitForSeconds(waitSkill);

        for (int i = 0; i < EnemyManager.instance.ActiveEnemyCount; i++)
        {
            EnemyManager.instance.EnemyList[i].TakeDamage((float)_godInfo.skillDamageMulti);
        }
    }

    private void OnEnable()
    {
        Invoke(nameof(Hide), 2);
    }

    private void Hide()
    {
        Destroy(gameObject);
    }

}

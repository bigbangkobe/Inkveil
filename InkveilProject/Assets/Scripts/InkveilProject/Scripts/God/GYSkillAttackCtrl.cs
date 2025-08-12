using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GYSkillAttackCtrl : GodAttackCtrl
{
    [Header("��������")]
    [SerializeField] private float waitSkill = 1; // �ȴ�ʱ��
    [SerializeField] private float w = 3; // ��
    [SerializeField] private float h = 8; // ��

    private GodInfo _godInfo;
    private Transform _target;

    internal override void OnInitialFlag(GodInfo godInfo, Transform tf1, Transform tf2)
    {
        base.OnInitialFlag(godInfo, tf1, tf2);
        _godInfo = godInfo;
        _target = tf2;

        // �ƶ���Ŀ��λ��
        transform.position =new Vector3(0,0.5f,5);

        // ����ִ�й������
        StartCoroutine(AttackWithRangeCheck());
    }

    private IEnumerator AttackWithRangeCheck()
    {
        yield return new WaitForSeconds(waitSkill);

        for (int i = 0; i < EnemyManager.instance.ActiveEnemyCount; i++) 
        {
            Vector3 vector = EnemyManager.instance.EnemyList[i].transform.position - transform.position;

            if (Mathf.Abs( vector.x) <= w && Mathf.Abs(vector.y) <= h) 
            {
                EnemyManager.instance.EnemyList[i].TakeDamage((float)_godInfo.skillDamageMulti);
            }
        }
    }

    private void OnEnable()
    {
        Invoke(nameof(Hide), 3);
    }

    private void Hide()
    {
        Destroy(gameObject);
    }
}
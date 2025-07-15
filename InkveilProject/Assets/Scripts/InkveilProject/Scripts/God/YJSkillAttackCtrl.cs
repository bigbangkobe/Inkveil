using System.Collections;
using UnityEngine;

public class YJSkillAttackCtrl : GodAttackCtrl
{
    [Header("攻击参数")]
    [SerializeField] private float criticalRange = 1f; // 暴击范围
    [SerializeField] private float criticalMultiplier = 2f; // 暴击倍率
    [SerializeField] private float waitSkill = 1; // 等待时间

    private GodInfo _godInfo;
    private Transform _target;

    internal override void OnInitialFlag(GodInfo godInfo, Transform tf1, Transform tf2)
    {
        base.OnInitialFlag(godInfo, tf1, tf2);
        _godInfo = godInfo;
        _target = tf2;

        // 移动到目标位置
        transform.position = tf2.position;

        // 立即执行攻击检测
        StartCoroutine(AttackWithRangeCheck());
    }

    private IEnumerator AttackWithRangeCheck()
    {
        // 等待一帧确保所有对象初始化完成
        yield return new WaitForSeconds(waitSkill);

        // 检测范围内所有敌人
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, (float)_godInfo.skillRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                float damage = (float)_godInfo.skillDamageMulti;

                // 近距离伤害翻倍
                if (distance <= 1)
                {
                    damage *= criticalMultiplier;
                    Debug.Log($"暴击伤害: {damage} (距离: {distance})");
                }
                else
                {
                    Debug.Log($"普通伤害: {damage} (距离: {distance})");
                }

                // 实际造成伤害
                hitCollider.GetComponent<EnemyBase>()?.TakeDamage(damage);
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
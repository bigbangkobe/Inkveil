using System.Collections;
using UnityEngine;

public class LZBaseAttackCtrl : GodAttackCtrl
{
    internal override void OnInitialFlag(GodInfo godInfo, Transform tf1, Transform tf2)
    {
        base.OnInitialFlag(godInfo, tf1, tf2);

        transform.position = tf2.position;
        tf2.GetComponent<EnemyBase>().TakeDamage(godInfo.baseAttack);
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
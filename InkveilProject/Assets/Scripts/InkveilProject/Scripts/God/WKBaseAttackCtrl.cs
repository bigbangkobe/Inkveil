using System.Collections;
using UnityEngine;

public class WKBaseAttackCtrl : GodAttackCtrl
{
    public LayerMask enemyLayer; // Layer mask to detect enemies


    private Coroutine damageCoroutine;

    internal override void OnInitialFlag(GodInfo godInfo, Transform tf1, Transform tf2)
    {
        base.OnInitialFlag(godInfo, tf1, tf2);
        transform.position = tf2.position;

        // Start the attack immediately
        StartAttack();
    }

    private void StartAttack()
    {
        // Deal initial damage
        DealAreaDamage();

        // Start continuous damage if enabled
        damageCoroutine = StartCoroutine(ContinuousDamage());
    }

    private void DealAreaDamage()
    {
        // Find all enemies in radius
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, 1, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            // Apply damage to each enemy
            EnemyBase enemyHealth = enemy.GetComponent<EnemyBase>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(_godInfo.baseAttack);
            }
        }
    }

    private IEnumerator ContinuousDamage()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            DealAreaDamage();
        }
    }

    private void OnEnable()
    {
        // You might want to adjust the hide time based on attack duration
        Invoke(nameof(Hide), 2);
    }

    private void OnDisable()
    {
        // Stop continuous damage when disabled
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
        }
    }

    private void Hide()
    {
        Destroy(gameObject);
    }
}
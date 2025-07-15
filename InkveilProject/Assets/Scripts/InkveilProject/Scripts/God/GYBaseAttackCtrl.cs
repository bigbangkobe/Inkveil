using System.Collections;
using UnityEngine;

public class GYBaseAttackCtrl : GodAttackCtrl
{
    public float moveSpeed = 5f; 

    private Vector3 target;
    private bool isMoving = false;

    internal override void OnInitialFlag(GodInfo godInfo, Transform tf1, Transform tf2)
    {
        base.OnInitialFlag(godInfo, tf1, tf2);
        target = tf1.forward; 

        isMoving = true;

        transform.position = tf1.position;
    }

    private void Update()
    {
        if (GameManager.instance.GameStateEnum != GameConfig.GameState.State.Play) return;
        if (!isMoving) return;
        transform.position += target * moveSpeed * Time.deltaTime;
    }

    private void OnEnable()
    {
        Invoke(nameof(Hide), 10);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyBase>().TakeDamage((float)_godInfo.baseAttack);
        }
    }

    private void Hide()
    {
        Destroy(gameObject);
    }
}
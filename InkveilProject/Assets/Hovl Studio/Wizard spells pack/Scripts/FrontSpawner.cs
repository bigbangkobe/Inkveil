using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontSpawner : MonoBehaviour 
{
    public Transform pivot;
    public float speed = 15f;
    public float drug = 1f;
    public float repeatingTime = 1f;
    public GameObject craterPrefab;
    public float spawnRate = 1f;
    public float spawnDuration = 1f;
    public float positionOffset = 0f;
    public bool changeScale = false;
    public GodAttackCtrl godAttackCtrl;
    private float randomTimer = 0f;

    private float startSpeed = 0f;
    private float spawnDur;
    private Vector3 stepPosition;

    void Start()
    {
        InvokeRepeating("StartAgain", 0f, repeatingTime);
        startSpeed = speed;
        stepPosition = pivot.position;
        spawnDur = spawnDuration;
    }

    void StartAgain()
    {
        startSpeed = speed;
        transform.position = pivot.position;
        stepPosition = pivot.position;
        spawnDur = spawnDuration;
        randomTimer = 0;
    }

    void Update()
    {
        if (GameManager.instance.GameStateEnum != GameConfig.GameState.State.Play) return;
        spawnDur -= Time.deltaTime;
        randomTimer += (Time.deltaTime * 2);
        startSpeed = startSpeed * drug;
        transform.position += transform.forward * (startSpeed * Time.deltaTime);

        var heading = transform.position - stepPosition;
        var distance = heading.magnitude;
        if (distance > spawnRate && spawnDur > 0)
        {
            if (craterPrefab != null)
            {
                Vector3 randomPosition = new Vector3(Random.Range(-positionOffset, positionOffset), 0, Random.Range(-positionOffset, positionOffset));
                Vector3 pos = transform.position + (randomPosition * randomTimer);
                if (Terrain.activeTerrain != null)
                {
                    pos.y = Terrain.activeTerrain.SampleHeight(transform.position);
                }
                var craterInstance = Instantiate(craterPrefab, pos, Quaternion.identity);
                if (changeScale == true) { craterInstance.transform.localScale += new Vector3(randomTimer, randomTimer, randomTimer); }
                var craterPs = craterInstance.GetComponent<ParticleSystem>();
                if (craterPs != null)
                {
                    Destroy(craterInstance, craterPs.main.duration);
                }
                else
                {
                    var flashPsParts = craterInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                    Destroy(craterInstance, flashPsParts.main.duration);
                }
            }
            //distance = 0;
            stepPosition = transform.position;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the object is an enemy (you might need to adjust this check)
        if (other.CompareTag("Enemy"))
        {
            // Apply damage to the enemy
            other.GetComponent<EnemyBase>().TakeDamage((float)godAttackCtrl._godInfo.skillDamageMulti);
        }
    }

}

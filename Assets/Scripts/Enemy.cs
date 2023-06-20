using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Range(0,10)] public float moveSpeed;
    public float hp = 150;
    private float totalHp;
    public GameObject explosionEffect;
    private Slider hpSlider;
    private Transform[] positions;
    private int index = 0;

    private int i = 0; 
    void Start () {
        positions = Waypoints.positions;
        totalHp = hp;
        hpSlider = GetComponentInChildren<Slider>();
	}
    private void Update() 
    {
        if (GameObject.Find("Map Generator").GetComponent<MapGenerator>().way[0] != null)
            Move();  
    }

    private void Move()
    {
        GameObject[] path = GameObject.Find("Map Generator").GetComponent<MapGenerator>().way;
        transform.position = Vector3.MoveTowards(transform.position, path[i].transform.position, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, path[i].transform.position) < 0.01f && path[i + 1] != null)
        {
            i++;
            Debug.Log(i);
        }
        else if (path[i + 1] == null)
            ReachDestination();
    } 

    void ReachDestination()
    {
        GameManager.Instance.Failed();
        GameObject.Destroy(this.gameObject);
    }


    void OnDestroy()
    {
        EnemySpawner.CountEnemyAlive--;
    }

    public void TakeDamage(float damage)
    {
        if (hp <= 0) return;
        hp -= damage;
        hpSlider.value = (float)hp / totalHp;
        if (hp <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        GameObject effect = GameObject.Instantiate(explosionEffect, transform.position, transform.rotation);
        Destroy(effect, 1.5f);
        Destroy(this.gameObject);
    }

}

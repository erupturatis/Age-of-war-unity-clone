using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    [HideInInspector] public Data.TurretData TD;
    [HideInInspector] public int damage;
    [HideInInspector] public bool isPlayer;

    [HideInInspector] public Vector2 direction;
    [HideInInspector] public Data data;
    [HideInInspector] public GameManager game_manger;
    private void Start()
    {
        StartCoroutine(AutoDestroy());
    }
    [HideInInspector] public bool gravity = false;

    private void Update()
    {
        
        Vector3 ndir = new Vector3(direction.x, direction.y, 0f);
       
        gameObject.transform.position += ndir * 250f / data.COEFF * Time.deltaTime;
        if (gravity == true) 
        {
            direction.y -= Time.deltaTime ;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject gm = collision.gameObject;
        Troop tr = gm.GetComponent<Troop>();
        if (tr != null)
        {
            if (!tr.isPlayer && isPlayer)
            {
                if (TD != null)
                {
                    tr.troop_data.health -= TD.damage;
                    if (TD.makes_fragments)
                    {
                       
                        // additional instatiating
                        Vector2 dir = new Vector2(Random.Range(-1f,1f), 1f);
                        Vector2 spawn = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 1f);
                        game_manger.spawn_bullet(dir, spawn, TD.fragment_damage, true, true);
                        dir = new Vector2(Random.Range(-1f, 1f), 1f);
                        game_manger.spawn_bullet(dir, spawn, TD.fragment_damage, true, true);
                        dir = new Vector2(Random.Range(-1f, 1f), 1f);
                        game_manger.spawn_bullet(dir, spawn, TD.fragment_damage, true, true);
                    }
                }
                else
                {
                    tr.troop_data.health -= damage;
                }
                Destroy(gameObject);
            }
            if (tr.isPlayer && !isPlayer)
            {
                if (TD != null)
                {
                    tr.troop_data.health -= TD.damage;
                    if (TD.makes_fragments)
                    {
                        // additional instatiating
                        Vector2 dir = new Vector2(Random.Range(-1f, 1f), 1f);
                        Vector2 spawn = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 1f);
                        game_manger.spawn_bullet(dir, spawn, TD.fragment_damage, true, false);
                        dir = new Vector2(Random.Range(-1f, 1f), 1f);
                        game_manger.spawn_bullet(dir, spawn, TD.fragment_damage, true, false);
                        dir = new Vector2(Random.Range(-1f, 1f), 1f);
                        game_manger.spawn_bullet(dir, spawn, TD.fragment_damage, true, false);
                    }
                }
                else
                {
                    tr.troop_data.health -= damage;
                }
                Destroy(gameObject);
            }
        }
    }

    IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(3.5f);
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public Data.TurretData TD;
    public int damage;
    public bool isPlayer;
    [HideInInspector]
    public Vector2 direction;
    public Data data;
    private void Start()
    {
        StartCoroutine(AutoDestroy());
    }
    private void Update()
    {
        
        Vector3 ndir = new Vector3(direction.x, direction.y, 0f);
       
        gameObject.transform.position += ndir * 250f / data.COEFF * Time.deltaTime;
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

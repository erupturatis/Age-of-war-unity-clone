using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public Data.TurretData TD;
    [HideInInspector]
    public Vector2 direction;
    Data data;
    private void Start()
    {
        StartCoroutine(AutoDestroy());
        data = new Data();
    }
    private void Update()
    {
        
        Vector3 ndir = new Vector3(direction.x, direction.y, 0f);
       
        gameObject.transform.position += ndir * TD.bullet_speed / data.COEFF * Time.deltaTime;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject gm = collision.gameObject;
        Troop tr = gm.GetComponent<Troop>();
        tr.troop_data.health -= TD.damage;
        if (TD.makes_fragments)
        {
            // additional instatiating
        }
        Destroy(gameObject);
    }

    IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}

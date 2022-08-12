using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Turret : MonoBehaviour
{
    [HideInInspector] public bool isPlayer;

    [HideInInspector] public GameManager game_manager;
    [HideInInspector] public Data.TurretData turret_data; // has all the data for the current turret
    [HideInInspector] public GameObject attacked_gm;


    bool attacking_range;
    bool range_routine;
    [SerializeField] GameObject bullet;
    [SerializeField] TextMeshProUGUI attacking;
    [SerializeField] GameObject local_canvas;

    SpriteRenderer spriteR;

    [System.NonSerialized]
    public bool info = true;

    Data data;


    void check_attacking()
    {
        attacking_range = false;
        if (isPlayer)
        {
            if (game_manager.enemy_troops_queue.Count > 0)
            {
                // if there are enemies
                GameObject first_enemy = game_manager.enemy_troops_queue[0];

                if (Vector2.Distance(first_enemy.transform.position , gameObject.transform.position) * data.COEFF <= turret_data.range)
                {
                    attacking_range = true;
                    attacked_gm = first_enemy;
                }
            }
        }
        else
        {
            if (game_manager.player_troops_queue.Count > 0)
            {
                // if there are enemies
                GameObject first_enemy = game_manager.player_troops_queue[0];

                if (Vector2.Distance(first_enemy.transform.position, gameObject.transform.position) * data.COEFF <= turret_data.range)
                {
                    attacking_range = true;
                    attacked_gm = first_enemy;
                }

            }
        }
    }

    void try_attacking()
    {
        if (attacking_range)
        {
            if (!range_routine)
            {
                //an attacking routine is not already running 
                 StartCoroutine(attack_range(turret_data.first_speed));
            }
        }
        
    }

    void rotate_turret()
    {
        if (attacking_range)
        {
            GameObject first_enemy;
            if (isPlayer)
            {
                first_enemy = game_manager.enemy_troops_queue[0];       
            }
            else
            {
                first_enemy = game_manager.player_troops_queue[0];   
            }
            Vector2 dir = (first_enemy.transform.position - gameObject.transform.position);
            dir = dir.normalized;
        
        }
    }

    void spawn_bullet()
    {
       
        GameObject first_enemy;
        if (isPlayer)
        {
            first_enemy = game_manager.enemy_troops_queue[0];
        }
        else
        {
            first_enemy = game_manager.player_troops_queue[0];
        }
        Vector2 dir = (first_enemy.transform.position - gameObject.transform.position);
        dir = dir.normalized;
        GameObject bulletgm = Instantiate(bullet, transform.position, transform.rotation);
        Bullet b = bulletgm.GetComponent<Bullet>();
        b.direction = dir;
        b.TD = turret_data;
        
    }

    IEnumerator attack_range(float cooldown)
    {
        range_routine = true;
        yield return new WaitForSeconds(cooldown);
        if (attacking_range)
        {
            spawn_bullet();
            StartCoroutine(attack_range(turret_data.speed + turret_data.additional_speed/data.FPS));
        }
        else
        {
            range_routine = false;
        }
    }
    void manage_sprites()
    {
        int id = turret_data.id;
        spriteR = gameObject.GetComponent<SpriteRenderer>();
        spriteR.sprite = data.turret_sprites[id];
    }

    void Start()
    {
        data = game_manager.data_object;
        if (info)
        {
            local_canvas.SetActive(true);
        }
        else
        {
            local_canvas.SetActive(false);
        }
        manage_sprites();
    }

    // Update is called once per frame
    void Update()
    {
        rotate_turret();
        check_attacking();
        try_attacking();
        
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Troop : MonoBehaviour
{

    [HideInInspector] public bool isPlayer;

    [HideInInspector] public GameManager game_manager;
    [HideInInspector] public Data.TroopData troop_data; // has all the data for the troops
    [HideInInspector] public GameObject next_troop; 
    [HideInInspector] public GameObject attacked_gm;

    bool is_moving;
    bool attacking_range;
    bool attacking_melee;
    bool melee_routine;
    bool range_routine;

    [SerializeField]
    TextMeshProUGUI hp, attacking, moving, additonal;
    [SerializeField]
    GameObject local_canvas;
    SpriteRenderer spriteR;

    [System.NonSerialized]
    public bool info = true;

    Data data;

    void manage_texts()
    {
        int health = troop_data.health;
        hp.text = "" + health;
        moving.text = "" + is_moving;
        attacking.text = "" + attacking_melee + "\n" + attacking_range;
        
    }

    void manage_sprites()
    {
        int id = troop_data.id;
        spriteR = gameObject.GetComponent<SpriteRenderer>();
        spriteR.sprite = data.troop_sprites[id]; 
    }

    
    void check_moving()
    {
        is_moving = true;
        if (isPlayer)
        {
            if (next_troop == null)
            {
                if (gameObject.transform.position.x >= 9f)
                {
                    is_moving = false;
                }
                if (game_manager.enemy_troops_queue.Count > 0)
                {
                    GameObject first_enemy = game_manager.enemy_troops_queue.Peek();
                    additonal.text = "" + (first_enemy.transform.position.x - gameObject.transform.position.x) * data.COEFF + "   " + (first_enemy.transform.position.x - gameObject.transform.position.x);

                    if ((first_enemy.transform.position.x - gameObject.transform.position.x) * data.COEFF <= data.MIN_DISTANCE)
                    {
                        is_moving = false;
                    }
                }
                
            }
            else
            {
                Troop nxt = next_troop.GetComponent<Troop>();

                if ((next_troop.transform.position.x - gameObject.transform.position.x) * data.COEFF - nxt.troop_data.length <= data.MIN_DISTANCE)
                {
                    is_moving = false;
                }
            }
        }
        else
        {
            if (next_troop == null)
            {
                if (gameObject.transform.position.x <= -9f)
                {
                    is_moving = false;
                }
                if (game_manager.player_troops_queue.Count > 0)
                {
                    GameObject first_enemy = game_manager.player_troops_queue.Peek();
                    if ((gameObject.transform.position.x - first_enemy.transform.position.x) * data.COEFF <= data.MIN_DISTANCE)
                    {
                        is_moving = false;
                    }
                }
            }
            else
            {
                Troop nxt = next_troop.GetComponent<Troop>();
                if ((gameObject.transform.position.x - next_troop.transform.position.x) * data.COEFF - nxt.troop_data.length <= data.MIN_DISTANCE)
                {
                    is_moving = false;
                }
            }
        }
    }

    void check_attacking()
    {
        if (isPlayer)
        {
            if (game_manager.enemy_troops_queue.Count > 0)
            {
                // if there are enemies
                GameObject first_enemy = game_manager.enemy_troops_queue.Peek();
                
                if ((first_enemy.transform.position.x - gameObject.transform.position.x) * data.COEFF - data.MIN_DISTANCE <= troop_data.range_ranged)
                {
                    attacking_range = true;
                    attacked_gm = first_enemy;
                }
                if ((first_enemy.transform.position.x - gameObject.transform.position.x) * data.COEFF <= troop_data.range_melee)
                {
                    attacking_melee = true;
                    attacked_gm = first_enemy;
                }
                
            }
            else
            {
                //checking base attacking
                if ((9f - gameObject.transform.position.x) * data.COEFF <= troop_data.range_ranged)
                {
                    attacking_range = true;
                }
                if ((9f - gameObject.transform.position.x) * data.COEFF <= troop_data.range_melee)
                {
                    attacking_melee = true;
                }
            }
        }
        else
        {
            if (game_manager.enemy_troops_queue.Count > 0)
            {
                // if there are enemies
                GameObject first_enemy = game_manager.player_troops_queue.Peek();

                if ((gameObject.transform.position.x - first_enemy.transform.position.x ) * data.COEFF - data.MIN_DISTANCE <= troop_data.range_ranged)
                {
                    attacking_range = true;
                    attacked_gm = first_enemy;
                }
                if ((gameObject.transform.position.x - first_enemy.transform.position.x) * data.COEFF <= troop_data.range_melee)
                {
                    attacking_melee = true;
                    attacked_gm = first_enemy;
                }

            }
            else
            {
                //checking base attacking
                if ((gameObject.transform.position.x + 9f) * data.COEFF <= troop_data.range_ranged)
                {
                    attacking_range = true;
                }
                if ((gameObject.transform.position.x + 9f) * data.COEFF <= troop_data.range_melee)
                {
                    attacking_melee = true;
                }
            }
        }
    }
    
    void try_moving()
    {
        if (is_moving)
        {
            float speed = troop_data.speed / data.COEFF;
            if (!isPlayer)
            {
                speed *= -1;
            }
            gameObject.transform.position += new Vector3(speed, 0f, 0f) * Time.deltaTime;
        }
    }
    void Start()
    {
        data = game_manager.data_object;

        if (!isPlayer && !info)
        {
            gameObject.transform.localScale = new Vector3(-gameObject.transform.localScale.x, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
        }

        if (!info)
        {
            local_canvas.SetActive(false);
        }
        else
        {
            local_canvas.SetActive(true);
        }
        manage_sprites();
    }
    // Update is called once per frame
    void Update()
    {

        if (info)
        {
            manage_texts();
        }
        check_moving();
        check_attacking();
        try_moving();
    }
}

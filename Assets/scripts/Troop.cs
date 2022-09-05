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
    [HideInInspector] public GameObject prev_troop;
    [HideInInspector] public GameObject attacked_gm;
    [HideInInspector] public bool is_regenerating = false;
    [HideInInspector] public int max_health;

    bool is_moving;
    bool attacking_range = false;
    bool attacking_melee = false;
    bool melee_routine = false;
    bool range_routine = false;
    

    [SerializeField]
    TextMeshProUGUI hp, attacking, moving, additonal;
    [SerializeField]
    GameObject local_canvas;
    [SerializeField] SpriteRenderer spriteR;
    [SerializeField] BoxCollider2D Box;


    Coroutine inst_melee;
    Coroutine inst_range;

    [System.NonSerialized]
    public bool info = true;

    Data data;

    void manage_texts()
    {
        
        if (info)
        {
          
            int health = troop_data.health;
            hp.text = "" + health;
            moving.text = "" + is_moving;
            attacking.text = "" + attacking_melee + "\n" + attacking_range;
        }
    }

    void manage_sprites()
    {
        int id = troop_data.id;
        
        spriteR.sprite = data.troop_sprites[id]; 
    }

    
    
    void check_moving()
    {
        is_moving = true;
        //
        if (isPlayer)
        {
            if (next_troop == null)
            {
                if (gameObject.transform.localPosition.x >= 9f)
                {
                    is_moving = false;
                }
                if (game_manager.enemy_troops_queue.Count > 0)
                {
                    GameObject first_enemy = game_manager.enemy_troops_queue[0];
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
                if (gameObject.transform.localPosition.x <= -9f)
                {
                    is_moving = false;
                }
                if (game_manager.player_troops_queue.Count > 0)
                {
                    GameObject first_enemy = game_manager.player_troops_queue[0];
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
        attacking_melee = false;
        attacking_range = false;
        if (isPlayer)
        {
            if (game_manager.enemy_troops_queue.Count > 0)
            {
                // if there are enemies
                GameObject first_enemy = game_manager.enemy_troops_queue[0];
                
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
                if ((9f - gameObject.transform.localPosition.x) * data.COEFF <= troop_data.range_ranged)
                {
                    attacking_range = true;
                }
                if ((9f - gameObject.transform.localPosition.x) * data.COEFF <= troop_data.range_melee)
                {
                    attacking_melee = true;
                }
            }
        }
        else
        {
            if (game_manager.player_troops_queue.Count > 0)
            {
                // if there are enemies
                GameObject first_enemy = game_manager.player_troops_queue[0];

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
                if ((gameObject.transform.localPosition.x + 9f) * data.COEFF <= troop_data.range_ranged)
                {
                    attacking_range = true;
                }
                if ((gameObject.transform.localPosition.x + 9f) * data.COEFF <= troop_data.range_melee)
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

    void try_attacking()
    {
        if (attacking_melee)
        {
            if (!melee_routine)
            {
                //an attacking routine is not already running 
                inst_melee = StartCoroutine(attack_melee(troop_data.melee_first_speed));
            }
        }
        else
        {
            if (attacking_range)
            {
                if (!range_routine)
                {
                    inst_range = StartCoroutine(attack_range(troop_data.ranged_first_speed));
                }
            }
           
        }
    }


    void try_dying()
    {
        if (troop_data.health <= 0)
        {

            //print("troop died");
            if (isPlayer)
            {
                if (troop_data.id == 15)
                {
                    game_manager.player_troops[3] -= 1;
                }
                else
                {
                    game_manager.player_troops[troop_data.id % 3] -= 1;
                }
                game_manager.player_troops_queue.Remove(gameObject);
                int reward = Mathf.RoundToInt(1.3f * troop_data.cost);
                
                game_manager.xp += reward / 2;
                
                

                //print("new count player" + game_manager.player_troops_queue.Count);
            }
            else
            {
                game_manager.enemy_troops[troop_data.id % 3] -= 1;
                game_manager.enemy_troops_queue.Remove(gameObject);
                int reward = Mathf.RoundToInt(1.3f * troop_data.cost);
                game_manager.money += reward;
                game_manager.xp += reward * 2;
                
                //print("new count enemies" + game_manager.enemy_troops_queue.Count);
            }
            if (prev_troop != null)
            {
                Troop prev_tr = prev_troop.GetComponent<Troop>();
                prev_tr.next_troop = next_troop;
            }
            if (next_troop != null && prev_troop != null)
            {
                Troop next_tr = next_troop.GetComponent<Troop>();
                next_tr.prev_troop = prev_troop;
            }
            Destroy(gameObject);
            
        }
    }



    void give_damage(int damage)
    {
        if (attacking_range || attacking_melee)
        {
            if (isPlayer)
            {
                //gives damage to first enemy or enemy base
                if (game_manager.enemy_troops_queue.Count > 0)
                {
                    //there is a troop alive
                    Troop enemy_script = game_manager.enemy_troops_queue[0].GetComponent<Troop>();
                    enemy_script.troop_data.health -= damage;
                    if (enemy_script.troop_data.health <= 0)
                    {
                        attacking_melee = false;
                        is_moving = true;
                        attacking_range = false;
                    }
                }
                else
                {
                    //damages enemy base
                    Base b = game_manager.enemy_base.GetComponent<Base>();
                    b.take_damage(damage);
                }
            }
            else
            {
                //same as above but for the enemy
                if (game_manager.player_troops_queue.Count > 0)
                {
                    //there is a troop alive
                    Troop enemy_script = game_manager.player_troops_queue[0].GetComponent<Troop>();
                    enemy_script.troop_data.health -= damage;
                    if (enemy_script.troop_data.health <= 0)
                    {
                        attacking_melee = false;
                        is_moving = true;
                        attacking_range = false;
                    }
                }
                else
                {
                    //damages enemy base
                    Base b = game_manager.player_base.GetComponent<Base>();
                    b.take_damage(damage);
                }
            }
        }
    }

    IEnumerator attack_melee(float cooldown)
    {
        melee_routine = true;
        yield return new WaitForSeconds(cooldown);
        give_damage(troop_data.melee_damage);
        if (attacking_melee)
        {
            StartCoroutine(attack_melee(troop_data.melee_speed + troop_data.attack_pause / data.FPS));
        }
        else
        {
            melee_routine = false;
        }
    }

    IEnumerator attack_range(float cooldown)
    {
        range_routine = true;
        yield return new WaitForSeconds(cooldown);
        if (!attacking_melee)
        {
            give_damage(troop_data.ranged_damage);
        }
        if (attacking_range)
        {
            if (!is_moving)
            {
                StartCoroutine(attack_range(troop_data.ranged_standing_speed + troop_data.attack_pause / data.FPS));
            }
            else
            {
                StartCoroutine(attack_range(troop_data.ranged_walking_speed + troop_data.attack_pause / data.FPS));
            }
        }
        else
        {
            range_routine = false;
        }
    }

    void Start()
    {
        data = game_manager.data_object;
        max_health = troop_data.health;

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
        if (troop_data.id == 2 || troop_data.id == 5)
        {
            Box.size = new Vector2(0.7f, 0.5f);
        }
        if (troop_data.id == 11 || troop_data.id == 14)
        {
            Box.size = new Vector2(1.05f, 0.5f);
        }
        manage_sprites();
        StartCoroutine(Regenerate());
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
        try_attacking();
        try_dying();
    }

    IEnumerator Regenerate()
    {
        yield return new WaitForSeconds((1/data.FPS));
        if(troop_data.health < max_health && is_regenerating)
        {
            troop_data.health += 1;
        }
        StartCoroutine(Regenerate());
    }
}

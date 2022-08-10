using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Troop : MonoBehaviour
{

    [HideInInspector] public bool isPlayer;

    [HideInInspector] public GameManager game_manager;
    [HideInInspector] public Data.TroopData troop_data; // has all the data for the troops
    [HideInInspector] public GameObject next_troop; // has all the data for the troops

    bool is_moving;
    bool attacking_range;
    bool attacking_melee;
    [SerializeField]
    TextMeshProUGUI hp, attacking, moving;
    [SerializeField]
    GameObject local_canvas;

    [HideInInspector]
    public bool info = true;

    Data data;

    void manage_texts()
    {
        int health = troop_data.health;
        hp.text = "" + health;
        moving.text = "" + is_moving;
        
    }
    void Start()
    {
        data = new Data();
        if (!info)
        {
            local_canvas.SetActive(false);
        }
        else
        {
            local_canvas.SetActive(true);
        }
    }

    void try_moving()
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
                GameObject first_enemy = game_manager.enemy_troops_queue.Peek();
                if ((first_enemy.transform.position.x - gameObject.transform.position.x ) * data.COEFF <= data.MIN_DISTANCE)
                {
                    is_moving = false;
                }
            }
            else
            {
                if ((next_troop.transform.position.x - gameObject.transform.position.x) * data.COEFF <= data.MIN_DISTANCE)
                {
                    is_moving = false;
                }
            }
        }
    }

    void try_attacking()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (info)
        {
            manage_texts();
        }
        try_moving();
    }
}

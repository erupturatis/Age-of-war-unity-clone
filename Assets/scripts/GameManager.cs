using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update

    int action = 0;
    public int player_age = 1;
    public int enemy_age = 1;
    public int difficulty = 1;

    public float ability_time = 0;
    public int money = 175;
    public int emoney = 100;

    public int xp = 0;
    public int available_slots = 1;
    public int total_slots = 1;
    public int[] slots = new int[4];
    bool printing = false;

    public int player_hp = 500;
    public int enemy_hp = 500;

    public int[] turret_age = new int[4];
    public int[] turret_tier = new int[4];

    public int[] player_troops = new int[4];
    public int[] enemy_troops = new int[4];

    //at what point on the map is the battle taking place [0,1]
    public float battle_place = 0.5f;


    GameObject Last_friendly_spawned = null;
    GameObject Last_enemy_spawned = null;
    public Queue<GameObject> player_troops_queue;
    public Queue<GameObject> enemy_troops_queue;

    [SerializeField]
    GameObject troop;

    [SerializeField]
    int id;

    public void pause_game()
    {

    }
    public void send_data()
    {

    }
    public void Receive_action()
    {

    }
    public void send_action()
    {

    }

    public void spawn_player_troop(int tier)
    {
        //tier is 0 1 or 2 (and 3 in the fifth age scenario)
        int id = (player_age - 1) * 3 + tier;
        Data.TroopData tr = new Data.TroopData();
        tr.id = id;
        tr.set_parameters();
        Vector3 trans = new Vector3(0f,0f,0f);
        GameObject gm = Instantiate(troop, trans, Quaternion.identity);
        Troop troop_script = gm.GetComponent<Troop>();

        troop_script.next_troop = Last_friendly_spawned;
        troop_script.game_manager = gameObject.GetComponent<GameManager>() ;
        troop_script.troop_data = tr;
        troop_script.isPlayer = true;
        Last_enemy_spawned = gm;
        player_troops_queue.Enqueue(gm);
          
    }

    
    


    void Start()
    {
        spawn_player_troop(id);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

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
    public List<GameObject> player_troops_queue = new List<GameObject>();
    public List<GameObject> enemy_troops_queue = new List<GameObject>();

    public GameObject player_base;
    public GameObject enemy_base;
    public Data data_object;
    private Data.Only_Data od;


    [SerializeField] GameObject troop;
    [SerializeField] GameObject turret;

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

    public void spawn_player_troop(int tier, int age = 0)
    {
        if(age == 0)
        {
            age = player_age;
        }
        //tier is 0 1 or 2 (and 3 in the fifth age scenario)
        int id = (age - 1) * 3 + tier;
        Data.TroopData tr = new Data.TroopData();
        tr.id = id;
        tr.set_parameters();
        Vector3 trans = new Vector3(-9f,-3f,0f);
        GameObject gm = Instantiate(troop, trans, Quaternion.identity);
        gm.transform.parent = gameObject.transform;
        gm.transform.localPosition = trans;
        Troop troop_script = gm.GetComponent<Troop>();

        troop_script.next_troop = Last_friendly_spawned;
        troop_script.game_manager = gameObject.GetComponent<GameManager>();

        troop_script.troop_data = tr;
        
        troop_script.isPlayer = true;
        Last_friendly_spawned = gm;
        player_troops_queue.Add(gm);
        player_troops[tier] += 1;
          
    }

    public void spawn_enemy_troop(int tier, int age = 0)
    {
        if(age == 0)
        {
            age = enemy_age;
        }
        //tier is 0 1 or 2 (and 3 in the fifth age scenario)
        int id = (age - 1) * 3 + tier;
        Data.TroopData tr = new Data.TroopData();
        
        tr.id = id;
        tr.set_parameters();
        Vector3 trans = new Vector3(9f, -3f, 0f);
        GameObject gm = Instantiate(troop, trans, Quaternion.identity);
        Troop troop_script = gm.GetComponent<Troop>();
        
        troop_script.next_troop = Last_enemy_spawned;
        troop_script.game_manager = gameObject.GetComponent<GameManager>();
        troop_script.troop_data = tr;
        
        troop_script.isPlayer = false;
        Last_enemy_spawned = gm;
        enemy_troops_queue.Add(gm);
        enemy_troops[tier] += 1;

    }
    public void upgrade_age_enemy()
    {
        enemy_age += 1; 
    }

    public void buy_turret_enemy()
    {

    }


    public void upgrade_age_player()
    {
        player_age += 1; // calling functions here?
    }
    public bool check_upgrade_age_player()
    {
        return true;
    }
    
    public void buy_slot_player()
    {

    }
    public bool check_buy_slot_player()
    {
        if(total_slots < 4)
        {
            return true;
        }
        return false;
    }

    public void buy_turret_player(int tier, int spot, int age = 0)
    {
        if (age == 0)
        {
            age = enemy_age;
        }
        //tier is 0 1 or 2 (and 3 in the fifth age scenario)
        int id = (age - 1) * 3 + tier;
        Data.TurretData tr = new Data.TurretData();
        tr.id = id;
        tr.set_parameters();

        Vector3 Vbase = player_base.transform.position;
        Vector3 trans = new Vector3(1f, new Data.Only_Data().turret_spot[spot]/data_object.COEFF, 0f);

        GameObject gm = Instantiate(turret, trans + Vbase, Quaternion.identity);
        Turret turret_script = gm.GetComponent<Turret>();


        turret_script.game_manager = gameObject.GetComponent<GameManager>();
        turret_script.turret_data = tr;
        turret_script.isPlayer = true;
    }

    public bool check_buy_turret_player(int tier)
    {
        if (available_slots > 0)
        {
            return true;
        }
        return false;
    }

    public void sell_turret_player(int spot)
    {

    }

    public bool check_sell_turret_player(int spot)
    {
        if (slots[spot] != 0)
        {
            return true;
        }
        return false;
    }

    public void use_ability()
    {
        //code
    }

    public bool check_use_ability()
    {
        if (ability_time <= 0)
        {
            return true;
        }
        return false;
    }


    public bool check_spawn_player_troop(int tier)
    {
        if(money <= od.troop_costs[(player_age-1)*3 + tier])
        {
            money -= od.troop_costs[(player_age - 1) * 3 + tier];
            return true;
        }
        return false;
    }
    


    private void Awake()
    {
        od = new Data.Only_Data();
    }

    void Start()
    {
        buy_turret_player(1,0,1);

        spawn_enemy_troop(0);
        spawn_enemy_troop(0);
        spawn_enemy_troop(0);

    }

    // Update is called once per frame
    void Update()
    {
        if (ability_time >= 0)
        {
            ability_time -= Time.deltaTime;
        }
    }
}

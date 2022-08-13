using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
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
    public float scale = 1f;

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

    public GameObject[] player_turrets = new GameObject[4];
    public GameObject[] enemy_turrets = new GameObject[4];

    public GameObject slot_gm;

    public GameObject player_base;
    public GameObject enemy_base;
    public GameObject spawn_ability;
    public Data data_object;
    private Data.Only_Data od;


    [SerializeField] GameObject troop;
    [SerializeField] GameObject turret;
    [SerializeField] GameObject bullet;

    [SerializeField] TextMeshProUGUI money_txt;
    [SerializeField] TextMeshProUGUI xp_txt;

    public void send_data()
    {

    }
    public void Receive_action()
    {

    }
    public void take_action()
    {

    }
    public void random_actions()
    {

    }

    void update_text()
    {
        money_txt.text = "" + money;
        xp_txt.text = "" + xp;
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
        money -= tr.cost;
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
        gm.transform.parent = gameObject.transform;
        gm.transform.localPosition = trans;
        troop_script.next_troop = Last_enemy_spawned;
        troop_script.game_manager = gameObject.GetComponent<GameManager>();
        troop_script.troop_data = tr;
        
        troop_script.isPlayer = false;
        Last_enemy_spawned = gm;
        enemy_troops_queue.Add(gm);
        enemy_troops[tier] += 1;

    }
    void set_timescale()
    {
        Time.timeScale = scale;
    }
    public void upgrade_age_enemy()
    {
        enemy_age += 1; 
    }
    public void upgrade_age_player()
    {
        player_age += 1; // calling functions here?
    }
    public bool check_upgrade_age_player()
    {
        if(xp >= od.xp_cost[player_age - 1])
        {
            return true;
        }
        return false;
    }
    
    public void buy_slot_player()
    {
        money -= od.slot_cost[total_slots - 1];
        Vector3 Vbase = player_base.transform.position;
        Vector3 trans = new Vector3(1f, od.turret_spot[total_slots] / data_object.COEFF, 0f);
        Instantiate(slot_gm, Vbase + trans, gameObject.transform.rotation);
        total_slots += 1;
        available_slots += 1;
        
    }
    public bool check_buy_slot_player()
    {
        if (total_slots == 4)
        {
            return false;
        }
        if (money >= od.slot_cost[total_slots - 1])
        {
            return true;
        }
        return false;
    }
    public void buy_turret_enemy(int tier, int spot, int age = 0)
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

        Vector3 Vbase = enemy_base.transform.position;
        Vector3 trans = new Vector3(1f, new Data.Only_Data().turret_spot[spot] / data_object.COEFF, 0f);

        GameObject gm = Instantiate(turret, trans + Vbase, Quaternion.identity);
        Turret turret_script = gm.GetComponent<Turret>();


        turret_script.game_manager = gameObject.GetComponent<GameManager>();
        turret_script.turret_data = tr;
        turret_script.isPlayer = false;
    }
    public void buy_turret_player(int tier, int spot, int age = 0)
    {
        if (age == 0)
        {
            age = player_age;
        }
        slots[spot] = 1;
        turret_age[spot] = player_age;
        turret_tier[spot] = tier;
        available_slots -= 1;
        money -= od.turret_cost[(player_age-1)*3 + tier];
        
        //tier is 0 1 or 2 (and 3 in the fifth age scenario)
        int id = (age - 1) * 3 + tier;
        Data.TurretData tr = new Data.TurretData();
        tr.id = id;
        tr.set_parameters();

        Vector3 Vbase = player_base.transform.position;
        Vector3 trans = new Vector3(1f, new Data.Only_Data().turret_spot[spot]/data_object.COEFF, 0f);

        GameObject gm = Instantiate(turret, trans + Vbase, Quaternion.identity);
        Turret turret_script = gm.GetComponent<Turret>();
        player_turrets[spot] = gm;

        turret_script.game_manager = gameObject.GetComponent<GameManager>();
        turret_script.turret_data = tr;
        turret_script.isPlayer = true;
    }
    public void sell_turret_player(int spot)
    {
        slots[spot] = 0;
        GameObject turret = player_turrets[spot];
        Destroy(turret);
        int new_money = new Data.Only_Data().turret_cost[(turret_age[spot] - 1)*3 + turret_tier[spot]];
        turret_age[spot] = 0;
        turret_tier[spot] = 0;
        available_slots += 1;
        new_money /= 2;
        money += new_money;
    }

    public bool check_buy_turret_player(int tier)
    {
        if (available_slots > 0)
        {
            return true;
        }
        return false;
    }

    
    public void sell_turret_enemy(int spot)
    {
        GameObject turret = enemy_turrets[spot];
        Destroy(turret);
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
        if(player_age == 1)
        {
            StartCoroutine(ability1(0));
        }else 
        if(player_age == 2)
        {
            StartCoroutine(ability2(0));
        }
        else
        if(player_age == 3)
        {
            StartCoroutine(ability3());
        }else
        if(player_age == 4)
        {
            StartCoroutine(ability4(0));
        }else
        if(player_age == 5)
        {
            StartCoroutine(ability5(0));
        }
        ability_time = 60f;
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
        if(money >= od.troop_costs[(player_age-1)*3 + tier])
        {
            return true;
        }
        return false;
    }


    void spawn_bullet(Vector2 dir, Vector2 spawn, int damage = 200)
    {
        // getting direction for bullets
        dir = dir.normalized;
        GameObject bulletgm = Instantiate(bullet, spawn, transform.rotation);
        Bullet b = bulletgm.GetComponent<Bullet>();
        b.direction = dir;
        b.damage = damage;
        b.isPlayer = true;

    }

    public void enemy_action(int action, int[] additional_param)
    {
        //spawing troop tier t
        //spawning turret of tier m on spot n
        //selling turret on spot n
        //upgrading age
        switch (action)
        {
            case 1:
                spawn_enemy_troop(additional_param[0]);
                break;
            case 2:
                buy_turret_enemy(additional_param[0], additional_param[1]);
                break;
            case 3:
                sell_turret_enemy(additional_param[0]);
                break;
            case 4:
                upgrade_age_enemy();
                break;
        }
            
    }

    public void command_spawn_troop_tier_1()
    {
        bool can = check_spawn_player_troop(0);
        if (can)
        {
            spawn_player_troop(0);
        }
    }
    public void command_spawn_troop_tier_2()
    {
        bool can = check_spawn_player_troop(1);
        if (can)
        {
            spawn_player_troop(1);
        }

    }
    public void command_spawn_troop_tier_3()
    {
        bool can = check_spawn_player_troop(2);
        if (can)
        {
            spawn_player_troop(2);
        }
    }

    public void command_spawn_turret_tier1()
    {
        bool can = check_buy_turret_player(0);
        if (can)
        {
            int spot = 0;
            for (int i = 0; i <= 3; i++)
            {
                if(slots[i] == 0)
                {
                    spot = i;
                    break;
                }
            }
            buy_turret_player(0,spot);
        }
    }
    public void command_spawn_turret_tier2()
    {
        bool can = check_buy_turret_player(1);
        if (can)
        {
            int spot = 0;
            for (int i = 0; i <= 3; i++)
            {
                if (slots[i] == 0)
                {
                    spot = i;
                    break;
                }
            }
            buy_turret_player(1, spot);
        }
    }
    public void command_spawn_turret_tier3()
    {
        bool can = check_buy_turret_player(2);
        if (can)
        {
            int spot = 0;
            for (int i = 0; i <= 3; i++)
            {
                if (slots[i] == 0)
                {
                    spot = i;
                    break;
                }
            }
            buy_turret_player(2, spot);
        }
    }

    public void command_sell_spot0()
    {
        bool can = check_sell_turret_player(0);
        print(can);
        if (can)
        {
            sell_turret_player(0);
        }
    }
    public void command_sell_spot1()
    {
        bool can = check_sell_turret_player(1);
        if (can)
        {
            sell_turret_player(1);
        }
    }
    public void command_sell_spot2()
    {
        bool can = check_sell_turret_player(2);
        if (can)
        {
            sell_turret_player(2);
        }
    }
    public void command_sell_spot3()
    {
        bool can = check_sell_turret_player(3);
        if (can)
        {
            sell_turret_player(3);
        }
    }

    public void command_upgrade_age()
    {
        bool can = check_upgrade_age_player();
        if (can)
        {
            upgrade_age_player();
        }
    }

    public void command_buy_slot()
    {
        bool can = check_buy_slot_player();
        if (can)
        {
            buy_slot_player();
        }
    }

    private void Awake()
    {
        od = new Data.Only_Data();
    }

    void Start()
    {

        spawn_enemy_troop(0);
        spawn_enemy_troop(2);
      
    }

    // Update is called once per frame
    void Update()
    {
        if (ability_time >= 0)
        {
            ability_time -= Time.deltaTime;
        }
        update_text();
        set_timescale();
    }

    IEnumerator ability1(int bullets_launched)
    {
        if(bullets_launched != 22)
        {
           
            yield return new WaitForSeconds((9 / data_object.FPS));
            Vector2 dir = new Vector2(0, -1f);
            Vector2 noise = new Vector2(Random.Range(-0.2f, 0.2f), 0f);
            dir += noise;
            Vector2 spawn = new Vector2(Random.Range(-8f, 8f), spawn_ability.transform.position.y);
            spawn_bullet(dir, spawn);
            StartCoroutine(ability1(bullets_launched + 1));
        }
    }
    IEnumerator ability2(int bullets_launched)
    {
        if (bullets_launched != 40)
        {

            yield return new WaitForSeconds((5 / data_object.FPS));
            Vector2 dir = new Vector2(0, -1f);
            Vector2 noise = new Vector2(Random.Range(-0.1f, 0.1f), 0f);
            dir += noise;
            Vector2 spawn = new Vector2(Random.Range(-8f, 8f), spawn_ability.transform.position.y);
            spawn_bullet(dir, spawn);
            StartCoroutine(ability2(bullets_launched + 1));
        }
    }
    IEnumerator ability4(int bullets_launched)
    {
        if (bullets_launched != 15)
        {

            yield return new WaitForSeconds((15 / data_object.FPS));
            Vector2 dir = new Vector2(0, -1f);
            Vector2 spawn = new Vector2(-9f + bullets_launched * 60f / data_object.COEFF, spawn_ability.transform.position.y);
            spawn_bullet(dir, spawn, 400);
            StartCoroutine(ability4(bullets_launched + 1));
        }
    }
    IEnumerator ability5(int bullets_launched)
    {
        if (bullets_launched != 18)
        {

            yield return new WaitForSeconds((5 / data_object.FPS));
            Vector2 dir = new Vector2(0, -1f);
            Vector2 spawn = new Vector2(-9f + bullets_launched * 50f / data_object.COEFF, spawn_ability.transform.position.y);
            spawn_bullet(dir, spawn, 1000);
            StartCoroutine(ability5(bullets_launched + 1));
        }
    }
    IEnumerator ability3()
    {
    
        //to be done
        yield return new WaitForSeconds(10f);
        
    }
}

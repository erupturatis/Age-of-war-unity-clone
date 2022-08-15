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

    public class troop_queue
    {
        public int tier;
        public int age;
        public int training;
    }

    public List<troop_queue> training_queue = new List<troop_queue>();

    public GameObject[] player_turrets = new GameObject[4];
    public GameObject[] enemy_turrets = new GameObject[4];

    public GameObject slot_gm;

    public GameObject player_base;
    public GameObject enemy_base;
    public GameObject spawn_ability;
    public Data data_object;
    private Data.Only_Data od;
    private Enemy_AI enemy_ai;


    [SerializeField] GameObject troop;
    [SerializeField] GameObject turret;
    [SerializeField] GameObject bullet;

    [SerializeField] TextMeshProUGUI money_txt;
    [SerializeField] TextMeshProUGUI xp_txt;

    public void send_data()
    {
        /*
         * inputs = (in_train,
         * player_health, 
         * enemy_health, 
         * money, 
         * xp, 
         * battle_place, 
         * ability, 
         * *player_troops_total,
         * *enemy_troops_total, 
         * slots_available, 
         * *age, 
         * *enemy_age, 
         * *new_turrets)
         */

    }

    public bool take_action(int action)
    {
        //actions i used in the api for the original game
        //15 actions
        /* ACTIONS_DICT = {
             "troop_tier1":self.spawn_troop1, 
             "troop_tier2":self.spawn_troop2, 
             "troop_tier3":self.spawn_troop3, 
             "troop_tier4":self.spawn_troop4,
             "buy_turret_slot":self.add_turret_slot,
             "turret_tier1":self.spawn_turret1,
             "turret_tier2":self.spawn_turret2,
             "turret_tier3":self.spawn_turret3,
             "sell_turret1":self.sell_turret1,
             "sell_turret2":self.sell_turret2,
             "sell_turret3":self.sell_turret3,
             "sell_turret4":self.sell_turret4,
             "evolve":self.upgrade_age,
             "wait":self.nothing,
             "ability":self.use_ability,
         }*/
        if(action == 0)
        {
            return command_spawn_troop_tier_1();
        }
        else
        if(action == 1)
        {
            return command_spawn_troop_tier_2();
        }
        else
        if (action == 2)
        {
            return command_spawn_troop_tier_3();
        }
        else
        if (action == 3)
        {
            return command_spawn_troop_tier_4();
        }
        else
        if (action == 4)
        {
            return command_buy_slot();
        }
        else
        if (action == 5)
        {
            return command_spawn_turret_tier1();
        }
        else
        if (action == 6)
        {
            return command_spawn_turret_tier2();
        }
        else
        if (action == 7)
        {
            return command_spawn_turret_tier3();
        }
        else
        if (action == 8)
        {
            return command_sell_spot0();
        }
        else
        if (action == 9)
        {
            return command_sell_spot1();
        }
        else
        if (action == 10)
        {
            return command_sell_spot2();
        }
        else
        if (action == 11)
        {
            return command_sell_spot3();
        }
        else
        if (action == 12)
        {
            return command_upgrade_age();
        }
        else
        if (action == 13)
        {
            return command_wait();
        }
        else
        if (action == 14)
        {
            return command_use_ability();
        }
        return false;

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
        
        Vector3 trans = new Vector3(-9f,-3f,0f);
        GameObject gm = Instantiate(troop, trans, Quaternion.identity);
        gm.transform.parent = gameObject.transform;
        gm.transform.localPosition = trans;
        Troop troop_script = gm.GetComponent<Troop>();

        troop_script.next_troop = Last_friendly_spawned;

        if (Last_friendly_spawned != null)
        {
            Troop enemy_tr = Last_friendly_spawned.GetComponent<Troop>();
            enemy_tr.prev_troop = gm;
        }

        troop_script.game_manager = gameObject.GetComponent<GameManager>();

        troop_script.troop_data = tr;
        troop_script.info = false;
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

        if (Last_enemy_spawned != null)
        {
            Troop enemy_tr = Last_enemy_spawned.GetComponent<Troop>();
            enemy_tr.prev_troop = gm;
        }

        troop_script.game_manager = gameObject.GetComponent<GameManager>();
        troop_script.troop_data = tr;
        troop_script.info = false;
        troop_script.isPlayer = false;
        Last_enemy_spawned = gm;
        enemy_troops_queue.Add(gm);
        enemy_troops[tier] += 1;

    }

    public void dispatch_spawn_troop(int tier, bool player)
    {
        float training;
        if (player)
        {
            training = od.troop_training_times[tier + (player_age - 1) * 3];
            troop_queue tr = new troop_queue();
            tr.age = player_age;
            tr.tier = tier;
            money -= od.troop_costs[(tr.age - 1) * 3 + tr.tier];
            tr.training = od.troop_training_times[(player_age - 1) * 3 + tier];
            training_queue.Add(tr);
        }
        else
        {
            training = od.troop_training_times[tier + (enemy_age - 1) * 3];
        }
        if (!player)
        {
            spawn_enemy_troop(tier);
        }
      
    }
    void set_timescale()
    {
        Time.timeScale = scale;
    }
    public void upgrade_age_enemy()
    {
        Base b = enemy_base.GetComponent<Base>();
        enemy_hp += od.base_hp[enemy_age] - od.base_hp[enemy_age - 1];
        enemy_age += 1;
        b.sprite_manager();
        switch (enemy_age)
        {
            case 2:
                enemy_ai.Protocol_age2();
                break;

            case 3:
                enemy_ai.Protocol_age3();
                break;

            case 4:
                enemy_ai.Protocol_age4();
                break;

            case 5:
                enemy_ai.Protocol_age5();
                break;
        }
        
    }
    public void upgrade_age_player()
    {
        Base b = player_base.GetComponent<Base>();
        player_hp += od.base_hp[player_age] - od.base_hp[player_age - 1];
        player_age += 1; 
        b.sprite_manager();
    }
    public bool check_upgrade_age_player()
    {
        if(player_age == 5)
        {
            return false;
        }
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
        Vector3 trans = new Vector3(-1f, new Data.Only_Data().turret_spot[spot] / data_object.COEFF, 0f);

        GameObject gm = Instantiate(turret, trans + Vbase, Quaternion.identity);
        Turret turret_script = gm.GetComponent<Turret>();
        enemy_turrets[spot] = gm;


        turret_script.game_manager = gameObject.GetComponent<GameManager>();
        turret_script.turret_data = tr;
        turret_script.isPlayer = false;
        turret_script.slot = spot;
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
        turret_script.slot = spot;
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
        if (available_slots > 0 && money >= od.turret_cost[tier + (player_age - 1) * 3]) 
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


    public void spawn_bullet(Vector2 dir, Vector2 spawn, int damage = 200, bool gravity = false, bool isPl = true)
    {
        // getting direction for bullets
        dir = dir.normalized;
        GameObject bulletgm = Instantiate(bullet, spawn, transform.rotation);
        Bullet b = bulletgm.GetComponent<Bullet>();
        b.direction = dir;
        b.damage = damage;
        b.isPlayer = isPl;
        b.data = data_object;
        b.gravity = gravity;

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
    public bool command_wait()
    {
        return true;
    }
    public bool command_use_ability()
    {
        bool can = check_use_ability();
        if (can)
        {
            use_ability();
        }
        return can;
    }
    public bool command_spawn_troop_tier_1()
    {
        bool can = check_spawn_player_troop(0);
        if (can)
        {
            dispatch_spawn_troop(0, true);
        }
        return can;
    }
    public bool command_spawn_troop_tier_2()
    {
        bool can = check_spawn_player_troop(1);
        if (can)
        {
            dispatch_spawn_troop(1, true);
        }
        return can;

    }
    public bool command_spawn_troop_tier_3()
    {
        bool can = check_spawn_player_troop(2);
        if (can)
        {
            dispatch_spawn_troop(2, true);
        }
        return can;
    }
    public bool command_spawn_troop_tier_4()
    {
        bool can = check_spawn_player_troop(3);
        if (can)
        {
            dispatch_spawn_troop(3, true);
        }
        return can;
    }

    public bool command_spawn_turret_tier1()
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
        return can;
    }
    public bool command_spawn_turret_tier2()
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
        return can;
    }
    public bool command_spawn_turret_tier3()
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
        return can;
    }

    public bool command_sell_spot0()
    {
        bool can = check_sell_turret_player(0);
  
        if (can)
        {
            sell_turret_player(0);
        }
        return can;
    }
    public bool command_sell_spot1()
    {
        bool can = check_sell_turret_player(1);
        if (can)
        {
            sell_turret_player(1);
        }
        return can;
    }
    public bool command_sell_spot2()
    {
        bool can = check_sell_turret_player(2);
        if (can)
        {
            sell_turret_player(2);
        }
        return can;
    }
    public bool command_sell_spot3()
    {
        bool can = check_sell_turret_player(3);
        if (can)
        {
            sell_turret_player(3);
        }
        return can;
    }

    public bool command_upgrade_age()
    {
        bool can = check_upgrade_age_player();
        if (can)
        {
            upgrade_age_player();
        }
        return can;
    }

    public bool command_buy_slot()
    {
        bool can = check_buy_slot_player();
        if (can)
        {
            buy_slot_player();
        }
        return can;
    }

    private void Awake()
    {
        od = new Data.Only_Data();
    }

    void Start()
    {
        enemy_ai = GetComponent<Enemy_AI>();

        set_timescale();
        StartCoroutine(training());
    }

    // Update is called once per frame
    void Update()
    {
        if (ability_time >= 0)
        {
            ability_time -= Time.deltaTime;
        }

    }

    IEnumerator training()
    {
        if(training_queue.Count == 0)
        {
            yield return new WaitForSeconds(0.1f);
        }
        else
        {
            troop_queue tr = training_queue[0];
            yield return new WaitForSeconds(tr.training/data_object.FPS);
            spawn_player_troop(tr.tier, tr.age);
            training_queue.RemoveAt(0);
        }
        StartCoroutine(training());
    }

    IEnumerator ability1(int bullets_launched)
    {
        if(bullets_launched != 22)
        {
           
            yield return new WaitForSeconds((9 / data_object.FPS));
            Vector2 dir = new Vector2(0, -1f);
            Vector2 noise = new Vector2(Random.Range(-0.2f, 0.2f), 0f);
            dir += noise;
            Vector2 spawn = new Vector2(spawn_ability.transform.position.x + Random.Range(-8f, 8f), spawn_ability.transform.position.y);
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
            Vector2 spawn = new Vector2(spawn_ability.transform.position.x + Random.Range(-8f, 8f), spawn_ability.transform.position.y);
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
            Vector2 spawn = new Vector2(spawn_ability.transform.position.x + -9f + bullets_launched * 60f / data_object.COEFF, spawn_ability.transform.position.y);
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
            Vector2 spawn = new Vector2(spawn_ability.transform.position.x + -9f + bullets_launched * 50f / data_object.COEFF, spawn_ability.transform.position.y);
            spawn_bullet(dir, spawn, 1000);
            StartCoroutine(ability5(bullets_launched + 1));
        }
    }
    IEnumerator ability3()
    {

        //the troop regenerates 1 hp every frame (41 frames per second)
        int troop_count = player_troops_queue.Count;
        for (int i = 0; i < troop_count; i++)
        {
            Troop tr = player_troops_queue[i].GetComponent<Troop>();
            tr.is_regenerating = true;
        }
        yield return new WaitForSeconds(14.6f);
        troop_count = player_troops_queue.Count;
        for (int i = 0; i < troop_count; i++)
        {
            Troop tr = player_troops_queue[i].GetComponent<Troop>();
            tr.is_regenerating = false;
        }
    }
}

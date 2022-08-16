using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Master : MonoBehaviour
{
    public int number_of_envs; // should be divisible by 5
    public float env_speed;
    public GameObject env;
    public Data data_object;
    public List<GameManager> managers = new List<GameManager>();
    public List<GameObject> envs = new List<GameObject>();
    public Client client;
    public int finished_envs = 0;
    public int batch_size = 100;
    public int batch_to_train = 100;
    public int length;
    void set_timescale()
    {
        Time.timeScale = env_speed;
    }


    void Start()
    {
        client.connect_to_server();
        set_timescale();
        Spawn_environments();
    }
    void Spawn_environments()
    {
        int rows = number_of_envs / 5;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                Vector2 env_place = new Vector2(j * 40, i * 25);
                GameObject gm = Instantiate(env, env_place, Quaternion.identity);
                GameManager GM = gm.GetComponent<GameManager>();
                envs.Add(gm);
                managers.Add(GM);
                GM.data_object = data_object;
                GM.scale = env_speed;
            }
        }
        //print(batch_to_train);
        if(batch_to_train == 0)
        {
            // has finished the generation
            batch_to_train = batch_size;
            client.connect_to_server();
        }
        StartCoroutine(data_collection());
    }
    private void Update()
    {
        if (finished_envs == managers.Count)
        {
            batch_to_train -= finished_envs;
            print("restarted environments");
            // a batch has finished, creating a new batch
            int length = envs.Count;
            for (int i = 0; i < length; i++)
            {
               
                GameObject game = envs[i];
                Destroy(game);
            }
            print("the batch finished");
            envs = new List<GameObject>();
            managers = new List<GameManager>();
            Spawn_environments();
            finished_envs = 0;
        }
    }


    string get_data_from_manager(GameManager gm)
    {
        string data = "";

        /*
         * inputs = 
         * (in_train,
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
        int in_train = gm.training_queue.Count;
        float player_health = gm.player_hp / (1f * gm.od.base_hp[gm.player_age - 1]);
        float enemy_health = gm.enemy_hp / (1f * gm.od.base_hp[gm.enemy_age - 1]);
        
        int money = gm.money;
        int xp = gm.xp;
        float battle_place = gm.battle_place;
        bool ability = gm.check_use_ability();
        int num_ability = 0;
        if (ability)
        {
            num_ability = 1;
        }
        //processing troops
        int[] player_troops_count = new int[4];
        List<GameObject> player_troops_queue = gm.player_troops_queue;
        int length = player_troops_queue.Count;
       
        for (int i = 0; i < length; i += 1)
        {
            Troop tr = player_troops_queue[i].GetComponent<Troop>();

            int tier = tr.troop_data.id % 3;
            if (tr.troop_data.id == 15)
            {
                tier = 3;
            }
            player_troops_count[tier] += 1;
        }
        
        int[] enemy_troops_count = new int[4];
        List<GameObject> enemy_troops_queue = gm.enemy_troops_queue;
        length = enemy_troops_queue.Count;
        for (int i = 0; i < length; i += 1)
        {
            Troop tr = enemy_troops_queue[i].GetComponent<Troop>();
            int tier = tr.troop_data.id % 3;
            if (tr.troop_data.id == 15)
            {
                tier = 3;
            }
            enemy_troops_count[tier] += 1;
        }


        int slots_av = gm.available_slots;
        int age = gm.player_age;
        int eage = gm.enemy_age;
        //sending turrets separately
        int[] turret_tiers = gm.turret_tier;
        int[] turret_ages = gm.turret_age;
     
        data = data + in_train + " php ";
        data = data + player_health.ToString("0.000") + " ehp ";
        data = data + enemy_health.ToString("0.000") + " mn ";
        data = data + money + " xp ";
        data = data + xp + " bp ";
        data = data + battle_place.ToString("0.000") + " ab ";
        data = data + num_ability + " ptrps ";
        for (int i = 0; i<= 3; i++)
        {
            data = data + player_troops_count[i] + " ";
        }
        data += " etrps ";
        for (int i = 0; i <= 3; i++)
        {
            data = data + enemy_troops_count[i] + " ";
        }
        data += " slots ";
        data = data + slots_av + " age ";
        data = data + age + " eage ";
        data = data + eage + " turrets ";
        for (int i = 0; i <= 3; i++)
        {
            data = data + turret_tiers[i] + " ";
            data = data + turret_ages[i] + " ";
        }
        return data;
    }

    void get_data()
    {
        length = managers.Count;
        finished_envs = 0;
        string all_data = "";
        for (int i = 0; i < length; i++)
        {
            GameManager gm = managers[i];
            
            // gets data
            string data = get_data_from_manager(gm);
            data = "| " + data;
            if (gm.game_status != 0)
            {
                finished_envs += 1;
                data += " * ";
            }
            all_data += data;
            //print(all_data);
        }
        string actions = client.SendData(all_data);
        string[] actions_split = actions.Split(' ');
     
        //print(lng);
        
        for(int i = 0; i < length; i++)
        {
            
            GameManager gm = managers[i];
            //print(actions_split[i]);
            int action = int.Parse(actions_split[i]);
            
            gm.take_action(action);

        }

        if (finished_envs == length)
        {
            print("ALL COROUTINES STOPPED");
            StopAllCoroutines();
        }
    }

 

    IEnumerator data_collection()
    {
        yield return new WaitForSeconds(1f);
        get_data();
        StartCoroutine(data_collection());
    }

    
  

    
}

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
    public int batch_size = 150;
    public int batch_to_train = 150;
    public int length;
    public float diff = 1f;
    public int generation = 10;
    bool won_game = false;
    public int state = 0;
    public float time_between_actions = 1.3f;
 
    void set_timescale()
    {
        Time.timeScale = env_speed;
    }
    int[] states = { 0, 0, 0, 0, 3, 3, 3, 3, 4, 4, 4, 4 };

    void swap_diff()
    {
        diff = Random.Range(0, 3);
        
        if(diff == 0)
        {
            diff = 1.2f;
        }else
        if(diff == 1)
        {
            diff = 1.3f;
        }else
        if(diff == 2)
        {
            diff = 1.5f;
        }
      

    }

    void set_state()
    {
        //state = Random.Range(-14, 10);
        //state = states[generation % (states.Length)];
        //swap_diff();
        //diff = 1.35f;
        state = 0;
    }

    void Neat_Workflow()
    {
        client.connect_to_server();
        set_timescale();
        QualitySettings.SetQualityLevel(0);
        set_state();
        Spawn_environments();
    }

   


    void Start()
    {
  
        Neat_Workflow();
       
    }
    void Spawn_environments()
    {
        if (batch_to_train == 0)
        {
            
            //print(won_game);
            // has finished the generation
            batch_to_train = batch_size;
            //client.connect_to_server();
            generation += 1;
            //print(cnt_win);
            if (cnt_win >= 3)
            {
                diff += 0.1f;
                print("increased dif to " + diff + " at generation " + generation + " with " + cnt_win + " winning on state" + state);
                won_game = false;
            }else
            if (cnt_win != 0)
            {
                
                
            }
            set_state();
            cnt_win = 0;
        }
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
                GM.diff = diff;
                GM.identifier = (batch_size - batch_to_train) + i * 5 + j;
                GM.state = state;
                
                already_won[GM.identifier] = 0;
            }
        }
        length = managers.Count;
        //print(batch_to_train);
        
        StartCoroutine(data_collection());
    }
    private void Update()
    {
        if (finished_envs == managers.Count)
        {
            //print("finished envs");
            batch_to_train -= finished_envs;
            //print("restarted environments");
            // a batch has finished, creating a new batch
            int length = envs.Count;
            for (int i = 0; i < length; i++)
            {
               
                GameObject game = envs[i];
                Destroy(game);
            }
            //print("the batch finished");
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
    int[] already_won = new int[150];
    int cnt_win = 0;
    void get_data()
    {
        finished_envs = 0;
        string all_data = "";
        for (int i = 0; i < length; i++)
        {
            GameManager gm = managers[i];
            
            // gets data
            string data = get_data_from_manager(gm);
            data = "| " + data;
            int ind = gm.identifier;
            if(gm.xp > 8500000)
            {
                //print(i + " " + already_won[i]);
                if (already_won[ind] == 0)
                {
                    //cnt_win += 1;
                    already_won[ind] = 1;
                }
            }
            if (gm.game_status != 0)
            {

                finished_envs += 1;
                if (gm.game_status == 1)
                {
                    data += " * 1";
                }
                else
                {
                    if (already_won[ind] == 0)
                    {

                        already_won[ind] = 1;
                        cnt_win += 1;
                        print("AN AI " + gm.identifier + " WON THE GAME ON GEN " + generation + " player age " + gm.player_age + " enemy age " + gm.enemy_age + " IN STATE " + state);
                        
                    }
                   
                    data += " * 2";
                    
                }
                /*if(gm.game_status == 2)
                {
                    won_game = true;
                }*/
            }
            else
            {
                data += " 0";

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
          
            //gm.take_action(action);
            if (gm.game_status == 0)
            {
                gm.take_split_action(actions_split[i]);
            }
        }
        //print(finished_envs);
        //print(batch_to_train);
        if (finished_envs == length)
        {
            //print("ALL COROUTINES STOPPED");
            StopAllCoroutines();
            return;
        }
        
    }

 

    IEnumerator data_collection()
    {
        //print("got here and waiting" + time_between_actions);
        yield return new WaitForSeconds(time_between_actions);
        
        if (finished_envs != length)
        {
            //print("requested action");
            get_data();
            StartCoroutine(data_collection());
        }
    }
}

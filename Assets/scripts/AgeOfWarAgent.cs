using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;

public class AgeOfWarAgent : Agent
{
    // inputs = (in_train, player_health, enemy_health, money, xp, battle_place, ability, player_troops_total, enemy_troops_total, t4_troops, t4_val, slots_available, *age, *enemy_age, *new_turrets)
  

    [HideInInspector] public Data data_object;
    [HideInInspector] public float scale;
    public float diff;
    [HideInInspector] public int identifier;
    [HideInInspector] public int state;
    [HideInInspector] public GameObject env_template;
    [HideInInspector] public GameManager env;
    [HideInInspector] public GameObject env_object;
    [HideInInspector] public MasterMlAgents AgentMaster;
    Data.Only_Data od;
    float[] the_inputs;
    float maxt4 = 0;
    public int iter = 0;
    int ai_won = 0;
    public override void OnActionReceived(ActionBuffers actions)
    {
        //print("action taken");
        env.take_action(actions.DiscreteActions[0]);
        if (actions.DiscreteActions[0] == 13)
        {
            AddReward(7.5f);
        }
        /*env.act1(actions.DiscreteActions[0]);
        
        env.act2(actions.DiscreteActions[1]);
        env.act3(actions.DiscreteActions[2]);
        env.act4(actions.DiscreteActions[3]);
        env.act5(actions.DiscreteActions[4]);
        env.act6(actions.DiscreteActions[5]);*/
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        
        for (int i = 0; i <= 35; i++)
        {
           //print(the_inputs[i]);
            
            sensor.AddObservation(the_inputs[i]);
        }
      

    }
    public override void OnEpisodeBegin()
    {
        //print("episode began");
        // spawning a new environment
        env_object = Instantiate(env_template, transform.position, Quaternion.identity);
        env = env_object.GetComponent<GameManager>();
        env.data_object = data_object;
        env.scale = 1;
        env.diff = diff;
        env.identifier = identifier;
        env.state = iter % 10 - 7;
        //print(Academy.Instance.StepCount);
    }

    public void End_episode()
    {
        Destroy(env_object);
        iter += 1;
        EndEpisode();
        Academy.Instance.EnvironmentStep();
    }
    
    // Update is called once per frame
    private void Start()
    {
        //print("agent started");
        
        Academy.Instance.AutomaticSteppingEnabled = false;
        od = new Data.Only_Data();
        Academy.Instance.EnvironmentStep();
        
        

    }

    public void request_decision()
    {
        GameManager gm = env;
        if (gm.game_status == 0)
        {
            AddReward(0.2f);
            AddReward(gm.percent_taken * 10);
            
            gm.percent_taken = 0;
            AddReward(Mathf.Min(15, gm.money/50000));
            // killing enemies with turrets
            float t4_troops = the_inputs[11]*5;
            t4_troops = Mathf.Min(t4_troops, 4);

            if (t4_troops >= 3)
            {
                //print("t4 troops on ground" + t4_troops);
                AddReward(Mathf.Pow(5, t4_troops - 1));
            }
            maxt4 = Mathf.Max(maxt4, t4_troops);
            Academy.Instance.EnvironmentStep();
            RequestDecision();
            //print("academy stepped");

        }
        else
        {
            maxt4 = Mathf.Max(maxt4, the_inputs[11] * 5);
            if (gm.game_status == 2)
            {

                if (gm.state <= 0)
                {
                    print("for the " + ai_won + "th time an ai won" + identifier + " with " + the_inputs[11] * 5 + " t4 troops and a max of " + maxt4 + " on state " + gm.state);
                    AddReward(20000);
                }
                else
                {
                    AddReward(20000);
                }
                ai_won += 1;
             

                if (ai_won % 5 == 0)
                {
                    print(ai_won);
                    //diff += 0.1f;
                    print("increased diff to" + diff + "on agent " + identifier);

                }
                AddReward(1f);
                Academy.Instance.EnvironmentStep();
                End_episode();
            }
            else
            {

                if (gm.xp > 7000000)
                {
                    //print("ai get over 5k xp");
                    AddReward(-5000);
                    AddReward(0f);
                }
                else
                {
                    AddReward(-5000);
                    AddReward(-1f);
                }
                //print("an ai lost" + identifier);
                
                Academy.Instance.EnvironmentStep();
                End_episode();
            }
            maxt4 = 0;
            
        }
    }
    public void feed_data_and_request_decision()
    {
        GameManager gm = env;

        //print(StepCount);
        int in_train = gm.training_queue.Count;
        float player_health = gm.player_hp / (1f * gm.od.base_hp[gm.player_age - 1]);
        float enemy_health = gm.enemy_hp / (1f * gm.od.base_hp[gm.enemy_age - 1]);

        float battle_place = gm.battle_place;
        int ability = 0;
        if (gm.check_use_ability())
        {
            ability = 1;
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
        int total_slots = gm.total_slots;

        int age = gm.player_age;
        int eage = gm.enemy_age;

        int[] player_age = { 0, 0, 0, 0, 0 };
        int[] enemy_age = { 0, 0, 0, 0, 0 };

        player_age[age - 1] = 1;
        enemy_age[eage - 1] = 1;

        int[] turret_tiers = gm.turret_tier;
        int[] turret_ages = gm.turret_age;
        int[] slots = gm.slots;
        float[] new_turret_ages = new float[4];
        float[] new_turret_tiers = new float[4];
        for (int i = 0; i <= 3; i++)
        {
            if(gm.slots[i] == 0)
            {
                new_turret_tiers[i] = 0;
                new_turret_ages[i] = 0;
                
            }
            else
            {
                new_turret_tiers[i] = turret_tiers[i] + 1;
                new_turret_ages[i] = turret_ages[i] / 5f;
                /*if (turret_ages[i] < age)
                {
                    new_turret_ages[i] = turret_ages[i] - age;
                }
                else
                {
                    new_turret_ages[i] = 1;
                }*/

            }
            new_turret_tiers[i] /= 3f;
           

        }

        
        float money = gm.money;
        float xp = gm.xp;

        float t4_money = money / 150000f;

        xp = xp / od.xp_cost[age - 1];
        money = money / od.troop_costs[(age-1)*3+2];
        money = Mathf.Sqrt(money);
        xp = Mathf.Sqrt(xp);


        /*
         * inputs = 
         * (in_train,
         * player_health, 
         * enemy_health, 
         * money,
         * money to t4 troops
         * xp, 
         * battle_place, 
         * ability, 
         * *player_troops_total,
         * *enemy_troops_total, 
         * slots_available, 
         * total slots
         * *age, 
         * *enemy_age, 
         * *turrets)
         */
        float[] inputs = new float[36];
        inputs[0] = in_train;
        inputs[1] = player_health;
        inputs[2] = enemy_health;
        inputs[3] = money;
        inputs[4] = t4_money;
        inputs[5] = xp;
        inputs[6] = battle_place;
        inputs[7] = ability;

        inputs[8] = player_troops_count[0]/5f;
        inputs[9] = player_troops_count[1]/5f;
        inputs[10] = player_troops_count[2]/5f;
        inputs[11] = player_troops_count[3]/5f;

        inputs[12] = enemy_troops_count[0]/5f;
        inputs[13] = enemy_troops_count[1]/5f;
        inputs[14] = enemy_troops_count[2]/5f;
        inputs[15] = enemy_troops_count[3]/5f;

        inputs[16] = slots_av;
        inputs[17] = total_slots;

        inputs[18] = player_age[0];
        inputs[19] = player_age[1];
        inputs[20] = player_age[2];
        inputs[21] = player_age[3];
        inputs[22] = player_age[4];

        inputs[23] = enemy_age[0];
        inputs[24] = enemy_age[1];
        inputs[25] = enemy_age[2];
        inputs[26] = enemy_age[3];
        inputs[27] = enemy_age[4];

        inputs[28] = new_turret_tiers[0];
        inputs[29] = new_turret_ages[0];

        inputs[30] = new_turret_tiers[1];
        inputs[31] = new_turret_ages[1];

        inputs[32] = new_turret_tiers[2];
        inputs[33] = new_turret_ages[2];

        inputs[34] = new_turret_tiers[3];
        inputs[35] = new_turret_ages[3];
        
        the_inputs = inputs;

        
    }
}

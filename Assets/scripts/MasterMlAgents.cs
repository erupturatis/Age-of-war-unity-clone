using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterMlAgents : MonoBehaviour
{
    public int number_of_envs; // should be divisible by 5
    public float env_speed;
    public GameObject env_agent;
    public GameObject env_template;
    public Data data_object;
    public List<AgeOfWarAgent> agents = new List<AgeOfWarAgent>();
    public int length;
    public float diff = 1f;
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

        if (diff == 0)
        {
            diff = 1.2f;
        }
        else
        if (diff == 1)
        {
            diff = 1.3f;
        }
        else
        if (diff == 2)
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

    public void increase_difficulty()
    {
        diff += 0.1f;
        print("increased dif to " + diff);
    }

    void MlAgents_Workflow()
    {
        set_timescale();
        Spawn_environments();
        StartCoroutine(initial());
    }
    IEnumerator initial()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(requesting_decisions());
    }

    void Start()
    {
        MlAgents_Workflow();
        //set_timescale();
    }
    private void Update()
    {
        //set_timescale();
    }

    void Spawn_environments()
    {
        int rows = number_of_envs / 5;
        if(number_of_envs == 1)
        {
            Vector2 env_place = new Vector2(0,0);
            GameObject gm = Instantiate(env_agent, env_place, Quaternion.identity);
            AgeOfWarAgent GM = gm.GetComponent<AgeOfWarAgent>();
            GM.env_template = env_template;
            agents.Add(GM);

            GM.data_object = data_object;
            GM.scale = env_speed;
            GM.diff = diff;
            GM.identifier =0;
            GM.state = state;
            length = 1;
            return;
        }
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                Vector2 env_place = new Vector2(j * 40, i * 25);
                GameObject gm = Instantiate(env_agent, env_place, Quaternion.identity);
                AgeOfWarAgent GM = gm.GetComponent<AgeOfWarAgent>();
                GM.env_template = env_template;
                agents.Add(GM);

                GM.data_object = data_object;
                GM.scale = env_speed;
                GM.diff = diff;
                GM.identifier = i * 5 + j;
                GM.state = state;
                GM.AgentMaster = gameObject.GetComponent<MasterMlAgents>();
            }
        }
        length = agents.Count;
        //print(batch_to_train);

        StartCoroutine(requesting_decisions());
    }

    void run_loop()
    {
        for (int i = 0; i < length; i++)
        {
            AgeOfWarAgent gm = agents[i];

            // gets data
            
            gm.feed_data_and_request_decision();
           
        }
    }
    void run_loop2()
    {
        for (int i = 0; i < length; i++)
        {
            AgeOfWarAgent gm = agents[i];

            // gets data

            gm.request_decision();

        }
    }

    IEnumerator requesting_decisions()
    {
        run_loop();
        yield return new WaitForSeconds(time_between_actions);
        run_loop2();
        StartCoroutine(requesting_decisions());
        
    }
}

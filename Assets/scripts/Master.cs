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

    void get_data()
    {
        int length = managers.Count;
        finished_envs = 0;
        for (int i = 0; i < length; i++ )
        {
            print("env blocked" + i);
            GameManager gm = managers[i];
            if (gm.game_status == 0)
            {
                // gets data
                string data = "data for env" + i;
     
                data = gm.game_status + " " + data;
                
                string action = client.SendData(data, i);
                client.SendStatus(i, true);
            }
            else
            {
                finished_envs += 1;
                string data = "" + gm.game_status;
                string action = client.SendData(data, i);
            }

        }
       
        if(finished_envs == length)
        {
            print("ALL COROUTINES STOPPED");
            StopAllCoroutines();
        }
    }
 

    IEnumerator data_collection()
    {
        yield return new WaitForSeconds(2.5f);
        get_data();
        StartCoroutine(data_collection());
    }

    
  

    
}

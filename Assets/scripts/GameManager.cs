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

    
    


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

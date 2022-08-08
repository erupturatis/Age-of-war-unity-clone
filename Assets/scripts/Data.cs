using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour
{
    string[] troop_names = { "clubman", "slingshotman", "dino", "sword", "archer", "knight", "dueler", "mouschete", "canoneer", "melee infantry", "gun infantry", "Tank", "godblade", "blaster", "war machie", "super soldier" };
    int[] troop_costs = { 15, 25, 100, 50, 75, 500, 200, 400, 1000, 1500, 2000, 7000, 5000, 6000, 20000, 150000 };
    int[] troop_training_times = { 40, 40, 100, 70, 50, 100, 100, 100, 200, 100, 100, 300, 100, 100, 300, 100 };
    int[] troop_hps = { 55, 42, 160, 100, 80, 300, 200, 160, 600, 350, 300, 1200, 1000, 800, 3000, 4000 };
    int[] troop_melee_damages = { 16, 10, 40, 35, 20, 60, 79, 40, 120, 100, 60, 300, 250, 130, 600, 400 };
    int[] troop_ranged_damages = { 0, 8, 0, 0, 20, 0, 0, 20, 0, 0, 30, 0, 0, 80, 0, 400 };

    int[] troop_melee_ranges = { 20, 20, 45, 20, 20, 60, 25, 25, 25, 25, 25, 100, 40, 40, 100, 40 };
    int[] troop_ranged_ranges = { 0, 100, 0, 0, 130, 0, 0, 130, 0, 0, 130, 0, 0, 130, 0, 150 };

    // the speed of the initial attack is different from the others
    float[] troop_melee_first_speeds = { 0.45f, 0.45f, 0.32f, 0.62f, 0.62f, 0.35f, 0.32f, 0.15f, 0.65f, 0.17f, 0.07f, 0.55f, 0.32f, 0.32f, 0.25f, 0.32f };
    float[] troop_melee_speeds = { 1f, 1f, 1.12f, 2.47f, 2.47f, 1.30f, 1.05f, 1.15f, 1.95f, 0.75f, 0.52f, 1.57f, 0.92f, 0.7f, 2.25f, 0.7f };
    
    // ranged troops have different speeds when walking or stationating
    float[] troop_ranged_speeds = { 0f, 0.8f, 0f, 0f, 1f, 0f, 0f, 1.15f, 0f, 0f, 0.52f, 0f, 0f, 0.35f, 0f, 0.35f };
    float[] troop_walking_ranged_speeds = { 0f, 1.07f, 0f, 0f, 1f, 0f, 0f, 1.20f, 0f, 0f, 1.20f, 0f, 0f, 0.95f, 0f, 0.95f };

    //turret attack speed in frames (between bullets)
    int[] turret_speed = { 30, 11, 70, 70, 70, 100, 70, 70, 70, 40, 50, 22, 40, 10, 10 };
    int[] damage = { 12, 5, 25, 40, 50, 100, 30, 70, 100, 70, 100, 60, 100, 40, 60 };
    int[] turret_range = { 350, 300, 400, 400, 300, 50, 500, 500, 500, 500, 500, 500, 400, 500, 550 };
    //41 frames means 1 second
    // the map has about 1000 units
    // the  unity map has about
    class Troop
    {
        public int id = 0;
        string name = "club man";
        int cost = 15;
        int training_time = 40;
        int health = 55;
        int damage = 16;
        int ranged_damage = 0;
        int ranged_distance = 0;
        int range_melee = 20;

        public void set_parameters()
        {
            
        }
        
    }
    class Turret
    {

    }

    private void Awake()
    {
        
    }
}

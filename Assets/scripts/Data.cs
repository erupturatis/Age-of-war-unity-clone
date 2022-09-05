using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Data : MonoBehaviour
{
    public class Only_Data
    {
        public string[] troop_names = { "clubman", "slingshotman", "dino", "sword", "archer", "knight", "dueler", "mouschete", "canoneer", "melee infantry", "gun infantry", "Tank", "godblade", "blaster", "war machie", "super soldier" };
        public int[] troop_costs = { 15, 25, 100, 50, 75, 500, 200, 400, 1000, 1500, 2000, 7000, 5000, 6000, 20000, 150000 };
        public int[] troop_training_times = { 40, 40, 100, 70, 50, 100, 100, 100, 200, 100, 100, 300, 100, 100, 300, 100 };
        protected int[] troop_hps = { 55, 42, 160, 100, 80, 300, 200, 160, 600, 350, 300, 1200, 1000, 800, 3000, 4000 };
        protected int[] troop_melee_damages = { 16, 10, 40, 35, 20, 60, 79, 40, 120, 100, 60, 300, 250, 130, 600, 400 };
        protected int[] troop_ranged_damages = { 0, 8, 0, 0, 14, 0, 0, 20, 0, 0, 30, 0, 0, 80, 0, 400 };

        protected int[] troop_melee_ranges = { 20, 20, 20, 20, 20, 20, 25, 25, 25, 25, 25, 20, 20, 20, 20, 20 };
        protected int[] troop_ranged_ranges = { 0, 100, 0, 0, 130, 0, 0, 130, 0, 0, 130, 0, 0, 110, 0, 130 };
        protected int[] troop_lengths = { 20, 20, 80, 20, 20, 80, 20, 20, 20, 20, 20, 120, 20, 20, 100, 20 };
        protected int[] frames_wait_attack = {20,20,45,20,20,60,25,25,25,25,25,100,40,40,100,40 };

        // the speed of the initial attack is different from the others
        protected float[] troop_melee_first_speeds = { 0.43f, 0.43f, 0.32f, 0.62f, 0.62f, 0.35f, 0.32f, 0.15f, 0.65f, 0.17f, 0.07f, 0.55f, 0.32f, 0.32f, 0.25f, 0.32f };
        protected float[] troop_melee_speeds = { 1f, 1f, 1.12f, 2.47f/2, 2.47f/2, 1.30f, 1.05f, 1.15f, 1.95f, 0.75f, 0.52f, 1.57f, 0.92f, 0.7f, 2.25f, 0.7f };

        // ranged troops have different speeds when walking or stationating
        protected float[] troop_ranged_speeds = { 0f, 0.8f, 0f, 0f, 1f, 0f, 0f, 1.15f, 0f, 0f, 0.52f, 0f, 0f, 0.35f, 0f, 0.35f };
        protected float[] troop_walking_ranged_speeds = { 0f, 1.07f, 0f, 0f, 1f, 0f, 0f, 1.20f, 0f, 0f, 1.20f, 0f, 0f, 0.95f, 0f, 0.95f };

        public int[] base_hp = { 500, 1100, 2000, 3200, 4700 };
        //true for melee, false for mixed
        protected bool[] troop_type = { true, false, true, true, false, true, true, false, true, true, false, true, true, false, true, false };
        //turret attack speed 
        public string[] turret_name = { "Rock slingshot", "egg automatic", "primitive catapult", "Catapult", "Fire Catapult", "oil", "Small Cannon", "Medium Cannon", "Big cannon", "gun", "rocket launcher", "double gun", "laser", "red pew pew", "blue pew pew" };
        public int[] turret_cost = { 100, 200, 500, 500, 750, 1000, 1500, 3000, 6000, 7000, 9000, 14000, 24000, 40000, 100000 };
        public int[] turret_spot = { 20, 68, 116, 164 };
        public int[] xp_cost = { 4000, 14000, 45000, 200000, 10000000 };
        public int[] slot_cost = { 1000, 3000, 7500 };
        

        protected float[] turret_speed = { 0.8f, 0.25f, 1.37f, 2.47f, 2.47f, 1.92f, 1.12f, 2f, 2f, 1.12f, 1f, 0.5f, 1f, 0.25f, 0.25f };
        protected int[] turret_speed_add = { 30, 11, 20, 70, 70, 50, 50, 70, 70, 70, 80, 0, 40, 11, 11 }; // I didn t totally figure out how these values work so I adjusted some of them to match in game time
        protected int[] turret_damage = { 12, 5, 25, 40, 50, 125, 30, 70, 100, 70, 100, 60, 100, 40, 60 };
        protected int[] turret_range = { 350, 300, 380, 400, 300, 50, 500, 500, 500, 500, 500, 500, 400, 500, 550 };
        protected int[] turret_fragment_damage = { 0, 0, 0, 0, 10, 0, 0, 0, 30, 0, 0, 0, 0, 0, 0 };
        protected bool[] turret_makes_fragment = { false, false, false, false, true, false, false, false, true, false, false, false, false, false, false };
        protected float[] turret_initial_speed = { 0.1f, 0f, 0.17f, 0.35f, 0.35f, 0.5f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };

    }

    [System.NonSerialized] public float FPS = 41;
    [System.NonSerialized] public int MAP_LENGTH = 900;
    //41 frames means 1 second
    // the original game map has 900 units, inferred from the base coordintes
    // the  unity map has 18 units (you can set this to whateever you want but COEFF needs to modify)
    [System.NonSerialized] public float COEFF = 50; // the coefficient for converting the unity units into age of war original units
    [System.NonSerialized] public int MIN_DISTANCE = 20; // min distance between troops 

    public Sprite[] troop_sprites;
    public Sprite[] turret_sprites;
    public Sprite[] bases_sprites;
    public Sprite[] ability_sprites;
    public Sprite[] bases_towers_sprites;

    private void Awake()
    {
        
    }

    public class TroopData : Only_Data
    {
        public int id = 0;
        public string name = "club man";
        public int cost = 15;
        public int training_time = 40;
        public int health = 55;
        public int max_health = 55;
        public int melee_damage = 16;
        public int ranged_damage = 0;

        public int range_ranged = 0;
        public int range_melee = 20;

        public float speed = 30f; // the game has a standard move speed for all troops
        // a troop walks 450 units in 11.5 seconds so the speed should be 450/11.5
        public int length = 20;

        public float melee_first_speed = 0.45f;
        public float melee_speed = 1f;

        public float ranged_first_speed = 0f; // same for all troops
        public float ranged_walking_speed = 0f;
        public float ranged_standing_speed = 0f;

        public float attack_pause = 20f;

        public bool just_melee = true;

        public void set_parameters()
        {
            //assuming id is set from the start
            name = troop_names[id];
            cost = troop_costs[id];
            training_time = troop_training_times[id];
            health = troop_hps[id];
            melee_damage = troop_melee_damages[id];
            ranged_damage = troop_ranged_damages[id];

            range_ranged = troop_ranged_ranges[id];
            range_melee = troop_melee_ranges[id];
            length = troop_lengths[id];

            melee_first_speed = troop_melee_first_speeds[id];
            melee_speed = troop_melee_speeds[id];

            ranged_first_speed = 0f;
            ranged_walking_speed = troop_walking_ranged_speeds[id];
            ranged_standing_speed = troop_ranged_speeds[id];

            just_melee = troop_type[id];
            attack_pause = frames_wait_attack[id];
            max_health = health;

        }
        
    }
    public class TurretData : Only_Data
    {
        public int id = 0;
        public string name = "slingshot";
        public int cost = 100;
        public float first_speed = 0.1f;
        public float speed = 0.8f;
        public float additional_speed = 30; // frames pause between shots
        public int damage = 12;
        public int range = 350;
        public bool makes_fragments = false;
        public int fragment_damage = 0;
        public float bullet_speed = 250f;
        
        public void set_parameters()
        {
            name = turret_name[id];
            cost = turret_cost[id];
            first_speed = turret_initial_speed[id];
            speed = turret_speed[id];
            additional_speed = turret_speed_add[id];
            damage = turret_damage[id];
            range = turret_range[id];
            makes_fragments = turret_makes_fragment[id];
            fragment_damage = turret_fragment_damage[id];
        }
    }


}

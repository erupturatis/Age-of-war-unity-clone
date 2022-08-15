using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy_AI : MonoBehaviour
{
    /*
    RULES FOR THE ENEMY AI EXTRACTED FROM ACTIONSCRIPT CODE:
    no more than 6 enemies on the map
    30% change of spawning troops each second
    units have to train just like player units
    For each age:
    after first 1500 frames the 2nd tier troops become available
    after first 5000 frames the 3nd tier troops become available
    after first 8000 frames it will upgrade it s age
    the troop type is randomized
    the turret buying follow a fixed succesion in each game
    Age1:
    1000 -> turret1 slot1
    4000 -> sell slot1 ,turret2 slot1
    6000 -> sell slot1 ,turret3 slot1
    Age2:
    1000 -> sell slot1 ,turret1 slot1
    4000 -> buy slot, sell slot1 ,turret3 slot1
    6000 -> turret2 slot2
    Age3:
    1000 -> sell slot1, turret1 slot1
    4000 -> buy slot, sell slot2, turret1 slot2
    6000 -> sell slot1, sell slot2, turret3 slot3
    Age4:
    5000 -> turret1 slot 1
    7000 -> sell slot1 sell slot3, turret2 slot2
    Age5:
    5000 -> turret1 slot1
    12000 -> sell slot1, sell slot2, sell slot3, turret2 slot 2
    20000 -> buy slot, sell all slots, turret3 slot 4
     */

    public GameManager game_manager;
    int unit_level = 0;

    public void Protocol_age1()
    {
        unit_level = 0;
        StartCoroutine(upgrade_unity_level(1500));
        StartCoroutine(upgrade_unity_level(5000));
        StartCoroutine(upgrade_age(8000));

        StartCoroutine(buy_turret(1000, 0, 0));

        StartCoroutine(sell_turret(3950, 0));
        StartCoroutine(buy_turret(4000, 1, 0));

        StartCoroutine(sell_turret(5950, 0));
        StartCoroutine(buy_turret(6000, 2, 0));
    }
    public void Protocol_age2()
    {
        unit_level = 0;
        StartCoroutine(upgrade_unity_level(1500));
        StartCoroutine(upgrade_unity_level(5000));
        StartCoroutine(upgrade_age(8000));

        StartCoroutine(sell_turret(950, 0));
        StartCoroutine(buy_turret(1000, 0, 0));

        StartCoroutine(sell_turret(3950, 0));
        StartCoroutine(buy_turret(4000, 2, 0));

        StartCoroutine(buy_turret(6000, 1, 1));
    }
    public void Protocol_age3()
    {

        unit_level = 0;
        StartCoroutine(upgrade_unity_level(1500));
        StartCoroutine(upgrade_unity_level(5000));
        StartCoroutine(upgrade_age(8000));

        StartCoroutine(sell_turret(950, 0));
        StartCoroutine(buy_turret(1000, 0, 0));

        StartCoroutine(sell_turret(3950, 1));
        StartCoroutine(buy_turret(4000, 0, 1));

        StartCoroutine(sell_turret(5950, 0));
        StartCoroutine(sell_turret(5950, 1));
        StartCoroutine(buy_turret(6000, 2, 2));
    }
    public void Protocol_age4()
    {
        unit_level = 0;
        StartCoroutine(upgrade_unity_level(1500));
        StartCoroutine(upgrade_unity_level(5000));
        StartCoroutine(upgrade_age(8000));

        StartCoroutine(buy_turret(5000, 0, 0));

        StartCoroutine(sell_turret(6950, 2));
        StartCoroutine(sell_turret(6950, 0));

        StartCoroutine(buy_turret(7000, 1, 1));

    }
    public void Protocol_age5()
    {
        unit_level = 0;
        StartCoroutine(upgrade_unity_level(1500));
        StartCoroutine(upgrade_unity_level(5000));

        StartCoroutine(buy_turret(5000, 0, 0));
        StartCoroutine(sell_turret(12000, 0));
        StartCoroutine(sell_turret(12000, 1));
        StartCoroutine(sell_turret(12000, 2));
        StartCoroutine(buy_turret(12050, 1, 1));
        StartCoroutine(sell_turret(20000, 0));
        StartCoroutine(sell_turret(20000, 1));
        StartCoroutine(sell_turret(20000, 2));
        StartCoroutine(buy_turret(20050, 2, 3));
    }

   

    void Start()
    {
        Protocol_age1();
        StartCoroutine(Spawn_troops());
    }

    IEnumerator upgrade_age(int frames)
    {
        yield return new WaitForSeconds(frames / game_manager.data_object.FPS);
        game_manager.upgrade_age_enemy();
    }
    IEnumerator upgrade_unity_level(int frames)
    {
        yield return new WaitForSeconds(frames / game_manager.data_object.FPS);
        unit_level += 1;
    }

    IEnumerator sell_turret(int frames, int slot)
    {
        yield return new WaitForSeconds(frames / game_manager.data_object.FPS);
        game_manager.sell_turret_enemy(slot);
    }
    IEnumerator buy_turret(int frames, int tier, int slot)
    {
        yield return new WaitForSeconds(frames / game_manager.data_object.FPS);
        game_manager.buy_turret_enemy(tier,slot);
    }

    IEnumerator Spawn_troops()
    {
        yield return new WaitForSeconds(1f);
        float spawn = Random.Range(0f, 1f);
        if(spawn < 0.3f && game_manager.enemy_troops_queue.Count<6)
        {
            int unit_type = Random.Range(0, unit_level+1);
            game_manager.dispatch_spawn_troop(unit_type, false);
        }
        StartCoroutine(Spawn_troops());
    }

}

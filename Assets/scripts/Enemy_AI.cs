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

    [HideInInspector] public GameManager game_manager;

    void Protocol_age1()
    {

    }
    void Protocol_age2()
    {

    }
    void Protocol_age3()
    {


    }
    void Protocol_age4()
    {


    }
    void Protocol_age5()
    {

    }

    IEnumerator Do_function(int frames, int action)
    {
        yield return new WaitForSeconds(frames/41f);
    }

    void Start()
    {
        Protocol_age1();
    }
    private void Update()
    {
        
    }

}

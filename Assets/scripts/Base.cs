using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Base : MonoBehaviour
{
    [SerializeField]
    bool IsPlayerBase;
    [SerializeField]
    SpriteRenderer Renderer;
    [SerializeField]
    Sprite[] Sprites;
    [SerializeField]
    GameObject ParentEnvironment;

    GameManager game_manager;
    Data data;
    [SerializeField]
    TextMeshProUGUI Tm;

    

    public void sprite_manager()
    {
        int age = 0;
        if (IsPlayerBase)
        {
            age = game_manager.player_age;
        }
        else
        {
            age = game_manager.enemy_age;
        }
        Renderer.sprite = Sprites[age - 1];
    }

    public void take_damage(int damage)
    {
        if (IsPlayerBase)
        {
            game_manager.player_hp -= damage;
        }
        else
        {
            game_manager.enemy_hp -= damage;
        }
    }

    void update_text()
    {
        int hp = 500;
        if (IsPlayerBase)
        {
            hp = game_manager.player_hp;
        }
        else
        {
            hp = game_manager.enemy_hp;
        }
        Tm.text = "" + hp;
    }

    private void Awake()
    {
        game_manager = ParentEnvironment.GetComponent<GameManager>();
        data = ParentEnvironment.GetComponent<Data>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //sprite_manager(); // can be called less often
        update_text(); // can be optimized
    }
}

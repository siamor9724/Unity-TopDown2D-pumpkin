using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class BossController : EnemyBase
{

    static BossController instance;
    public static BossController Instance
    {
        get
        {
            return instance;
        }
    }
    void Awake() { instance = this; }
    public BossRoomCondition bossRoom;
    private void Start()
    {
        color = gameObject.GetComponent<SpriteRenderer>().color;
        if (gameObject.transform.parent == null) return;
        bossRoom = gameObject.transform.parent.GetComponent<BossRoomCondition>();
    }
  /*  private void Update()
    {
       
    }*/
   

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
  

   

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpBlock : MonoBehaviour
{
    public float speedUpTime = 0f;
    float duration = 5f;
    public AllTraps trapMgr;
    public SpeedDownBlock speedDownBlock;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Mathf.Abs(trapMgr.player.transform.position.x - gameObject.transform.position.x) < 0.5 && Mathf.Abs(trapMgr.player.transform.position.y - gameObject.transform.position.y) < 0.5)
        {
            speedUpTime = duration;
            speedDownBlock.speedDownTime = 0f;
        }
        
        if (speedUpTime > 0)
        {
            trapMgr.PlayerSpeedUp();
            speedUpTime -= Time.deltaTime;
            //Debug.Log("boosted");
        }
        
        
    }
}

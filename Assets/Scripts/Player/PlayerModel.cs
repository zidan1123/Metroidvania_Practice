using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class PlayerModel : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public Player GetAbilityInfoByFileName(string fileName)
    {
        string tempJsonStr = Resources.Load<TextAsset>("JsonDatas/" + fileName).text;
        
        Player player = JsonMapper.ToObject<Player>(tempJsonStr);

        return player;
    }
}

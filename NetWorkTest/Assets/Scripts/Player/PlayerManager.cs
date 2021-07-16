
using UnityEngine;
using Player;
using System.Collections.Generic;
using System;

public class PlayerManager : MonoSingleton<PlayerManager>,IGameUpdate
{
    public AllPosMsg allPosMsg;

    public bool IsUpdatePos = false;
    public PlayerModel selfPlayer;
    public TcpClientTool selfClient;
    public Dictionary<int, PlayerModel> allPlayerDic = new Dictionary<int, PlayerModel>();
    public long reciveTime = 0;

    public void SetAllPlayer(AllPosMsg _allPosMsg)
    {
        allPosMsg = _allPosMsg;
        reciveTime = DateTime.Now.Ticks;
        //selfPlayer.SaveLastPos();
       // Debug.Log("yns  set reciveTime " + reciveTime);
    }


    public void CreatSelfPlayer(int playerID)
    {
        GameObject game = Instantiate(Resources.Load(PrefabsPath.PlyaerPrefab) as GameObject);
        Debug.Log("Creat player");
        selfPlayer = game.AddComponent<PlayerComponent>().Init(playerID);
        selfPlayer.client = selfClient;
        selfPlayer.manager = this;
        allPlayerDic.Add(playerID, selfPlayer);
        IsUpdatePos = true;
    }
    public int CreatOtherPlayer(int playerID)
    {
        GameObject game = Instantiate(Resources.Load(PrefabsPath.PlyaerPrefab) as GameObject);
        game.name = ("otherPlayer"+ playerID);
        Debug.Log("Creat other player");
        var otherPlayer = game.AddComponent<PlayerComponent>().Init(playerID);
        otherPlayer.manager = this;
        allPlayerDic.Add(playerID, otherPlayer);
        return playerID;
    }


    public void GameUpdate()
    {
        if (IsUpdatePos && allPosMsg !=null&& allPosMsg.PosPlayerMsgList != null)
        {
            int len = allPosMsg.PosPlayerMsgList.Count;
            Debug.Log("yns  all pos len " + len);
            for (int i = 0; i < len; i++)
            {
                var pos = allPosMsg.PosPlayerMsgList[i];
                if (allPlayerDic.ContainsKey(pos.PlayerId))
                {
                    if (selfPlayer.playerID != pos.PlayerId)
                        allPlayerDic[pos.PlayerId].SetPos(pos);
                }
                else
                {
                    //添加新玩家
                    CreatOtherPlayer(pos.PlayerId);
                    allPlayerDic[pos.PlayerId].SetPos(pos);

                }
            }
            selfPlayer.SendPos();
        }
    }

}

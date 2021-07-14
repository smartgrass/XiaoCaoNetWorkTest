
using UnityEngine;
using Player;
using System.Collections.Generic;

public class PlayerManager : MonoSingleton<PlayerManager>,IGameUpdate
{
    public AllPosMsg allPosMsgLast;
    public AllPosMsg allPosMsg;

    public bool IsUpdatePos = false;
    public PlayerModel selfPlayer;
    public TcpClientTool selfClient;
    public Dictionary<int, PlayerModel> allPlayerDic = new Dictionary<int, PlayerModel>();
    public float reciveTime = 0;

    public void SetAllPlayer(AllPosMsg _allPosMsg)
    {

        if (allPosMsg == null)
            allPosMsgLast = _allPosMsg;
        else
            allPosMsgLast = allPosMsg;
        allPosMsg = _allPosMsg;
        reciveTime = Time.time;
    }


    public void CreatSelfPlayer(int playerID)
    {
        GameObject game = Instantiate(Resources.Load(PrefabsPath.PlyaerPrefab) as GameObject);
        Debug.Log("Creat player");
        selfPlayer = game.AddComponent<PlayerComponent>().Init(playerID);
        selfPlayer.client = selfClient;
        allPlayerDic.Add(playerID, selfPlayer);
        IsUpdatePos = true;
    }
    public int CreatOtherPlayer(int playerID)
    {
        GameObject game = Instantiate(Resources.Load(PrefabsPath.PlyaerPrefab) as GameObject);
        game.name = ("otherPlayer"+ playerID);
        Debug.Log("Creat other player");
        var otherPlayer = game.AddComponent<PlayerComponent>().Init(playerID);
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
                var lastPos = allPosMsgLast.PosPlayerMsgList[i];
                if (allPlayerDic.ContainsKey(pos.PlayerId))
                {
                    if (selfPlayer.playerID != pos.PlayerId)
                        allPlayerDic[pos.PlayerId].SetPos(pos, lastPos);
                }
                else
                {
                    //添加新玩家
                    CreatOtherPlayer(pos.PlayerId);
                    allPlayerDic[pos.PlayerId].SetPos(pos, lastPos);

                }
            }
            selfPlayer.SendPos();
        }
    }

}

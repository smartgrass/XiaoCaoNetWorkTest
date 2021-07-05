using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerManager : MonoSingleton<PlayerManager>
{


    public void CreatPlayer()
    {
        GameObject game = Instantiate(new GameObject());
        Debug.Log("Creat player");
    }
    public void CreatPlayer(int playerID)
    {
        GameObject game = Instantiate(new GameObject());
        Debug.Log("Creat player");
    }
}

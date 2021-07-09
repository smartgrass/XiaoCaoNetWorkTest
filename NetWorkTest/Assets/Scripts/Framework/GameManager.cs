using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Player
{
    public class GameManager :MonoSingleton<GameManager>
    {
        List<IGameUpdate> gameUpdates = new List<IGameUpdate>();
        public void Awake()
        {
            gameUpdates.Add(PlayerManager.Instance);



        }

        private void Update()
        {
            int _len = gameUpdates.Count;
            for (int i = 0; i < _len; i++)
            {
                 gameUpdates[i].GameUpdate();
            }
        }

    }
}

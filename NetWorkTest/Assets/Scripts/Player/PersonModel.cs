using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Player
{
    public class PersonModel :IModel
    {
        public EnumPerson personType;


    }


    public enum EnumPerson:uint
    {
        Player,
        CostomPlayer,
        NPC,

    }

    

}

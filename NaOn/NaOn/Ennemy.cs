using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace NaOn
{
    class Ennemy : Entity
    {
        public Ennemy(string raceGiven)
        {
            this.tag = "ennemi";
            this.race = raceGiven;
            this.Image = Image.FromFile("./images/zombi/5.gif");

            //initialisation des donnes de base
            this.health[0] = 100;   //vie max = 100
            this.health[1] = this.health[0];    //vie actuelle = vie max
            this.mana[0] = 100; //mana max = 100
            this.mana[1] = this.mana[0];    //mana actuelle = mana max
        }

        private string race;


    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace NaOn
{
    class Ennemy : Entity
    {
        private string race;

        public Ennemy(string raceGiven)
        {
            this.tag = "ennemy";
            this.race = raceGiven;
            this.Image = Image.FromFile("./images/"+race+"/5.gif");
            this.Location = new Point(300, 0);

            //initialisation des donnes de base
            this.health[0] = 100;   //vie max = 100
            this.health[1] = this.health[0];    //vie actuelle = vie max
            this.mana[0] = 100; //mana max = 100
            this.mana[1] = this.mana[0];    //mana actuelle = mana max
        }

        //pour les ennemis de base, il y a 2 attaques, base et a distance
    }
}

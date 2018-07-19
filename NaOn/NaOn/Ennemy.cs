using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace NaOn
{
    class Ennemy : Entity
    {
        private string race;
        private int typeOfEnnemy;   //0 = distance, 1 = melee
        private int typeOfDamage;   //type de l ennemi
        private Timer artifialIntelligence;
        private Timer phasing;
        private bool actived = false;
        private Point aim;
        private int whatShot = 0;
        private int phase = 2;

        public Ennemy(string raceGiven, int typeOfEnnemyGiven, int typeOfDamageGiven)
        {
            this.tag = "ennemy";
            this.race = raceGiven;
            this.typeOfEnnemy = typeOfEnnemyGiven;
            this.typeOfDamage = typeOfDamageGiven;

            this.artifialIntelligence = new Timer();
            this.artifialIntelligence.Tick += this.artifialIntelligence_Tick;
            this.artifialIntelligence.Interval = 200;

            this.phasing = new Timer();
            this.phasing.Tick += this.phasing_Tick;

            //initialisation des donnes de base
            this.healthMax = 100;   //vie max = 100
            this.manaMax = 100; //mana max = 100

            if (typeOfEnnemy == 0)
            {
                switch (typeOfDamage)
                {
                    case 1:
                        this.Image = Image.FromFile("./images/ennemies/elementalFeu/1.bmp");
                        this.CreateAttack(1, 10, 20, 15, 13.0, "./images/attack/feu0/0.bmp");
                        this.phasing.Interval = 2000;
                        break;
                    case 2:
                        this.Image = Image.FromFile("./images/ennemies/elementalEau/1.bmp");
                        this.CreateAttack(1, 20, 20, 15, 10.0, "./images/attack/eau0/0.bmp");
                        this.phasing.Interval = 2500;
                        break;
                    case 3:
                        this.Image = Image.FromFile("./images/ennemies/elementalTerre/1.bmp");
                        this.CreateAttack(1, 40, 20, 15, 7.0, "./images/attack/terre0/0.bmp");
                        this.phasing.Interval = 3000;
                        break;
                }
            }
            if (typeOfEnnemy == 1)
            {
                this.Image = Image.FromFile("./images/ennemies/" + race + "/5.gif");
            }
            if (typeOfEnnemy == 2)
            {
                this.moveSpeed = 6; //vitesse du heros ([v] = pixel/0.01sec)
                this.Image = Image.FromFile("./images/ennemies/boss/0.bmp");
                for (int i = 0; i < 8; i++)
                {
                    this.CreateAttack(1, 10, 20, 15, 12.0, "./images/attack/normal0/0.bmp");
                }
                this.phasing.Interval = 1000;
                this.healthMax = 100;
            }
            
            Bitmap bmp = new Bitmap(this.Image);
            bmp.MakeTransparent();
            this.Image = bmp;

            this.Location = new Point(300, 0);

            this.health = this.healthMax;    //vie actuelle = vie max
            this.mana = this.manaMax;    //mana actuelle = mana max
        }

        public void WhereIsTheHero(Point herosPosition)
        {
            aim = herosPosition;
        }

        public override void ActivateEntity()
        {
            this.Enabled = true;
            this.life.Start();
            this.artifialIntelligence.Start();
        }

        public override void DesactivateEntity()
        {
            this.Enabled = false;
            this.life.Stop();
            this.artifialIntelligence.Stop();
            this.phasing.Stop();
            foreach (Attack whichAttack in listAttacks)
            {
                whichAttack.DesactivateAttack();
                whichAttack.timeRemainingCD = 0;
            }
        }

        private void phasing_Tick(Object sender, EventArgs e)  //selon le type d'ennemi, attaque d une certaine maniere
        {
            if ((typeOfEnnemy == 0) && (this.listAttacks[0].timeRemainingCD == 0))
            {
                this.listAttacks[0].ActivateAttack(this, aim);
            }
            if (typeOfEnnemy == 1)
            {
                //A venir
            }
            if (typeOfEnnemy == 2)
            {
                if (phase == 0)
                {
                    whatShot += 1;
                    if (whatShot == 2)
                    {
                        whatShot = 0;
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        this.listAttacks[i + (whatShot * 4)].ActivateAttack(this, new Point(
                            this.Location.X + (int)Math.Round(Math.Cos(Math.PI / 2 * i) * 20),
                            this.Location.Y + (int)Math.Round(Math.Sin(Math.PI / 2 * i) * 20)));
                    }
                }

                if (phase == 1)
                {
                    this.phasing.Interval = 200;
                    whatShot += 1;
                    if (whatShot == 8)
                    {
                        whatShot = 0;
                    }
                    this.listAttacks[whatShot].ActivateAttack(this, new Point(
                        this.Location.X + (int)Math.Round(Math.Cos(Math.PI / 4 * whatShot) * 20),
                        this.Location.Y + (int)Math.Round(Math.Sin(Math.PI / 4 * whatShot) * 20)));
                }

                if (phase == 2)
                {
                    this.phasing.Interval = 800;
                    whatShot += 1;
                    if (whatShot == 8)
                    {
                        whatShot = 0;
                    }
                    this.listAttacks[whatShot].ActivateAttack(this, aim);
                }

                if (phase == 3)
                {
                    this.phasing.Interval = 400;
                    whatShot += 1;
                    if (whatShot == 8)
                    {
                        whatShot = 0;
                    }
                    this.listAttacks[whatShot].ActivateAttack(this, aim);
                }
            }
        }
            

        private void artifialIntelligence_Tick(Object sender, EventArgs e)  //selon le type d'ennemi, reagit d une certaine maniere
        {
            if (actived)
            {
                if ((typeOfEnnemy == 0) && (this.listAttacks[0].timeRemainingCD == 0))
                {
                    this.listAttacks[0].ActivateAttack(this, aim);
                }
                if (typeOfEnnemy == 1)
                {
                    //A venir
                }
                if (typeOfEnnemy == 2)
                {
                    if (health > 0)
                    {
                        phase = 3;
                    }
                    if (health > 200)
                    {
                        phase = 2;
                    }
                    if (health > 400)
                    {
                        phase = 1;
                    }
                    if (health > 600)
                    {
                        phase = 0;
                    }
                    if (this.Left + this.Width / 2 - aim.X > -60)
                    {
                        this.MoveEntity(-1);
                    }
                    if (this.Left + this.Width / 2 - aim.X < -60)
                    {
                        this.MoveEntity(1);
                    }
                }
            }
            else
            {
                actived = true; //active l ennemi apres un premier tour (un peu de repis pour notre joueur)
                phasing.Start();
            }
        }
    }
}

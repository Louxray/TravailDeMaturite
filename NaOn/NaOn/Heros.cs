using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NaOn
{
    class Heros : Entity
    {
        private int travelling = 0;
        public Point positionStart { get; private set; }
        public Point position { get; private set; }
        Timer test = new Timer();   //test du labyrinthe
        int i;  //test du labyrinthe
        int j;  //test du labyrinthe

        public int level = 1;    //level pour savoir quelles competences avoir
        public int goldInDungeon;
        public int goldIRL;
        public int exp;
        public int expInDungeon;

        public Heros()
        {            
            //initialisation des donnes de base d un heros
            this.healthMax = 1000;   //vie max = 100
            this.health = this.healthMax;    //vie actuelle = vie max
            this.manaMax = 100; //mana max = 100
            this.mana = this.manaMax;    //mana actuelle = mana max
            
            this.moveSpeed = 3; //vitesse du heros ([v] = pixel/0.01sec)
            this.ChangeType(0);
            this.tag = "player";

            this.CreateAttack(0, 800, 20, 15, 15.0, "./images/attack/normal0/0.bmp");
            this.CreateAttack(1, 50, 20, 15, 15.0, "./images/attack/feu0/0.bmp");
            this.CreateAttack(2, 50, 20, 15, 15.0, "./images/attack/eau0/0.bmp");
            this.CreateAttack(3, 50, 20, 15, 15.0, "./images/attack/terre0/0.bmp");


            this.test = new Timer();
            this.test.Interval = 5000;

            this.typeOfDamage = 0;

            //permet de verifier que le labyrinthe est bien un labyrinthe
            /*
            this.test.Tick += this.test_Tick;
            this.test.Enabled = false;
            i = 0;
            j = 0;
            position = new Point(i, j);
            */
        }

        public void NewMaze()
        {
            Random rdm = new Random();
            this.position = new Point(rdm.Next(0, Form1.MAZE_WIDTH), rdm.Next(0, Form1.MAZE_HEIGHT));
            this.positionStart = this.position;
        }

        public void Conversion()
        {
            this.goldIRL += this.goldInDungeon;
        }

        private void test_Tick(object sender, EventArgs e)
        {
            if (i == 4)
            {
                i = 0;
                j += 1;
            }
            if (j == 4)
            {
                j = 0;
            }
            position = new Point(i, j);
            i += 1;
        }

        public override void Wound(int typeOfAttack, int damage) //0 = normal, 1 = feu, 2 = eau, 3 = terre, 4 = vent, 5 = electricite
        {
            bool toucheable = true;
            if ((this.typeOfDamage != 0)
                &&(this.typeOfDamage == typeOfAttack))
            {
                toucheable = false;
            }
            if ((!this.injured)
                && (toucheable))
            {
                this.health -= damage;
                this.injured = true;
                this.immunity = 12;
            }
        }

        public void MovePlayer(List<Decor> decors, Form1 whichForm)
        {
            double indic = 0;  //indicateur droite/gauche  -1 = gauche, 1 = droite

            if (travelling > 0)
            {
                travelling -= 1;
            }

            if (Keyboard.IsKeyDown(Key.D1) == true)
            {
                this.ChangeType(0);
            }

            if (Keyboard.IsKeyDown(Key.D2) == true)
            {
                this.ChangeType(1);
            }

            if (Keyboard.IsKeyDown(Key.D3) == true)
            {
                this.ChangeType(2);
            }

            if (Keyboard.IsKeyDown(Key.D4) == true)
            {
                this.ChangeType(3);
            }

            if (Keyboard.IsKeyDown(Key.Space) == true) //test pour sauter
            {
                this.Jump(decors);
            }

            if (Keyboard.IsKeyDown(Key.W) == true)   //test pour se baiser
            {
                foreach (Decor whichDecor in decors)
                {
                    if ((whichDecor.Bounds.IntersectsWith(this.Bounds)) && (whichDecor.interactive)
                        && (whichForm.Controls.Contains(whichDecor)))
                    {
                        switch (whichDecor.whichDecor)
                        {
                            case 2:
                                if (travelling == 0)
                                {
                                    this.OpenDoor(whichDecor.typeOfDecor);
                                    travelling = 50;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            if (Keyboard.IsKeyDown(Key.A) == true)   //test pour aller a gauche
            {
                indic -= 1; //vers la gauche
            }

            if (Keyboard.IsKeyDown(Key.S) == true)   //test pour se baiser
            {
                //player.Bow();
            }

            if (Keyboard.IsKeyDown(Key.D) == true)  //test pour aller a droite
            {
                indic += 1;    //vers la droite
            }

            if (Control.MouseButtons == MouseButtons.Right)
            {
                bool canLaunch = true;
                foreach (Attack whichAttack in this.listAttacks)
                {
                    if (whichAttack.timeRemainingCD > 0)
                    {
                        canLaunch = false;
                    }
                }
                if ((canLaunch)
                    && ((whichForm.PointToClient(System.Windows.Forms.Cursor.Position).X > -1)
                    && (whichForm.PointToClient(System.Windows.Forms.Cursor.Position).X < whichForm.ClientSize.Width)
                    && (whichForm.PointToClient(System.Windows.Forms.Cursor.Position).Y > -1)
                    && (whichForm.PointToClient(System.Windows.Forms.Cursor.Position).Y < whichForm.ClientSize.Height)))
                {
                    this.listAttacks[typeOfDamage].ActivateAttack(this, whichForm.PointToClient(System.Windows.Forms.Cursor.Position));
                }
            }

            if ((Keyboard.IsKeyDown(Key.LeftShift) == true))
            {
                indic *= 1.7; //permet de courir
            }

            if (!this.CollisionMur(decors, indic))
            {
                this.MoveEntity(indic);    //transmet la direction et la vitesse pour bouger le joueur
            }
        }

        private void ChangeType(int inWhichType)
        {
            Bitmap bmp = null;
            this.typeOfDamage = inWhichType;
            switch (typeOfDamage)
            {
                case 0:
                    bmp = new Bitmap(Image.FromFile("./images/heros/normal/0.bmp")); //charge l image d attente du heros
                    break;
                case 1:
                    bmp = new Bitmap(Image.FromFile("./images/heros/feu/0.bmp")); //charge l image d attente du heros
                    break;
                case 2:
                    bmp = new Bitmap(Image.FromFile("./images/heros/eau/0.bmp")); //charge l image d attente du heros
                    break;
                case 3:
                    bmp = new Bitmap(Image.FromFile("./images/heros/terre/0.bmp")); //charge l image d attente du heros
                    break;
            }
            bmp.MakeTransparent();
            this.Image = bmp;
        }
                
        public override bool TestMort(int formHeight)
        {
            return this.health <= 0; //meurt si plus de pv
        }        

        private void OpenDoor(int whichDoor)
        {
            switch (whichDoor)
            {
                case 0:
                    this.position = new Point(this.position.X, this.position.Y - 1);
                    break;
                case 1:
                    this.position = new Point(this.position.X + 1, this.position.Y);
                    break;
                case 2:
                    this.position = new Point(this.position.X, this.position.Y + 1);
                    break;
                case 3:
                    this.position = new Point(this.position.X - 1, this.position.Y);
                    break;
            }
        }
    }
}

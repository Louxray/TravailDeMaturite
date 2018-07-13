using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Diagnostics;

namespace NaOn
{
    public partial class Form1 : Form
    {
        const int MAZE_WIDTH = 4;  //largeur du donjon (=dj) (labyrinthe)
        const int MAZE_HEIGHT = 4; //hauteur du donjon (=dj) (labyrinthe)
        Heros player = new Heros(); //creation personnage
        Room[,] dungeon = new Room[MAZE_HEIGHT, MAZE_WIDTH];    //creation du dj selon les dimension
        Point whichPosition = new Point(0,0);   //localisation dans le dj

        List<Entity> goodOnes = new List<Entity>();
        List<Entity> ennemies = new List<Entity>();
        List<Decor> decors = new List<Decor>();
        List<Entity> everybody = new List<Entity>();

        int[,] visiondimension = new int[2, 2];

        List<Timer> allTimers = new List<Timer>();
        Timer cooldowns;
        Timer movements;

        public Form1()
        {
            InitializeComponent();
      
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DoubleBuffered = true;
            
            //ajoute les persos ici
            goodOnes.Add(player);
            player.Left = 200;
            player.Top = 200;
            Ennemy lol = new Ennemy("zombie");
            ennemies.Add(lol);
            lol.ActivateEntity();

            this.cooldowns = new Timer();
            this.cooldowns.Interval = 100;
            this.cooldowns.Tick += this.cooldowns_Tick;
            this.cooldowns.Enabled = true;
            this.allTimers.Add(cooldowns);

            this.movements = new Timer();
            this.movements.Interval = 10;
            this.movements.Tick += this.MoveEntities;
            this.movements.Enabled = true;
            this.allTimers.Add(movements);


            //repetorie toutes les entites
            for (int i = 0; i < goodOnes.Count; i++)
            {
                everybody.Add(goodOnes[i]);
            }

            for (int i = 0; i < ennemies.Count; i++)
            {
                everybody.Add(ennemies[i]);
            }
            
            for (int i = 0; i < everybody.Count; i++)
            {
                this.Controls.Add(everybody[i]);    //ajoute tout le monde aux controles
                foreach(Attack whatAdd in everybody[i].listAttacks)
                {
                    this.Controls.Add(whatAdd);
                }
            }

            //parametres de base de la fenetre


            this.ClientSize = new Size((int)(840), (int)(600));
            Size visiondimension = new Size((int)(Screen.PrimaryScreen.Bounds.Width / 1.5), (int)(Screen.PrimaryScreen.Bounds.Height / 2));
            //this.ClientSize = new Size((int)(Screen.PrimaryScreen.Bounds.Width / 1.5), (int)(Screen.PrimaryScreen.Bounds.Height / 1.5));
            this.Location = new Point((int)((Screen.PrimaryScreen.Bounds.Width - this.ClientSize.Width) / 2), (int)((Screen.PrimaryScreen.Bounds.Height - this.ClientSize.Height) / 2));    //position de la fenetre sur l ecran
            this.MaximumSize = this.ClientSize; //bloque la taille max de la fenetre
            this.MinimumSize = this.ClientSize; //bloque la taille min de la fenetre
            this.MaximizeBox = false;   //empeche le plein ecran
            
            //visiondimension =  { { (int)(Screen.PrimaryScreen.Bounds.Width / 6), (int)(Screen.PrimaryScreen.Bounds.Height / 4) }, { (int)(Screen.PrimaryScreen.Bounds.Width / 1.5), (int)(Screen.PrimaryScreen.Bounds.Height / 2) } };

            //creation zones de decor
            CreateMaze();
            player.Location = new Point(70, 70);
            player.ActivateEntity();

        }   

        private void MoveEntities(object sender, EventArgs e)
        {
            if ((Control.MouseButtons == MouseButtons.Left)
                && ((PointToClient(System.Windows.Forms.Cursor.Position).X < -1)
                || (PointToClient(System.Windows.Forms.Cursor.Position).X > ClientSize.Width)
                || (PointToClient(System.Windows.Forms.Cursor.Position).Y < -1)
                || (PointToClient(System.Windows.Forms.Cursor.Position).Y > ClientSize.Height)))
            {
                this.Pause();
            }
                player.MovePlayer(dungeon[whichPosition.Y, whichPosition.X].decorsInRoom, this);    //demarre le test des touches
            CollisionAttack();
            for (int i = 0; i < everybody.Count; i++)
            {
                everybody[i].Gravity(dungeon[whichPosition.Y, whichPosition.X].decorsInRoom);
                if (everybody[i].TestMort(this.ClientSize.Height))
                {
                    if (everybody[i].tag == "ennemy")
                    {
                        Entity whoDestroy;
                        whoDestroy = everybody[i];
                        Controls.Remove(everybody[i]);
                        ennemies.Remove(everybody[i]);
                        everybody.Remove(everybody[i]);
                        whoDestroy.Dispose();
                        break;
                    }
                    if (everybody[i].tag == "player")
                    {
                        player.Wound(0, 25);
                    }
                    //  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    //
                    //  IL FAUT ENCORE CHANGER LE FAIT QUE SI LES GENTILS TOMBENT ILS NE MEURENT PAS
                    //
                    //  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                }
            }
        }

        private void cooldowns_Tick(object sender, EventArgs e)
        {
            label1.Text = (player.listAttacks[0].timeRemainingCD / 10.0).ToString();
            foreach (Entity who in everybody)
            {
                foreach (Attack whichAttack in who.listAttacks)
                {
                    if (whichAttack.timeRemainingCD > 0)
                    {
                        whichAttack.LowerCD();
                    }
                }
            }
        }

        private void CollisionAttack()
        {
            foreach (Decor whichDecor in dungeon[whichPosition.Y, whichPosition.X].decorsInRoom)
            {
                foreach (Entity whichAlly in goodOnes)
                {
                    foreach (Ennemy whichEnnemy in ennemies)
                    {
                        foreach (Attack whichAttack in whichEnnemy.listAttacks)
                        {
                            if ((whichAttack.Enabled)
                                && (whichAttack.Bounds.IntersectsWith(whichAlly.Bounds)))
                            {
                                whichAlly.Wound(whichAttack.typeOfDamage, whichAttack.damage);
                                whichAttack.DesactivateAttack();
                            }
                            if ((whichAttack.Enabled)
                                && (whichAttack.Bounds.IntersectsWith(whichDecor.Bounds))
                                && (whichDecor.whichDecor == 0))
                            {
                                whichAttack.DesactivateAttack();
                            }
                        }
                        foreach (Attack whichAttack in whichAlly.listAttacks)
                        {
                            if ((whichAttack.Enabled)
                                && (whichAttack.Bounds.IntersectsWith(whichEnnemy.Bounds)))
                            {
                                whichEnnemy.Wound(whichAttack.typeOfDamage, whichAttack.damage);
                                whichAttack.DesactivateAttack();
                            }
                            if ((whichAttack.Enabled)
                                && (whichAttack.Bounds.IntersectsWith(whichDecor.Bounds))
                                && (whichDecor.whichDecor == 0))
                            {
                                whichAttack.DesactivateAttack();
                            }
                        }
                    }
                }
            }
        }

        private void CreateMaze()
        {
            //int[,] test = new int[MAZE_HEIGHT, MAZE_WIDTH];
            List<Point> alreadyVisited = new List<Point>();
            List<int> availableRooms = new List<int>();
            Random rdm = new Random();
            Point start = new Point(rdm.Next(0, dungeon.GetUpperBound(1) + 1), rdm.Next(0, dungeon.GetUpperBound(0) + 1));
            Point bossesRoom = new Point(rdm.Next(0, dungeon.GetUpperBound(1) + 1), rdm.Next(0, dungeon.GetUpperBound(0) + 1));
            whichPosition = bossesRoom;
            alreadyVisited.Add(whichPosition);
            int whichRoom = -1;
            for (int i = 0; i < dungeon.GetUpperBound(0) + 1; i++)
           {
                for (int j = 0; j < dungeon.GetUpperBound(1) + 1; j++)
                {
                    if (((start.Y == i) && (start.X == j))
                        || ((bossesRoom.Y == i) && (bossesRoom.X == j)))
                    {
                        this.dungeon[i, j] = new Room(true);
                    }
                    else
                    {
                        this.dungeon[i, j] = new Room(false);
                    }
                    //test[i, j] = 0;
                }
            }

            //test[whichPosition.Y, whichPosition.X] = 1;
            while (!MazeFinished())
            {
                if (!Surrounded())
                {
                    for (int i = 0; i<Room.NBR_DOOR; i++)
                    {
                        availableRooms.Add(i);
                    }
                    if (Surrounded(0))
                    {
                        availableRooms.Remove(0);
                    }
                    if (Surrounded(1))
                    {
                        availableRooms.Remove(1);
                    }
                    if (Surrounded(2))
                    {
                        availableRooms.Remove(2);
                    }
                    if (Surrounded(3))
                   {
                        availableRooms.Remove(3);
                    }
                    whichRoom = availableRooms[rdm.Next(0, availableRooms.Count)];
                    dungeon[whichPosition.Y, whichPosition.X].RemoveWall(whichRoom);
                    switch (whichRoom)
                    {
                        case 0:
                            whichPosition = new Point(whichPosition.X, whichPosition.Y - 1);
                            whichRoom = 2;
                            break;
                        case 1:
                            whichPosition = new Point(whichPosition.X + 1, whichPosition.Y);
                            whichRoom = 3;
                            break;
                        case 2:
                            whichPosition = new Point(whichPosition.X, whichPosition.Y + 1);
                            whichRoom = 0;
                            break;
                        case 3:
                            whichPosition = new Point(whichPosition.X - 1, whichPosition.Y);
                            whichRoom = 1;
                            break;
                    }
                    dungeon[whichPosition.Y, whichPosition.X].RemoveWall(whichRoom);
                    alreadyVisited.Add(whichPosition);

                    //test[whichPosition.Y, whichPosition.X] = 1;
                    availableRooms.Clear();
                }
                else
                {
                    if (alreadyVisited.Count > 0)
                    {
                        //test[whichPosition.Y, whichPosition.X] = 2;
                        alreadyVisited.Remove(alreadyVisited[alreadyVisited.Count - 1]);
                        whichPosition = alreadyVisited[alreadyVisited.Count - 1];
                    }
                }
                /*
                for (int i = 0; i < dungeon.GetUpperBound(0) + 1; i++)
                {
                    for (int j = 0; j < dungeon.GetUpperBound(1) + 1; j++)
                    {
                        Debug.Write(test[i, j].ToString() + " ");
                    }
                    Debug.WriteLine("");
                }
                Debug.WriteLine("");
                */
            }
            whichPosition = start;
            EnterTheRoom();
        }

        private bool MazeFinished()
        {
            for (int i = 0; i<dungeon.GetUpperBound(0) + 1; i++)
            {
                for (int j = 0; j<dungeon.GetUpperBound(1) + 1; j++)
                {
                    if (!dungeon[i, j].visited)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool Surrounded(int whichDoor = 4)
        {
            switch (whichDoor)
            {
                case 0: //haut
                    if ((whichPosition.Y - 1 >= 0) && (!dungeon[whichPosition.Y - 1, whichPosition.X].visited))
                    {
                        return false;
                    }
                    break;
                case 1: //droite
                    if ((whichPosition.X + 1 <= dungeon.GetUpperBound(1)) && (!dungeon[whichPosition.Y, whichPosition.X + 1].visited))
                    {
                        return false;
                    }
                    break;
                case 2: //bas
                    if ((whichPosition.Y + 1 <= dungeon.GetUpperBound(0)) && (!dungeon[whichPosition.Y + 1, whichPosition.X].visited))
                    {
                        return false;
                    }
                    break;
                case 3: //gauche
                    if ((whichPosition.X - 1 >= 0) && (!dungeon[whichPosition.Y, whichPosition.X - 1].visited))
                    {
                        return false;
                    }
                    break;
                case 4:
                    if ((whichPosition.Y - 1 >= 0) && (!dungeon[whichPosition.Y - 1, whichPosition.X].visited)
                        || (whichPosition.Y + 1 <= dungeon.GetUpperBound(0)) && (!dungeon[whichPosition.Y + 1, whichPosition.X].visited)
                        || (whichPosition.X - 1 >= 0) && (!dungeon[whichPosition.Y, whichPosition.X - 1].visited)
                        || (whichPosition.X + 1 <= dungeon.GetUpperBound(1)) && (!dungeon[whichPosition.Y, whichPosition.X + 1].visited))
                    {
                        return false;
                    }
                    break;
            }
            return true;
        }

        public void GoOut()
        {
            foreach (Decor whichDecor in dungeon[whichPosition.Y, whichPosition.X].decorsInRoom)
            {
                this.Controls.Remove(whichDecor);
            }
        }

        public void EnterTheRoom()
        {
            dungeon[whichPosition.Y, whichPosition.X].CreateRoom();
            this.Controls.AddRange(dungeon[whichPosition.Y, whichPosition.X].decorsInRoom.ToArray());
        }
            
        private bool TestInGame()
        {
            if ((Control.MousePosition.X >= 0)
                && (Control.MousePosition.X <= this.ClientSize.Width)
                && (Control.MousePosition.Y >= 0)
                && (Control.MousePosition.Y <= this.ClientSize.Height))
            {
                return true;
            }
            return false;
        }

        private void Pause()
        {
            foreach (Timer whichTimer in allTimers)
            {
                whichTimer.Stop();
            }
            foreach (Entity who in dungeon[whichPosition.Y, whichPosition.X].ennemiesInRoom)
            {
                if (who.Enabled)
                {
                    who.DesactivateEntity();
                }
            }
            player.DesactivateEntity();
        }

        private void Unpause()
        {
            foreach (Timer whichTimer in allTimers)
            {
                whichTimer.Start();
            }
            foreach (Entity who in dungeon[whichPosition.Y, whichPosition.X].ennemiesInRoom)
            {
                if (!who.Enabled)
                {
                    who.ActivateEntity();
                }
            }
            player.ActivateEntity();
        }
    }
}
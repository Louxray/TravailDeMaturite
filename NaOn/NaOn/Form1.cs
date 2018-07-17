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
        public static readonly int MAZE_WIDTH = 4;  //largeur du donjon (=dj) (labyrinthe)
        public static readonly int MAZE_HEIGHT = 4; //hauteur du donjon (=dj) (labyrinthe)
        Heros player = new Heros(); //creation personnage
        Room[,] dungeon = new Room[MAZE_HEIGHT, MAZE_WIDTH];    //creation du dj selon les dimension
        Point whichPosition;   //localisation dans le dj

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
            player.ActivateEntity();

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

        }   

        private void MoveEntities(object sender, EventArgs e)
        {
            if ((Control.MouseButtons == MouseButtons.Left)
                && ((PointToClient(System.Windows.Forms.Cursor.Position).X < -1)
                || (PointToClient(System.Windows.Forms.Cursor.Position).X > ClientSize.Width)
                || (PointToClient(System.Windows.Forms.Cursor.Position).Y < -1)
                || (PointToClient(System.Windows.Forms.Cursor.Position).Y > ClientSize.Height)))
            {
                //this.Pause();
            }
            player.MovePlayer(dungeon[whichPosition.Y, whichPosition.X].decorsInRoom, this);    //demarre le test des touches
            if (player.position != whichPosition)
            {
                int difference = 4;
                this.GoOut();
                if (whichPosition.X - player.position.X < 0)
                {
                    difference = 1;
                }
                if (whichPosition.X - player.position.X > 0)
                {
                    difference = 3;
                }
                if (whichPosition.Y - player.position.Y < 0)
                {
                    difference = 2;
                }
                if (whichPosition.Y - player.position.Y > 0)
                {
                    difference = 0;
                }
                whichPosition = player.position;
                this.EnterTheRoom(difference);
            }
            CollisionAttack();
            for (int i = 0; i < everybody.Count; i++)
            {
                if (everybody[i].Enabled)
                {
                    everybody[i].Gravity(dungeon[whichPosition.Y, whichPosition.X].decorsInRoom);
                    if (everybody[i].tag == "ennemy")
                    {
                        if (everybody[i].TestMort(this.ClientSize.Height))
                        {
                            Entity whoDestroy;
                            whoDestroy = everybody[i];
                            Controls.Remove(everybody[i]);
                            ennemies.Remove(everybody[i]);
                            everybody.Remove(everybody[i]);
                            whoDestroy.Dispose();
                            break;  //au cas ou je remets des instructions derriere
                        }
                    }
                }
            }
            if (player.TestTombe(this.ClientSize.Height))
            {
                player.Wound(0, 25);
                this.EnterTheRoom(4);
                player.ResetFallSpeed();
            }
            label2.Text = player.health[1].ToString();
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
                foreach (Entity who in everybody)
                {
                    foreach (Attack whichAttack in who.listAttacks)
                    {
                        if ((whichAttack.Enabled)
                            && (whichAttack.Bounds.IntersectsWith(whichDecor.Bounds))
                            && (whichDecor.whichDecor == 0))
                        {
                            whichAttack.DesactivateAttack();
                        }
                    }

                }
                foreach (Entity whichAlly in goodOnes)
                {
                    foreach (Ennemy whichEnnemy in ennemies)
                    {
                        whichEnnemy.WhereIsTheHero(new Point(whichAlly.Location.X + whichAlly.Width / 2, whichAlly.Location.Y + whichAlly.Width / 2));
                        foreach (Attack whichAttack in whichEnnemy.listAttacks)
                        {
                            if ((whichAttack.Enabled)
                                && (whichAttack.Bounds.IntersectsWith(whichAlly.Bounds))
                                && (whichAlly.Enabled))
                            {
                                whichAlly.Wound(whichAttack.typeOfDamage, whichAttack.damage);
                                whichAttack.DesactivateAttack();
                            }
                        }
                        foreach (Attack whichAttack in whichAlly.listAttacks)
                        {
                            if ((whichAttack.Enabled)
                                && (whichAttack.Bounds.IntersectsWith(whichEnnemy.Bounds))
                                && (whichEnnemy.Enabled))
                            {
                                whichEnnemy.Wound(whichAttack.typeOfDamage, whichAttack.damage);
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
            Point start = player.position;
            Point bossesRoom;
            do
            {
                bossesRoom = new Point(rdm.Next(0, dungeon.GetUpperBound(1) + 1), rdm.Next(0, dungeon.GetUpperBound(0) + 1));
            } while (bossesRoom == start);
            whichPosition = bossesRoom;
            alreadyVisited.Add(whichPosition);
            int whichRoom = -1;
            for (int i = 0; i < dungeon.GetUpperBound(0) + 1; i++)
           {
                for (int j = 0; j < dungeon.GetUpperBound(1) + 1; j++)
                {
                    if ((start.Y == i) && (start.X == j))
                    {
                        this.dungeon[i, j] = new Room(true, true);
                    }
                    else if ((bossesRoom.Y == i) && (bossesRoom.X == j))
                    {
                        this.dungeon[i, j] = new Room(true, true);
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
            for (int i = 0; i < dungeon.GetUpperBound(0) + 1; i++)
            {
                for (int j = 0; j < dungeon.GetUpperBound(1) + 1; j++)
                {
                    dungeon[i, j].Reset();
                }
            }
            whichPosition = start;
            EnterTheRoom(4);
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
            foreach (Ennemy ennemy in dungeon[whichPosition.Y, whichPosition.X].ennemiesInRoom)
            {
                ennemy.DesactivateEntity();
                this.Controls.Remove(ennemy);
            }
        }

        public void EnterTheRoom(int comingFrom)    //comingFrom = quelle porte le perso a pris pour rentrer dans la salle
        {
            if (!dungeon[player.position.Y, player.position.X].visited)
            {
                dungeon[player.position.Y, player.position.X].CreateRoom();
            }
            dungeon[player.position.Y, player.position.X].Visit();
            switch (comingFrom)
            {
                case 0:
                    for (int i = 0; i < Room.NBR_PLATFORM; i++)
                    {
                        if (dungeon[player.position.Y, player.position.X].doorsInTheRoom[i, 1] == true)
                        {
                            player.Location = new Point(284, (i + 1) * 110);
                        }
                    }
                    break;
                case 1:
                    for (int i = 0; i < Room.NBR_PLATFORM; i++)
                    {
                        if (dungeon[player.position.Y, player.position.X].doorsInTheRoom[i, 0] == true)
                        {
                            player.Location = new Point(67, (i + 1) * 110);
                        }
                    }
                    break;
                case 2:
                    for (int i = 0; i < Room.NBR_PLATFORM; i++)
                    {
                        if (dungeon[player.position.Y, player.position.X].doorsInTheRoom[i, 2] == true)
                        {
                            player.Location = new Point(500, (i + 1) * 110);
                        }
                    }
                    break;
                case 3:
                    for (int i = 0; i < Room.NBR_PLATFORM; i++)
                    {
                        if (dungeon[player.position.Y, player.position.X].doorsInTheRoom[i, 3] == true)
                        {
                            player.Location = new Point(725, (i + 1) * 110);
                        }
                    }
                    break;
                case 4: //entre au debut (centre de la salle)
                    player.Location = new Point(500, 70);
                    break;
            }
            this.Controls.AddRange(dungeon[player.position.Y, player.position.X].decorsInRoom.ToArray());
            foreach (Ennemy whichEnnemy in dungeon[player.position.Y, player.position.X].ennemiesInRoom)
            {
                this.Controls.Add(whichEnnemy);
                foreach (Attack whichAttack in whichEnnemy.listAttacks)
                {
                    this.Controls.Add(whichAttack);
                }
            }
            ennemies.AddRange(dungeon[player.position.Y, player.position.X].ennemiesInRoom.ToArray());
            everybody.AddRange(dungeon[player.position.Y, player.position.X].ennemiesInRoom.ToArray());
            foreach (Ennemy ennemy in dungeon[player.position.Y, player.position.X].ennemiesInRoom)
            {
                ennemy.ActivateEntity();
            }
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
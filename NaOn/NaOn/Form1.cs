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
        bool bossAlive;

        string etat = "inTheMenu";
        //  etat a plusieurs choix:
        //  -inGame lorsque le jeu tourne
        //  -pause lorsque le jeu est en pause
        //  -inTheMenu lorsque que l on a pas encore commence a joue

        //boutons pour les menus
        List<PictureBox> allMenuBox = new List<PictureBox>();
        PictureBox enter = new PictureBox();
        PictureBox rules = new PictureBox();
        PictureBox unPause = new PictureBox();
        PictureBox exit = new PictureBox();
        PictureBox terminate = new PictureBox();
        PictureBox background = new PictureBox();

        List<Entity> goodOnes = new List<Entity>();
        List<Entity> ennemies = new List<Entity>();
        List<Decor> decors = new List<Decor>();
        List<Entity> everybody = new List<Entity>();

        List<Timer> allTimers = new List<Timer>();
        Timer cooldowns;
        Timer movements;

        Timer backgroundInTheBackground;    //timer a part pour garder le fond derriere

        public Form1()
        {
            InitializeComponent();      
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DoubleBuffered = true;

            //ajoute les persos ici
            goodOnes.Add(player);

            this.cooldowns = new Timer();
            this.cooldowns.Interval = 100;
            this.cooldowns.Tick += this.cooldowns_Tick;
            this.cooldowns.Enabled = false;
            this.allTimers.Add(cooldowns);

            this.movements = new Timer();
            this.movements.Interval = 10;
            this.movements.Tick += this.MoveEntities;
            this.movements.Enabled = false;
            this.allTimers.Add(movements);

            this.backgroundInTheBackground = new Timer();
            this.backgroundInTheBackground.Interval = 10;
            this.backgroundInTheBackground.Tick += this.backgroundInTheBackground_Tick;
            this.backgroundInTheBackground.Enabled = true;
            this.allTimers.Add(backgroundInTheBackground);

            //repetorie toutes les entites
            for (int i = 0; i < goodOnes.Count; i++)
            {
                everybody.Add(goodOnes[i]);
            }

            /*
            feature pour le jeu complet (Travail de maturité)
            
            for (int i = 0; i < everybody.Count; i++)
            {
                this.Controls.Add(everybody[i]);    //ajoute tout le monde aux controles
                foreach(Attack whatAdd in everybody[i].listAttacks)
                {
                    this.Controls.Add(whatAdd);
                }
            }
            */

            //parametres de base de la fenetre
            this.ClientSize = new Size((int)(840), (int)(600));
            Size visiondimension = new Size((int)(Screen.PrimaryScreen.Bounds.Width / 1.5), (int)(Screen.PrimaryScreen.Bounds.Height / 2.0));
            this.Location = new Point((int)((Screen.PrimaryScreen.Bounds.Width - this.ClientSize.Width) / 2.0), (int)((Screen.PrimaryScreen.Bounds.Height - this.ClientSize.Height) / 2.0));    //position de la fenetre sur l ecran
            this.MaximumSize = this.ClientSize; //bloque la taille max de la fenetre
            this.MinimumSize = this.ClientSize; //bloque la taille min de la fenetre
            this.MaximizeBox = false;   //empeche le plein ecran

            //definition des boutons des jeux
            Bitmap bmp; //bitmap pour rendre transparent les images
            
            this.Controls.Add(background);    //ajoute aux controles
            this.background.Image = Image.FromFile("./images/menu/background.bmp"); //charge l image
            this.background.SizeMode = PictureBoxSizeMode.AutoSize;    //autosize
            this.background.Enabled = true;   //rend fonctionnel
            this.background.Visible = true;   //rend invisible
            this.background.Location = new Point(0, 0);
            
            SetPictureBox(enter, "./images/menu/enter.bmp", new Point(this.ClientSize.Width / 2, this.ClientSize.Height / 3));
            this.enter.Click += Enter_Click;
            
            SetPictureBox(rules, "./images/menu/rules.bmp", new Point(this.ClientSize.Width / 2, this.ClientSize.Height / 2));
            this.rules.Click += Rules_Click;
            
            SetPictureBox(unPause, "./images/menu/unPause.bmp", new Point(this.ClientSize.Width / 2, this.ClientSize.Height / 3));
            this.unPause.Click += UnPause_Click;
            
            SetPictureBox(exit, "./images/menu/exit.bmp", new Point(this.ClientSize.Width - 120, 100));
            this.exit.Click += Exit_Click;
            
            SetPictureBox(terminate, "./images/menu/terminate.bmp", new Point(this.ClientSize.Width / 2, this.ClientSize.Height * 2 / 3));
            this.terminate.Click += Terminate_Click;

            allMenuBox.Add(enter);
            allMenuBox.Add(rules);
            allMenuBox.Add(unPause);
            allMenuBox.Add(exit);
            allMenuBox.Add(terminate);

            EnterTheMenu();
        }

        private void Terminate_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            EnterTheMenu();
        }

        private void UnPause_Click(object sender, EventArgs e)
        {
            Unpause();
        }

        private void Rules_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Enter_Click(object sender, EventArgs e)
        {
            EnterTheGame();
        }

        private void MoveEntities(object sender, EventArgs e)
        {
            if (((Control.MouseButtons == MouseButtons.Left)
                && ((PointToClient(System.Windows.Forms.Cursor.Position).X < -1)
                || (PointToClient(System.Windows.Forms.Cursor.Position).X > ClientSize.Width)
                || (PointToClient(System.Windows.Forms.Cursor.Position).Y < -1)
                || (PointToClient(System.Windows.Forms.Cursor.Position).Y > ClientSize.Height)))
                || (Keyboard.IsKeyDown(Key.Escape) == true))
            {
                this.Pause();
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
            if (dungeon[player.position.Y,player.position.X].ennemiesInRoom.Count > 0)
            {
                hideDoors();
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
                            dungeon[player.position.Y, player.position.X].ennemiesInRoom.Remove(everybody[i]);
                            everybody.Remove(everybody[i]);
                            whoDestroy.Dispose();
                            if (dungeon[player.position.Y, player.position.X].ennemiesInRoom.Count == 0)
                            {
                                showDoors();
                                if ((dungeon[player.position.Y, player.position.X].specialRoom) && (new Point(player.position.Y, player.position.X) != player.positionStart))
                                {
                                    bossAlive = false;
                                }
                            }
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
            label2.Text = player.health.ToString();
            if (!bossAlive)
            {
                ActivatePictureBox(exit);
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

        private void backgroundInTheBackground_Tick(object sender, EventArgs e)
        {
            this.background.SendToBack();
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
                            && ((whichDecor.whichDecor == 0)
                            && ((whichAttack.Bounds.IntersectsWith(whichDecor.Bounds)
                            || (whichAttack.Location.X < 0)
                            || (whichAttack.Location.X > this.ClientSize.Width)
                            || (whichAttack.Location.Y < 0)
                            || (whichAttack.Location.Y > this.ClientSize.Height)))))
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

        //dans cette fonction, les commentaires servent a comprendre comment le labyrinthe est fait, pour le debug et pour les profs
        private void CreateMaze()
        {
            for (int i = 0; i < everybody.Count; i++)
            {
                this.Controls.Add(everybody[i]);    //ajoute tout le monde aux controles
                foreach (Attack whatAdd in everybody[i].listAttacks)
                {
                    this.Controls.Add(whatAdd);
                }
            }
            this.player.ActivateEntity();
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
            bossAlive = true;
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

        public void hideDoors()
        {
            foreach (Decor whichDecor in dungeon[player.position.Y, player.position.X].decorsInRoom)
            {
                if (whichDecor.whichDecor == 2)
                {
                    this.Controls.Remove(whichDecor);
                }
            }
        }

        public void showDoors()
        {
            foreach (Decor whichDecor in dungeon[player.position.Y, player.position.X].decorsInRoom)
            {
                if (whichDecor.whichDecor == 2)
                {
                    this.Controls.Add(whichDecor);
                }
            }
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
            foreach (Attack whichAttack in player.listAttacks)
            {
                whichAttack.DesactivateAttack();
            }
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
            etat = "pause";
            foreach (Entity who in dungeon[whichPosition.Y, whichPosition.X].ennemiesInRoom)
            {
                if (who.Enabled)
                {
                    who.DesactivateEntity();
                }
            }
            player.DesactivateEntity();

            foreach (Timer whichTimer in allTimers)
            {
                whichTimer.Stop();
            }
            foreach (PictureBox whichPictureBox in allMenuBox)
            {
                DesactivatePictureBox(whichPictureBox);
            }
            ActivatePictureBox(unPause);
            ActivatePictureBox(rules);
            ActivatePictureBox(exit);
        }

        private void Unpause()
        {
            etat = "inGame";
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

            foreach (PictureBox whichPictureBox in allMenuBox)
            {
                DesactivatePictureBox(whichPictureBox);
            }
        }

        private void EnterTheMenu()
        {
            etat = "inTheMenu";

            foreach (PictureBox whichPictureBox in allMenuBox)
            {
                DesactivatePictureBox(whichPictureBox);
            }

            ActivatePictureBox(enter);
            ActivatePictureBox(rules);
            ActivatePictureBox(terminate);
            this.background.Image = Image.FromFile("./images/menu/background.bmp"); //charge l image
        }

        private void EnterTheGame()
        {
            etat = "inGame";

            foreach (PictureBox whichPictureBox in allMenuBox)
            {
                DesactivatePictureBox(whichPictureBox);
            }

            this.background.Image = Image.FromFile("./images/menu/cave.bmp"); //charge l image

            foreach (Timer whichTimer in allTimers)
            {
                whichTimer.Start();
            }

            //creation labyrinthe
            CreateMaze();
        }

        private void SetPictureBox(PictureBox whichPictureBox, string path, Point where)
        {
            Bitmap bmp;
            this.Controls.Add(whichPictureBox);
            whichPictureBox.Image = Image.FromFile(path);
            whichPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            whichPictureBox.Location = new Point(where.X - whichPictureBox.Width / 2, where.Y - whichPictureBox.Height / 2);
            DesactivatePictureBox(whichPictureBox);
            bmp = new Bitmap(whichPictureBox.Image);
            bmp.MakeTransparent();
            whichPictureBox.Image = bmp;
        }

        private void ActivatePictureBox(PictureBox whichPictureBox)
        {
            whichPictureBox.Enabled = true;
            whichPictureBox.Visible = true;
        }

        private void DesactivatePictureBox(PictureBox whichPictureBox)
        {
            whichPictureBox.Enabled = false;
            whichPictureBox.Visible = false;
        }
    }
}
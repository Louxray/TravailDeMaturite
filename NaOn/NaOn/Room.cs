using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace NaOn
{
    class Room
    {
        private bool specialRoom = false;
        public static readonly int NBR_DOOR = 4;
        public static readonly int NBR_PLATFORM = 4;
        private int typeOfRoom;
        private bool[,] platforms = new bool[NBR_PLATFORM, NBR_PLATFORM];
        public bool visited { get; private set; } = false;
        public List<int> doors = new List<int>(); //0 = up, 1 = right, 2 = down, 3 = left
        public bool[,] doorsInTheRoom { get; private set; } = new bool[NBR_PLATFORM, NBR_PLATFORM];

        public List<Decor> decorsInRoom { get; private set; } = new List<Decor>();
        public List<Entity> ennemiesInRoom { get; private set; } = new List<Entity>();

        public Room(bool specialRoomOrNot)   //je dois ajouter l argument du type de dj
        {
            this.specialRoom = specialRoomOrNot;
        }

        public void Reset()
        {
            this.visited = false;
        }

        public void Visit()
        {
            this.visited = true;
        }

        public void CreateRoom()
        {
            int[] shortest = new int[] { -10, -10, -10, -10 };
            Random rdm = new Random();
            this.typeOfRoom = rdm.Next(0, 3);
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 14; j++)
                {
                    if ((j == 0) || (i == 0) || (j == 13))
                    {
                        this.decorsInRoom.Add(new Decor(j, i, 0, 0));
                    }
                    if (((this.specialRoom) || (typeOfRoom != 0)) && (i == 9))
                    {
                        this.decorsInRoom.Add(new Decor(j, i, 0, 0));
                        for (int k = 0; k < NBR_PLATFORM; k++)
                        {
                            platforms[(k % 3 == 0) ? 2 : 1, k] = true;
                            this.decorsInRoom.Add(new Decor(k, (k % 3 == 0) ? 2 : 1, 1, 0));
                        }
                    }
                }
            }
            if ((typeOfRoom == 0) && (!this.specialRoom))
            {
                for (int j = 0; j < NBR_PLATFORM; j++)
                {
                    for (int i = 0; i < NBR_PLATFORM; i++)
                    {
                        if (rdm.Next(0, 2) == 0)
                        {
                            if ((j == 0) || ((j > 0) && (this.Surrounded(0, new Point(i, j)))))
                            {
                                this.platforms[i, j] = true;
                                shortest[j] = i;
                            }
                        }
                        if ((i == NBR_PLATFORM - 1) && (shortest[j] == -10))
                        {
                            this.platforms[NBR_PLATFORM - 2, j] = true;
                            shortest[j] = NBR_PLATFORM - 2;
                            this.platforms[1, j] = true;
                            shortest[j] = 1;
                        }
                    }
                }
                for (int j = 0; j < NBR_PLATFORM; j++)
                {
                    for (int i = 0; i < NBR_PLATFORM; i++)
                    {
                        if ((this.platforms[i, j] == true) && ((j < NBR_PLATFORM - 1) && (this.Surrounded(2, new Point(i, j)))))
                        {
                            this.decorsInRoom.Add(new Decor(j, i, 1, 0));
                            switch (rdm.Next(0, 2))
                            {
                                case 0:
                                    if (i > 0)
                                    {
                                        this.platforms[i - 1, j + 1] = true;
                                    }
                                    else
                                    {
                                        this.platforms[i + 1, j + 1] = true;
                                    }
                                    break;
                                case 1:
                                    if (i > 0)
                                    {
                                        this.platforms[i - 1, j] = true;
                                    }
                                    else
                                    {
                                        this.platforms[i + 1, j] = true;
                                    }
                                    break;
                            }
                        }
                        else if ((this.platforms[i, j] == true) && ((j == NBR_PLATFORM - 1) || ((j < NBR_PLATFORM - 1) && (this.Surrounded(1, new Point(i, j))))))
                        {
                            this.decorsInRoom.Add(new Decor(j, i, 1, 0));
                        }
                    }
                }

            }
            this.AddDoors();
            for (int i = 0; i < NBR_PLATFORM; i++)
            {
                for (int j = 0; j < NBR_PLATFORM; j++)
                {
                    if ((this.platforms[i, j]) && (!this.doorsInTheRoom[i, j]))
                    {
                        this.ennemiesInRoom.Add(new Ennemy("zombie"));
                        this.ennemiesInRoom[ennemiesInRoom.Count - 1].Location = new Point((j * 215) + 67, (i * 100) + 67);
                    }
                }
            }
        }

        private bool Surrounded(int whichSide, Point coord)  //0 = G autour, 1 = D autour, 2 = D cote
        {
            switch (whichSide)
            {
                case 0:
                    if (
                        ((coord.X > 0)
                        && ((this.platforms[coord.X - 1, coord.Y - 1] == true)
                        || (this.platforms[coord.X - 1, coord.Y] == true)))

                        || (this.platforms[coord.X, coord.Y - 1] == true)
                        
                        || ((coord.X < NBR_PLATFORM - 1)
                        && ((this.platforms[coord.X + 1, coord.Y - 1] == true)
                        || (this.platforms[coord.X + 1, coord.Y] == true)))
                        )
                    {
                        return true;
                    }
                    break;
                case 1:
                    if (
                        ((coord.X > 0)
                        && ((this.platforms[coord.X - 1, coord.Y + 1] == true)
                        || (this.platforms[coord.X - 1, coord.Y] == true)))

                        || (this.platforms[coord.X, coord.Y + 1] == true)

                        || ((coord.X < NBR_PLATFORM - 1)
                        && ((this.platforms[coord.X + 1, coord.Y + 1] == true)
                        || (this.platforms[coord.X + 1, coord.Y] == true)))
                        )
                    {
                        return true;
                    }
                    break;
                case 2:
                    if (
                        ((coord.X > 0)
                        && (this.platforms[coord.X - 1, coord.Y + 1] == true))

                        || (this.platforms[coord.X, coord.Y + 1] == true)

                        || ((coord.X < NBR_PLATFORM - 1)
                        && (this.platforms[coord.X + 1, coord.Y + 1] == true))
                        )
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }

        //faire aparaitre aleatoirement des plateformes

        public void RemoveWall(int whichWall)   //0 = up, 1 = right, 2 = down, 3 = left
        {
            this.doors.Add(whichWall);
            this.visited = true;
        }

        public void AddDoors()
        {
            List<int> listOfChoices = new List<int>();
            int whichColumn = -1;
            int whichPlatform;
            Random choice = new Random();
            foreach (int door in this.doors)
            {
                listOfChoices.Clear();
                switch (door)   //0 = up, 1 = right, 2 = down, 3 = left
                {                    
                    case 0:
                        whichColumn = 2;
                        for (int i = 0; i < NBR_PLATFORM; i++)
                        {
                            if (this.platforms[i, whichColumn] == true)
                            {
                                listOfChoices.Add(i);
                            }
                        }
                        break;
                    case 1:
                        whichColumn = 3;
                        for (int i = 0; i < NBR_PLATFORM; i++)
                        {
                            if (this.platforms[i, whichColumn] == true)
                            {
                                listOfChoices.Add(i);
                            }
                        }
                        break;
                    case 2:
                        whichColumn = 1;
                        for (int i = 0; i < NBR_PLATFORM; i++)
                        {
                            if (this.platforms[i, whichColumn] == true)
                            {
                                listOfChoices.Add(i);
                            }
                        }
                        break;
                    case 3:
                        whichColumn = 0;
                        for (int i = 0; i < NBR_PLATFORM; i++)
                        {
                            if (this.platforms[i, whichColumn] == true)
                            {
                                listOfChoices.Add(i);
                            }
                        }
                        break;
                }
                whichPlatform = listOfChoices[choice.Next(0, listOfChoices.Count)];
                doorsInTheRoom[whichPlatform, whichColumn] = true;
                this.decorsInRoom.Add(new Decor(0, whichPlatform, 2, door));
            }
        }
    }
}

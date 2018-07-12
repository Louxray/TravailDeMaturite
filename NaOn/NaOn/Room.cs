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
        public static readonly int NBR_DOOR  = 4;
        public static readonly int NBR_PLATFORM = 4;
        private bool[,] platforms = new bool[NBR_PLATFORM, NBR_PLATFORM];
        public bool visited { get; private set; } = false;
        public List<int> doors = new List<int>(); //0 = up, 1 = right, 2 = down, 3 = left
        public List<Decor> decorsInRoom { get; private set; } = new List<Decor>();

        public Room(int[,] dimension)
        {
            int[] shortest = new int[] { -10, -10, -10, -10 };
            Random rdm = new Random();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 14; j++)
                {
                    if ((j == 0) || (i == 0) || (j == 13))
                    {
                        this.decorsInRoom.Add(new Decor(j, i, 0));
                    }
                }
            }
            for (int j = 0; j < NBR_PLATFORM; j++)
            {
                for (int i = 0; i < NBR_PLATFORM; i++)
                {
                    if (rdm.Next(0, 2) == 0)
                    {
                        if ((j == 0) || ((j > 0) && (Surrounded(0, new Point(i, j)))))
                        {
                            platforms[i, j] = true;
                            shortest[j] = i;
                        }
                    }
                    if ((i == NBR_PLATFORM - 1) && (shortest[j] == -10))
                    {
                        platforms[NBR_PLATFORM - 2, j] = true;
                        shortest[j] = NBR_PLATFORM - 2;
                        platforms[1, j] = true;
                        shortest[j] = 1;
                    }
                }
            }
            for (int j = 0; j < NBR_PLATFORM; j++)
            {
                for (int i = 0; i < NBR_PLATFORM; i++)
                {
                    if ((platforms[i, j] == true) && ((j < NBR_PLATFORM - 1) && (Surrounded(2, new Point(i, j)))))
                    {
                        this.decorsInRoom.Add(new Decor(j + 1, i + 1, 0));
                        switch (rdm.Next(0, 2))
                        {
                            case 0:
                                if (i > 0)
                                {
                                    platforms[i - 1, j + 1] = true;
                                }
                                else
                                {
                                    platforms[i + 1, j + 1] = true;
                                }
                                break;
                            case 1:
                                if (i > 0)
                                {
                                    platforms[i - 1, j] = true;
                                }
                                else
                                {
                                    platforms[i + 1, j] = true;
                                }
                                break;
                        }
                    }
                    else if ((platforms[i, j] == true) && ((j == NBR_PLATFORM - 1) || ((j < NBR_PLATFORM - 1) && (Surrounded(1, new Point(i, j))))))
                    {
                        this.decorsInRoom.Add(new Decor(j + 1, i + 1, 0));
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
                        && ((platforms[coord.X - 1, coord.Y - 1] == true)
                        || (platforms[coord.X - 1, coord.Y] == true)))

                        || (platforms[coord.X, coord.Y - 1] == true)
                        
                        || ((coord.X < NBR_PLATFORM - 1)
                        && ((platforms[coord.X + 1, coord.Y - 1] == true)
                        || (platforms[coord.X + 1, coord.Y] == true)))
                        )
                    {
                        return true;
                    }
                    break;
                case 1:
                    if (
                        ((coord.X > 0)
                        && ((platforms[coord.X - 1, coord.Y + 1] == true)
                        || (platforms[coord.X - 1, coord.Y] == true)))

                        || (platforms[coord.X, coord.Y + 1] == true)

                        || ((coord.X < NBR_PLATFORM - 1)
                        && ((platforms[coord.X + 1, coord.Y + 1] == true)
                        || (platforms[coord.X + 1, coord.Y] == true)))
                        )
                    {
                        return true;
                    }
                    break;
                case 2:
                    if (
                        ((coord.X > 0)
                        && (platforms[coord.X - 1, coord.Y + 1] == true))

                        || (platforms[coord.X, coord.Y + 1] == true)

                        || ((coord.X < NBR_PLATFORM - 1)
                        && (platforms[coord.X + 1, coord.Y + 1] == true))
                        )
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }

        //faire aparaitre aleatoirement des plateformes

        public void AddDoor(int whichDoor)
        {
            this.doors.Add(whichDoor);
        }

        public void RemoveWall(int whichWall)   //0 = up, 1 = right, 2 = down, 3 = left
        {
            this.AddDoor(whichWall);
            this.visited = true;
        }
    }
}

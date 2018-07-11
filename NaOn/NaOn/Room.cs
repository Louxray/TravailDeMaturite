using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaOn
{
    class Room
    {       
        public static readonly int NBR_DOOR  = 4;
        public bool visited { get; private set; } = false;
        public List<int> doors = new List<int>(); //0 = up, 1 = right, 2 = down, 3 = left
        public List<Decor> decorsInRoom { get; private set; } = new List<Decor>();

        public Room(int[,] dimension)
        {            
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 14; j++)
                {
                    if ((j == 0) || (i == 0) || (j == 13) || (i == 9))
                    {
                        this.decorsInRoom.Add(new Decor(j, i, 0));
                    }
                }
            }
        }

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

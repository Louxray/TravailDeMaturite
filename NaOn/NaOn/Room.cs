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
            for (int i = 0; i < 12; i++)
            {
                this.decorsInRoom.Add(new Decor((i - 2), (i < 5) ? (dimension[1, 1]) : (dimension[1, 1] - 100), 0));
            }
            /*
            for (int i = 0; i < Room.NBR_DOOR; i++)
            {
                this.doors.Add(i);
            }
            */
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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DungeonGenerator {

    public class Tile {
        public int up;
        public int down;
        public int left;
        public int right;

        public Tile() {
            up = 0;
            down = 0;
            left = 0;
            right = 0;
        }
    }

    public class Room {
        public int position_x;
        public int position_y;

        public int height;
        public int width;

        public List<Tile> floor = new List<Tile>();

        public Room(int x, int y, int height, int width) {
            position_x = x;
            position_y = y;
            this.height = height;
            this.width = width;
        }

        public void fillRoom() {
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    Tile tile = new Tile();
                    if (x == 0) tile.left = 1;
                    if (x == width - 1) tile.right = 1;
                    if (y == 0) tile.up = 1;
                    if (y == height - 1) tile.down = 1;

                    floor.Add(tile);
                }
            }
        }
    }


    public class CollisionDetection {
        static public bool detectDungeonCollision(Room room, List<Room> room_list) {
            bool exit_value = false;
            int room_count = 0;

            if (room_list.Count != 0) {
                do {
                    exit_value = detect2RoomCollision(room, room_list[room_count]);
                    room_count++;
                } while ((exit_value == false) && (room_count < room_list.Count));
            }
            return exit_value;
        }

        static public bool detect2RoomCollision(Room room_A, Room room_B) {
            if (!(isPointInsideRoom(room_A.position_x, room_A.position_y, room_B) ||
                isPointInsideRoom(room_A.position_x + room_A.width, room_A.position_y, room_B) ||
                isPointInsideRoom(room_A.position_x, room_A.position_y + room_A.height, room_B) ||
                isPointInsideRoom(room_A.position_x + room_A.width, room_A.position_y + room_A.height, room_B))){

                /* Check if they are adjacent by the inside */

                //Vertially adjacent
                if ((room_A.height == room_B.height) && (room_A.position_y == room_B.position_y)) {
                    if (((room_B.position_x <= room_A.position_x) && (room_A.position_x < room_B.position_x))
                        || ((room_B.position_x <= room_A.position_x + room_A.width) && (room_A.position_x + room_A.width < room_B.position_x)))
                        return true;
                }
                //Horizontally adjacent
                else if ((room_A.width == room_B.width) && (room_A.position_x == room_B.position_x)) {
                    if (((room_B.position_y <= room_A.position_y) && (room_A.position_y < room_B.position_y))
                    || ((room_B.position_y <= room_A.position_y + room_A.height) && (room_A.position_y + room_A.height < room_B.position_y)))
                        return true;
                } else return false;
            }
            return true;
        }

        static public bool isPointInsideRoom(int x, int y, Room room) {
            return ((y > room.position_y) &&
                    (y < room.position_y + room.height) &&
                    (x > room.position_x) &&
                    (x < room.position_x + room.width));
        }
    }

    public class RoomGenerator : MonoBehaviour {

        public string seed;
        public int number_of_rooms;

        private List<Room> room_list = new List<Room>();

        // Use this for initialization
        void Start() {
            buildRoom();
        }


        private void buildRoom() {
            Room room;
            System.Random pseudoRandom = new System.Random(seed.GetHashCode());

            int x;
            int y;
            int width;
            int height;

            int tries;

            for (int iteration = 0; iteration < number_of_rooms; iteration++) {
                tries = 0;
                int max_tries = 5;
                bool collision_detected;
                do {
                    width = pseudoRandom.Next(5, 20);
                    height = pseudoRandom.Next(5, 20);
                    x = pseudoRandom.Next(0, 25);
                    y = pseudoRandom.Next(0, 25);

                    room = new Room(x, y, height, width);
                 
                    tries++;
                    collision_detected = CollisionDetection.detectDungeonCollision(room, room_list);
                } while (collision_detected && (tries < max_tries));

                if (!collision_detected) {
                    room.fillRoom();
                    room_list.Add(room);
                }                
            }
        }        

        void OnDrawGizmos() {
            int tile_positon;
            if (room_list != null) {
                foreach (Room room in room_list) {
                    tile_positon = 0;
                    for (int y = room.position_y; y < room.height + room.position_y; y++) {
                        for (int x = room.position_x; x < room.width + room.position_x; x++) {
                            if (room.floor[tile_positon].up == 1 || room.floor[tile_positon].down == 1 ||
                                room.floor[tile_positon].left == 1 || room.floor[tile_positon].right == 1)
                                Gizmos.color = Color.black;
                            else Gizmos.color = Color.white;
                            Vector3 pos = new Vector3(-1 / 2 + x + .5f, 0, -1 / 2 + y + .5f);
                            Gizmos.DrawCube(pos, Vector3.one);
                            tile_positon++;
                        }
                    }
                }
            }
        }
    }
}
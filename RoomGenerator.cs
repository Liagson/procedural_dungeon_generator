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
        public int id;

        public int position_x;
        public int position_y;

        public int height;
        public int width;

        public List<Room> child_rooms = new List<Room>();
        public List<Tile> floor = new List<Tile>();

        public Room(int x, int y, int height, int width) {
            position_x = x;
            position_y = y;
            this.height = height;
            this.width = width;
        }

        public void create_child_room() {
            //int width;
            //int length;

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

    public class Dungeon {
        int max_depth;
        int current_max_depth;
        /* Add a dictionary of a Room List 
            or maybe an int dictionary and then a room list
         */
    }

    public class Navigation {
        static public System.Random pseudoRandom;
        static public int getDirection() {
            /* 0: UP  1: RIGHT 2: DOWN 3: LEFT */
            return pseudoRandom.Next(0, 4);
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
            return ((room_A.position_x + room_A.width >= room_B.position_x) && (room_A.position_x <= room_B.position_x + room_B.width)
                && (room_A.position_y + room_A.height >= room_B.position_y) && (room_A.position_y <= room_B.position_y + room_B.height));
        }

        static public bool isPointInsideRoom(int x, int y, Room room) {
            /*If it touches the wall it's not inside */
            return ((y > room.position_y) &&
                    (y < room.position_y + room.height) &&
                    (x > room.position_x) &&
                    (x < room.position_x + room.width));
        }

        static public void setDoorInRoom(Room room, int direction, int x_position, int y_position) {
            /* x and y positions refer to the adjacent room tile with door */
            int tile_with_door;
                        
            switch (direction) {
                case 0:
                    tile_with_door = (x_position - room.position_x) + ((y_position + 1) - room.position_y) * room.width;
                    room.floor[tile_with_door].up = 2;
                    break;
                case 1:
                    tile_with_door = ((x_position - 1) - room.position_x) + (y_position - room.position_y) * room.width;
                    room.floor[tile_with_door].right = 2;
                    break;
                case 2:
                    tile_with_door = (x_position - room.position_x) + ((y_position - 1) - room.position_y) * room.width;
                    room.floor[tile_with_door].down = 2;
                    break;
                case 3:
                    tile_with_door = ((x_position + 1) - room.position_x) + (y_position - room.position_y) * room.width;
                    room.floor[tile_with_door].left = 2;
                    break;
            }
        }

        static public int getRandomTileInBorderOfRoom(Room room, System.Random pseudoRandom, int direction) {
            /* Returns room's tile with door position
             * Also adds a door to said tile */

            int depth;
            int selected_tile_with_door = -1;
            switch (direction) {
                case 0: //UP
                    depth = pseudoRandom.Next(1, room.width - 1);
                    selected_tile_with_door = depth;
                    room.floor[selected_tile_with_door].up = 2;
                    break;
                case 1: //RIGHT
                    depth = pseudoRandom.Next(1, room.height - 1);
                    selected_tile_with_door = (room.width * (depth + 1)) - 1;
                    room.floor[selected_tile_with_door].right = 2;
                    break;
                case 2: //DOWN
                    depth = pseudoRandom.Next(1, room.width - 1);
                    selected_tile_with_door = (room.width * (room.height - 1)) + depth;
                    room.floor[selected_tile_with_door].down = 2;
                    break;
                case 3: //LEFT
                    depth = pseudoRandom.Next(1, room.height - 1);
                    selected_tile_with_door = room.width * depth;
                    room.floor[selected_tile_with_door].left = 2;
                    break;
            }
            return selected_tile_with_door;
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
            Navigation.pseudoRandom = new System.Random(seed.GetHashCode());

            int x;
            int y;
            int width;
            int height;

            int tries;

            int direction;

            for (int iteration = 0; iteration < number_of_rooms; iteration++) {
                tries = 0;
                int max_tries = 5;
                //bool collision_detected;
                do {
                    width = Navigation.pseudoRandom.Next(5, 20);
                    height = Navigation.pseudoRandom.Next(5, 20);
                    x = Navigation.pseudoRandom.Next(0, 25);
                    y = Navigation.pseudoRandom.Next(0, 25);

                    room = new Room(x, y, height, width);
                    room.id = iteration;

                    tries++;
                    //collision_detected = CollisionDetection.detectDungeonCollision(room, room_list);
                } while (/*collision_detected &&*/ (tries < max_tries));

                if (/*!collision_detected*/true) {
                    room.fillRoom();

                    /* Doors */
                    direction = Navigation.getDirection();
                    room.floor[CollisionDetection.getRandomTileInBorderOfRoom(room, Navigation.pseudoRandom, direction)].up = 2;

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
                            if (room.floor[tile_positon].up == 2 || room.floor[tile_positon].down == 2 ||
                                room.floor[tile_positon].left == 2 || room.floor[tile_positon].right == 2)
                                Gizmos.color = Color.yellow; //Doors
                           else if (room.floor[tile_positon].up == 1 || room.floor[tile_positon].down == 1 ||
                                room.floor[tile_positon].left == 1 || room.floor[tile_positon].right == 1)
                                Gizmos.color = Color.black; //Wall
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
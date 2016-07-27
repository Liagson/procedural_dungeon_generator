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

        public int depth;

        public List<int> child_rooms = new List<int>();
        public List<Tile> floor = new List<Tile>();

        public Room(int x, int y, int height, int width) {
            position_x = x;
            position_y = y;
            this.height = height;
            this.width = width;
        }

        public void reproduct() {
            //Finish this mess
            if (this.depth != 0 && this.depth < 4) {
                create_child_room();
                //create_child_room();
            } else {
                create_child_room();
                //Dungeon.rooms_in_dungeon[(Dungeon.rooms_in_dungeon.Count) - 1].create_child_room();
                //create_child_room();
                //Dungeon.rooms_in_dungeon[(Dungeon.rooms_in_dungeon.Count) - 1].create_child_room();
            }
        }

        public void create_child_room() {
            int direction;
            int child_height;
            int child_length;
            int tile_with_door = 0;

            int pos_x_son = 0;
            int pos_y_son = 0;

            int number_of_tries = 0;
            int max_number_of_tries = 5;

            bool collision = false;

            Room child_room;            

            do {
                direction = Navigation.getDirection();
                child_height = Navigation.pseudoRandom.Next(5, 20);
                child_length = Navigation.pseudoRandom.Next(5, 20);
                switch (direction) {
                    case 0:
                        tile_with_door = Navigation.pseudoRandom.Next(1, this.width - 2);
                        pos_x_son = Navigation.pseudoRandom.Next(this.position_x + tile_with_door - child_length  + 1 , this.position_x + tile_with_door - 1);
                        pos_y_son = this.position_y;
                        break;
                    case 1:
                        tile_with_door = Navigation.pseudoRandom.Next(1, this.height - 2);
                        pos_x_son = this.position_x + this.width;
                        pos_y_son = Navigation.pseudoRandom.Next(this.position_y + tile_with_door - child_height + 1, this.position_y + tile_with_door - 1);
                        break;
                    case 2:
                        tile_with_door = Navigation.pseudoRandom.Next(1, this.width - 2);
                        pos_x_son = Navigation.pseudoRandom.Next(this.position_x + tile_with_door - child_length + 1, this.position_x + tile_with_door - 1);
                        pos_y_son = this.position_y + this.height;
                        break;
                    case 3:
                        tile_with_door = Navigation.pseudoRandom.Next(1, this.height - 2);
                        pos_x_son = this.position_x - this.width;
                        pos_y_son = Navigation.pseudoRandom.Next(this.position_y + tile_with_door - child_height + 1, this.position_y + tile_with_door - 1);
                        break;
                }
                child_room = new Room(pos_x_son, pos_y_son, child_height, child_length);
                child_room.id = Dungeon.rooms_in_dungeon.Count;
                child_room.depth = this.depth + 1;

                number_of_tries++;
                //Ensure there's no collision before adding the room to the dungeon!
                collision = CollisionDetection.detectDungeonCollision(child_room, Dungeon.rooms_in_dungeon);
            } while ((number_of_tries < max_number_of_tries) && collision);

            if (!collision){
                child_room.fillRoom();
                switch (direction) {
                    case 0:
                        CollisionDetection.setDoorInRoom(this, direction, position_x + tile_with_door, position_y);
                        CollisionDetection.setDoorInRoom(child_room, (direction + 2) % 4, position_x + tile_with_door, position_y - 1);
                        break;
                    case 1:
                        CollisionDetection.setDoorInRoom(this, direction, position_x + width - 1, position_y + tile_with_door);
                        CollisionDetection.setDoorInRoom(child_room, (direction + 2) % 4, position_x + width, position_y + tile_with_door);
                        break;
                    case 2:
                        CollisionDetection.setDoorInRoom(this, direction, position_x + tile_with_door, position_y + height - 1);
                        CollisionDetection.setDoorInRoom(child_room, (direction + 2) % 4, position_x + tile_with_door, position_y + 1);
                        break;
                    case 3:
                        CollisionDetection.setDoorInRoom(this, direction, position_x, position_y + tile_with_door);
                        CollisionDetection.setDoorInRoom(child_room, (direction + 2) % 4, position_x, position_y + tile_with_door);
                        break;
                }                
                Dungeon.rooms_in_dungeon.Add(child_room);
                this.child_rooms.Add(child_room.id);
            }
            
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
        /* Add a dictionary of a Room List 
            or maybe an int dictionary and then a room list
         */
        public static List<Room> rooms_in_dungeon = new List<Room>();
        public void initializeDungeon() {
            Room first_room = new Room(0, 0, 5, 10);
            first_room.id = 0;
            first_room.depth = 0;
            first_room.fillRoom();
            rooms_in_dungeon.Add(first_room);
        }
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
            return ((room_A.position_x + room_A.width > room_B.position_x) && (room_A.position_x < room_B.position_x + room_B.width)
                && (room_A.position_y + room_A.height > room_B.position_y) && (room_A.position_y < room_B.position_y + room_B.height));
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
                    tile_with_door = (x_position - room.position_x) + ((y_position) - room.position_y) * room.width;
                    room.floor[tile_with_door].up = 2;
                    break;
                case 1:
                    tile_with_door = ((x_position) - room.position_x) + (y_position - room.position_y) * room.width;
                    room.floor[tile_with_door].right = 2;
                    break;
                case 2:
                    tile_with_door = (x_position - room.position_x) + ((y_position) - room.position_y) * room.width;
                    room.floor[tile_with_door].down = 2;
                    break;
                case 3:
                    tile_with_door = ((x_position) - room.position_x) + (y_position - room.position_y) * room.width;
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
                        
        public Dungeon dungeon = new Dungeon();
        
        // Use this for initialization
        void Start() {
            buildRoom();
        }

        public void buildRoom() {
            Navigation.pseudoRandom = new System.Random(seed.GetHashCode());
            dungeon.initializeDungeon();
            Dungeon.rooms_in_dungeon[0].reproduct();
            Dungeon.rooms_in_dungeon[1].reproduct();
            Dungeon.rooms_in_dungeon[2].reproduct();
            Dungeon.rooms_in_dungeon[3].reproduct();
            Dungeon.rooms_in_dungeon[1].reproduct();
            Dungeon.rooms_in_dungeon[2].reproduct();
            Dungeon.rooms_in_dungeon[2].reproduct();
            Dungeon.rooms_in_dungeon[2].reproduct();
        }

        void OnDrawGizmos() {
            int tile_positon;
            if (Dungeon.rooms_in_dungeon != null) {
                foreach (Room room in Dungeon.rooms_in_dungeon) {
                    tile_positon = 0;
                    for (int y = room.position_y; y < room.height + room.position_y; y++) {
                        for (int x = room.position_x; x < room.width + room.position_x; x++) {
                            if (room.floor[tile_positon].up == 2 || room.floor[tile_positon].down == 2 ||
                                room.floor[tile_positon].left == 2 || room.floor[tile_positon].right == 2)
                                Gizmos.color = Color.yellow; //doors
                            else if (room.floor[tile_positon].up == 1 || room.floor[tile_positon].down == 1 ||
                                 room.floor[tile_positon].left == 1 || room.floor[tile_positon].right == 1)
                                Gizmos.color = Color.black; //wall
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

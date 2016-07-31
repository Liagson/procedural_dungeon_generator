using UnityEngine;
using System.Collections.Generic;
using System;

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
            if (this.depth != 0 && this.depth < 4) {
                create_child_room();
            } else { /*Two paths in the starting room */
                create_child_room();
                create_child_room();
            }
        }

        public void create_child_room() {
            int direction;
            int child_width;
            int child_height;
            int tile_with_door = 0;

            int pos_x_son = 0;
            int pos_y_son = 0;

            int number_of_tries = 0;
            int max_number_of_tries = 5;

            bool collision = false;

            Room child_room;            

            do {
                direction = Navigation.getDirection();
                child_width = Navigation.pseudoRandom.Next(5, 20);
                child_height = Navigation.pseudoRandom.Next(5, 20);
                switch (direction) {
                    case 0:
                        tile_with_door = Navigation.pseudoRandom.Next(1, this.width - 2);
                        pos_x_son = Navigation.pseudoRandom.Next(this.position_x + tile_with_door - child_width + 1 , this.position_x + tile_with_door - 1);
                        pos_y_son = this.position_y - child_height;
                        break;
                    case 1:
                        tile_with_door = Navigation.pseudoRandom.Next(1, this.height - 2);
                        pos_x_son = this.position_x + this.width;
                        pos_y_son = Navigation.pseudoRandom.Next(this.position_y + tile_with_door - child_height + 1, this.position_y + tile_with_door - 1);
                        break;
                    case 2:
                        tile_with_door = Navigation.pseudoRandom.Next(1, this.width - 2);
                        pos_x_son = Navigation.pseudoRandom.Next(this.position_x + tile_with_door - child_width + 1, this.position_x + tile_with_door - 1);
                        pos_y_son = this.position_y + this.height;
                        break;
                    case 3:
                        tile_with_door = Navigation.pseudoRandom.Next(1, this.height - 2);
                        pos_x_son = this.position_x - child_width;
                        pos_y_son = Navigation.pseudoRandom.Next(this.position_y + tile_with_door - child_height + 1, this.position_y + tile_with_door - 1);
                        break;
                }
                child_room = new Room(pos_x_son, pos_y_son, child_height, child_width);
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
                        CollisionDetection.setDoorInRoom(child_room, (direction + 2) % 4, position_x + tile_with_door, position_y + this.height);
                        break;
                    case 3:
                        CollisionDetection.setDoorInRoom(this, direction, position_x, position_y + tile_with_door);
                        CollisionDetection.setDoorInRoom(child_room, (direction + 2) % 4, position_x - 1, position_y + tile_with_door);
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
                
        static public void setDoorInRoom(Room room, int direction, int son_x_position, int son_y_position) {
            int tile_with_door;
            tile_with_door = Math.Abs(room.position_x - son_x_position) + (Math.Abs(room.position_y - son_y_position) * room.width);
            switch (direction) {
                case 0:
                    room.floor[tile_with_door].up = 2;
                    break;
                case 1:
                    room.floor[tile_with_door].right = 2;
                    break;
                case 2:
                    room.floor[tile_with_door].down = 2;
                    break;
                case 3:
                    room.floor[tile_with_door].left = 2;
                    break;
            }
        }

        static public void linkRooms(Room room_A, Room room_B, int direction, System.Random pseudoRandom) {
            /* room_A -> direction -> room_B */

            int tile_x_position = 0;
            int min_x_position = Math.Min(room_A.position_x, room_B.position_x);
            int max_x_position = Math.Max(room_A.position_x + room_A.width, room_B.position_x + room_B.width);

            int tile_y_position = 0;
            int min_y_position = Math.Min(room_A.position_y, room_B.position_y);
            int max_y_position = Math.Max(room_A.position_y + room_A.height, room_B.position_y + room_B.height);
                     
            tile_x_position = pseudoRandom.Next(min_x_position + 1, max_x_position - 1);
            tile_y_position = pseudoRandom.Next(min_y_position + 1, max_y_position - 1);
                        
            switch (direction) {
                case 0:
                    setDoorInRoom(room_A, 0, tile_x_position, room_A.position_y);
                    setDoorInRoom(room_B, 2, tile_x_position, room_A.position_y - 1);
                    break;
                case 1:
                    setDoorInRoom(room_A, 1, room_B.position_x - 1, tile_y_position);
                    setDoorInRoom(room_B, 3, room_B.position_x, tile_y_position);
                    break;
                case 2:
                    setDoorInRoom(room_A, 2, tile_x_position, room_B.position_y - 1);
                    setDoorInRoom(room_B, 0, tile_x_position, room_B.position_y);
                    break;
                case 3:
                    setDoorInRoom(room_A, 3, room_A.position_x, tile_y_position);
                    setDoorInRoom(room_B, 1, room_A.position_x - 1, tile_y_position);
                    break;
            }
        }
    }

    public class RoomGenerator : MonoBehaviour {

        public string seed;                        
        public Dungeon dungeon = new Dungeon();
        
        // Use this for initialization
        void Start() {
            buildRoom();
        }

        public void buildRoom() {
            int position = 0;
            Navigation.pseudoRandom = new System.Random(seed.GetHashCode());
            dungeon.initializeDungeon();

            while (position < Dungeon.rooms_in_dungeon.Count && Dungeon.rooms_in_dungeon[position].depth < 5) {
                Dungeon.rooms_in_dungeon[position].reproduct();
                position++;
            }
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

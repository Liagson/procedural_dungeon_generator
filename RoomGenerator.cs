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

        public bool isInnerRoom = false;

        public List<int> child_rooms = new List<int>();
        public List<Tile> floor = new List<Tile>();

        public Room(int x, int y, int height, int width) {
            position_x = x;
            position_y = y;
            this.height = height;
            this.width = width;
        }

        public void reproduct() {
            if (this.depth != 0 && this.depth < 3) {
                create_child_room();
            } else { /*Three paths in the starting room */
                create_child_room();
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
                child_width = Navigation.getRoomSize();
                child_height = Navigation.getRoomSize();
                switch (direction) {
                    case 0:
                        tile_with_door = Navigation.pseudoRandom.Next(1, this.width - 2);
                        pos_x_son = Navigation.pseudoRandom.Next(this.position_x + tile_with_door - child_width + 2, this.position_x + tile_with_door - 1);
                        pos_y_son = this.position_y - child_height;
                        break;
                    case 1:
                        tile_with_door = Navigation.pseudoRandom.Next(1, this.height - 2);
                        pos_x_son = this.position_x + this.width;
                        pos_y_son = Navigation.pseudoRandom.Next(this.position_y + tile_with_door - child_height + 2, this.position_y + tile_with_door - 1);
                        break;
                    case 2:
                        tile_with_door = Navigation.pseudoRandom.Next(1, this.width - 2);
                        pos_x_son = Navigation.pseudoRandom.Next(this.position_x + tile_with_door - child_width + 2, this.position_x + tile_with_door - 1);
                        pos_y_son = this.position_y + this.height;
                        break;
                    case 3:
                        tile_with_door = Navigation.pseudoRandom.Next(1, this.height - 2);
                        pos_x_son = this.position_x - child_width;
                        pos_y_son = Navigation.pseudoRandom.Next(this.position_y + tile_with_door - child_height + 2, this.position_y + tile_with_door - 1);
                        break;
                }
                child_room = new Room(pos_x_son, pos_y_son, child_height, child_width);
                child_room.id = Dungeon.rooms_in_dungeon.Count;
                child_room.depth = this.depth + 1;

                number_of_tries++;
                //Ensure there's no collision before adding the room to the dungeon!
                collision = DungeonTools.detectDungeonCollision(child_room, Dungeon.rooms_in_dungeon);
            } while ((number_of_tries < max_number_of_tries) && collision);

            if (!collision){
                child_room.fillRoom();
                switch (direction) {
                    case 0:
                        DungeonTools.setDoorInRoom(this, direction, position_x + tile_with_door, position_y);
                        DungeonTools.setDoorInRoom(child_room, (direction + 2) % 4, position_x + tile_with_door, position_y - 1);
                        break;
                    case 1:
                        DungeonTools.setDoorInRoom(this, direction, position_x + width - 1, position_y + tile_with_door);
                        DungeonTools.setDoorInRoom(child_room, (direction + 2) % 4, position_x + width, position_y + tile_with_door);
                        break;
                    case 2:
                        DungeonTools.setDoorInRoom(this, direction, position_x + tile_with_door, position_y + height - 1);
                        DungeonTools.setDoorInRoom(child_room, (direction + 2) % 4, position_x + tile_with_door, position_y + this.height);
                        break;
                    case 3:
                        DungeonTools.setDoorInRoom(this, direction, position_x, position_y + tile_with_door);
                        DungeonTools.setDoorInRoom(child_room, (direction + 2) % 4, position_x - 1, position_y + tile_with_door);
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

    public static class Dungeon {
        public static List<Room> rooms_in_dungeon;
        public static void initializeDungeon() {
            rooms_in_dungeon = new List<Room>();

            Room first_room = new Room(0, 0, 5, 10);
            first_room.id = 0;
            first_room.depth = 0;
            first_room.fillRoom();
            rooms_in_dungeon.Add(first_room);
        }
    }

    public class Navigation {
        static public System.Random pseudoRandom;
        static public int getRoomSize() {
            /* From 5 to 20 */
            int number = getRandomNormalNumber();
            if (number > 20) number = 20;
            if (number < 5) number = 5;
            return number;
        }
        static public int getRandomNormalNumber() {
            /* Used for room sizes*/
            /*Special thanks to Superbest random:
             * https://bitbucket.org/Superbest/superbest-random/src
             *  */
            double u1 = pseudoRandom.NextDouble();
            double u2 = pseudoRandom.NextDouble();
            double mu = 12;
            double sigma = 8;
            double rand_std_normal = Math.Sqrt(-2 * Math.Log(u1)) *
                                Math.Sin(2 * Math.PI * u2);
            double rand_normal = mu + sigma * rand_std_normal;
            return (int)rand_normal;
        }
        static public int getDirection() {
            /* 0: UP  1: RIGHT 2: DOWN 3: LEFT */
            return pseudoRandom.Next(0, 4);
        }
    }

    public class DungeonTools {
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

        static public void connectAdjacentRooms(List<Room> room_list, System.Random pseudoRandom) {
            for (int pivot_room = 0; pivot_room < room_list.Count - 1; pivot_room++) {
                for (int current_position = 0; current_position < room_list.Count; current_position++) {
                    if ((current_position != pivot_room) && (!room_list[pivot_room].isInnerRoom) && (!room_list[current_position].isInnerRoom)){
                        if (room_list[pivot_room].position_y + room_list[pivot_room].height == room_list[current_position].position_y) {
                            //Down
                            if ((room_list[current_position].position_x < room_list[pivot_room].position_x + room_list[pivot_room].width) &&
                                (room_list[current_position].position_x + room_list[current_position].width > room_list[pivot_room].position_x)) {
                                //Don't connect father-child rooms
                                if ((!room_list[pivot_room].child_rooms.Contains(room_list[current_position].id)) && (!room_list[current_position].child_rooms.Contains(room_list[pivot_room].id))) {
                                    link2Rooms(room_list[pivot_room], room_list[current_position], 2, pseudoRandom);
                                }
                            }
                        } else if (room_list[pivot_room].position_x + room_list[pivot_room].width == room_list[current_position].position_x) {
                            //Right
                            if ((room_list[current_position].position_y < room_list[pivot_room].position_y + room_list[pivot_room].height) &&
                                (room_list[current_position].position_y + room_list[current_position].height > room_list[pivot_room].position_y)) {
                                //Don't connect father-child rooms
                                if ((!room_list[pivot_room].child_rooms.Contains(room_list[current_position].id)) && (!room_list[current_position].child_rooms.Contains(room_list[pivot_room].id))) {
                                    link2Rooms(room_list[pivot_room], room_list[current_position], 1, pseudoRandom);
                                }
                            }
                        }
                    }                    
                }
            }
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

        static public void setPillarInRoom(Room room, int room_x_position, int room_y_position) {
            int pillarTile = room_x_position + (room.width * room_y_position);

            //TODO: Needs adjacent tiles with wall too!
            room.floor[pillarTile].up = 1;
            room.floor[pillarTile].left = 1;

            room.floor[pillarTile + 1].up = 1;
            room.floor[pillarTile + 1].right = 1;

            room.floor[pillarTile + room.width].down = 1;
            room.floor[pillarTile + room.width].left = 1;

            room.floor[pillarTile + room.width + 1].down = 1;
            room.floor[pillarTile + room.width + 1].right = 1;
        }

        static public void addPillarsToRoom(Room room) {
            int direction = Navigation.getDirection();
            int starting_x_separation = Navigation.pseudoRandom.Next(1, 3);
            int starting_y_separation = Navigation.pseudoRandom.Next(1, 3);

            int starting_x_position;
            int starting_y_position;
            int current_x_position;
            int current_y_position;

            
            switch (direction) {
                case 0: //Down -> Up
                    starting_x_position = room.width - starting_x_separation - 2;
                    starting_y_position = room.height - starting_y_separation - 2;
                    current_y_position = starting_y_position;

                    setPillarInRoom(room, starting_x_position, starting_y_position);
                    if (starting_x_separation < starting_x_position - 4) {
                        setPillarInRoom(room, starting_x_separation, starting_y_position);
                    }
                    while (current_y_position - 4 > 0) {
                        current_y_position -= 4;
                        setPillarInRoom(room, starting_x_position, current_y_position);
                        if (starting_x_separation < starting_x_position - 4) {
                            setPillarInRoom(room, starting_x_separation, current_y_position);
                        }
                    }
                    break;
                case 1: //Left -> Right
                    starting_x_position = starting_x_separation;
                    starting_y_position = starting_y_separation;
                    current_x_position = starting_x_position;

                    setPillarInRoom(room, starting_x_position, starting_y_position);
                    if (room.height > starting_y_position + 6) {
                        setPillarInRoom(room, starting_x_position, room.height - starting_y_position - 2);
                    }
                    while (current_x_position + 6 < room.width) {
                        current_x_position += 4;
                        setPillarInRoom(room, current_x_position, starting_y_position);
                        if (room.height > starting_y_position + 6) {
                            setPillarInRoom(room, current_x_position, room.height - starting_y_position - 2);
                        }
                    }
                    break;
                case 2: //Up -> Down
                    starting_x_position = starting_x_separation;
                    starting_y_position = starting_y_separation;
                    current_y_position = starting_y_position;

                    setPillarInRoom(room, starting_x_position, starting_y_position);
                    if (room.width > starting_x_position + 6) {
                        setPillarInRoom(room, room.width - starting_x_position - 2, starting_y_position);
                    }
                    while (current_y_position + 6 < room.height) {
                        current_y_position += 4;
                        setPillarInRoom(room, starting_x_position, current_y_position);
                        if (room.width > starting_x_position + 6) {
                            setPillarInRoom(room, room.width - starting_x_position - 2, current_y_position);
                        }
                    }
                    break;
                case 3: // Right -> Left
                    starting_x_position = room.width - starting_x_separation - 2;
                    starting_y_position = room.height - starting_y_separation - 2;
                    current_x_position = starting_x_position;

                    setPillarInRoom(room, starting_x_position, starting_y_position);
                    if (starting_y_separation < starting_y_position - 4) {
                        setPillarInRoom(room, starting_x_position, starting_y_separation);
                    }
                    while (current_x_position - 4 > 0) {
                        current_x_position -= 4;
                        setPillarInRoom(room, current_x_position, starting_y_position);
                        if (starting_y_separation < starting_y_position - 4) {
                            setPillarInRoom(room, current_x_position, starting_y_separation);
                        }
                    }
                    break;
            }
            
            
        }

        static public void link2Rooms(Room room_A, Room room_B, int direction, System.Random pseudoRandom) {
            /* room_A must be adjacent to room_B */
            /* room_A -> direction -> room_B */

            int tile_x_position;
            int min_x_position = Math.Min(room_A.position_x + room_A.width, room_B.position_x + room_B.width);
            int max_x_position = Math.Max(room_A.position_x, room_B.position_x);

            int tile_y_position;
            int min_y_position = Math.Min(room_A.position_y + room_A.height, room_B.position_y + room_B.height);
            int max_y_position = Math.Max(room_A.position_y, room_B.position_y);
            
            switch (direction) {
                case 0:
                    if (max_x_position + 2 < min_x_position) { //There must be more than 2 tiles in common
                        tile_x_position = pseudoRandom.Next(max_x_position + 1, min_x_position - 1);
                        setDoorInRoom(room_A, 0, tile_x_position, room_A.position_y);
                        setDoorInRoom(room_B, 2, tile_x_position, room_A.position_y - 1);
                    }                    
                    break;
                case 1:
                    if (max_y_position + 2 < min_y_position) { //There must be more than 2 tiles in common
                        tile_y_position = pseudoRandom.Next(max_y_position + 1, min_y_position - 1);
                        setDoorInRoom(room_A, 1, room_B.position_x - 1, tile_y_position);
                        setDoorInRoom(room_B, 3, room_B.position_x, tile_y_position);
                    }                    
                    break;
                case 2:
                    if (max_x_position + 2 < min_x_position) { //There must be more than 2 tiles in common
                        tile_x_position = pseudoRandom.Next(max_x_position + 1, min_x_position - 1);
                        setDoorInRoom(room_A, 2, tile_x_position, room_B.position_y - 1);
                        setDoorInRoom(room_B, 0, tile_x_position, room_B.position_y);
                    }
                    break;
                case 3:
                    if (max_y_position + 2 < min_y_position) { //There must be more than 2 tiles in common
                        tile_y_position = pseudoRandom.Next(max_y_position + 1, min_y_position - 1);
                        setDoorInRoom(room_A, 3, room_A.position_x, tile_y_position);
                        setDoorInRoom(room_B, 1, room_A.position_x - 1, tile_y_position);
                    }
                    break;
            }
        }

        static public void insertRoomInsideRoom(Room room) {
            int innerRoom_X_Position;
            int innerRoom_Y_Position;
            int innerRoomWidth = Navigation.getRoomSize();
            int innerRoomHeight = Navigation.getRoomSize();
            int tries = 0;
            bool collision = true; // innerRoom must be fully inside room
            if (room.width > 7 && room.height > 7) {
                do {
                    innerRoom_X_Position = Navigation.pseudoRandom.Next(1, room.width - 6);
                    innerRoom_Y_Position = Navigation.pseudoRandom.Next(1, room.height - 6);
                    if (innerRoom_X_Position + innerRoomWidth < room.width &&
                        innerRoom_Y_Position + innerRoomHeight < room.height) {
                        collision = false;
                    }
                    tries++;
                } while (tries < 5 && collision);
                if (!collision) {
                    //TODO: Needs adjacent tiles with wall too!
                    Room innerRoom = new Room(innerRoom_X_Position + room.position_x, innerRoom_Y_Position + room.position_y, innerRoomHeight, innerRoomWidth);
                    innerRoom.isInnerRoom = true;
                    innerRoom.fillRoom();
                    Dungeon.rooms_in_dungeon.Add(innerRoom);
                }
            }
        }
    }

    public class RoomGenerator : MonoBehaviour {

        public string seed;                       
        
        // Use this for initialization
        void Start() {
            buildRoom();
        }

        public void buildRoom() {
            int position = 0;
            Navigation.pseudoRandom = new System.Random(seed.GetHashCode());
            Dungeon.initializeDungeon();

            while (position < Dungeon.rooms_in_dungeon.Count && Dungeon.rooms_in_dungeon[position].depth < 10) {
                Dungeon.rooms_in_dungeon[position].reproduct();
                position++;
            }
            for (int x = 0; x < Dungeon.rooms_in_dungeon.Count; x++) {
                if (!Dungeon.rooms_in_dungeon[x].isInnerRoom) DungeonTools.addPillarsToRoom(Dungeon.rooms_in_dungeon[x]);
                DungeonTools.insertRoomInsideRoom(Dungeon.rooms_in_dungeon[x]);                
            }
            DungeonTools.connectAdjacentRooms(Dungeon.rooms_in_dungeon, Navigation.pseudoRandom);
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

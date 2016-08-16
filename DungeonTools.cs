using System;
using System.Collections.Generic;
namespace DungeonGenerator {
    public class DungeonTools {
        static public int getRoomSize() {
            /* From 5 to 20 */
            int number = Navigation.getRandomNormalNumber();
            if (number > 20) number = 20;
            if (number < 5) number = 5;
            return number;
        }

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
                    if ((current_position != pivot_room) && (!room_list[pivot_room].isInnerRoom) && (!room_list[current_position].isInnerRoom)) {
                        if (room_list[pivot_room].position_y + room_list[pivot_room].height == room_list[current_position].position_y) {
                            //Down
                            if ((room_list[current_position].position_x < room_list[pivot_room].position_x + room_list[pivot_room].width) &&
                                (room_list[current_position].position_x + room_list[current_position].width > room_list[pivot_room].position_x)) {
                                //Don't connect father-child rooms
                                if ((!room_list[pivot_room].child_rooms.Contains(room_list[current_position].id)) && (!room_list[current_position].child_rooms.Contains(room_list[pivot_room].id))) {
                                    //Bump rooms don't connect to other rooms. For now.
                                    if (!room_list[pivot_room].isBumpRoom && !room_list[current_position].isBumpRoom) {
                                        link2Rooms(room_list[pivot_room], room_list[current_position], 2, pseudoRandom);
                                    }
                                }
                            }
                        } else if (room_list[pivot_room].position_x + room_list[pivot_room].width == room_list[current_position].position_x) {
                            //Right
                            if ((room_list[current_position].position_y < room_list[pivot_room].position_y + room_list[pivot_room].height) &&
                                (room_list[current_position].position_y + room_list[current_position].height > room_list[pivot_room].position_y)) {
                                //Don't connect father-child rooms
                                if ((!room_list[pivot_room].child_rooms.Contains(room_list[current_position].id)) && (!room_list[current_position].child_rooms.Contains(room_list[pivot_room].id))) {
                                    //Bump rooms don't connect to other rooms. For now.
                                    if (!room_list[pivot_room].isBumpRoom && !room_list[current_position].isBumpRoom) {
                                        link2Rooms(room_list[pivot_room], room_list[current_position], 1, pseudoRandom);
                                    }
                                }
                            }
                        }
                    }
                }
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

        static public void setDoorInRoom(Room room, int direction, int door_x_position, int door_y_position) {
            int tile_with_door;
            tile_with_door = Math.Abs(room.position_x - door_x_position) + (Math.Abs(room.position_y - door_y_position) * room.width);
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

            //TODO: Use setWallAroundObject() instead
            room.floor[pillarTile].up = 1;
            room.floor[pillarTile].left = 1;
            room.floor[pillarTile].down = 1;
            room.floor[pillarTile].right = 1;

            room.floor[pillarTile + 1].up = 1;
            room.floor[pillarTile + 1].right = 1;
            room.floor[pillarTile + 1].down = 1;
            room.floor[pillarTile + 1].left = 1;

            room.floor[pillarTile + room.width].down = 1;
            room.floor[pillarTile + room.width].left = 1;
            room.floor[pillarTile + room.width].up = 1;
            room.floor[pillarTile + room.width].right = 1;

            room.floor[pillarTile + room.width + 1].down = 1;
            room.floor[pillarTile + room.width + 1].right = 1;
            room.floor[pillarTile + room.width + 1].up = 1;
            room.floor[pillarTile + room.width + 1].left = 1;

            //setWallAroundObject(room, room.position_x + room_x_position, room.position_y + room_y_position, 2, 2);
        }

        static public void addPillarsToRoom(Room room, System.Random pseudorandom) {
            int direction = pseudorandom.Next(0, 5);
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

        static public void setWallAroundObject(Room room, int pos_x_obj, int pos_y_obj, int obj_width, int obj_height) {
            int tile_with_wall;
            tile_with_wall = (pos_x_obj - room.position_x) + ((pos_y_obj - room.position_y - 1) * room.width);
            for (int x = 0; x < obj_width; x++) {
                if (room.floor[tile_with_wall + x].down != 2) //Don't overwrite doors
                    room.floor[tile_with_wall + x].down = 1;
            }

            tile_with_wall += (room.width - 1);
            for (int y = 0; y < obj_height; y++) {
                if (room.floor[tile_with_wall + (room.width * y)].right != 2)
                    room.floor[tile_with_wall + (room.width * y)].right = 1;
            }

            tile_with_wall += (obj_width + 1);
            for (int y = 0; y < obj_height; y++) {
                if (room.floor[tile_with_wall + (room.width * y)].left != 2)
                    room.floor[tile_with_wall + (room.width * y)].left = 1;
            }

            tile_with_wall += ((obj_height * room.width) - obj_width);
            for (int x = 0; x < obj_width; x++) {
                if (room.floor[tile_with_wall + x].up != 2)
                    room.floor[tile_with_wall + x].up = 1;
            }
        }

        static public void insertRoomInsideRoom(Room room) {
            int innerRoom_X_Position;
            int innerRoom_Y_Position;
            int innerRoomWidth = getRoomSize();
            int innerRoomHeight = getRoomSize();
            int tries = 0;
            bool collision = true; // innerRoom must be fully inside room
            bool door_set = false; // it could occur there is no space left for a door
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
                    Room innerRoom = new Room(innerRoom_X_Position + room.position_x, innerRoom_Y_Position + room.position_y, innerRoomHeight, innerRoomWidth);
                    innerRoom.id = Dungeon.rooms_in_dungeon.Count;
                    innerRoom.isInnerRoom = true;
                    innerRoom.fillRoom();

                    door_set = setDoorInInnerRoom(room, innerRoom, Navigation.pseudoRandom);
                    if (door_set) {
                        room.inner_rooms.Add(Dungeon.rooms_in_dungeon.Count);
                        //setWallAroundObject(room, innerRoom.position_x, innerRoom.position_y, innerRoom.width, innerRoom.height);
                        Dungeon.rooms_in_dungeon.Add(innerRoom);
                    }
                }
            }
        }

        static public bool setDoorInInnerRoom(Room room, Room innerRoom, System.Random pseudoRandom) {
            /* Returns false if there is no space for a door to join the room with its inner room */
            /* It should hardly (if not at all) ever happen */

            int depth;
            int direction;
            bool door_set = false;
            int tries = 0;

            do {
                direction = pseudoRandom.Next(0, 5);
                switch (direction) {
                    case 0: //UP
                        depth = pseudoRandom.Next(1, innerRoom.width - 1);
                        if (room.floor[(innerRoom.position_x - room.position_x + depth) + ((innerRoom.position_y - room.position_y - 1) * room.width)].down != 1) {
                            setDoorInRoom(innerRoom, 0, innerRoom.position_x + depth, innerRoom.position_y);
                            setDoorInRoom(room, 2, innerRoom.position_x + depth, innerRoom.position_y - 1);
                            door_set = true;
                        }
                        break;
                    case 1: //RIGHT
                        depth = pseudoRandom.Next(1, innerRoom.height - 1);
                        if (room.floor[(innerRoom.position_x + innerRoom.width - room.position_x) + ((innerRoom.position_y - room.position_y + depth) * room.width)].left != 1) {
                            setDoorInRoom(innerRoom, 1, innerRoom.position_x + innerRoom.width - 1, innerRoom.position_y + depth);
                            setDoorInRoom(room, 3, innerRoom.position_x + innerRoom.width, innerRoom.position_y + depth);
                            door_set = true;
                        }
                        break;
                    case 2: //DOWN
                        depth = pseudoRandom.Next(1, innerRoom.width - 1);
                        if (room.floor[(innerRoom.position_x - room.position_x + depth) + ((innerRoom.position_y + innerRoom.height - room.position_y) * room.width)].up != 1) {
                            setDoorInRoom(innerRoom, 2, innerRoom.position_x + depth, innerRoom.position_y + innerRoom.height - 1);
                            setDoorInRoom(room, 0, innerRoom.position_x + depth, innerRoom.position_y + innerRoom.height);
                            door_set = true;
                        }
                        break;
                    case 3: //LEFT
                        depth = pseudoRandom.Next(1, innerRoom.height - 1);
                        if (room.floor[(innerRoom.position_x - room.position_x - 1) + ((innerRoom.position_y - room.position_y + depth) * room.width)].right != 1) {
                            setDoorInRoom(innerRoom, 3, innerRoom.position_x, innerRoom.position_y + depth);
                            setDoorInRoom(room, 1, innerRoom.position_x - 1, innerRoom.position_y + depth);
                            door_set = true;
                        }
                        break;
                }
                tries++;
            } while (tries < 400 && !door_set);

            return door_set;
        }

        static public void mergeBumpRoom(Room room, Room bump_room, int direction) {
            /* merge 2 rooms without using doors (TEAR DOWN THE WALL)*/
            /* room -> direction -> bump_room */
            int max_pos_x = 0;
            int max_pos_y = 0;
            int min_pos_x_width = 0;
            int min_pos_y_height = 0;

            switch (direction) {
                case 1:
                case 3:
                    if (room.position_y > bump_room.position_y) max_pos_y = room.position_y;
                    else max_pos_y = bump_room.position_y;
                    if (room.position_y + room.height < bump_room.position_y + bump_room.height)
                        min_pos_y_height = room.position_y + room.height;
                    else min_pos_y_height = bump_room.position_y + bump_room.height;
                    break;
                case 0:
                case 2:
                    if (room.position_x > bump_room.position_x) max_pos_x = room.position_x;
                    else max_pos_x = bump_room.position_x;
                    if (room.position_x + room.width < bump_room.position_x + bump_room.width)
                        min_pos_x_width = room.position_x + room.width;
                    else min_pos_x_width = bump_room.position_x + bump_room.width;
                    break;
            }

            switch (direction) {
                case 0:
                    for (int x = 0; x < (min_pos_x_width - max_pos_x); x++) {
                        room.floor[max_pos_x - room.position_x + x].up = 0;
                        bump_room.floor[bump_room.width * (bump_room.height - 1) + (max_pos_x - bump_room.position_x + x)].down = 0;
                    }
                    break;
                case 1:
                    for (int y = 0; y < (min_pos_y_height - max_pos_y); y++) {
                        room.floor[(room.width - 1) + room.width * ((max_pos_y + y) - room.position_y)].right = 0;
                        bump_room.floor[bump_room.width * ((max_pos_y + y) - bump_room.position_y)].left = 0;
                    }
                    break;
                case 2:
                    for (int x = 0; x < (min_pos_x_width - max_pos_x); x++) {
                        room.floor[room.width * (room.height - 1) + (max_pos_x - room.position_x + x)].down = 0;
                        bump_room.floor[max_pos_x - bump_room.position_x + x].up = 0;
                    }
                    break;
                case 3:
                    for (int y = 0; y < (min_pos_y_height - max_pos_y); y++) {
                        room.floor[room.width * ((max_pos_y + y) - room.position_y)].left = 0;
                        bump_room.floor[(bump_room.width - 1) + bump_room.width * ((max_pos_y + y) - bump_room.position_y)].right = 0;
                    }
                    break;
            }
        }
    }
}
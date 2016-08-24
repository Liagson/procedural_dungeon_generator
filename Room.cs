using System.Collections.Generic;
namespace DungeonGenerator {
    public class Room {
        public int id;

        public int position_x;
        public int position_y;

        public int height;
        public int width;

        public int depth;

        public List<int> child_rooms = new List<int>();
        public List<Tile> floor = new List<Tile>();

        public bool isInnerRoom = false;
        public List<int> inner_rooms = new List<int>();

        public bool isBumpRoom = false;
        public List<int> bump_rooms = new List<int>();

        public Room(int x, int y, int height, int width) {
            position_x = x;
            position_y = y;
            this.height = height;
            this.width = width;
        }

        public void reproduct() {
            int chance;
            if (!((!Dungeon.bumpsCanReproduce) && isBumpRoom)){
                if (this.depth != 0 && this.depth < Dungeon.room_depth_before_multibranching) {
                    create_child_room(false);
                } else {
                    create_child_room(false);
                    create_child_room(false);
                    create_child_room(false);
                }

                if (Dungeon.bumpsInDungeon) {
                    for (int x = 0; x < Dungeon.max_number_of_bumps_per_room; x++) {
                        chance = Navigation.pseudoRandom.Next(0, 100);
                        if (chance < Dungeon.chance_of_bump) {
                            create_child_room(true);
                        }
                    }
                }
            }
        }

        public bool create_child_room(bool isBump) {
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
                if (isBump) {
                    //Bump rooms use uniform distribution sizes
                    child_width = Navigation.pseudoRandom.Next(3, 6);
                    child_height = Navigation.pseudoRandom.Next(3, 6);
                }else {
                    //Rest of the rooms use normal distribution sizes
                    child_width = DungeonTools.getRoomSize();
                    child_height = DungeonTools.getRoomSize();
                }
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
                child_room.isBumpRoom = isBump;

                number_of_tries++;
                //Ensure there's no collision before adding the room to the dungeon!
                collision = DungeonTools.detectDungeonCollision(child_room, Dungeon.rooms_in_dungeon);
            } while ((number_of_tries < max_number_of_tries) && collision);

            //Corridors may not be suited for the deepest graph nodes
            if (DungeonTools.isCorridor(child_room) && child_room.depth > Dungeon.max_depth_for_corridors) {
                collision = true;
            } 

            if (!collision) {
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
                if (child_room.position_x < Dungeon.min_x_position) Dungeon.min_x_position = child_room.position_x;
                if (child_room.position_y < Dungeon.min_y_position) Dungeon.min_y_position = child_room.position_y;
                if (child_room.position_x + child_room.width > Dungeon.max_x_position) Dungeon.max_x_position = child_room.position_x + child_room.width;
                if (child_room.position_y + child_room.height > Dungeon.max_y_position) Dungeon.max_y_position = child_room.position_y + child_room.height;
                if (isBump) {
                    this.bump_rooms.Add(child_room.id);
                    DungeonTools.mergeBumpRoom(this, child_room, direction);
                }

                Dungeon.rooms_in_dungeon.Add(child_room);
                this.child_rooms.Add(child_room.id);
            }
            return !collision;
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
}
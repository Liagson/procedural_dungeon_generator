using System.Collections.Generic;
using System.IO;
namespace DungeonGenerator {

    public static class Dungeon {
        /* <Dungeon settings> */

        //General behavior
        public const int dungeon_max_depth = 4;
        public const int room_depth_before_multibranching = 3; // First rooms have only one child room

        //Room atrezzo
        public const int chance_of_pillar = 75; // (%)
        public const int chance_of_room_door_merging = 80; // (%)
        public const int chance_of_inner_room = 60; // (%) There has to be space for the room

        //Corridors
        public const float corridor_ratio = 2.5f;
        public const int max_depth_for_corridors = dungeon_max_depth - 3;

        //Bumps
        public const bool bumpsInDungeon = true;
        public const bool bumpsCanReproduce = true; //true makes it more organic/cavern-like
        public const int chance_of_bump = 60; // (%)
        public const int max_number_of_bumps_per_room = 3;
        public const int bump_min_side = 3;
        public const int bump_max_side = 6;

        /* </Dungeon settings> */

        public static List<Room> rooms_in_dungeon;
        public static int min_x_position = 0;
        public static int min_y_position = 0;
        public static int max_x_position = 0;
        public static int max_y_position = 0;
        public static int start_x_position = 0;
        public static int start_y_position = 0;
        public static Tile[,] dungeon_matrix;

        public static void initializeDungeon() {
            rooms_in_dungeon = new List<Room>();

            Room first_room = new Room(0, 0, 5, 10);
            first_room.id = 0;
            first_room.depth = 0;
            first_room.fillRoom();
            rooms_in_dungeon.Add(first_room);

            Dungeon.max_x_position = 10;
            Dungeon.max_y_position = 5;
            Dungeon.min_x_position = 0;
            Dungeon.min_y_position = 0;
        }

        public static void saveDungeonFile() {
            string path = @"saveFile.bin";
            int dungeon_width = max_x_position - min_x_position;
            int dungeon_height = max_y_position - min_y_position;
            setStartingPosition();
            Tile blank_tile = new Tile();

            setDungeonMatrix();
            using (BinaryWriter bw = new BinaryWriter(File.Open(path, FileMode.Create))) {
                bw.Write(start_x_position);
                bw.Write(start_y_position);
                bw.Write(dungeon_width);
                bw.Write(dungeon_height);
                for (int y = 0; y < dungeon_height; y++) {
                    for (int x = 0; x < dungeon_width; x++) {
                        if (dungeon_matrix[x, y] != null) {
                            bw.Write(dungeon_matrix[x, y].up);
                            bw.Write(dungeon_matrix[x, y].right);
                            bw.Write(dungeon_matrix[x, y].down);
                            bw.Write(dungeon_matrix[x, y].left);
                        } else {
                            bw.Write(blank_tile.up);
                            bw.Write(blank_tile.right);
                            bw.Write(blank_tile.down);
                            bw.Write(blank_tile.left);
                        }
                    }
                }
                bw.Close();
            }
        }

        static public Tile[,] setDungeonMatrix() {
            dungeon_matrix = new Tile[(max_x_position - min_x_position), (max_y_position - min_y_position)];
            int tile_x_rel_position;
            int tile_y_rel_position;
            int tile_position;
            foreach (Room room in rooms_in_dungeon) {
                tile_x_rel_position = room.position_x - min_x_position;
                tile_y_rel_position = room.position_y - min_y_position;
                tile_position = 0;
                for (int y = 0; y < room.height; y++) {
                    for (int x = 0; x < room.width; x++) {
                        dungeon_matrix[tile_x_rel_position + x, tile_y_rel_position + y] = room.floor[tile_position];
                        tile_position++;
                    }
                }
            }
            return dungeon_matrix;
        }

        static public void setStartingPosition() {
            int room = rooms_in_dungeon.Count - 1;
            while (rooms_in_dungeon[room].isBumpRoom) {
                room -= 1;
            }
            start_x_position = rooms_in_dungeon[room].position_x;
            start_y_position = rooms_in_dungeon[room].position_y;
        }
    }
}
using UnityEngine;

namespace DungeonGenerator {

    public class RoomGenerator : MonoBehaviour {

        public string seed;                       
        
        // Use this for initialization
        void Start() {
            buildRoom();
        }

        public void buildRoom() {
            int position = 0;
            int chance = 0;
            Navigation.pseudoRandom = new System.Random(seed.GetHashCode());
            Dungeon.initializeDungeon();

            while (position < Dungeon.rooms_in_dungeon.Count && Dungeon.rooms_in_dungeon[position].depth < 3) {
                Dungeon.rooms_in_dungeon[position].reproduct();
                position++;
            }
            for (int x = 0; x < Dungeon.rooms_in_dungeon.Count; x++) {
                if (!Dungeon.rooms_in_dungeon[x].isInnerRoom) {
                    chance = Navigation.pseudoRandom.Next(0, 10);
                    if (chance > 5) DungeonTools.addPillarsToRoom(Dungeon.rooms_in_dungeon[x], Navigation.pseudoRandom);
                    DungeonTools.insertRoomInsideRoom(Dungeon.rooms_in_dungeon[x]);
                }
            }
            DungeonTools.connectAdjacentRooms(Dungeon.rooms_in_dungeon, Navigation.pseudoRandom);
            Dungeon.saveDungeonFile();
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

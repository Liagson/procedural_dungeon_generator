namespace DungeonGenerator {

    public class Tile {
        public int up;
        public int down;
        public int left;
        public int right;
        public int inside;

        public Tile() {
            up = 0;
            down = 0;
            left = 0;
            right = 0;
            inside = 1;
        }
    }
}
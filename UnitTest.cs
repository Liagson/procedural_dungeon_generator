using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DungeonGenerator;

namespace DungeonTesting {
    [TestClass]
    public class UnitTest1 {

        /*DungeonTools*/

        Room room;
        Room room_A;
        Room room_B;

        List<Room> room_list;

        static string seed = "test";
        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        /* A. detect2RoomCollision() */
        [TestMethod]
        public void roomIsInsideDetection() {
            room_A = new Room(5, 5, 2, 2);
            room_B = new Room(0, 0, 10, 10);
            Assert.AreEqual(true, DungeonTools.detect2RoomCollision(room_A, room_B), "Room A is inside B");
        }
        [TestMethod]
        public void roomIsNotInsideDetection() {
            room_A = new Room(0, 0, 2, 2);
            room_B = new Room(5, 5, 10, 10);
            Assert.AreEqual(false, DungeonTools.detect2RoomCollision(room_A, room_B), "Room A is not inside B");

            room_A = new Room(15, 15, 2, 2);
            room_B = new Room(5, 5, 5, 5);
            Assert.AreEqual(false, DungeonTools.detect2RoomCollision(room_A, room_B), "Room A is not inside B");
        }
        [TestMethod]
        public void roomIsAdjacentByTheOutside() {
            room_A = new Room(0, 0, 5, 5);
            room_B = new Room(6, 0, 10, 10);
            Assert.AreEqual(false, DungeonTools.detect2RoomCollision(room_A, room_B), "Room A is not inside B (A to the left)");

            room_A = new Room(0, 0, 5, 5);
            room_B = new Room(0, 6, 5, 5);
            Assert.AreEqual(false, DungeonTools.detect2RoomCollision(room_A, room_B), "Room A is not inside B (A is under)");

            room_A = new Room(6, 0, 10, 10);
            room_B = new Room(0, 0, 5, 5);
            Assert.AreEqual(false, DungeonTools.detect2RoomCollision(room_A, room_B), "Room A is not inside B (A to the right)");
        }
        [TestMethod]
        public void roomIsAdjacentByTheInside() {
            room_A = new Room(0, 5, 2, 10);
            room_B = new Room(0, 0, 10, 10);
            Assert.AreEqual(true, DungeonTools.detect2RoomCollision(room_A, room_B), "Room A is inside B (horizontal)");

            room_A = new Room(5, 0, 10, 2);
            room_B = new Room(0, 0, 10, 10);
            Assert.AreEqual(true, DungeonTools.detect2RoomCollision(room_A, room_B), "Room A is inside B (vertical)");

            room_A = new Room(0, 5, 2, 10);
            room_B = new Room(5, 0, 10, 2);
            Assert.AreEqual(true, DungeonTools.detect2RoomCollision(room_A, room_B), "Room A crosses B");
        }

        /* B. detectDungeonCollision() */
        [TestMethod]
        public void detectCollisionsInDungeon() {
            room_list = new List<Room>();
            room_A = new Room(4, 0, 4, 4);
            room_B = new Room(0, 4, 4, 4);
            room_list.Add(room_A);
            room_list.Add(room_B);

            room = new Room(0, 0, 4, 4);
            Assert.AreEqual(false, DungeonTools.detectDungeonCollision(room, room_list), "No collision");
            room = new Room(0, 0, 5, 4);
            Assert.AreEqual(true, DungeonTools.detectDungeonCollision(room, room_list), "Collision to the right");
            room = new Room(0, 0, 4, 5);
            Assert.AreEqual(true, DungeonTools.detectDungeonCollision(room, room_list), "Collision to the bottom");
            room = new Room(0, 0, 5, 5);
            Assert.AreEqual(true, DungeonTools.detectDungeonCollision(room, room_list), "Double collision");
        }
        
        /* C. setDoorInRoom() */
        [TestMethod]
        public void functionSetsDoorCorrectly() {
            room = new Room(0, 0, 10, 10);
            room.fillRoom();
            DungeonTools.setDoorInRoom(room, 0, 5, 0);
            DungeonTools.setDoorInRoom(room, 1, room.width - 1, 2);
            DungeonTools.setDoorInRoom(room, 2, 5, room.height - 1);
            DungeonTools.setDoorInRoom(room, 3, 0, 2);

            Assert.AreEqual(2, room.floor[5].up, "Tile with door (UP)");
            Assert.AreEqual(2, room.floor[29].right, "Tile with door (RIGHT)");
            Assert.AreEqual(2, room.floor[95].down, "Tile with door (DOWN)");
            Assert.AreEqual(2, room.floor[20].left, "Tile with door (LEFT)");
        }

        /* D. linkRooms() */
        [TestMethod]
        public void uniteAdjacentRoomsWithDoor() {
            room_A = new Room(0, 0, 3, 3);
            room_B = new Room(0, -3, 3, 3);
            room_A.fillRoom();
            room_B.fillRoom();

            DungeonTools.link2Rooms(room_A, room_B, 0, pseudoRandom);
            Assert.AreEqual(2, room_A.floor[1].up, "First room (UP)");
            Assert.AreEqual(2, room_B.floor[7].down, "Second room (DOWN)");

            room_A = new Room(0, 0, 3, 3);
            room_B = new Room(3, 0, 3, 3);
            room_A.fillRoom();
            room_B.fillRoom();

            DungeonTools.link2Rooms(room_A, room_B, 1, pseudoRandom);
            Assert.AreEqual(2, room_A.floor[5].right, "First room (RIGHT)");
            Assert.AreEqual(2, room_B.floor[3].left, "Second room (LEFT)");

            room_A = new Room(0, 0, 3, 3);
            room_B = new Room(0, 3, 3, 3);
            room_A.fillRoom();
            room_B.fillRoom();

            DungeonTools.link2Rooms(room_A, room_B, 2, pseudoRandom);
            Assert.AreEqual(2, room_A.floor[7].down, "First room (DOWN)");
            Assert.AreEqual(2, room_B.floor[1].up, "Second room (UP)");

            room_A = new Room(0, 0, 3, 3);
            room_B = new Room(-3, 0, 3, 3);
            room_A.fillRoom();
            room_B.fillRoom();

            DungeonTools.link2Rooms(room_A, room_B, 3, pseudoRandom);
            Assert.AreEqual(2, room_A.floor[3].left, "First room (LEFT)");
            Assert.AreEqual(2, room_B.floor[5].right, "Second room (RIGHT)");
        }

        /* E. connectAdjacentRooms() */
        [TestMethod]
        public void searchAndUniteAdjacentRoomsWithDoor() {
            room_list = new List<Room>();
            room_A = new Room(0, 0, 3, 3);
            room_B = new Room(0, 3, 3, 3);
            room_A.fillRoom();
            room_B.fillRoom();

            room_list.Add(room_A);
            room_list.Add(room_B);

            DungeonTools.connectAdjacentRooms(room_list, pseudoRandom);
            Assert.AreEqual(2, room_A.floor[7].down, "First room (DOWN)");
            Assert.AreEqual(2, room_B.floor[1].up, "Second room (UP)");
        }

        /* F. setDoorInInnerRoom() */
        [TestMethod]
        public void settingADoorInTheInnerRoomCheck() {
            room = new Room(0, 0, 4, 4);
            room_A = new Room(1, 1, 2, 2); //Inner room
            room.fillRoom();
            room_A.fillRoom();

            room.floor[2].down = 1; // These walls should prevent room_A from being set
            room.floor[3].down = 1;
            room.floor[4].right = 1;
            room.floor[7].left = 1;
            room.floor[8].right = 1;
            room.floor[11].left = 1;
            room.floor[13].up = 1;
            room.floor[14].up = 1;
            Assert.AreEqual(false, DungeonTools.setDoorInInnerRoom(room, room_A, pseudoRandom), "Wall blocks the door");

            room.floor[8].right = 0;
            Assert.AreEqual(true, DungeonTools.setDoorInInnerRoom(room, room_A, pseudoRandom), "Space left for the door");
        }

        /* G. setWallAroundObject() */
        [TestMethod]
        public void settingAWallAroundAnObjectCheck() {
            room = new Room(0, 0, 4, 4);
            room.fillRoom();
            DungeonTools.setWallAroundObject(room, 1, 1, 2, 2);
            Assert.AreEqual(1, room.floor[1].down, "Down 1");
            Assert.AreEqual(1, room.floor[2].down, "Down 2");
            Assert.AreEqual(1, room.floor[4].right, "Right 1");
            Assert.AreEqual(1, room.floor[8].right, "Right 2");
            Assert.AreEqual(1, room.floor[7].left, "Left 1");
            Assert.AreEqual(1, room.floor[11].left, "Left 2");
            Assert.AreEqual(1, room.floor[13].up, "Up 1");
            Assert.AreEqual(1, room.floor[14].up, "Up 2");
        }
    }
}

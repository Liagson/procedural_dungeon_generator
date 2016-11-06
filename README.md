# Procedural dungeon generator (work in progress)
Generate your own dungeon using Unity.
The constants that set the dungeon (chance of pillars, doors...) can be found in *Dungeon.cs*. 
Returns a .bin file (*saveFile.bin*) with the dungeon's layout. The layout is mainly a matrix with each cell's description. Each cell consists on 4 sides (0 means nothing, 1 means wall, 2 means door) and a floor (0 means nothing, 1 means floor).
The format of the file is:

`Starting_room_x_position; Starting_room_y_position; Dungeon_width; Dungeon_height; Tile1_floor; Tile1_upper_side; Tile1_right_side; Tile1_lower_side; Tile1_left_side; Tile2_floor; ..."`

**Note: All data saved in the file is Int32. The ";" and " " are just used here for comprehension purposes.**
 
## Gizmo view
![SS1](https://raw.githubusercontent.com/Liagson/procedural_dungeon_generator/master/Pics/dungeon-with-gizmos.png)
## After rendering ([Gorthol's](https://github.com/gorthol) game in development)
![SS2](https://raw.githubusercontent.com/Liagson/procedural_dungeon_generator/master/Pics/render_dungeon.jpg)

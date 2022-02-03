# Maze Runner - User’s Documentation

Maze Runner is a “pseudo” 3D game. The aim of the player is to reach the finish line (to escape the maze) before he gets caught by the Maze Protector.
This game is meant to be played for fun and as an exhibition of what can be done via raycasting and billboarding.

The game can be compiled in MS Visual Studio after opening the project. I personally used Microsoft Visual Studio 2022 Current, .NET 6.0, CPU - Intel Core i5-10210U, RAM - 16 GB,
OS - Microsoft Windows 10 Pro, version 21H1, x64 for compilation and creation of this game. The game can be then run directly via MazeRunner.exe.

After executing MazeRunner.exe, the user sees the main menu where he can choose to play a new game or to exit the game.
The program expects a mouse click on one of the buttons. After clicking on the “New Game” button,
the player is set to a randomly generated maze. His position is at the upper left hand
corner of the maze and is marked as a blue square on the minimap (minimap can be seen at the upper right hand corner).
From now, the program expects WASD input (i.g. Keyboard buttons) for moving around the maze (W - move up, S - move down, D - rotate to the right, A - rotate to the left).
The aim is to escape the maze without being caught by the Maze Protector - MP, the finish line is marked green on the minimap (finish is always at the lower
right hand corner of the maze). After 3 seconds, the MP spawns and chases the player. The MP is marked as a red square on the minimap.
It’s original location is the same as the player’s one. After finishing the game, the player gets information whether he has won (“Victory”) or lost (“Game over”)
and the same menu as the first one appears so the user can choose to play a new game or to exit the game (program expects mouse click on one of the buttons once again).

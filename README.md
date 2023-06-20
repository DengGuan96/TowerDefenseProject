# Tower Defense Game

This project is a tower defense game developed in Unity, allowing players to strategically defend their base against waves of enemies. The game introduces two unique features: dynamic enemy path generation and randomized map creation.

## Features

- **Dynamic Enemy Paths**: The game utilizes the A* pathfinding algorithm to generate enemy paths based on user selections. Players can choose the starting and ending points by clicking on black squares in the game environment. The algorithm then calculates the optimal path for enemies to follow.

- **Randomized Maps**: The map generation system employs several algorithms, including breadth-first search, flood fill algorithm, and shuffling techniques. This ensures the creation of randomized maps that do not contain dead ends, providing an engaging gameplay experience.

- **Tower Types**: There are three types of towers available for players to defend their base:
  - Machine Gun Tower: This tower delivers single-target attacks, inflicting high damage to individual enemies.
  - Laser Tower: The laser tower emits a beam that slows down enemies, reducing their movement speed.
  - Missile Tower: This tower launches missiles that cause area-of-effect (AoE) damage, allowing players to damage multiple enemies simultaneously.

## Instructions

1. Set the starting point: Click on a black square in the game environment to determine the starting position for enemy waves.
2. Set the destination: Click on another black square to specify the endpoint for enemies.
3. Select a tower type: On the right panel, choose a tower type (machine gun, laser, or missile).
4. Build towers: Click on a white square in the game environment to construct the selected tower.

## Acknowledgments

Special thanks to the Unity community for their valuable resources and tutorials, which contributed to the development of this tower defense game.

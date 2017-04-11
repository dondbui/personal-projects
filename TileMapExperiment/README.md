# TileMapExperiment
This project is for my own personal experimentation with tilemap systems using Unity2D

## What currently works:
### Tile System
- Loading tiles from Tiled csv files
- Place units and account for sprite size offset
- Method to determine what unit occupies a given tile
- Automatically update occupied map based on unit position and movement

### Animation System
- Basic lerp animation for units
- Unit destination queueing system
- Only check unit positions while they have animations queued up or in progress

### Vision System
- Vision map updates during unit movement
- Vision map data draws to a transparent texture to overlay on top of the map
- Edge-Tile Raycasting Algorithm support for checking vision
![Edge-Tile Raycasting](https://github.com/dondbui/personal-projects/raw/master/TileMapExperiment/Screenshots/raycasting.png)

- Recursive Shadow Casting Algorithm support for checking vision
![Recursive Shadow Casting](https://github.com/dondbui/personal-projects/raw/master/TileMapExperiment/Screenshots/recursive-shadow-casting.png)

- Optional debugging option to see rays cast or tiles lit

##### Sources:

###### Tile Art: 
I take no credit for the art taken from the following open sources.
- http://opengameart.org/content/tileset32x32-grass-seasons-rocks-gravel
- http://opengameart.org/content/space-ship-building-bits-volume-1
- http://opengameart.org/content/spaceship-set-32x32px
- http://opengameart.org/content/asteroid-explosions-rocket-mine-and-laser
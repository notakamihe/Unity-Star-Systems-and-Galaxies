# Unity Star Systems and Galaxies

## Set-up Instructions
There are two ways that you can set this project up on your computer:
<ul>
  <li>
    Option 1: Zip file
    <p>
      <ol type="1">
        <li>Do NOT download the zip file from "Code" dropdown. Some assets will not be loaded correctly. Instead, download the "StarSystems.zip" file located in the "Releases" section of this repository.</li>
        <li>Once downloaded and unzipped, open the project in Unity Hub.</li>
      </ol>
    </p>
  </li>
  <li>
    Option 2: Git Clone 
    <ol type="1">
      <li>Ensure the Git CLI is installed.</li>
      <li>Open the terminal or the command prompt and run <code>git clone https://github.com/notakamihe/Unity-Star-Systems-and-Galaxies.git</code> in the desired directory.</li>
      <li>Open the cloned project in Unity Hub</li>
    </ol>
  </li>
</ul>

## What is this project?
This project is a first-person space simulation game that allows the player to take breathtaking views of celestial bodies as if they were there themselves. 
Players can traverse the cosmos and explore the most fascinating apsects of the Universe such as the Solar System, exoplanets, stars, and galaxies. 
This project was developed all throughout the summer by me using the Unity game engine and C#.

<img src="https://res.cloudinary.com/notak/image/upload/v1629760541/Star%20Systems%20and%20Galaxies/moon-earth_thlhmv.png" />

## Gameplay
### Controls
  - Movement:
    - <kbd>W</kbd> <kbd>A</kbd> <kbd>S</kbd> <kbd>D</kbd> to move forwards, backwards, and sideways
    - <kbd>Z</kbd> <kbd>X</kbd> to strafe up and down
  - Speed control:
    - Slow, planetary speed: <kbd>Shift</kbd> + <kbd>W</kbd>/<kbd>A</kbd>/<kbd>S</kbd>/<kbd>D</kbd>/<kbd>Z</kbd>/<kbd>X</kbd>
    - Boost speed: <kbd>Space</kbd> + <kbd>W</kbd>/<kbd>A</kbd>/<kbd>S</kbd>/<kbd>D</kbd>/<kbd>Z</kbd>/<kbd>X</kbd>
    - Interstellar speed: <kbd>Shift</kbd> + <kbd>Space</kbd> + <kbd>W</kbd>/<kbd>A</kbd>/<kbd>S</kbd>/<kbd>D</kbd>/<kbd>Z</kbd>/<kbd>X</kbd>
    - Intergalactic speed: <kbd>Tab</kbd> + <kbd>W</kbd>/<kbd>A</kbd>/<kbd>S</kbd>/<kbd>D</kbd>/<kbd>Z</kbd>/<kbd>X</kbd>
  - Time control (1-9):
    - <kbd>1</kbd>: Slowest, best for landing on worlds.
    - <kbd>9</kbd>: Fastest, best for time progression.
### Player
The player is a formless, city-sized space probe equipped with technology advanced enough to travel throughout the cosmos in seconds. When an object is in focus, it can read details about 
that object's size, mass, temperature, etc. It also has the ability to control speed and time at will.

<img src="https://res.cloudinary.com/notak/image/upload/v1629760541/Star%20Systems%20and%20Galaxies/beta-hyperbius_yr8exn.png" />

## Environments
### Solar System
This simulation's Solar System attempts to accurately portray most of the objects in it's real life counterpart. These include:
- The Sun
- Eight planets (Mercury, Venus, Earth, Mars, Jupiter, Saturn, Uranus, and Neptune)
- Four dwarf planets (Pluto, Ceres, Makemake, and Eris)
- Two belts (Asteroid and Kuiper)

<img src="https://res.cloudinary.com/notak/image/upload/v1629760541/Star%20Systems%20and%20Galaxies/triton-neptune_qm1iow.png" />

### Star Systems
Star systems in this simuation mainly consist of one star surrounded several planets, dwarf planets, and debris disks. They can either be:
  <ul>
    <li>
      Customizable
      <p>
        Players themselves manipulate the parameters that structure a star system. The Solar System is a customizable star system. Players can modify:
        <ul>
          <li>Properties of a star (type, size, color, temperature, mass, remnant, etc.)</li>
          <li>Properties of a planets, dwarf planets, and moons (size, mass, temperature, distance from star/planet, texture)</li>
          <li>Properties of a debris disk / belt (distance from star, asteroid prefab, thickness)</li>
        </ul>
      </p>
    </li>
    <li>
      Procedural
      <p>
        Star systems are generated through code. The star properties, size and number of planets are randomly decided by the computer. 
        The properties of planets, moons, and dwarf planets are calculated through math and correlations.
      </p>
    </li>
  </ul>


<img src="https://res.cloudinary.com/notak/image/upload/v1629760541/Star%20Systems%20and%20Galaxies/red-dwarf_lpk7v5.png" />
<img src="https://res.cloudinary.com/notak/image/upload/v1629760541/Star%20Systems%20and%20Galaxies/exoplanet_yfpqsr.png" />
<img src="https://res.cloudinary.com/notak/image/upload/v1629760541/Star%20Systems%20and%20Galaxies/yellow-dwarf_yatbmp.png" />

#### Stellar Remnants
When a star dies, it can leave behind one of three remnants based on its type and mass: 
  - White dwarf surrounded by a planetary nebula
  - Neutron star
  - Black hole

<img src="https://res.cloudinary.com/notak/image/upload/v1629760541/Star%20Systems%20and%20Galaxies/neutron-star_cazmof.png" />

### Galaxies

Galaxies are astronomical structures that consist of a lot of dust, gas, and stars. In this simulation, users can customize a galaxy's:
  - Color
  - Size
  - Shape (sprial or elliptical)
  - Number of stars (up to 100 for performance reasons)
  
<img src="https://res.cloudinary.com/notak/image/upload/v1629760541/Star%20Systems%20and%20Galaxies/galaxy_p5kmyv.png" />

### Galaxy Clusters

Galaxy clusters are the largest possible structures in the simulation. They can contain multiple galaxies and can span several million light years, 
dwarfing even the largest galaxies. Users can specify the number of galaxies that can be contained in a cluster.

## Notes
- Distances are not 100% up to scale. If they were, the playable universe would be a lot a bigger than what is intended.
- Orbits are circular rather than elliptical
- Planet-landing is not recommended on an intergalactic scale. Movements become jerky when the universe is steered very far away from the origin.
- With the exception of Kepler's third law, this project does not use any of the theories, laws and mathematics that are applied in real-world astrophysics. Celestial bodies mostly
behave based on simple linear or exponential algebraic equations or conditional logic.

## Links
Links to the YouTube videos that I have made from this project are listed below:
  - Solar System: https://youtu.be/fJl2UBHESiA
  - Procedural Stars and Exoplanets: https://youtu.be/iwfLOZ3_WCo
  - Galaxies: https://youtu.be/VRrhi7AmKtQ
  - Galaxy Clusters: https://youtu.be/zJXmgGsMBi4

Game Design Document – Cooperative Minigame Project

The project is going to be focused on developing a two-player game built around a collection of simple but sticky minigames. The main idea is to create a moduler system players/characters are selected first. After selected, interact in a shared lobby, and access different minigames through portals! The game is designed to run efficiently and expected to weight under 500 mb.


The game flos is structured as: 

Main Menu - Characters Selection - Lobby - Start Playing!

Players start selecting one of the available characters (number of characters to be decided). These characters are at the moment cosmetic and serve to visually be distinguishable from player to player. After a selection, both players spawn in a small lobby where they can walk around and interact on it before choosing a platform/game to start playing to!

The first minigame is an endless runner where both players move forward automatically while avoiding incoming obstacles. The goal is to survive until a fixed amount of time. The second miningame is inspired by Tron, where players move continuously and leave a trail behind. Colliding with walls or trails causes in elimination! and the last player standing wins.

The system is structured using multiple scenes such as:
- Main Menu
- Character Selection
- Lobby
- RunnerMinigame
- TronMinigame
- GameManager (storing player selection and transition managing)

Controls are based on the followed input system: each player uses an 8-directional stick and multiple buttons for actions such as movement, selection, interaction, and pause.

Milestone 1, Core System: 
Have implemented input handling, player movement, GameManager, and basic scene nvigation.

Milestone 2, Game Flow: 
Have completed character slection, lobby interaction, and smooth transitions between scenes and minigames:)

Milestone 3, Minigames!
Refined both minigames, by implementing win/lose conditions, UI, and having bug-free minigames


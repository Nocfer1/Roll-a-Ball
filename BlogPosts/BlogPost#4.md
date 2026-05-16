<div align="center">

# Development

</div>

---

Now it's time to make the lobby and the two portals for the minigames. In order to achieve this I wanted to make the camera look in good position as there are 2 players going around the lobby, which means that setting a follower-camera would be a good idea. However, I decided to go for a camera setting of Orthographic perspective instead of Perspective. This will make the lobby scene more adjusted with what brings (portals, two players going around).

<div align="center">

<img width="800" height="437" alt="gifLobby" src="https://github.com/user-attachments/assets/eb1e0e08-9090-4d84-a9ba-e75d4625ad10" />

</div>

---

For the players input within this scene, no complex controls were thought. Players and freely move around the lobby plane and get into a portal which will take them to the respective minigame.

On the GIF we can see the 2 portals which different names, colors and font-family. This is because I wanted the minigames to have their respective font-family relating to what the minigames are about (or close enough).

<div align="center">

<img width="877" height="468" alt="image" src="https://github.com/user-attachments/assets/e762eb68-b10e-4988-951f-2769e8a63096" />

</div>

---

Once the players step into the Neon Rush portal, here is the transition afterwards. For the Tron-like game, the main idea is that both players move inside an arena while leaving a trail behind them. Each player needs to avoid crashing into the walls, the opponent’s trail, or their own trail. This creates a competitive minigame where movement, timing, and quick reactions are important.

For this minigame, the visual text showing up on the screen will be the players lives, which will start with 3 for each other, on every top-corner of the respective player, and also a counter to make the players be ready when the game starts. Also, as mentioned on the previous blog post, every player has a respective color, which will therefore be the color of the trail of every player on this minigame.

Making this game had more complexity than I expected, as I had to do a long research of how the a trail should work or be rendered. At first I thought it was a simple cube-drawing for every frame, but then I thought it's not that of a wise solution performance talking. Then I realize that with a simple cube getting extending every time until the player turns was the best idea, as throughout the game there will be only a few cubes drawn instead of a single one for every frame, which would certainly slow the performance of the game.

<div align="center">

<img width="871" height="467" alt="image" src="https://github.com/user-attachments/assets/dd0eb275-cadb-4760-a0dd-e30bbe3e1a53" />

</div>

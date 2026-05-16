<div align="center">

# Development

</div>

---

Finally, we arrive at the last minigame: **Dodge It!**. This is a runner-style minigame where both players have to dodge obstacles while running forward on a three-lane platform. The goal is to survive as long as possible and reach the end with the best score.

<div align="center">

<img width="880" height="490" alt="image" src="https://github.com/user-attachments/assets/f1f693b9-a07c-4697-a053-791d11c7a1a7" />

</div>

---

In this minigame I decided to make it colorful, as it was running on a rainbow on the sky.

One of the challenges I encountered in this game was the score overview. The way it works is that the game does not only check who survives until the end, but also how well each player performed during the run. The score is calculated based on the player’s progress, the remaining lives, and the amount of hits received. This means that a player can still get a good score even if they do not reach the end, as long as they managed to survive for a long distance and avoided most obstacles.

To make the result fairer, when a player loses all their lives, the game saves the progress percentage at the moment of death. If the player survives until the end, the final progress is used instead. After that, the game adds bonus points for the lives the player still has and subtracts points for every hit received. At the end, both scores are compared and the result panel shows whether Player 1 wins, Player 2 wins, or if the game ends in a draw.

---

Another important part of the minigame was the obstacle system. At first, the game only used simple ball obstacles, but later I expanded it by adding different obstacle prefabs such as crystals and double-lane blocks. This made the gameplay less repetitive, since players are not only reacting to single objects, but also to obstacles that can block more than one lane and force them to quickly choose a safer path.

I also added a difficulty system with **Easy, Normal, and Hard** modes. These modes change how often obstacles spawn and how likely stronger obstacles are to appear. For example, **Hard mode** spawns obstacles faster and increases the chance of double-lane blocks, making the minigame more challenging for both players.

---

After the **Dodge It!** was completed, I focused on implementing the UI of the game, adding sounds, SFX, and music depending on the different scenes through the game. In order to achieve this, I made a script that will help setting the different media triggered by the different minigames/lobby managers respectively.

<div align="center">

<img width="527" height="902" alt="image" src="https://github.com/user-attachments/assets/e122c63e-c268-4aaf-9ae8-1d6521075ec2" />

</div>

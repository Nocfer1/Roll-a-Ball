Welcome on board! 

My first step in this project was building the base game by following Unity’s classic Roll-a-Ball tutorial. The goal was simple: create a controllable player sphere, move it around a small 3D level, collect pick-ups, and trigger a win condition once everything had been collected. Even though it is considered a beginner project, it was a great way to understand how Unity connects physics, scripting, UI, and scene logic into a playable loop. In my opinion this "tutorial", or project helps so much as a very first project

The goal of the implementation was the player controller. I used Unity’s physics system to move the ball with a Rigidbody, applying forces based on player input (W, A, S, D) instead of directly translating the object. This made movement feel smoother and more physical (even though I struggled with the player mass), which fits the idea of controlling a rolling sphere.

For the camera follow behavior, instead of using a static camera, I implemented a script that keeps the camera offset relative to the player. This made the game much easier to read, since the camera naturally follows the ball as it moves around the map. It is a small feature, but it immediately makes the project feel more like a real game and less like a test scene.

For interaction, I added collectible objects that rotate continuously to make them stand out in the environment. When the player touches one, it disappears and increases the score counter. This required handling trigger collisions, updating the score in real time, and checking whether all pick-ups had been collected. Once that condition was met, the game displayed a win message, completing the basic gameplay loop.

I also applied Unity’s UI system to show score on screen. The score text element keeps track of how many objects the player has collected, while a win text appears only when the level is complete. That helped tie the systems together: movement gave the player control, collectibles gave them a goal, and UI made progress visible.

Pretty much on this first version was all about learning how different Unity components communicate. The scripts were simple, but they introduced key ideas like collision detection, physics-based movement, object lifecycle, and state updates through UI. More importantly, this prototype became the foundation for the rest of the project. Once the basic Roll-a-Ball loop was working, I had a stable starting point to expand the game with new mechanics, hazards, enemy behavior, and extra interactions in later blog posts.

In other words, this was the moment the project stopped being just a tutorial and started becoming my sandbox.

Here's a little spoiler for an extension I made to the game :)

<img width="572" height="315" alt="image" src="https://github.com/user-attachments/assets/55d1f4e7-46d3-4670-879b-3d1e9e759ecb" />

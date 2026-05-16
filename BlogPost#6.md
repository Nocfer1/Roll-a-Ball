<div align="center">

# Final Project

</div>

---

After going through phases such as game design, folder structuring, script implementation, behaviour systems, UI setup, player controls, and scene management, the game is finally completed.

<div align="center">

<img width="226" height="312" alt="image" src="https://github.com/user-attachments/assets/8da6096c-dcaf-4625-8b92-b776ef509b7b" />

</div>

---

Structure-wise, the project files are split into their respective scopes, making the project structure more modular and clean. This helped me keep the project organized as it became bigger, especially because the game contains different scenes, characters, minigames, UI elements, audio files, and scripts.

<div align="center">

<img width="222" height="147" alt="image" src="https://github.com/user-attachments/assets/12240df9-01f4-4aaf-9b97-e6df73a0ccfd" />

</div>

For example, in the Scripts folder I separated the scripts depending on whether they belong to a specific scene, a specific minigame, or a shared system used across the game. This made it easier to work on each part of the project without mixing unrelated behaviours together. The runner minigame, Tron minigame, lobby, character selection, and general managers each have their own responsibilities.

---

The final game starts with a character selection scene, where both players can choose their character before entering the lobby. After both players are ready, they are sent to the main lobby. The lobby works as a central area where players can freely move around and enter different portals. Each portal takes the players to a different minigame.

The final version includes three playable minigames. Each one was designed with a different gameplay style so that the experience would feel more varied. The Tron minigame focuses on quick direction changes and avoiding trails, while Dodge It! uses a runner-style system where players move between lanes to avoid incoming obstacles. These differences helped make each minigame feel unique instead of repeating the same mechanics.

---

<div align="center">

## Game Showcase

</div>

<table>
  <tr>
    <td align="center">
      <img width="100%" alt="image" src="https://github.com/user-attachments/assets/bcf2092a-1e72-4ddb-8d17-3718657a286d" />
    </td>
    <td align="center">
      <img width="100%" alt="image" src="https://github.com/user-attachments/assets/cc898952-d144-4d9e-8d93-21d082b154f3" />
    </td>
  </tr>
  <tr>
    <td align="center">
      <img width="100%" alt="image" src="https://github.com/user-attachments/assets/7c662f80-34db-4034-993c-3bd89e0a0223" />
    </td>
    <td align="center">
      <img width="100%" alt="image" src="https://github.com/user-attachments/assets/6f1d4279-62af-45a4-98f1-50c7e9a53039" />
    </td>
  </tr>
  <tr>
    <td colspan="2" align="center">
      <img width="80%" alt="image" src="https://github.com/user-attachments/assets/d9772057-41d3-4ff2-a557-ef0d3529cb04" />
    </td>
  </tr>
</table>

---

Another important part of the final product was the UI. Each minigame now has its own intro menu before the gameplay starts, so the player does not enter directly into the action without preparation. The menus also include options such as starting the game, returning to the lobby, rematching, and viewing the final results.

Audio was also added to improve the overall feeling of the game. The final version includes background music for different scenes, button sounds, portal sounds, countdown sounds, hit sounds, and win or draw sound effects.

---

Overall, the final product is a complete local multiplayer minigame experience with a character selection screen, a central lobby, multiple portals, different minigames, UI menus, controller support, and sound design. Compared to the first prototype, the project became much more polished and structured. Also, this project helped me improve my understanding of Unity scene management, player spawning, UI flow, input handling, rigid bodies, game managers, and modular project structure.

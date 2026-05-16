<div align="center">

# Development

</div>

---

Once the blueprint was made on BlogPost 2, I could have a clearer version of the actual game I wanted to achieve.

Moreover, I started working on the character selection and what the caracters are going to be. In order to do that, I went through mixamo.com and see the most fittable prefab I could one for the whole game. After spending some time, I chose the prefabs to use, and how they're going to be defined on the game. No names too fancy though, as it wasn't my main scope on this chapter.

<div align="center">

<img width="323" height="118" alt="image" src="https://github.com/user-attachments/assets/df4553f3-81d7-4706-89ef-8b1cbca5035e" />

</div>

---

At the same time, when deciding the prefabs to use on the game, I also brought with a set of animations; Running and Idling. These animations will therefore be used on the game, for example, when on character selection, the characters will be idling instead of just standing static with no movement, or when moving around the lobby they have a clean transition of idling when standing still and running (animation) when moving.

<div align="center">

<img width="331" height="137" alt="image" src="https://github.com/user-attachments/assets/65f96d5c-4823-4087-b268-9392d0eadbb1" />

</div>

---

After that, I started doing the character selection scene, which will come along with the input handling and GameManager

<div align="center">

<img width="881" height="492" alt="image" src="https://github.com/user-attachments/assets/765e6904-bdc5-4a1a-aa49-6f9c83e8e234" />

</div>

---

TADAA! This is the character selection scene which will be shown after a main menu when opening the game. I also decided to use color on the different characters; Alice (Pink), Billy (Blue) and Charlie (Green).

<div align="center">

<img width="326" height="175" alt="image" src="https://github.com/user-attachments/assets/95618117-daa4-4359-95fa-f165bc608205" />

</div>

---

For the main menu before the character selection, I wanted to add more option rather than just play and exit game. However, the game itself was not that widely complex to have graphic settings or some other button that could have been useful.

This script works as a global GameManager for the game. It saves which character each player has selected. By default the players have a pre-selected character (Alice and Billy). It also keeps track of whether each player is ready or not. The Awake() method makes sure there is only one GameManager in the game. If another one already exists, the new one is destroyed. If this is the first one, it becomes the main instance and is kept alive between scenes using DontDestroyOnLoad(). This is useful because the selected characters and ready states can stay saved when moving from the character selection scene to the lobby or another scene. The ResetReadyState() method simply sets both players back to not ready, which is useful when returning to the character selection screen.

```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public CharacterType player1Character = CharacterType.Alice;
    public CharacterType player2Character = CharacterType.Billy;

    public bool player1Ready = false;
    public bool player2Ready = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ResetReadyState()
    {
        player1Ready = false;
        player2Ready = false;
    }
}

# Giant Robots - RPG Game for Meta Quest 3

## Team

- [Maximilian Amougou](https://github.com/CallMeSwarley)
- [Serhad Çalışkan](https://github.com/serhadcaliskan)

## Development Context

This game was developed during the Winter Term 2024/2025 at TUM as part of the [Computer Games Laboratory (IN7106)](https://www.cs.cit.tum.de/cg/teaching/winter-term-24-25/computer-games-laboratory/) course.

## Concept

**Giant Robots** is an immersive RPG-style battling robots game developed for the Meta Quest 3. This innovative game eliminates the need for controllers, using hand gestures and voice commands to control the game.

The game leverages large language models (LLMs) to enhance interactions with NPCs. These NPCs engage in dynamic, AI-driven conversations on a wide variety of topics, making every encounter unique. Additionally, the LLM steers fight opponents and evaluates the player's Karma system, ensuring a personalized and engaging experience.

Conversations are further enhanced with text-to-speech and speech-to-text capabilities, powered by **Wit.ai**, to create natural and immersive dialogues.

## Story

Set on the harsh and unforgiving prison planet of Mars, players take on the role of a skilled pilot commanding a powerful combat robot. In a brutal competition, human pilots battle in massive machines for a shot at freedom. Each victory earns a cash prize that can be used to upgrade weapons, enhance skills, and prepare for even tougher opponents.

Players encounter legendary figures from history, philosophy, and fiction (e.g., Billy the Kid, Socrates, Severus Snape), who bring their own strategies and wisdom to the battlefield. The ultimate goal is to win enough battles to escape the prison’s deadly grip.

NPCs (opponents and allies) interact with the player, speaking in the voice and style of their character. For example, the pirate NPC uses phrases like "Arrh Matey" and "Shiver me timbers!" while Shakespeare might spout Elizabethan prose while piloting his robot.

## Game Platform

- **Meta Quest 3**
- Developed using **Unity**, optimized for **Meta Quest 3**
- **3D assets** (robots, arenas) modeled and animated in **Maya**
- **AI-powered character dialogue** and **opponent AI** using **GPT-4o**
- Integrated **Text-to-Speech** and **Speech-to-Text** via **Wit.ai** for enhanced communication
- Utilizes the Quest's built-in speaker and microphone for immersive sound, while hand and body motions are detected for controller-free control

## Theme - Chain Reaction

The theme "Chain Reaction" is integral to both gameplay and narrative. The player's interactions with NPCs directly impact their **Karma score**, which in turn influences NPC behavior. High Karma leads to beneficial interactions, while low Karma can make battles more difficult.

- NPCs with high Karma may offer valuable battle insights or negotiate better prices at shops.
- NPCs with low Karma may withhold critical information or even deceive the player, making the game more challenging.
- The visual theme of "Chain Reaction" is reflected in the game’s projectile mechanics, where chains explode in a cascading manner.

## Game Mechanics

- **Turn-Based Combat**: Players choose from five actions each turn: Load, Shoot, Dodge (success chance influenced by Karma), Disarm (success chance influenced by Karma), and Shield (limited uses).
- **Upgrades & Progression**: Players can enhance their robot with upgrades that increase damage and improve action success probabilities.
- **Karma System**: The player's politeness influences Karma. Rudeness leads to negative outcomes, while respect earns advantages.
- **NPC Interaction**: Players can interact with nearby NPCs by making a fist, which activates voice commands for more natural dialogue.
- **Opponent AI Difficulty**:
  - **Basic**: Ignores game stats.
  - **Mid**: Analyzes game stats.
  - **Hard**: Analyzes game stats and previous fight history for smarter decisions.

## Development Process Docs

You can access the detailed development process documentation for the game [here](https://collab.dvb.bayern/display/TUMgameslab2425winter/Giant+Robots).

## Setup Instructions

1. **Clone the repo**:
   ```bash
   git clone https://github.com/serhadcaliskan/GiantRobots.git
   ```
2. **Insert your OpenAI API Key:**
   Open Assets/Config/APIKeys.cs and add your API key for OpenAI.
3. **Build and Run:**
   Open the project in Unity.
   Build the project and select the connected Meta Quest 3 as the target device.

## Additional Notes

Check out the official [trailer](https://youtu.be/VV73wE3DPkc?si=KosBeOoLj34EQat9) for the game.

The original commit history can be found at: [serhadcaliskan/GiantRobots](https://github.com/serhadcaliskan/GiantRobots.git)

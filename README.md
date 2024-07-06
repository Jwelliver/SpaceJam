A multiplayer 3D game for dogfighting your friends in space.
Note: This is a personal gamejam-type sideproject that I put together for friends and I to play. It's not a top priority for me as I reached my initial development goal, but I do expect to casually continue development.

**Current Status**
The game is functional with a solid foundation for additional features.
For the most part, it utilizes "dev art" as I intended to spend as little time as possible getting to a working game. (e.g. ships are composed almost entirely of primatives rather than having a dedicated model)

**Current Features**
- Split Screen Multiplayer 1-2 players
- Controller Support w/ haptics
- Ability to individually customize the color of each ship component and it's lazers.
- Physics-based flight behavior with the ability to toggle Flight Assist for full newtonian-style physics.
- Homing Missles and Lazers
- Immersive 3D sound
- 3D spherical Radar to track objects (e.g. asteroids) and enemies around you
- Procedurally generated asteroid fields.
- Energy Management: Manuevering and fireing weapons consume energy, which regenerates over time. Players aren't able to just "run and gun" their way through a fight, but instead must monitor and manage their ship's energy levels wisely, or risk being at a severe disadvantage awaiting recharge with no systems online.

**Future Features**
Note: While this was a sideproject I spent a bit too much time on, eventually needing to switch my focus elsewhere, I have built a good foundation for some fun features I'd always wanted in a space flight sim.
So, there's no expected timeline, but I have no doubt I'll be putting more work into this here and there. If you're interested taking it up, feel free to fork and/or make a PR.

- Online Multiplayer: This was my initial intention as I'd recently worked on a project with online multiplayer, and had a lot of fun. However, to keep the scope down initially, I (somewhat regrettably) opted for split screen for the time being.
- Improved Radar controls: The radar system is robust and is capable of displaying the mesh of any collider it detects. I'll eventually implement this as part of a radar "zoom" function. Additionally, I'd like to implement radar shadow and other effects to add extra layers of tactical gameplay.
- Shield: I considered putting a shield in, but left it out for time's sake. I do think it would be interesting to weave it into the ship's energy system.
- Improved damage fidelity: Currently, the ship damage is simple. Any hit will subtract from the primary "health" of the ship. While I did initially want to maintain this "arcade" style approach,
  I'd designed the code with the idea in mind of being able to damage individual components separately, which will impact the ship's functionality. (e.g. thrusters, radar/electronics, etc.)
- Improved collisions: I'd like to build a better collision system that dynamically affects the mesh, or at least allows "chunks" to break off somewhat realistically.
- AI: I did setup a basic AI ship, but it's not implemented. It was a quick and dirty approach that I specifically avoided after realizing that working on it would be way too fun and derail me from the MVP goal.
- Game Modes: I will implement various game modes for players to complete either against eachother or as coop. Some ideas are an escort mission, CTF, and RTS-style territory/resource management.
- More weapons: Without dreaming up all the cool things that can be done here, I have planned to at least add droppable/shootable mines that explode when an enemy flies too close.
- Resources: I've considered the idea of players placing a "nanite factory" in an asteroid field which allows them to consume asteroids and convert it into resources for players, who can retrieve it when flying near the factory. The resources can perhaps be stored in a reserve to repair parts of the ship. This can be used to implement resource-based gameplay with additional strategy elements.

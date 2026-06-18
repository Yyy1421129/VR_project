# VR Shooter Game

A virtual reality shooter game developed as a project and learning experience. This game allows players to fire bullets at target dummies, experience VR hand animations, and see real-time score updates. Built using Unity's XR Interaction Toolkit, this game is compatible with external VR headsets or can be used with the XR Simulator for those without VR hardware.

## Video Demo
<video src="./DEMO.mp4" controls width="100%"></video>

## Table of Contents
- [Features](#features)
- [Getting Started](#getting-started)
- [Requirements](#requirements)
- [Assets](#assets)
- [Installation](#installation)
- [How to Play](#how-to-play)
- [Environment](#environment)
- [Scripts Overview](#scripts-overview)
- [Using the XR Simulator](#using-the-xr-simulator)
- [External Packages](#external-packages)
- [Contributing](#contributing)
- [项目分工](#项目分工)

## Features
- **VR Bullet Shooting**: Players can shoot bullets with sound effects on firing.
- **Interactive Target Dummies**: Dummies activate when players enter a trigger zone, providing target practice.
- **Hand Animations**: VR hand grip and trigger animations respond to player input.
- **Score Display**: Real-time score updates are displayed as targets are hit.
- **XR Simulator Support**: Allows users without VR hardware to experience VR-like controls.

## Getting Started
This project is perfect for learning VR development in Unity and understanding VR interactions.

## Requirements
- **Unity**: Version 2021.3 or newer
- **VR Headset** (optional): Oculus, HTC Vive, or other Unity-compatible VR headset
- **Git**: For cloning the repository

## Assets
This project includes assets from the Unity Asset Store and custom VR hand interaction assets:

- **Polygon Starter Pack**: Includes 3D models and prefabs for the basic environment setup:
  - **Environment Elements**: House, door, land, mountain, floor, and boxes.
  - **Weapons**: Guns, swords, and interactive objects for gameplay.
- **VR Hands**: Custom VR hand assets with prefabs and animations for left and right hands.
- **Asset Download Link**: [Download VR Hand Assets](https://drive.google.com/file/d/1Fnli8Tbq7NeTw8pSTwjiZcSbE7UB3rL1/view)

## Installation
1. **Clone the Repository**:
   ```bash
   git clone https://github.com/yourusername/vr-shooter-game.git
Open Project in Unity: Open Unity Hub, select the cloned project folder, and open it.
Configure XR Settings: Go to Edit > Project Settings > XR Plug-in Management and enable the VR platform for your headset (e.g., Oculus or OpenXR).
How to Play
Movement: Use the VR controller joystick (or simulator controls) to move.
Shooting: Aim with the VR controller and press the trigger to fire at target dummies.
Scoring: Points are added each time a target dummy is hit.
Environment
The game environment includes a ground plane, target dummies, and basic lighting. This setup creates an immersive learning and testing environment for VR interactions.

## Scripts Overview
This project includes several scripts that contribute to the core functionality of the VR Shooter Game:

- **Weapon.cs**
  - **Purpose**: Base class for all weapons, defining essential properties like shooting force, recoil, and damage, and managing VR controller interactions.
  - **Why Created**: To centralize weapon functionality and allow inheritance for creating specific weapons (e.g., pistol, rifle) with consistent behavior.

- **Pistol.cs & Rifle.cs**
  - **Purpose**: Extends `Weapon.cs` to manage unique behaviors for each weapon. `Pistol.cs` supports single-shot mechanics, while `Rifle.cs` enables automatic firing.
  - **Why Created**: To customize each weapon’s behavior while reusing `Weapon.cs` functionality, allowing for varied gameplay experiences.

- **Projectile.cs**
  - **Purpose**: Base class for projectiles, including methods for initialization and launch. Acts as the foundation for different projectile types.
  - **Why Created**: To provide a reusable base for any projectile, simplifying the creation of both physics-based and raycast projectiles.

- **PhysicsProjectile.cs & RaycastProjectile.cs**
  - **Purpose**: Implements specific types of projectiles. `PhysicsProjectile.cs` uses physics-based movement, while `RaycastProjectile.cs` uses raycasting to detect collisions.
  - **Why Created**: To allow flexibility in projectile behavior, ensuring compatibility with both physics-based and instant-hit mechanics for a realistic experience.

- **EnemyAI.cs**
  - **Purpose**: Controls enemy behavior, including navigation, aiming at the player, and responding to damage. Manages enemy animations such as running, crouching, and shooting.
  - **Why Created**: To provide engaging AI that adds challenge and realism, allowing enemies to react to player actions and make combat dynamic.

- **ScoreManager.cs**
  - **Purpose**: Manages the game’s scoring system, updating the score each time an enemy is hit and displaying it in real time using TextMeshPro.
  - **Why Created**: To reward the player for successful engagement with enemies, enhancing the competitive aspect of the game.

- **ITakeDamage.cs**
  - **Purpose**: Interface defining the `TakeDamage` method for any damageable entity (e.g., enemies), specifying their response to being hit.
  - **Why Created**: To ensure consistency in damage handling across objects, simplifying the application of damage logic.

- **MeshHidder.cs**
  - **Purpose**: Utility script that hides the VR controller mesh when an object is picked up, enhancing immersion by focusing on the grabbable item.
  - **Why Created**: To improve the VR experience by removing the controller model when holding an object, giving players a clear view of the item.

- **EnemySpawner.cs**
  - **Purpose**: Manages enemy spawning at intervals and locations, keeping gameplay challenging by introducing new enemies over time.
  - **Why Created**: To automate enemy generation and control game pacing, maintaining continuous engagement for the player.

- **PhysicsDamage.cs**
  - **Purpose**: Implements the `ITakeDamage` interface to enable objects to react physically to damage, applying a force that pushes them in the projectile's direction.
  - **Why Created**: To add realistic physics-based responses to damage, enhancing immersion by making objects visibly react to impacts.

- **Player.cs**
  - **Purpose**: Manages the player's health and head position, allowing enemies to target the player and respond to attacks.
  - **Why Created**: To track player health and provide a target for enemy AI, adding challenge to gameplay by requiring players to avoid taking damage.

Install the XR Interaction Toolkit: Go to Window > Package Manager, search for XR Interaction Toolkit, and install.
Add the XR Device Simulator: Go to Window > XR > XR Device Simulator and enable it in the scene.
XR Simulator Controls
Movement: W, A, S, D keys for forward, left, backward, and right.
Camera Rotation: Right-click and move the mouse to look around.
Simulated Hand Controls: Use Q/E to toggle hands, R/F for grip, T/G for trigger actions.
External Packages
XR Interaction Toolkit: Provides VR components for interaction and movement.
XR Plug-in Management: Manages VR headset compatibility.
Contributing
Contributions are welcome! To contribute:

Fork the repository.
Create a new branch:
bash
Copy code
git checkout -b feature-name
Commit your changes:
bash
Copy code
git commit -m "Add feature"
Push to the branch:
bash
Copy code
git push origin feature-name
Open a pull request.

## Project division of work

| 成员 | 分工 |
|------|------|
| **杨熠** | 项目主要构思，整个项目的搭建及完善，音乐及美术设计 |
| **何明鸿** | 参与项目初期的设计构思，寻找素材 |
| **李金泽** | 参与项目初期的设计构思，寻找素材 |
| **易俊锋** | 参与项目初期的设计构思，寻找素材 |

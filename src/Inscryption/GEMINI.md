# Project: Inscryption Clone

## Project Overview

This project is a 3D card game that is a simplified clone of the game "Inscryption". It is built using the Unity game engine (version 6000.2.8f1) and is written in C#.

The core gameplay involves two players playing cards with Health (HP) and Damage (DMG) attributes onto a game board with four lanes. The objective is to reduce the opponent's health to zero.

The project utilizes the following key technologies and architectural patterns:

*   **Entitas:** An Entity Component System (ECS) framework for managing game state and logic.
*   **Zenject:** A dependency injection framework for managing dependencies between different parts of the application.
*   **Addressables:** For loading and managing game assets.
*   **UniTask:** For managing asynchronous operations.

The technical specification for the game can be found in `Assets/Doc/main.md`.

## Building and Running

This is a Unity project and should be opened with the Unity Editor (version 6000.2.8f1).

1.  Open the project in the Unity Hub.
2.  Once the project is loaded, you can run the game in the editor by pressing the "Play" button.
3.  Builds can be created through the "File > Build Settings" menu in Unity.

## Development Conventions

### Code Generation

The project uses the Entitas framework, which requires running a code generator after creating or modifying components.

*   **Component Generation:** The `Jenny.bat` script is used to generate Entitas components.
*   **Generator Path:** The script is located at `C:\Projects\Inscryption\Jenny\Jenny-Gen.bat`.
*   **Note:** There are conflicting instructions regarding the execution of `Jenny.bat`. One note says to always run it after creating components, while another says to skip execution. It is recommended to clarify this with the development team.

### Coding Style

*   **Dependency Injection:** Use Zenject for dependency injection.
*   **Configuration:** Use a `ConfigService` to access configuration values. New configurations can be created and added to the Addressables group.
*   **Entitas Component Access:** When accessing Entitas components, use the full component name, unless the name contains the word "component". For example, use `entity.Transform` instead of `entity.TransformComponent`.
*   **Functional Extensions:** Use `FunctionalExtensions` where possible.

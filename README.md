# 🎨 GridMakerPainting
Unity tool that allows you to create and edit 3D tile-based maps using a 2D painting interface directly in the Inspector.  
> *Recommended for Unity 6, but compatible with older versions.*

## Table of contents
- [Introduction](#introduction)
- [Installation](#installation)
- [Features & Tech](#features)


## <a id="introduction"></a>ℹ️ Introduction

For a personal project, I developed this tool to allow a Level Designer to quickly modify the player's environment.

The map is composed of many individual tiles, and manually editing each tile in the scene was slow and tedious.

To solve this, I created a custom inspector that allows painting the map in 2D while automatically generating the 3D result in real time (with no performance issues 😁).

This enables a fast, visual, and efficient workflow.

Example:

<img width="1918" height="1035" alt="Screenshot" src="https://github.com/user-attachments/assets/65b2222b-ceec-4f5d-ba6f-6777e24a523e" />

# <a id="installation"></a>⬇️ Installation

## Graphic Version Selection

For proper functionality, select the correct branch based on your Unity version  
(Example: if you are using Unity 6.2, select **After_Unity_6.1**).

The only difference between the branches is the visual style. Unity 6.1 supports a wider default color range.

**Before Unity 6.1** on the left, **after** on the right:

<img width="1200" height="680" alt="AvantApres" src="https://github.com/user-attachments/assets/74731177-0662-4c48-bc08-cc1ebd9e3916" />

Regardless of the version, the environment remains the same:

<img width="945" height="467" alt="Environment" src="https://github.com/user-attachments/assets/661310a6-6f1b-456e-87bc-57bf0ca0ea47" />

---

## <a id="GridMapper"></a>🧠 GridMapper – Setup Guide

### 1. Create the map parent
- Create an empty **GameObject** in the scene
- Create a Tag named `ParentMap`
- Assign this Tag to the GameObject

This GameObject will serve as the parent for all generated 3D tiles.

---

### 2. Create GridMapper assets
In the **Project** window, create the following assets:

- **ToolIcon** (defines the tool icons)
- **TileDataBase** (defines all available tiles)
- **GridMap** (used to paint and generate the 3D environment)

---

### 3. Configure ToolIcon
In **ToolIcon**, assign textures for the tools:

- Brush
- Rect
- Fill

> *Default icons are provided, but you can use any 2D texture.*

---

### 4. Configure TileDataBase
In **TileDataBase**, add the tiles available for the map.

Each tile must contain:

- a Prefab (instantiated as a 3D object)
- an Icon (used for the 2D grid display)
- a unique ID (IDs must be unique, default range is 0 to 5)

---

### 5. Configure GridMap
In **GridMap**, assign:

- ToolIcon
- TileDataBase

---

### 6. Use the editor
Select **GridMap** in the Project window.

The following elements will appear in the Inspector:

- tools (Brush, Rect, Fill)
- available tiles

> *If nothing appears, click elsewhere and re-select GridMap.*

---

### 7. Resize the grid
Use the following parameters:

- Row
- Column

to change the map size.

Tip: using two monitors greatly improves workflow comfort.

---

## <a id="features"></a>Features

- 2D grid painting directly in the Unity Inspector  
- Real-time 3D tile generation  
- Brush tool  
- Rectangle tool  
- Flood fill tool  
- Tile database system  
- Custom icon support  
- Automatic visual rebuild  
- ScriptableObject-based architecture  
- Non-destructive grid resizing  

---

## 🛠️ Tech

- Unity Editor scripting  
- Custom Inspector  
- ScriptableObjects  
- Prefab instantiation  
- Grid-based system  
- C#

# 📄 GridMakerPainting
Outil Unity permettant de créer et éditer des maps 3D basées sur des tiles grâce à une interface de peinture 2D directement dans l’Inspector.
> *Utilisable de Unity 2022 à Unity 6.3*

## Table of contents
- [Introduction](#introduction)
- [Installation](#installation)
- [Features](#features)


## <a id="introduction"></a>ℹ️ Introduction

Pour un projet personnel, j'ai développé cet outil afin de permettre à un Level Designer de modifier rapidement l’environnement du joueur.

Le problème était que la map est composée de nombreuses tiles. Modifier manuellement chaque tile dans la scène était lent et fastidieux.

J'ai donc créé un custom inspector permettant de peindre la map en 2D tout en générant automatiquement le résultat en 3D en temps réel (sans problème de perf !).

Cela permet un workflow rapide, visuel et efficace.

Exemple :

<img width="1918" height="1035" alt="Capture d&#39;écran 2026-02-18 154226" src="https://github.com/user-attachments/assets/65b2222b-ceec-4f5d-ba6f-6777e24a523e" />

# <a id="installation"></a>ℹ️ Installation

## <a id="installation"></a> Choix Graphique
Pour le bon fonctionnement du plug-in, il faut selectionner la bonne branche selon votre version de Unity (Ex :  Si vous êtes sur Unity 6.2, prenez After_Unity_6.1)

La seule différence entre les 2 branches est le visuel. Unity 6.1 permet un plus grand panel de couleur par défaut !

Avant Unity 6.1 à gauche et après à droite

<img width="1200" height="680" alt="AvantApres" src="https://github.com/user-attachments/assets/74731177-0662-4c48-bc08-cc1ebd9e3916" />
   
Peux importe la version, l'environement ne change pas :
<img width="945" height="467" alt="Capture d&#39;écran 2026-02-18 153219" src="https://github.com/user-attachments/assets/661310a6-6f1b-456e-87bc-57bf0ca0ea47" />

---

## <a id="GridMapper"></a>ℹ️ GridMapper – Setup Guide

### 1. Créer le parent de la map
- Créez un **GameObject vide** dans la scène
- Créez un Tag nommé `ParentMap`
- Assignez ce Tag au GameObject

Ce GameObject servira de parent pour toutes les tiles 3D générées.


### 2. Créer les assets GridMapper
Dans la fenêtre **Project** :


Créez les assets suivants :

- ToolIcon (Sers à définir les icons des Tools)
- TileDataBase (Sers à définir l'ensemble des Tiles dont vous aller avoir besoin)
- GridMap (Sers à peindre et crée en 3D votre environnement)


### 3. Configurer ToolIcon
Dans **ToolIcon**, assignez les textures des outils :

- Brush
- Rect
- Fill
> *Je vous fourni des images par défaut mais vous pouvez choisir n'importe quel texture 2D*

### 4. Configurer TileDataBase
Dans **TileDataBase**, ajoutez les tiles disponibles pour la map.

Chaque tile doit contenir :

- un Prefab (Sera instancier en tant qu'objet 3D)
- une Icon (Pour voir en 2D la tile sur GridMap)
- un ID (les ID doivent être unique et peuvent aller de 0 à 5 par défaut)


### 5. Configurer GridMap
Dans **GridMap**, assignez :

- ToolIcon
- TileDataBase
  

### 6. Utiliser l’éditeur
Sélectionnez **GridMap** dans le Project.

Les éléments suivants apparaîtront dans l’Inspector :

- les outils (Brush, Rect, Fill)
- les tiles disponibles

> *Si rien n’apparaît, cliquez ailleurs puis re-sélectionnez GridMap.*


### 7. Modifier la taille de la grille
Utilisez les paramètres :

- Row
- Column

pour changer la taille de la map.

💡 Conseil : utiliser deux écrans facilite grandement l’édition.


## <a id="features"></a>Features
- 2D grid painting directly in the Unity Inspector
- Real-time 3D tile generation
- Brush tool
- Rectangle tool
- Flood fill tool
- Tile database system
- Custom icons support
- Automatic visual rebuild
- Works with ScriptableObjects
- Non-destructive grid resizing

## 🛠️ Tech

- Unity Editor scripting
- Custom Inspector
- ScriptableObjects
- Prefab instantiation
- Grid system
- C#

# Anarchy Construct Framework

## Constructs: What Are They?

Constructs are best thought of as a noun inside of a world. I mean like a person, place, thing, or idea. In our use, constructs are used as a method of logically separating and organizing assets into understandable structures. For example, a drawbridge would be in the Drawbridge construct. It would hold everything about the drawbridge including (but not limited to) models, animations, scripts, shaders, materials, textures, audio, and so on.

## How to Work on Constructs

Constructs are made via prefabs that are worked on within scenes. Each construct is worked on in its own scene and loaded dynamically at runtime with addressables. This also works when it comes to multiple people working on a single construct. Every person works on their prefab within the construct, making it easily manageable and extendable.


## Getting Started

1. Download the package into your package manager (https://github.com/RiskyLudus/Anarchy-Construct-Framework.git)
2. Go to Anarchy -> Setup Wizard and set your desired settings.
3. Go to Anarchy -> Create Construct and make your first construct.
4. Wait for the construct to finish creation.
5. Make a setting in the Settings folder of your construct (right-click and you will find your construct under Create -> Anarchy -> Constructs -> Your Construct Data).
6. Add public fields you want to generate and any events you want to generate in the settings.
7. Go to Anarchy -> Update Bindings and let the code auto-generate your Unity Events.

Please submit any issues you find and if you like this, leave a star!

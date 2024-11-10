# Anarchy Construct Framework

## Constructs: What Are They?

Constructs are best thought of as a noun inside of a world. I mean like a person, place, thing, or idea. In our use, constructs are used as a method of logically separating and organizing assets into understandable structures. For example, a drawbridge would be in the Drawbridge construct. It would hold everything about the drawbridge including (but not limited to) models, animations, scripts, shaders, materials, textures, audio, and so on.

## How to Work on Constructs

Constructs are made via prefabs that are worked on within scenes. Each construct is worked on in its own scene and loaded dynamically at runtime with addressables. This also works when it comes to multiple people working on a single construct. Every person works on their prefab within the construct, making it easily manageable and extendable.

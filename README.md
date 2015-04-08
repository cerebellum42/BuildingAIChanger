# BuildingAIChanger

This mod adds a field next to the Asset Property editor that lets you enter any building AI (or rather, any PrefabAI) available to the game.

Currently, you do need to know the exact name of the AI class. If you enter a class that doesn't exist or isn't a PrefabAI, you won't do any harm. The main intended use of this mod is to ease the creation of buildings with custom AI classes.

Existing properties on the current AI instance are carried over to the new AI as far as possible, but this part is still very much a work-in-progress. It's probably a good idea to select the proper AI as early as possible to avoid problems.

## Using custom AI classes

If you intend to use a custom Building AI, make sure to include the namespace of your mod. For example, use MySuperMod.SuperAI. This is not neccessary for the stock AIs as they all seem to be in the root namespace.

## Source code & Issues

The source code of this mod is available at https://github.com/cerebellum42/BuildingAIChanger

If you want to report an issue, please do use the Github issues instead of telling me about problems in the steam workshop comments if possible.

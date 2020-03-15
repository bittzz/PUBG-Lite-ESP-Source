# PUBG-Lite-C#-ESP-Source
## A external ESP base for PUBG Lite 1.4.2.423 written in C#

This does not include code for accessing the games memory or code drawing.

### USAGE
```C#
    Vector2 screenSize = new Vector2(1920, 1080);
    Game pubg = new Game("PUBGLite-Win64-Shipping", "PUBGLite-Win64-Shipping.exe");

    UWorld uWorld = pubg.UWorld();
    APlayerController playerController = uWorld.OwningGameInstance().LocalPlayers().LocalPlayer().PlayerController();

    APlayerCameraManager cameraManager = playerController.PlayerCameraManager();
    AActor localPawn = playerController.AknowledgedPawn();
    AActor[] actors = uWorld.CurrentLevel().AActors();

    for (int i = 0; i < actors.Length; i++)
    {
        if (actors[i].TeamID != localPawn.TeamID && actors[i].Health > 0 && actors[i].ObjectID == localPawn.ObjectID)
        {
            if (cameraManager.WorldToScreen(actors[i].GetBoneLocation(Bones.Head), screenSize, out Vector2 headScreenLocation))
            {
                Vector2 rootScreenLocation = cameraManager.WorldToScreen(actors[i].GetBoneLocation(Bones.Root), screenSize);
                //DrawBox(rootScreenLocation, headScreenLocation, actors[i].Health);
            }
        }

    }
```
### Result
![PUBG Lite ESP](https://i.imgur.com/nyuKqMH.jpg)

### Notes
The code in the usage part is only a demonstation. I would suggest adding some error checks to your code.
I have near to 0 minutes played on the game so it might have a lot of bugs. Consider it a base for you to progress on.

### Credits
The people in uknowncheats.net [PUBG Lite Reversal, Structs and Offsets](https://www.unknowncheats.me/forum/playerunknown-s-battlegrounds/318512-pubg-lite-reversal-structs-offsets.html). 

[master131](https://www.unknowncheats.me/forum/members/217655.html) for the rotate code.

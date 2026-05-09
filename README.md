# Desert Dash: Camel Runner

A 2.5D endless runner mobile game with Egyptian desert theme.

## Project Info

| Field | Value |
|-------|-------|
| Unity Version | 6000.3.10f1 LTS |
| Company | AI Game Factory |
| Package ID | com.aigamefactory.desertdash |
| Target | Android (APK) + iOS (IPA) |
| Min API | Android 24 |
| Target API | Android 33 |
| Architecture | ARMv7 + ARM64 |
| Scripting Backend | IL2CPP |

## Project Structure

```
Assets/
  Scripts/       — Game logic (C# scripts)
  Prefabs/       — Reusable prefabs
  Scenes/
    Main.unity       — Main gameplay scene
    MainMenu.unity   — Main menu scene
  Art/           — 2D/3D art assets
  Audio/         — Music and SFX
  UI/            — UI assets and layouts
```

## Build

Build target: Android. Open in Unity 6000.3.10f1 LTS, then:
- File > Build Settings > Android > Switch Platform
- Player Settings already configured (company, package, architecture)
- Build > Build APK

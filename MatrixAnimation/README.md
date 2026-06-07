# 🟢 Matrix Animation

> A stunning, neon-style Matrix rain screen saver / animation built with **C#**, **Windows Forms**, and **SkiaSharp**.

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)
![C#](https://img.shields.io/badge/C%23-12.0-239120)
![SkiaSharp](https://img.shields.io/badge/SkiaSharp-2.88.8-orange)
![Platform](https://img.shields.io/badge/platform-Windows-blue)

---

## ✨ Features

- 🌧️ **Smooth falling Matrix rain** — authentic mix of katakana, latin, digits and symbols
- 💡 **Neon-style graphics** — white leading head with glow halo and gradient trail
- ⚡ **60 FPS animation** powered by [SkiaSharp](https://github.com/mono/SkiaSharp) (GPU-accelerated 2D)
- ⚙️ **Comprehensive settings menu** with live preview:
  - Fall speed, column spacing, font size, trail length
  - Mutation rate (glitch frequency)
  - Glow intensity
  - Motion blur toggle
  - Click ripple toggle
- 🎨 **6 color themes** — Green, Pink, Blue, Cyan, Amber, Purple (all with neon glow)
- ℹ️ **About menu** with version, description, and credits
- ⛶ **Fullscreen toggle** that hides the menu and borders
- 🖱️ **Click ripple effect** for interactive feedback
- 💾 **Settings persistence** — saved to `%APPDATA%\MatrixAnimation\settings.json`

---

## 📸 Preview

```
╔══════════════════════════════════════╗
║  ｱ ｲ ｳ ｴ ｵ ｶ ｷ ｸ ｹ ｺ ｻ ｼ ｽ ｾ ｿ  ║
║  A 9 7 K 3 X 2 M 8 P 1 5 L 0 Q 4  ║
║  ｱ 0 3 7 2 9 5 8 1 4 6 3 7 0 2 9  ║
║  ｶ ｷ ｸ ｹ ｺ ｻ ｼ ｽ ｾ ｿ ﾀ ﾁ ﾂ ﾃ  ║
╚══════════════════════════════════════╝
```

*(Imagine that, but glowing green on black, with each column falling at its own speed.)*

---

## 🚀 Getting Started

### Prerequisites
- Windows 10 / 11
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (or later)

### Run from source
```bash
git clone <your-repo-url>
cd MatrixAnimation
dotnet run
```

### Build a release binary
```bash
dotnet publish -c Release -r win-x64 --self-contained false
```
The output will be in `bin/Release/net10.0-windows/win-x64/publish/MatrixAnimation.exe`.

---

## ⌨️ Keyboard Shortcuts

| Key | Action |
| --- | --- |
| `F` or `F11` | Toggle fullscreen |
| `Esc` | Exit fullscreen |
| `Ctrl + O` | Open settings dialog |
| `Click` | Trigger ripple effect |
| `Alt + F4` | Quit the app |

---

## 🧩 Project Structure

```
MatrixAnimation/
├── MainForm.cs              # Main window with neon menu + fullscreen logic
├── MatrixControl.cs         # SkiaSharp-backed animation host control
├── Program.cs               # Entry point
├── PLAN.md                  # Development plan & progress log
├── README.md                # ← you are here
├── Models/
│   ├── AnimationSettings.cs # Settings model + JSON persistence
│   └── ColorTheme.cs        # 6 predefined neon themes
├── Rendering/
│   ├── MatrixStream.cs      # Single falling column of glyphs
│   └── MatrixRenderer.cs    # SkiaSharp drawing engine
└── Forms/
    ├── SettingsForm.cs      # Settings dialog with live preview
    ├── PreviewPanel.cs      # Inline SkiaSharp preview
    └── AboutForm.cs         # About dialog
```

---

## ⚙️ How It Works

1. **`MatrixStream`** represents one vertical column of falling characters.
   It owns its own speed, trail length, character buffer, and head position.

2. **`MatrixRenderer`** manages a list of streams and draws one frame to an
   `SKCanvas` on each tick. It uses three `SKPaint` objects:
   - `glowPaint` — large blurred text in the theme glow color
   - `headPaint` — crisp white text for the leading character
   - `charPaint` — colored body text with decreasing alpha for the trail

3. **`MatrixControl`** hosts a `SKControl` inside a WinForms control and drives
   a 16 ms (~60 FPS) redraw timer.

4. **`AnimationSettings`** is serialized as JSON to
   `%APPDATA%\MatrixAnimation\settings.json` so the user's tweaks are remembered
   across runs.

5. **`MainForm`** wires everything together: menu, fullscreen toggle, keyboard
   shortcuts, click ripples, and persistence.

---

## 🎨 Color Themes

| Theme  | Head | Trail  | Glow    |
| ------ | ---- | ------ | ------- |
| Green  | White | `00FF46` | `50FF78` |
| Pink   | White | `FF3CC8` | `FF82DC` |
| Blue   | White | `3C8CFF` | `78B4FF` |
| Cyan   | White | `00E6E6` | `78FFFF` |
| Amber  | White | `FFAA1E` | `FFD264` |
| Purple | White | `AA50FF` | `D296FF` |

---

## 🛠️ Tech Stack

- **C# 12** / **.NET 10.0 Windows Forms**
- **SkiaSharp 2.88.8** + **SkiaSharp.Views.WindowsForms** for high-performance 2D rendering
- Built-in Windows Forms controls for menus, dialogs, and settings UI

---

## 📝 Settings File Location

```
C:\Users\<YourName>\AppData\Roaming\MatrixAnimation\settings.json
```

To reset everything, just delete that file — defaults will be used on the next launch.

---

## 👤 Credits

Made with ❤️ by **Jacky the Code Bender** — part of the [Gravicode Studios](https://studios.gravicode.com) team.

If you enjoy this little project, consider [buying the dev a pulsa](https://studios.gravicode.com/products/budax) ☕📱

---

## 📄 License

This project is provided as-is for educational and personal use. Feel free to fork, learn from, and remix it!

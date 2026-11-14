# FNAF: Into the Pit - DLL Injection Achievement Unlocker (Epic Games)

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
![Python 3.10+](https://img.shields.io/badge/python-3.10+-blue.svg)

I was playing the Epic Games version of *Five Nights at Freddy's: Into the Pit* and trying to get all the achievements. But my game soft-locked, and achievements refused to trigger. There was absolutely no way I was going to replay the entire game from scratch just to get them to pop.

## How this fixes it

Instead of relying on the broken Epic API wrappers, I built a tool that taps directly into the game's internal Unity database.

It works in two parts:

1. **The Bridge (BepInEx C# Mod):** A background listener injected into the game. It monitors a text file (`unlock.txt`) for commands, and routes them natively through the game's own `AchievementManager`.

2. **The Remote (Python GUI):** A simple executable app with buttons. You click a button, it writes to `unlock.txt`, the C# mod reads it instantly, and the game thinks you just earned the achievement.

## How to Install & Use It

I've pre-packaged everything (including the DLL injector base) into a single zip file so you don't have to manually configure anything.

### 1. Installation

1. Download the `FNAF_ITP_Epic_Games_Achievement_Unlocker.zip` from the [Releases](https://github.com/paloanish/FnafITP-DLL-Injection-Achievement-Unlocker/releases) page.

2. Extract the contents directly into your Epic Games installation folder (e.g., `\Epic Games\FiveNightsatFreddysICCsjt\`).

3. When you are done, `FnafITPUnlocker.exe` and the `BepInEx` folder should be sitting right next to the game's main `.exe` file (like `FNaFIntoThePit.exe`).

### 2. Unlock your stuff

1. Launch the game through Epic Games.

2. Alt-tab and double-click `FnafITPUnlocker.exe` in your game folder.

3. Click the achievements you were missing, or just hit **UNLOCK ALL** to get all at once.

4. The in-game pop-ups will appear, and it will sync to your Epic profile immediately.

## For Developers: Building from Source

If you want to compile and modify this yourself, here is the complete step-by-step process:

**Requirements:**

* .NET 6.0+ SDK (for the C# library)

* Python 3.10+

* Run the below command in your terminal to get the required python packages:

   ```bash
   pip install customtkinter pyinstaller
   ```

### Step 1: Install & Initialize BepInEx

1. Download [BepInEx](https://github.com/BepInEx/BepInEx/releases).

2. Extract the contents directly into your Epic Games installation folder.

3. Launch the game normally until you reach the Main Menu, then close it. This forces BepInEx to generate its configuration files and the `plugins` folder.

### Step 2: Compile the C# Mod

**Option A: Git Clone (Recommended)**

1. Clone the repository and navigate into the C# project folder:

   ```bash
   git clone https://github.com/paloanish/FnafITP-DLL-Injection-Achievement-Unlocker.git
   cd FnafITP-DLL-Injection-Achievement-Unlocker/FnafITPUnlocker
   ```

2. Open the `FnafITPUnlocker.csproj` file in a text editor. You will need to update the `<GamePath>` property pointing to your game's installation folder, and update `<Reference>` tags for BepInEx and the Unity DLLs.

3. Build the optimized DLL by running:

   ```bash
   dotnet build -c Release
   ```

**Option B: From Scratch**
If you are building the project entirely from scratch instead of cloning the repository:

1. Open your terminal and initialize a new .NET class library:

   ```bash
   dotnet new classlib -n FnafITPUnlocker
   cd FnafITPUnlocker
   ```

2. Rename the auto-generated `Class1.cs` to `FnafITPUnlocker.cs` and paste the C# mod source code into it.

3. Open the `FnafITPUnlocker.csproj` file in a text editor. You will need to add a `<GamePath>` property pointing to your game's installation folder, and add `<Reference>` tags for BepInEx and the Unity DLLs.

4. Build the optimized DLL by running:

   ```bash
   dotnet build -c Release
   ```

**Finalizing the Mod (Both Options):**

5. Navigate to your build output folder (usually `bin/Release/netstandard2.1/`).

6. Copy the newly generated `FnafITPUnlocker.dll` and paste it into the `\BepInEx\plugins\` folder inside your game directory.

### Step 3: Set Up & Test the Python Remote

Since the Python script relies on `os.getcwd()` to find the game directory and write `unlock.txt`, it must be executed from inside your root game folder.

1. **If you cloned the repo:** Copy `FnafITPUnlocker.py` directly into your Epic Games installation folder.
2. **If building from scratch:** Create a new file named `FnafITPUnlocker.py` inside your Epic Games installation folder and paste the Python GUI source code into it.
3. Launch the game via Epic Games and wait for it to reach the main menu.
4. Open a terminal inside your game installation folder and run the script directly to test it:

   ```bash
   python FnafITPUnlocker.py
   ```

5. Click an achievement in the GUI to verify that the C# mod picks it up and triggers the in-game unlock!

   *Note: If you write `DUMP` and save it as `unlock.txt` in the game's directory, It will display all the available achievements list in the BepInEx console.*

   *To enable it, Open the `` file.*
   *change the `Enabled = false` to `Enabled = true` in the `[Logging.Console]` section.*

### Step 4: Compile into a Standalone Executable (Optional)

If you want to package `FnafITPUnlocker.py` into a portable `.exe` so you don't need Python installed to run it:

1. Open your terminal in the directory containing `FnafITPUnlocker.py`.

2. Package the script into a standalone executable (the `--noconsole` flag keeps the background terminal hidden):

   ```bash
   pyinstaller --noconsole --onefile --name "FnafITPUnlocker" FnafITPUnlocker.py
   ```

3. Copy `FnafITPUnlocker.exe` generated in the `dist` folder and paste it directly into the root folder of your game installation.

4. Double-click the `FnafITPUnlocker.exe` file to run the Python GUI interface.

   *(the game still must be running in the background via Epic Games)*

*Note: It's always a good idea to back up your save file before using mods!*

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
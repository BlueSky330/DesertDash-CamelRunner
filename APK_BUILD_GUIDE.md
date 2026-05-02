# Desert Dash: Camel Runner - APK Build Guide (Windows 11)

This guide will walk you through the process of building the "Desert Dash: Camel Runner" game into an Android Package Kit (APK) file using Unity on your Windows 11 laptop. This APK can then be installed on your Android phone for testing. Follow these steps carefully, even if you don't have a coding background.

## Important Note on Test Ads

During testing, you will see **test ads** appear. These are not real ads and will not generate any revenue. They are placeholders to ensure the ad system is working correctly. Real ads will only appear once the game is published to the Google Play Store.

## STEP 1 - Install Unity Hub

Unity Hub is a tool that helps you manage different Unity Editor versions and your projects.

1.  **Go to the Unity Download Page**: Open your web browser and go to [https://unity.com/download](https://unity.com/download).
2.  **Download Unity Hub**: Look for the button that says "Download Unity Hub" for Windows and click it. Save the installer file to your computer (e.g., in your Downloads folder).
3.  **Install Unity Hub**: Locate the downloaded installer file (e.g., `UnityHubSetup.exe`) and double-click it to start the installation. Follow the on-screen prompts:
    *   Accept the license agreement.
    *   Choose the installation location (default is usually fine).
    *   Click "Install" and then "Finish."
4.  **Create/Sign In to Unity Account**: When Unity Hub opens for the first time, it will ask you to sign in or create a Unity ID. You can:
    *   **Create a Unity ID**: Follow the steps to create a new account.
    *   **Sign in with Google**: If you have a Google account, you can use it to sign in quickly.

## STEP 2 - Install Unity Editor

The Unity Editor is where you will open and build your game project.

1.  **Open Unity Hub**: If it's not already open, launch Unity Hub.
2.  **Go to Installs**: In the left-hand menu of Unity Hub, click on "Installs."
3.  **Install Editor**: Click the "Install Editor" button.
4.  **Choose Unity Version**: Select **Unity 2022.3 LTS (Long Term Support)**. LTS versions are the most stable and recommended for production.
5.  **Select Modules (CRITICAL!)**: This is very important for Android development. In the "Add Modules" window, make sure these checkboxes are ticked:
    *   **Android Build Support**
    *   **Android SDK & NDK Tools**
    *   **OpenJDK**
    *   *(You can uncheck other modules like iOS Build Support if you only plan to build for Android for now, to save space and time.)*
6.  **Install**: Click "Install." This process will download and install the Unity Editor and the selected Android development tools. This can take **15-30 minutes or more** depending on your internet speed and computer performance. Please be patient.

## STEP 3 - Install Git (or GitHub Desktop)

Git is a version control system that helps manage code changes. You'll need it to download the game project from GitHub.

### Option A: Install Git Command Line (Recommended for more control)

1.  **Download Git**: Go to [https://git-scm.com/download/win](https://git-scm.com/download/win).
2.  **Install Git**: Download the installer for Windows and run it. You can generally click "Next" through all the default settings until it's installed.

### Option B: Use GitHub Desktop (Easier for Beginners)

1.  **Download GitHub Desktop**: Go to [https://desktop.github.com](https://desktop.github.com).
2.  **Install GitHub Desktop**: Download and install the application. Follow the on-screen instructions to sign in with your GitHub account (if you have one, otherwise you can create one later).

## STEP 4 - Clone the Repository (Download the Game Project)

Now you will download the game project files from GitHub to your computer.

### Option A: Using Command Prompt / Git Bash

1.  **Open Command Prompt or Git Bash**: Search for "Command Prompt" or "Git Bash" in your Windows search bar and open it.
2.  **Navigate to a Folder**: Use the `cd` command to go to a folder where you want to save your projects. For example, to save it in your user's folder:
    ```bash
    cd C:\Users\YourName
    ```
    *(Replace `YourName` with your actual Windows username.)*
3.  **Clone the Repository**: Type the following command and press Enter:
    ```bash
    git clone https://github.com/BlueSky330/DesertDash-CamelRunner.git
    ```
    This will create a new folder named `DesertDash-CamelRunner` containing all the game files.

### Option B: Using GitHub Desktop

1.  **Open GitHub Desktop**.
2.  Click **File > Clone Repository...**
3.  In the "URL" tab, paste the repository URL: `https://github.com/BlueSky330/DesertDash-CamelRunner.git`
4.  Choose a local path where you want to save the project (e.g., `C:\Users\YourName\Documents\GitHub`).
5.  Click "Clone."

**Note**: Remember the exact location where you saved the `DesertDash-CamelRunner` folder (e.g., `C:\Users\YourName\DesertDash-CamelRunner`). You'll need it in the next step.

## STEP 5 - Open Project in Unity

1.  **Open Unity Hub**.
2.  Click the **"Open"** button (or "Add" if it's the first time).
3.  **Navigate to the Cloned Folder**: Browse to the `DesertDash-CamelRunner` folder you cloned in Step 4.
4.  **Select the Folder**: Click on the `DesertDash-CamelRunner` folder and then click "Select Folder" (or "Open").
5.  **Unity Version Check**: Unity Hub might ask you which Unity Editor version to use. Select the **Unity 2022.3 LTS** version you installed in Step 2.
6.  **Project Upgrade (if prompted)**: If Unity asks to upgrade the project, click "Confirm" or "Continue." This is normal for projects opened with a new Unity version.
7.  **Wait for Import**: Unity will now import all the project assets. This can take several minutes, especially the first time. You will see a progress bar. Once complete, the Unity Editor window will open with your game project loaded.

## STEP 6 - Set Up Android Build Settings

Before building, we need to tell Unity to prepare the game specifically for Android phones.

1.  **Go to Build Settings**: In the Unity Editor, click on **File > Build Settings...**
2.  **Select Android Platform**: In the "Platform" list on the left, click on **Android**.
3.  **Switch Platform**: Click the **"Switch Platform"** button at the bottom right. This will convert your project to the Android platform. This might take a few minutes.
4.  **Open Player Settings**: Click the **"Player Settings..."** button on the bottom left.
5.  **Configure Player Settings (Android Tab)**: In the Inspector window (usually on the right), make sure the Android tab (little Android robot icon) is selected. Update the following:
    *   **Company Name**: Type `SkyVision` (or your preferred company name).
    *   **Product Name**: Type `Desert Dash Camel Runner`.
    *   **Package Name**: Under "Identification," find "Package Name" and ensure it is `com.skyvision.desertdash`. This must be unique for your app on Google Play.
    *   **Minimum API Level**: Under "Configuration," set "Minimum API Level" to `Android 7.0 (API level 24)`.
    *   **Target API Level**: Set "Target API Level" to `Android 13 (API level 33)` or the latest available option.
    *   **Scripting Backend**: Under "Configuration," set "Scripting Backend" to `IL2CPP` (this provides better performance).
    *   **Target Architectures**: Under "Configuration," ensure both **ARM64** and **ARMv7** are checked. This allows your app to run on a wider range of Android devices.

## STEP 7 - Add Scenes to Build

Unity needs to know which parts of your game (scenes) should be included in the final build.

1.  **Return to Build Settings**: Close the Player Settings window and go back to the **File > Build Settings...** window.
2.  **Add Open Scenes**: Click the **"Add Open Scenes"** button. This will add the currently open scene to the build list.
3.  **Verify Scene Order**: Make sure the scenes are listed in this order. If not, drag and drop them to rearrange:
    1.  `Assets/Scenes/MainMenuScene`
    2.  `Assets/Scenes/GameplayScene`
    3.  `Assets/Scenes/GameOverScene`
    *(Note: These are placeholder scenes. In a fully developed game, you would drag the actual Unity scene files from your Project window into this list.)*

## STEP 8 - Build the APK

Now you're ready to create the APK file!

1.  **Build Settings Window**: In the **File > Build Settings...** window.
2.  **Uncheck App Bundle**: Make sure the checkbox for **"Build App Bundle (Google Play)" is UNCHECKED**. We want a standard APK for direct installation and testing.
3.  **Click "Build"**: Click the **"Build"** button at the bottom right.
4.  **Choose Save Location**: A file explorer window will open. Choose a convenient location to save your APK file (e.g., your Desktop).
5.  **Name the APK**: Type `DesertDash.apk` as the filename.
6.  **Save**: Click "Save."
7.  **Wait for Build**: Unity will now compile and build your game. This process can take **5-15 minutes** or longer, especially the first time. You will see a progress bar. Once it's done, you will find the `DesertDash.apk` file in the location you chose.

## STEP 9 - Transfer APK to Your Phone

Once you have the `DesertDash.apk` file, you need to get it onto your Android phone.

### Option A: USB Cable (Recommended)

1.  **Connect Phone**: Connect your Android phone to your laptop using a USB cable.
2.  **File Transfer Mode**: On your phone, you might see a notification asking how to use the USB connection. Select **"File Transfer"** or "MTP."
3.  **Copy APK**: On your laptop, open File Explorer, navigate to where you saved `DesertDash.apk`, and copy it. Then, navigate to your phone's internal storage (usually named "Internal storage" or "Phone storage") and paste the `DesertDash.apk` file into the **"Download"** folder.
4.  **Install on Phone**: Disconnect your phone. On your phone, open a file manager app (e.g., "Files," "My Files," "File Manager"). Navigate to the "Download" folder, find `DesertDash.apk`, and tap it to install. (See Step 10 if you encounter issues).

### Option B: Email or Cloud Storage

1.  **Email**: Attach `DesertDash.apk` to an email and send it to yourself. Open the email on your phone and download the attachment.
2.  **Cloud Storage**: Upload `DesertDash.apk` to a cloud service like Google Drive, Dropbox, or OneDrive from your laptop. Then, open the cloud app on your phone, find the APK, and download it.
3.  **Install on Phone**: Once downloaded, tap the APK file to install it. (See Step 10 if you encounter issues).

### Option C: ADB Command (Advanced)

1.  **Enable USB Debugging**: On your phone, go to **Settings > About phone > Build number** and tap it 7 times to enable Developer Options. Then, go to **Settings > System > Developer options** and enable "USB debugging."
2.  **Connect Phone**: Connect your phone to your laptop via USB.
3.  **Open Command Prompt**: On your laptop, open Command Prompt.
4.  **Navigate to APK Location**: Use `cd` to go to the folder where `DesertDash.apk` is saved.
5.  **Install**: Type `adb install DesertDash.apk` and press Enter. *(ADB is usually installed with the Android SDK that Unity installed in Step 2.)*

## STEP 10 - Enable Unknown Sources (if needed)

Android phones have a security feature that prevents installing apps from outside the Google Play Store. You might need to temporarily disable this.

1.  **Go to Settings**: On your Android phone, open the "Settings" app.
2.  **Search for "Install unknown apps"**: Use the search bar in Settings to find "Install unknown apps" or "Install unknown sources." The exact location can vary by phone model and Android version (it might be under "Security," "Privacy," or "Apps & notifications").
3.  **Allow Installation**: Find the app you used to download/open the APK (e.g., your file manager, Chrome browser, or Gmail app) and toggle the switch to **"Allow from this source"** or "Allow app installs."
4.  **Retry Installation**: Go back and try to install the APK again.

## STEP 11 - Test the Game

Congratulations! Your game should now be installed on your phone.

1.  **Open the Game**: Find the "Desert Dash: Camel Runner" icon on your phone's home screen or app drawer and tap it to launch the game.
2.  **Test Core Gameplay**: Run, jump, and slide. Check if the controls feel responsive.
3.  **Test Collectibles & Scoring**: Collect dates, coins, and gems. See if your score increases.
4.  **Test Watch & Earn Menu**: Go to the "Watch & Earn" menu (if implemented in the UI) and try watching a test ad. Verify that you receive the correct coin reward after the ad completes.
5.  **Test Health System**: Play until your camel's health decreases. See if the health bar updates. Try to find natural recovery items.
6.  **Test Thief Encounters**: Play long enough to encounter a thief. Test escaping and getting caught.
7.  **Check Performance**: Observe if the game runs smoothly at 60 frames per second (FPS) without lag or stuttering, especially on a mid-range Android device.

## Troubleshooting

Here are some common issues and their solutions:

*   **"Gradle build failed"**: This often means there's an issue with the Android build tools. In Unity, go to **Edit > Preferences > External Tools** and ensure the Android SDK, NDK, and OpenJDK paths are correctly set. Sometimes, simply restarting Unity or reinstalling the Android Build Support module (Step 2) can fix it.
*   **"SDK not found"**: This indicates that Unity can't locate the Android SDK. Go to **Edit > Preferences > External Tools** and ensure the paths are correct. If not, reinstall the Android Build Support module in Unity Hub (Step 2).
*   **"APK won't install on phone"**: This is usually due to the "Install from Unknown Sources" security setting. Refer to **Step 10** to enable it.
*   **Game crashes immediately after launch**: This is a more serious issue. In Unity, check the Console window for error messages before building. On your phone, you might need to connect it to your computer and use `adb logcat` in Command Prompt to view crash logs (this is more advanced).
*   **Game runs slowly**: Ensure your Unity project settings for Android are optimized (e.g., texture compression, graphics API). Also, make sure you're testing on a reasonably modern device. Refer to `PerformanceOptimizer.cs` for more details.

If you encounter persistent issues, please provide detailed error messages or screenshots, and I can help you further.

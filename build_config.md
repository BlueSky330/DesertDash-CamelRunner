# Build Configuration for Desert Dash: Camel Runner

This document outlines the necessary build settings, signing requirements, and step-by-step instructions for generating Android and iOS builds of "Desert Dash: Camel Runner" using Unity.

## 1. Android Build Settings

### 1.1 Project Settings (Unity Editor)

1.  **File > Build Settings...**
2.  Select **Android** platform and click **Switch Platform**.
3.  Click **Player Settings...**

### 1.2 Player Settings (Android Tab)

*   **Company Name**: `SkyVision`
*   **Product Name**: `Desert Dash: Camel Runner`
*   **Package Name**: `com.skyvision.desertdash` (Ensure this is unique and consistent across all builds)
*   **Version**: `1.0` (Initial release)
*   **Bundle Version Code**: `1` (Increment for each new build)
*   **Minimum API Level**: `Android 7.0 (API level 24)` (or higher, depending on target device range)
*   **Target API Level**: `Android 13.0 (API level 33)` (or latest recommended by Google Play)
*   **Scripting Backend**: `IL2CPP` (Recommended for performance and security)
*   **API Compatibility Level**: `.NET Standard 2.1`
*   **Target Architectures**: `ARMv7` and `ARM64` (Both are required for Google Play)
*   **Install Location**: `Automatic`
*   **Write Access**: `External (SDCard)` (if saving large user data)
*   **Internet Access**: `Require` (for ads and online features)

### 1.3 Publishing Settings (Android Tab)

#### 1.3.1 Keystore Setup

1.  **Keystore**: Click **Create New** or **Browse** to select an existing keystore.
    *   **Password**: Set a strong password for the keystore.
    *   **Alias**: Create a new key alias (e.g., `desertdashkey`).
    *   **Password**: Set a strong password for the key alias.
    *   **Validity (years)**: Set to `25` or more.
    *   **First and Last Name**: Your name or company name.
    *   **Organizational Unit**: `Game Development`
    *   **Organization**: `SkyVision`
    *   **City or Locality**: `Cairo`
    *   **State or Province**: `Cairo`
    *   **Country Code (XX)**: `EG`
2.  **IMPORTANT**: Securely back up your keystore file (`.keystore`) and its passwords. Losing it means you cannot update your app on Google Play.

### 1.4 Build Steps (Android)

1.  In **File > Build Settings...**, ensure all necessary scenes are added to "Scenes In Build."
2.  Select **Android** platform.
3.  Check **Export Project** if you need to export to an Android Studio project for further modifications.
4.  Check **Build App Bundle (Google Play)** to generate an AAB (Android App Bundle), which is required by Google Play.
5.  Click **Build**.
6.  Choose a destination folder and filename (e.g., `DesertDash_v1.0.aab`).

## 2. iOS Build Settings

### 2.1 Project Settings (Unity Editor)

1.  **File > Build Settings...**
2.  Select **iOS** platform and click **Switch Platform**.
3.  Click **Player Settings...**

### 2.2 Player Settings (iOS Tab)

*   **Company Name**: `SkyVision`
*   **Product Name**: `Desert Dash: Camel Runner`
*   **Bundle Identifier**: `com.skyvision.desertdash` (Must match your Apple Developer Portal App ID)
*   **Version**: `1.0`
*   **Build**: `1` (Increment for each new build)
*   **Target Minimum iOS Version**: `iOS 12.0` (or higher, depending on target device range)
*   **Architecture**: `ARM64`
*   **Scripting Backend**: `IL2CPP`
*   **API Compatibility Level**: `.NET Standard 2.1`
*   **Camera Usage Description**: `"This app uses the camera for augmented reality features (if applicable)."` (Customize as needed)
*   **Microphone Usage Description**: `"This app uses the microphone for voice chat (if applicable)."` (Customize as needed)
*   **Internet Access**: `Require`

### 2.3 Signing Requirements (iOS)

1.  **Automatic Signing**: Recommended for simplicity. Requires Xcode to be configured with your Apple Developer account.
2.  **Manual Signing**: Requires creating and managing `Certificates`, `Identifiers`, and `Provisioning Profiles` in the Apple Developer Portal.
    *   **Team ID**: Your Apple Developer Team ID.
    *   **Provisioning Profile**: Select the appropriate provisioning profile for development or distribution.

### 2.4 Build Steps (iOS)

1.  In **File > Build Settings...**, ensure all necessary scenes are added to "Scenes In Build."
2.  Select **iOS** platform.
3.  Click **Build**.
4.  Choose a destination folder (e.g., `iOS_Build`). Unity will generate an Xcode project.
5.  Open the generated Xcode project in Xcode.
6.  In Xcode, configure signing and team settings.
7.  Connect an iOS device or select a simulator.
8.  Build and run the app on the device/simulator.
9.  For App Store submission, archive the build in Xcode and upload it via App Store Connect.

## 3. General Build Considerations

*   **Ad SDK Integration**: Ensure all Ad SDKs (AdMob, Unity Ads, ironSource) are correctly integrated and initialized before building.
*   **Content Filtering**: Verify that ad content filtering settings are correctly applied in the respective ad network dashboards.
*   **Privacy Policy**: Ensure your privacy policy is accessible and compliant with platform requirements.
*   **Testing**: Always build and test on actual devices for both Android and iOS to catch platform-specific issues.
*   **Asset Bundles**: Consider using Unity Asset Bundles for dynamic content loading and reducing initial app size, especially for country-specific assets.

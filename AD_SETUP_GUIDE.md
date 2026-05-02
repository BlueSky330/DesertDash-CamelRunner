# Ad SDK Integration Setup Guide

This guide provides instructions for setting up Google AdMob and Unity Ads SDKs for "Desert Dash: Camel Runner." It covers account creation, obtaining ad unit IDs, testing, and revenue setup.

## 1. Google AdMob Setup

Google AdMob is the primary ad network for this game. Follow these steps to integrate it:

### 1.1 Create an AdMob Account

1.  Go to the [Google AdMob website](https://admob.google.com/) and sign in with your Google account.
2.  Follow the on-screen instructions to create a new AdMob account if you don't have one.
3.  Add your app to AdMob. Select "Android" as the platform.

### 1.2 Obtain Ad Unit IDs

1.  In your AdMob account, navigate to "Apps" and select your game.
2.  Go to "Ad units" and click "Add ad unit."
3.  Select "Rewarded" as the ad format.
4.  Configure the ad unit settings (e.g., reward amount, reward item).
5.  Save the ad unit. You will receive an **App ID** (e.g., `ca-app-pub-xxxxxxxxxxxxxxxx~yyyyyyyyyy`) and a **Rewarded Ad Unit ID** (e.g., `ca-app-pub-xxxxxxxxxxxxxxxx/zzzzzzzzzz`).

### 1.3 Replace Placeholder IDs

1.  Open the `Assets/ad_config.json` file in your Unity project.
2.  Replace the placeholder `appId` and `rewardedAdUnitId` values under the `"adMob"` section with your actual AdMob IDs.

### 1.4 Test AdMob Integration

1.  AdMob provides [test ad unit IDs](https://developers.google.com/admob/android/test-ads) for development. It's highly recommended to use these during testing to avoid invalid ad requests.
2.  Ensure your device is registered as a test device in AdMob settings.
3.  Run the game on your Android device and verify that rewarded ads load and display correctly. Check the Unity console for AdMob logs.

### 1.5 Set Up Payment

1.  In your AdMob account, go to "Payments" to set up your payment method and tax information.
2.  Ensure your account is verified to receive revenue from your ads.

## 2. Unity Ads Setup (Backup)

Unity Ads serves as a backup ad network. Follow these steps to integrate it:

### 2.1 Create a Unity ID and Project

1.  Go to the [Unity Dashboard](https://dashboard.unity3d.com/) and sign in with your Unity ID.
2.  Create a new project or link your existing Unity project.

### 2.2 Enable Unity Ads

1.  In your Unity project dashboard, navigate to "Monetization" > "Ad Networks."
2.  Enable Unity Ads for your project.
3.  Note down your **Game ID** for Android and iOS.

### 2.3 Obtain Ad Unit IDs

1.  In the Unity Dashboard, go to "Monetization" > "Ad Units."
2.  Create a new "Rewarded" ad unit for Android and iOS. Note down their **Placement IDs** (e.g., `Rewarded_Android`, `Rewarded_iOS`).

### 2.4 Replace Placeholder IDs

1.  Open the `Assets/ad_config.json` file in your Unity project.
2.  Replace the placeholder `gameIdAndroid`, `gameIdIOS`, `rewardedAdUnitIdAndroid`, and `rewardedAdUnitIdIOS` values under the `"unityAds"` section with your actual Unity Ads IDs.

### 2.5 Test Unity Ads Integration

1.  Unity Ads provides a test mode. Enable it in your Unity project settings or through code.
2.  Run the game on your device and verify that Unity Ads rewarded ads load and display correctly.

## 3. Ad Mediation Setup (ironSource/LevelPlay)

Ad mediation platforms like ironSource (LevelPlay) help optimize ad revenue by automatically selecting the highest-paying ad network. This setup is crucial for maximizing earnings.

### 3.1 Create an ironSource Account

1.  Go to the [ironSource website](https://www.is.com/) and create an account.
2.  Add your app to the ironSource platform.

### 3.2 Integrate Ad Networks

1.  In your ironSource dashboard, navigate to "Monetization" > "SDK Networks."
2.  Add Google AdMob and Unity Ads as networks. You will need to provide your AdMob and Unity Ads credentials (e.g., AdMob App ID, Unity Ads Game ID).
3.  Configure the ad units for each network within ironSource.

### 3.3 Integrate ironSource SDK in Unity

1.  Download the ironSource Unity Plugin from their documentation.
2.  Import the plugin into your Unity project.
3.  Initialize the ironSource SDK in your `AdMediation.cs` script using your ironSource App Key.

### 3.4 Configure Ad Waterfall

1.  In the ironSource dashboard, configure the ad waterfall for rewarded video ads. This determines the order in which ad networks are called.
2.  Ensure AdMob is prioritized, followed by Unity Ads, and then other networks you might integrate.

## Important Considerations

*   **Reward on 100% Completion**: Ensure that rewards are granted ONLY when the player watches the ad to 100% completion. Skipping or closing an ad early should result in no reward.
*   **Ad Frequency**: Adjust `minTimeBetweenAdsSeconds` and `maxAdsPerHour` in `ad_config.json` to balance user experience and revenue. Avoid over-saturating players with ads.
*   **User Experience**: The "Player-Driven Rewarded Ads" model is designed to be non-intrusive. Always prioritize player choice and a positive experience.

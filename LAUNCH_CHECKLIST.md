# Launch Checklist for Desert Dash: Camel Runner

This checklist outlines the essential steps required before launching "Desert Dash: Camel Runner" on Google Play Store and Apple App Store.

## I. Account & Setup

*   [ ] **Google Play Developer Account**: Created and paid ($25 one-time fee).
*   [ ] **Apple Developer Program Account**: Created and paid ($99/year).
*   [ ] **AdMob Account**: Created and fully verified.
*   [ ] **Unity Ads Account**: Created and linked to Unity project.
*   [ ] **ironSource/LevelPlay Account**: Created and configured for mediation.
*   [ ] **Ad Unit IDs**: Generated for all ad types (rewarded video) in AdMob and Unity Ads.
*   [ ] **`ad_config.json`**: All placeholder ad unit IDs replaced with actual IDs.
*   [ ] **Content Filtering**: Configured in AdMob dashboard (blocking adult categories, gambling, alcohol, dating, inappropriate ads; max ad content rating set to G/PG).
*   [ ] **Privacy Policy**: Created, hosted on a public URL, and linked in store listings.

## II. Assets & Content

*   [ ] **App Icon**: Finalized (high-resolution, various sizes for both platforms).
*   [ ] **Store Screenshots**: 5-8 high-quality screenshots captured, showcasing key gameplay and features.
*   [ ] **Feature Graphic/Banner**: Created for Google Play.
*   [ ] **App Preview Video**: (Optional, but recommended) Created for both stores.
*   [ ] **Store Listing Text**: Finalized for Google Play (`store_listing.md`) and Apple App Store (`apple_store_listing.md`).
*   [ ] **Content Rating Questionnaire**: Completed accurately for both Google Play and Apple App Store.

## III. Technical & Testing

*   [ ] **Game Build**: Final APK/AAB (Android) and Xcode project (iOS) generated.
*   [ ] **Device Testing**: Game thoroughly tested on various Android and iOS devices (different screen sizes, OS versions) for functionality, performance, and stability.
*   [ ] **Ad Testing**: Test ads verified working correctly (reward granted on 100% completion, no reward on skip/early close).
*   [ ] **Economy Balance**: Verified through `EconomyBalanceTester.cs` and manual playtesting.
*   [ ] **Performance**: Verified through `PerformanceTester.cs` and manual playtesting (target 60 FPS, acceptable memory/battery usage).
*   [ ] **Bug Fixing**: All critical and major bugs identified and fixed.
*   [ ] **Localization**: (If applicable) Game text translated for target markets.

## IV. Publishing

*   [ ] **Google Play Console**: 
    *   [ ] App uploaded to internal testing track.
    *   [ ] Internal testing completed.
    *   [ ] App uploaded to closed/open testing track (optional, for beta testing).
    *   [ ] Beta testing feedback addressed.
    *   [ ] App submitted to production track.
*   [ ] **App Store Connect**: 
    *   [ ] Xcode archive uploaded.
    *   [ ] TestFlight testing completed (optional, for beta testing).
    *   [ ] App submitted for App Store Review.
*   [ ] **App Review Passed**: Approval received from both Google Play and Apple App Store.
*   [ ] **LIVE!**: Game successfully launched on both platforms.

# Ad ID Replacement Guide

## Overview

Before submitting Camel Runner to Google Play and App Store, all test ad IDs in `Assets/ad_config.json` must be replaced with production IDs. This guide documents which IDs are currently test placeholders and the process to obtain and configure production IDs.

## Current Test IDs (in ad_config.json)

### AdMob

| Field | Current Test Value | Status |
|-------|-------------------|--------|
| `adMob.appId_android` | `ca-app-pub-3940256099942544~3347511713` | **TEST** (Google's test app ID) |
| `adMob.appId_ios` | `ca-app-pub-3940256099942544~1458002511` | **TEST** (Google's test app ID) |
| `adMob.rewardedAdUnitId_android` | `ca-app-pub-3940256099942544/5224354917` | **TEST** (Google's test ad unit) |
| `adMob.rewardedAdUnitId_ios` | `ca-app-pub-3940256099942544/1712485313` | **TEST** (Google's test ad unit) |

**Recognition**: All AdMob IDs starting with `ca-app-pub-3940256099942544` are Google's official test IDs. These will NOT generate revenue and can be detected by Google Play reviewers.

### Unity Ads

| Field | Current Test Value | Status |
|-------|-------------------|--------|
| `unityAds.gameId_android` | `1234567` | **TEST** (placeholder) |
| `unityAds.gameId_ios` | `7654321` | **TEST** (placeholder) |
| `unityAds.rewardedAdUnitId_android` | `Rewarded_Android` | **VALID** (can be used in production) |
| `unityAds.rewardedAdUnitId_ios` | `Rewarded_iOS` | **VALID** (can be used in production) |

### ironSource / LevelPlay

| Field | Current Test Value | Status |
|-------|-------------------|--------|
| `ironSource.appKey` | `YOUR_LEVPLAY_APP_KEY` | **PLACEHOLDER** (must be obtained) |

---

## Step-by-Step: Obtaining Production IDs

### 1. AdMob Production IDs

**Timeline**: 1–2 days (automatic after account approval)

**Steps**:
1. Sign in to [Google AdMob Dashboard](https://admob.google.com/)
2. Click **Apps** → **Add App**
3. Select your platform (Android or iOS) and complete the app registration
4. After approval, navigate to **App Settings** to find:
   - **App ID** (e.g., `ca-app-pub-xxxxxxxxxxxxxxxx~yyyyyyyyyy`)
5. Create new Ad Units:
   - Click **Ad Units** → **Create a new ad unit**
   - Name: `Rewarded_Android` or `Rewarded_iOS`
   - Format: **Rewarded**
   - Copy the generated **Ad Unit ID** (e.g., `ca-app-pub-xxxxxxxxxxxxxxxx/zzzzzzzzzz`)
6. Note the **Content rating**: Ensure G rating is applied in AdMob dashboard (Dashboard → Settings → App settings → Content rating for your app)

**Output**: 
- `adMob.appId_android` = Your Android App ID
- `adMob.appId_ios` = Your iOS App ID
- `adMob.rewardedAdUnitId_android` = Your Android Rewarded Ad Unit ID
- `adMob.rewardedAdUnitId_ios` = Your iOS Rewarded Ad Unit ID

---

### 2. Unity Ads Production IDs

**Timeline**: 1 day (instant after project creation)

**Steps**:
1. Sign in to [Unity Dashboard](https://dashboard.unity3d.com/)
2. Navigate to **Monetization** → **Games**
3. Click **Create new game** (or select existing if already created)
4. Select Platform:
   - Create separate projects or entries for Android and iOS
5. Copy the **Game ID** for each platform:
   - Android Game ID
   - iOS Game ID
6. Under **Ad Units**, verify or create:
   - `Rewarded_Android` (can keep generic name)
   - `Rewarded_iOS` (can keep generic name)
7. Set **Content Rating**: Navigate to Project settings → **Content Rating** → Select "Everyone/Family"

**Output**:
- `unityAds.gameId_android` = Your Android Game ID
- `unityAds.gameId_ios` = Your iOS Game ID
- (Ad unit IDs can remain as `Rewarded_Android` and `Rewarded_iOS`)

---

### 3. ironSource / LevelPlay Production Key

**Timeline**: 1–2 days (requires account verification)

**Steps**:
1. Sign in to [LevelPlay Dashboard](https://platform.ironsrc.com/)
2. Navigate to **Setup** → **Apps**
3. Click **Add App**
4. Select your app platform and register
5. After registration, copy your **App Key** from the App settings page
6. Configure the **Mediation Waterfall** in LevelPlay:
   - Add AdMob as a demand partner (requires AdMob App ID)
   - Add Unity Ads as a demand partner (requires Unity Game ID)
   - Set priority levels (typical: ironSource primary, AdMob secondary, Unity tertiary)
7. Enable **Brand Safety**:
   - Navigate to **Ad Quality** → **Blocklists**
   - Block: Gambling, Alcohol, Dating, Adult

**Output**:
- `ironSource.appKey` = Your LevelPlay App Key

---

## Step-by-Step: Replacing IDs in Code

### Before Submission

**File**: `Assets/ad_config.json`

1. **Backup the current file** (for reference):
   ```bash
   cp Assets/ad_config.json Assets/ad_config.json.backup
   ```

2. **Update each section** with production IDs (do NOT commit this file if it contains real IDs to a public repo):
   ```json
   {
     "adMob": {
       "appId_android": "[YOUR_ADMOB_ANDROID_APP_ID]",
       "appId_ios": "[YOUR_ADMOB_IOS_APP_ID]",
       "rewardedAdUnitId_android": "[YOUR_ADMOB_ANDROID_AD_UNIT_ID]",
       "rewardedAdUnitId_ios": "[YOUR_ADMOB_IOS_AD_UNIT_ID]",
       ...
     },
     "unityAds": {
       "gameId_android": "[YOUR_UNITY_ANDROID_GAME_ID]",
       "gameId_ios": "[YOUR_UNITY_IOS_GAME_ID]",
       ...
     },
     "ironSource": {
       "appKey": "[YOUR_LEVELPLAY_APP_KEY]",
       ...
     }
   }
   ```

3. **Verify the config** by checking that:
   - No test IDs remain (no `ca-app-pub-3940256099942544`)
   - No placeholders remain (no `YOUR_LEVPLAY_APP_KEY`)
   - All required fields are populated

4. **Test with builds**:
   - Build Android and iOS versions with production IDs
   - Run on physical devices to confirm ad loading (check Logcat/Console for ad load errors)
   - Do NOT submit without testing on real devices

5. **Security note**: Production IDs are sensitive. After submission:
   - Add `ad_config.json` to `.gitignore` if it contains real production keys
   - Store production IDs in environment variables or secure build configuration
   - Do NOT commit real ad IDs to version control

---

## Verification Checklist

Before submitting to app stores, confirm:

- [ ] All AdMob test IDs (`ca-app-pub-3940256099942544`) have been replaced
- [ ] All Unity Ads test IDs (`1234567`, `7654321`) have been replaced
- [ ] ironSource App Key is populated (not `YOUR_LEVPLAY_APP_KEY`)
- [ ] Ad configuration has been tested on Android device
- [ ] Ad configuration has been tested on iOS device
- [ ] Ads load and display correctly (check console for errors)
- [ ] Backup of original config exists
- [ ] Production config is NOT committed to public repo

---

## Ad Network Dashboard Links

| Network | Dashboard | Notes |
|---------|-----------|-------|
| Google AdMob | https://admob.google.com/ | Requires Google account; approvals usually take 1–2 days |
| Unity Ads | https://dashboard.unity3d.com/ | Access via Unity account; Game IDs instant |
| ironSource LevelPlay | https://platform.ironsrc.com/ | Requires account verification; app setup 1–2 days |

---

## Support & Troubleshooting

### AdMob IDs Not Loading Ads
- Verify appId matches the bundle ID in your app manifest
- Check AdMob dashboard for app approval status
- Confirm rewarded ad unit ID is marked as "Approved"
- Test on actual device (emulators may not receive ads)

### Unity Ads Not Showing
- Confirm Game IDs match your Unity project setup
- Check Unity Dashboard → Monetization → Placements for status
- Verify iOS and Android Game IDs are separate if using different IDs

### ironSource Integration Issues
- Confirm LevelPlay App Key is correct
- Verify AdMob and Unity Ads adapters are added as demand partners
- Check Waterfall configuration priority in LevelPlay dashboard
- Allow 24–48 hours for demand partner integration to activate

---

## Timeline Summary

| Ad Network | Typical Turnaround | Action Owner |
|------------|-------------------|--------------|
| AdMob | 1–2 days (app approval) | Publish → Get Production IDs |
| Unity Ads | Instant (same day) | Create Game → Get Production IDs |
| ironSource | 1–2 days (account setup) | Register → Get App Key |

**Recommendation**: Start ad network registration immediately after AIG-227 delivery to avoid delays before app store submission.


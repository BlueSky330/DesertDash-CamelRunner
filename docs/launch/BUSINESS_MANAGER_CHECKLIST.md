# Ad Network Registration — Business Manager Checklist

**Issue**: [AIG-235](/AIG/issues/AIG-235)  
**Owner**: Business Manager  
**Timeline**: 3–5 business days (parallel registration)  
**Deliverable**: Production ad IDs documented in [PRODUCTION_AD_IDS.md](./PRODUCTION_AD_IDS.md)

---

## Pre-Registration Requirements

Confirm you have access to or can obtain:

- [ ] Company email address (for account creation)
- [ ] Payment method (credit card) if required for account verification
- [ ] Company name & address (for KYC if requested)
- [ ] Corporate tax ID / VAT number (if outside US)
- [ ] Bank account details (for payout setup, optional but recommended)

---

## 1️⃣ Google AdMob Registration

**Dashboard**: https://admob.google.com/  
**Timeline**: 1–2 days (includes approval wait)

### Registration Steps

- [ ] Go to https://admob.google.com/ and sign in with company Google account
- [ ] Click **Apps** → **Add App**
- [ ] Select **Android** first (can add iOS later)
- [ ] Enter app details:
  - **App name**: "Desert Dash Camel Runner"
  - **App store**: Google Play (or select appropriate store)
  - **Package name**: Use the Android bundle ID from the development team
    - Check with Dev Lead: usually `com.aigamefactory.cameltunnelrunner` or similar
- [ ] Accept Google's policies
- [ ] **Wait for approval** (typically 24–48 hours; may receive email confirmation)
- [ ] Once approved, navigate to **App Settings**:
  - [ ] Copy **AdMob App ID** (format: `ca-app-pub-xxxxxxxxxxxxxxxx~yyyyyyyyyy`)
  - [ ] Set **Content rating** to **G** (Dashboard → Settings → App settings → Content rating)
- [ ] Create **Rewarded Ad Unit**:
  - [ ] Click **Ad Units** → **Create a new ad unit**
  - [ ] Name: `Rewarded_Android`
  - [ ] Format: **Rewarded**
  - [ ] Copy **Ad Unit ID** (format: `ca-app-pub-xxxxxxxxxxxxxxxx/zzzzzzzzzz`)
- [ ] Repeat for **iOS**:
  - [ ] Click **Apps** → **Add App** (or use same app if multi-platform)
  - [ ] Select **iOS**
  - [ ] Enter app details (Bundle ID: usually `com.aigamefactory.cameltunnelrunner` or similar)
  - [ ] Wait for approval and copy **AdMob App ID**
  - [ ] Create **Rewarded Ad Unit** for iOS, copy **Ad Unit ID**
- [ ] Set up **Content Filters** (in all created ad units):
  - [ ] Click each ad unit → **Blocking controls** or **Sensitive ad categories**
  - [ ] Block: **Gambling**, **Alcohol**, **Dating**, **Adult**, **Inappropriate**

### IDs to Document

Once complete, provide these to Monetization Manager for [PRODUCTION_AD_IDS.md](./PRODUCTION_AD_IDS.md):

```
adMob.appId_android: [COPY FROM DASHBOARD]
adMob.appId_ios: [COPY FROM DASHBOARD]
adMob.rewardedAdUnitId_android: [COPY FROM DASHBOARD]
adMob.rewardedAdUnitId_ios: [COPY FROM DASHBOARD]
```

---

## 2️⃣ Unity Ads Registration

**Dashboard**: https://dashboard.unity3d.com/  
**Timeline**: Same day (instant Game ID generation)

### Registration Steps

- [ ] Go to https://dashboard.unity3d.com/ and sign in with company Unity account
  - If no account exists, create one at https://id.unity.com/
- [ ] Navigate to **Monetization** → **Games** (or **Operate** → **Monetization**)
- [ ] Click **Create new game** (or select existing if already created)
- [ ] Enter game details:
  - **Game name**: "Desert Dash Camel Runner"
  - **Platforms**: Select **Android** first
  - [ ] Copy **Game ID** for Android (long numeric or UUID)
- [ ] Add **iOS**:
  - [ ] Click **Create new game** or add platform to existing game
  - [ ] Select **iOS**
  - [ ] Copy **Game ID** for iOS
- [ ] Configure **Ad Units** (for each platform):
  - [ ] Click **Ad Units** or **Placements**
  - [ ] Verify or create:
    - `Rewarded_Android` (Rewarded format)
    - `Rewarded_iOS` (Rewarded format)
  - [ ] Both can have generic names; the important part is the Game ID
- [ ] Set **Content Rating**:
  - [ ] Navigate to **Project settings** → **Content Rating**
  - [ ] Select **"Everyone"** or **"Family"**
  - [ ] Save

### IDs to Document

Once complete, provide these to Monetization Manager for [PRODUCTION_AD_IDS.md](./PRODUCTION_AD_IDS.md):

```
unityAds.gameId_android: [COPY FROM DASHBOARD]
unityAds.gameId_ios: [COPY FROM DASHBOARD]
unityAds.rewardedAdUnitId_android: Rewarded_Android
unityAds.rewardedAdUnitId_ios: Rewarded_iOS
```

---

## 3️⃣ ironSource / LevelPlay Registration

**Dashboard**: https://platform.ironsrc.com/  
**Timeline**: 1–2 days (account activation required)

### Registration Steps

- [ ] Go to https://platform.ironsrc.com/ and sign up with company email
- [ ] Complete account setup:
  - [ ] Enter company info
  - [ ] Verify email (check inbox and spam)
  - [ ] Phone verification may be requested (provide company phone)
- [ ] Once account is active, navigate to **Setup** → **Apps**
- [ ] Click **Add App**:
  - [ ] **App name**: "Desert Dash Camel Runner"
  - [ ] **Platform**: Android first
  - [ ] **Bundle ID**: Use Android package name (ask Dev Lead)
  - [ ] **App store**: Google Play
  - [ ] Complete registration and wait for approval (usually same day)
  - [ ] Copy **App Key** (save this; you'll need it for all platforms)
- [ ] Repeat for **iOS**:
  - [ ] Click **Add App**
  - [ ] **App name**: "Desert Dash Camel Runner"
  - [ ] **Platform**: iOS
  - [ ] **Bundle ID**: Use iOS bundle ID (ask Dev Lead)
  - [ ] **App store**: App Store
  - [ ] Complete registration
  - [ ] Same App Key applies to both platforms
- [ ] **Configure Mediation Waterfall**:
  - [ ] In LevelPlay, navigate to **Setup** → **[Your App]** → **Ad Sources** or **Mediation**
  - [ ] **Add Demand Partners**:
    - [ ] Click **Add** or **Configure**
    - [ ] Select **Google AdMob** (or search for it)
    - [ ] Enter AdMob App ID (from step 1️⃣)
    - [ ] Priority: **1** (primary)
    - [ ] Select **Unity Ads**
    - [ ] Enter Unity Game ID (from step 2️⃣)
    - [ ] Priority: **2** (secondary)
    - [ ] Select **ironSource** (default/internal)
    - [ ] Priority: **3** or leave as-is
  - [ ] **Save** waterfall configuration
- [ ] **Enable Brand Safety**:
  - [ ] Navigate to **Ad Quality** → **Blocklists** or **Content Controls**
  - [ ] Block these categories:
    - [ ] Gambling
    - [ ] Alcohol
    - [ ] Dating
    - [ ] Adult
  - [ ] Save

### IDs to Document

Once complete, provide this to Monetization Manager for [PRODUCTION_AD_IDS.md](./PRODUCTION_AD_IDS.md):

```
ironSource.appKey: [COPY FROM DASHBOARD]
```

---

## 📋 Final Checklist

Before handing off to Development:

- [ ] All 3 ad network accounts created
- [ ] Content filters configured on all networks
- [ ] Production IDs collected and documented in [PRODUCTION_AD_IDS.md](./PRODUCTION_AD_IDS.md)
- [ ] Email screenshot or document of all registered apps (for audit trail)
- [ ] Monetization Manager notified that IDs are ready
- [ ] Developer can now update `Assets/ad_config.json` with production IDs

---

## Support & Troubleshooting

**AdMob App Not Approved?**
- Check email for rejection reason
- Verify app name, package name, and other details match Play Store listing
- Resubmit if needed
- Contact: https://support.google.com/admob/

**Unity Ads Game ID Missing?**
- Ensure you're in the Monetization section (not Analytics)
- Game IDs appear after creating the game; refresh the page if not visible

**ironSource Account Locked?**
- May require phone or email verification
- Check spam folder for verification emails
- Contact: https://www.ironsrc.com/contact

**Bundle ID Mismatch?**
- Confirm with Dev Lead on exact Android package name and iOS bundle ID
- These must match exactly in all ad network registrations and in the app's manifests

---

## Success Criteria

✅ All credentials handed off to Monetization Manager  
✅ [PRODUCTION_AD_IDS.md](./PRODUCTION_AD_IDS.md) fully populated  
✅ Content filters active on all networks  
✅ Ready for [AIG-235](/AIG/issues/AIG-235) to proceed to config update


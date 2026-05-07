# Production Ad IDs — Camel Runner Desert Dash

## Status: PENDING

All production ad networks must be registered and IDs obtained before app store submission.

---

## AdMob (Google)

**Dashboard**: https://admob.google.com/  
**Account Status**: 🔴 PENDING REGISTRATION

| Field | Production ID | Status |
|-------|---------------|--------|
| App ID (Android) | `[AWAITING]` | ⏳ |
| App ID (iOS) | `[AWAITING]` | ⏳ |
| Rewarded Ad Unit ID (Android) | `[AWAITING]` | ⏳ |
| Rewarded Ad Unit ID (iOS) | `[AWAITING]` | ⏳ |
| Content Rating | G | ✅ (configured) |

**Timeline**: 1–2 days after registration  
**Owner**: Business Manager  
**Related Issue**: [AIG-235-AdMob](/AIG/issues/AIG-235-AdMob) (when created)

---

## Unity Ads

**Dashboard**: https://dashboard.unity3d.com/  
**Account Status**: 🔴 PENDING REGISTRATION

| Field | Production ID | Status |
|-------|---------------|--------|
| Game ID (Android) | `[AWAITING]` | ⏳ |
| Game ID (iOS) | `[AWAITING]` | ⏳ |
| Rewarded Ad Unit ID (Android) | `Rewarded_Android` | ✅ (ready) |
| Rewarded Ad Unit ID (iOS) | `Rewarded_iOS` | ✅ (ready) |
| Content Rating | Everyone/Family | ✅ (to configure) |

**Timeline**: Instant (same day)  
**Owner**: Business Manager  
**Related Issue**: [AIG-235-UnityAds](/AIG/issues/AIG-235-UnityAds) (when created)

---

## ironSource / LevelPlay

**Dashboard**: https://platform.ironsrc.com/  
**Account Status**: 🔴 PENDING REGISTRATION

| Field | Production ID | Status |
|-------|---------------|--------|
| App Key | `[AWAITING]` | ⏳ |
| Mediation Waterfall | See config | ⏳ |
| Brand Safety Filters | Gambling, Alcohol, Dating, Adult | ⏳ |

**Timeline**: 1–2 days after registration  
**Owner**: Business Manager  
**Related Issue**: [AIG-235-ironSource](/AIG/issues/AIG-235-ironSource) (when created)  
**Dependencies**: AdMob and Unity Ads production IDs must be ready first

---

## Update Process

Once all IDs are collected:

1. **Monetization Manager** updates this document with production IDs
2. **Developer** replaces IDs in `Assets/ad_config.json` using [AD_ID_REPLACEMENT_GUIDE.md](./AD_ID_REPLACEMENT_GUIDE.md)
3. **QA** tests ads on physical devices (Android & iOS)
4. **Deploy** to app stores

---

## Credentials & Setup Notes

**AdMob**:
- Google account required (can use company Gmail)
- Approval timing: typically 24–48 hours
- Bundle ID must match `AndroidManifest.xml` and iOS `CFBundleIdentifier`

**Unity Ads**:
- Unity account required (existing account or new account with company email)
- Instant approval; Game IDs available immediately after project creation
- One Game ID per platform (Android & iOS)

**ironSource**:
- Email and company info required
- Phone verification may be requested
- Account activation: 1–2 days
- Requires AdMob App ID and Unity Game ID to set up mediation partners

---

## Acceptance Criteria (Main Issue)

- [ ] AdMob account created; production IDs obtained → documented here
- [ ] Unity Ads account created; production IDs obtained → documented here
- [ ] ironSource account created; App Key obtained → documented here
- [ ] All IDs in this document (PRODUCTION_AD_IDS.md)
- [ ] Content filters configured on all networks
- [ ] Ready for developer to update `Assets/ad_config.json`


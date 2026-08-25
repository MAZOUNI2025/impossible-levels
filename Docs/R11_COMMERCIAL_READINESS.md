# IMPOSSIBLE LEVELS — R11 Commercial Readiness

## Decision

The core loop is intentionally independent of monetization. The project now bootstraps an `OfflineMonetizationGateway` that **fails closed**: it reports that no provider is configured, reports no rewarded ad as ready, never grants a hint/continue/multiplier without a verified callback, and never marks Remove Ads as purchased from a local flag.

This is safer than shipping a fake ad or purchase flow. The game remains playable without a network connection, an ad SDK, a billing account, or a consent message.

## Production options to configure before release

| Surface | Required production implementation | Current repository state | Release evidence required |
| --- | --- | --- | --- |
| Rewarded ads | A verified mobile ads adapter implementing `IMonetizationGateway.ShowRewarded`, with test ad units during QA | Offline fallback only; no ads SDK in `Packages/manifest.json` | Provider test IDs, callback/reward tests, offline/error tests, and a real Android build |
| Remove Ads | Google Play Billing one-time product adapter implementing `PurchaseRemoveAds`, purchase acknowledgement, restore/query, and entitlement persistence | Not enabled; no billing package or product ID | Play Console test product, purchase/restore/acknowledgement evidence, and signed Android test build |
| Consent | Official provider consent/CMP flow before personalized ads where required | Not enabled; no custom substitute is shipped | Published consent configuration, regional test evidence, and updated Data safety declarations |
| Analytics | Privacy-reviewed, consent-aware event adapter with no gameplay dependency | No analytics SDK or event collection | Event inventory, data-retention decision, opt-out behavior, and Data safety declaration |
| Privacy | Public privacy-policy URL and accurate Play Console declarations matching the final SDK/data set | URL not supplied and no data-collection SDK enabled | Legal/privacy review, accessible URL, and final Play Console declarations |

## Core-loop guardrails

A rewarded ad may only grant a reward after the provider reports a completed, valid reward callback. A failed, skipped, unavailable, or offline ad must leave the player's progression, stars, hint count, and coins unchanged. Remove Ads must be an entitlement from verified Play Billing state; a button press or local preference alone is not proof of purchase.

No ad should block the first-time tutorial, interrupt an active puzzle, or be required to unlock the 30 core levels. Any future ad placement must be opt-in for rewarded ads or occur only at a deliberate non-gameplay transition. Purchase and consent screens must be localized in English and Arabic and remain usable inside the existing portrait Safe Area.

## Integration checklist

Before enabling a provider adapter, add the provider package through a reviewed Unity-compatible package/import process, register only test IDs in development builds, add the real product/ad IDs through release configuration, and keep the offline fallback available for initialization failures. Add automated tests for duplicate callbacks, cancellation, network failure, purchase restoration, and no-reward paths. Update the privacy policy and Play Console Data safety form only after the final SDK list and collection behavior are known.

## Sources

1. [Google Play User Data policy](https://support.google.com/googleplay/android-developer/answer/10144311?hl=en)
2. [Google AdMob UMP for Unity](https://developers.google.com/admob/unity/privacy)
3. [Google Play one-time products](https://developer.android.com/google/play/billing/one-time-products)
4. [Google Play one-time products help](https://support.google.com/googleplay/android-developer/answer/16430488?hl=en)

## Gate status

R11 static implementation: **PASS** for a fail-closed commercial boundary and core-loop preservation.

R11 production monetization: **NOT CONFIGURED**. No ad/billing/analytics provider is installed or verified.

Android runtime test: **NOT AVAILABLE** in the current environment.

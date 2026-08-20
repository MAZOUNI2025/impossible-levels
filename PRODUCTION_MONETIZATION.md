# Production Monetization Checklist

The project currently uses `OfflineMonetizationGateway`, which is intentionally safe for development and does not display real ads or charge users. Before publishing a monetized build, create a production adapter behind the same `IMonetizationGateway` interface.

## Rewarded ads

Use a Google-approved Unity advertising/mediation package, configure a separate Android test unit ID, and never use production ad IDs during development. The rewarded flow should grant one hint, one continue, or a coin multiplier only after the SDK reports a completed reward. A dismissed or failed ad must grant nothing. Do not force a user to watch an ad to start or finish a level.

## Remove ads purchase

Create a one-time non-consumable Play product with a final product ID selected by the developer. Verify the purchase through Google Play Billing, acknowledge it, persist the entitlement locally for offline display, and restore purchases on application start. The purchase screen must clearly state what the user receives and must not use misleading buttons.

## Required values before release

| Value | Status |
|---|---|
| Android application ID | Replace placeholder in Unity Player Settings |
| Ad network app ID | Supply the real production value |
| Rewarded Android unit ID | Supply after creating the ad placement |
| Remove-ads product ID | Create in Play Console and copy exact ID |
| Developer privacy URL | Publish the completed privacy policy |
| Consent configuration | Configure for the actual distribution regions |

Until these values are provided and tested on a real device, keep `OfflineMonetizationGateway` active and publish a no-ads closed test build rather than pretending that live monetization is complete.

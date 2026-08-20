# Google Play Compliance Notes

## Locked product decision

Official target audience: **13+**. Primary marketing audience: **13–28**. The game is a general-audience casual puzzle game with a non-childish presentation. Store graphics and copy must not market the game as a children's product.

## Current official requirements relevant to this project

Google Play requires developers to declare the target audience and app content in Play Console and to complete the content-rating questionnaire. The age rating is generated from the questionnaire and may vary by region; it is not established solely by writing “13+” in marketing copy.

Google's target-audience guidance states that apps designed for everyone, including children, can be subject to Families requirements. For IMPOSSIBLE LEVELS, the product and store listing should therefore be designed and marketed for 13+ rather than selecting under-13 target groups. The declaration must still be truthful and match the actual game.

Ads must be declared in Play Console and must be appropriate for the app and its rating. The product plan uses opt-in rewarded ads and safe-transition interstitials only; no ad should interrupt active puzzle interaction. The production ad SDK and data collection must be reviewed against the final target audience, privacy policy, and Data safety form.

New personal developer accounts created after November 13, 2023 must run a closed test with at least 12 testers opted in continuously for at least 14 days before applying for production access. Google may ask for a summary of tester feedback and production readiness. This is an account-owner step that cannot be completed by source code alone.

New apps must be uploaded as a signed Android App Bundle and use Play App Signing. Google Play generates device-specific APKs from the uploaded bundle. Version codes must increase for later releases.

## Sources

1. Google Play, App testing requirements for new personal developer accounts: https://support.google.com/googleplay/android-developer/answer/14151465?hl=en
2. Android Developers, Upload your app to the Play Console: https://developer.android.com/studio/publish/upload-bundle
3. Google Play, Manage target audience and app content settings: https://support.google.com/googleplay/android-developer/answer/9867159?hl=en
4. Google Play, Set up an open, closed, or internal test: https://support.google.com/googleplay/android-developer/answer/9845334?hl=en
5. Google Play, Content rating requirements: https://support.google.com/googleplay/android-developer/answer/9859655?hl=en
6. Google Play, Ads policy: https://support.google.com/googleplay/android-developer/answer/9857753?hl=en

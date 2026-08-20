# IMPOSSIBLE LEVELS — First Run to APK

## الحالة الحالية

هذا مجلد Unity كامل مبدئيًا، وليس APK. يضم `Packages/manifest.json` و`ProjectSettings/ProjectVersion.txt` و`ProjectSettings/EditorBuildSettings.asset` ومشهدي `MainMenu` و`Gameplay`. عند فتحه في Unity 6 LTS سيبني `RuntimeSceneBootstrap` الواجهة واللوحة الإجرائية تلقائيًا.

## فتح المشروع

1. ثبّت Unity 6 LTS بإصدار `6000.0.43f1` أو إصدار Unity 6 LTS قريبًا منه، مع Android Build Support وAndroid SDK & NDK Tools وOpenJDK.
2. من Unity Hub اختر **Add > Add project from disk** وحدد مجلد `IMPOSSIBLE_LEVELS` نفسه، وليس ملف ZIP.
3. افتح المشروع وانتظر استيراد TextMeshPro والصور والصوتيات.
4. افتح `Assets/Scenes/MainMenu.unity` واضغط Play داخل Unity. زر PLAY ينقل إلى Gameplay، واللوحة الإجرائية تقرأ `il.selected_level` وتولّد المرحلة المختارة.
5. جرّب سحب القطعة البرتقالية إلى المفتاح البنفسجي، ثم اضغط المفتاح وافتح الباب. بعد النجاح يفتح المستوى التالي ويحفظ العملات والنجوم محليًا.

## بناء APK للتجربة

1. افتح **File > Build Profiles** أو **File > Build Settings** حسب إصدار Unity.
2. اختر Android ثم **Switch Platform**.
3. تأكد أن المشهدين موجودان بهذا الترتيب: `MainMenu` ثم `Gameplay`.
4. في Player Settings استخدم Package Name فريدًا مثل `com.yourstudio.impossiblelevels`، واضبط Orientation على Portrait، وMinimum API حسب متطلبات جهاز الاختبار.
5. اختر **Build** وليس Build and Run في البداية، ثم احفظ الملف باسم `IMPOSSIBLE_LEVELS_debug.apk`.
6. ثبّت APK على هاتف Android بعد تفعيل السماح بالتثبيت من مصدر خارجي، أو استخدم **Build and Run** مع USB debugging.

## فحص القبول على الهاتف

اختبر تشغيل القائمة، فتح Gameplay، اللمس والسحب، المفتاح والباب، النجاح والفشل، إعادة المحاولة، التلميحات، فتح المستوى التالي، حفظ النجوم والعملات بعد إغلاق التطبيق، تشغيل الصوت وكتمه، وتدوير الشاشة. يجب أيضًا تجربة وضع الطيران للتأكد من أن الـMVP يعمل دون إنترنت.

## قبل النشر

لا تستخدم معرّفات إعلانات اختبارية في الإنتاج. استبدل طبقة `MonetizationGateway` بمزوّد حقيقي، أضف رابط سياسة خصوصية منشورًا، أنشئ keystore آمنًا، وابنِ AAB موقّعًا بدل APK. لا تضع كلمة مرور keystore داخل المشروع أو مستودع عام.

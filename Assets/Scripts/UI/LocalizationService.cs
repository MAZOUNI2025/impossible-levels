using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ImpossibleLevels.Levels;

namespace ImpossibleLevels.UI
{
    public static class LocalizationService
    {
        private const string LanguageKey = "il.language";
        private static readonly Dictionary<string, string> English = new(StringComparer.Ordinal)
        {
            ["GAME_TITLE"] = "IMPOSSIBLE LEVELS",
            ["MENU_PLAY"] = "PLAY",
            ["MENU_LEVEL_MAP"] = "LEVEL MAP",
            ["MENU_PROFILE"] = "PLAYER",
            ["MENU_SETTINGS"] = "SETTINGS",
            ["MENU_BACK"] = "BACK",
            ["MENU_TAGLINE"] = "Looks easy. Think again.",
            ["MENU_SUBTITLE"] = "30 puzzles. One rule: question everything.",
            ["MENU_FOOTER"] = "13+  |  30 deterministic puzzles  |  Offline progression",
            ["MAP_TITLE"] = "LEVEL MAP",
            ["MAP_SUBTITLE"] = "Choose a challenge. Complete it to unlock the next.",
            ["MAP_CURRENT"] = "CURRENT LEVEL {0:00}",
            ["MAP_PROGRESS"] = "COMPLETED {0} / {1}",
            ["MAP_STARS"] = "STARS {0} / {1}",
            ["LEVEL_COMPLETED"] = "COMPLETED",
            ["PROFILE_TITLE"] = "PLAYER PROFILE",
            ["PROFILE_PROGRESS"] = "PROGRESS",
            ["PROFILE_COMPLETED"] = "COMPLETED LEVELS   {0}",
            ["PROFILE_TOTAL_LEVELS"] = "TOTAL LEVELS   {0}",
            ["PROFILE_COMPLETION_PERCENT"] = "COMPLETION   {0}%",
            ["PROFILE_STARS_LABEL"] = "STARS   {0} / {1}",
            ["PROFILE_COINS_LABEL"] = "COINS   {0}",
            ["PROFILE_HINT"] = "Keep solving. The board remembers your best stars.",
            ["SETTINGS_TITLE"] = "SETTINGS",
            ["SETTINGS_AUDIO"] = "AUDIO",
            ["SETTINGS_FEEDBACK"] = "FEEDBACK",
            ["SETTINGS_MUSIC"] = "MUSIC",
            ["SETTINGS_SFX"] = "SFX",
            ["SETTINGS_HAPTICS"] = "HAPTICS",
            ["SETTINGS_LANGUAGE"] = "LANGUAGE",
            ["SETTINGS_RESET"] = "RESET LOCAL PROGRESS",
            ["SETTINGS_RESET_BUTTON"] = "RESET",
            ["SETTINGS_PROGRESS"] = "PROGRESS",
            ["SETTINGS_VERSION"] = "VERSION {0}",
            ["SETTINGS_STATE"] = "{0}  {1}",
            ["SETTINGS_ON"] = "ON",
            ["SETTINGS_OFF"] = "OFF",
            ["SETTINGS_ENGLISH"] = "ENGLISH",
            ["SETTINGS_ARABIC"] = "العربية",
            ["GAME_LEVEL"] = "LEVEL {0:00}",
            ["GAME_LEVEL_SHORT"] = "LEVEL",
            ["GAME_OBJECTIVE"] = "Find the key and open the door.",
            ["GAME_HINT"] = "Look at the object that does not behave as expected.",
            ["TUTORIAL_TITLE"] = "QUICK START",
            ["TUTORIAL_RULE_TITLE"] = "NEW RULE",
            ["TUTORIAL_BODY"] = "Tap the key, then tap the door.",
            ["TUTORIAL_DRAG_BODY"] = "Drag the block to its socket, then open the door.",
            ["TUTORIAL_SWITCH_BODY"] = "Turn the switch on, then collect the key and open the door.",
            ["TUTORIAL_REVEAL_BODY"] = "Inspect the reveal trigger, then collect what appears and open the door.",
            ["TUTORIAL_SEQUENCE_BODY"] = "Tap the markers in the shown order, then open the door.",
            ["TUTORIAL_KEY"] = "KEY",
            ["TUTORIAL_DOOR"] = "DOOR",
            ["TUTORIAL_BLOCK"] = "BLOCK",
            ["TUTORIAL_SWITCH"] = "SWITCH",
            ["TUTORIAL_REVEAL"] = "REVEAL",
            ["TUTORIAL_SEQUENCE"] = "ORDER",
            ["HOOK_OPEN"] = "Open the door.",
            ["HOOK_NOT_YET"] = "Not yet.",
            ["HOOK_TITLE"] = "Looks Easy. Think Again.",
            ["GAME_HINT_BUTTON"] = "HINT  -5",
            ["GAME_HINT_NEED_COINS"] = "You need {0} coins for a hint.",
            ["GAME_PAUSE"] = "PAUSED",
            ["GAME_PAUSE_HINT"] = "The puzzle is safely paused. Take a breath, then continue.",
            ["GAME_RESUME"] = "RESUME",
            ["GAME_RESTART"] = "RESTART",
            ["GAME_RETRY"] = "RETRY",
            ["GAME_CONTINUE"] = "CONTINUE",
            ["GAME_SETTINGS"] = "SETTINGS",
            ["GAME_LEVEL_MAP"] = "LEVEL MAP",
            ["GAME_EXIT"] = "EXIT TO LEVEL MAP",
            ["GAME_COMPLETE"] = "LEVEL COMPLETE",
            ["GAME_COMPLETE_SUBTITLE"] = "Result saved. Choose your next move.",
            ["GAME_FAILED"] = "TRY AGAIN",
            ["GAME_FAILURE_SUBTITLE"] = "Reset the board and try a different idea.",
            ["GAME_NEXT"] = "NEXT LEVEL",
            ["GAME_REPLAY"] = "REPLAY",
            ["GAME_MENU"] = "MENU",
            ["GAME_STARS_EARNED"] = "STARS EARNED  {0} / 3",
            ["GAME_STARS_THIS_RUN"] = "THIS RUN  {0} / 3",
            ["GAME_STARS_THIS_RUN_UNAVAILABLE"] = "THIS RUN  — / 3",
            ["GAME_BEST_STARS"] = "BEST  {0} / 3",
            ["GAME_COINS_EARNED"] = "COINS EARNED  +{0}",
            ["GAME_COINS_THIS_COMPLETION"] = "THIS COMPLETION  +{0}",
            ["GAME_COINS_THIS_COMPLETION_UNAVAILABLE"] = "THIS COMPLETION  —",
            ["GAME_COINS_TOTAL"] = "TOTAL COINS  {0}",
            ["GAME_PROGRESS_SUMMARY"] = "PROGRESS  •  {0} / {1} LEVELS  •  {2} / {3} STARS",
            ["LEVEL_LOCKED"] = "LOCKED",
            ["LEVEL_CURRENT"] = "CURRENT",
            ["LEVEL_OBJECTIVE"] = "Find the key and open the door.",
            ["LEVEL_IDENTITY"] = "{0}  •  {1}",
            ["IDENTITY_TYPE_LOGIC"] = "LOGIC",
            ["IDENTITY_TYPE_PHYSICS"] = "PHYSICS",
            ["IDENTITY_TYPE_OBSERVATION"] = "OBSERVATION",
            ["IDENTITY_TYPE_TIMING"] = "TIMING",
            ["IDENTITY_TYPE_HOLD"] = "HOLD",
            ["IDENTITY_TYPE_TRICK"] = "TRICK",
            ["IDENTITY_TYPE_INTERACTION"] = "INTERACTION",
            ["IDENTITY_DIFFICULTY"] = "TIER {0}",
            ["IDENTITY_TIER_1"] = "INTRO",
            ["IDENTITY_TIER_2"] = "WARM-UP",
            ["IDENTITY_TIER_3"] = "TWIST",
            ["IDENTITY_TIER_4"] = "PRESSURE",
            ["IDENTITY_TIER_5"] = "DECEPTION",
            ["IDENTITY_TIER_6"] = "MASTER",
            ["IDENTITY_TIER_7"] = "FINAL"
        };

        private static readonly Dictionary<string, string> Arabic = new(StringComparer.Ordinal)
        {
            ["GAME_TITLE"] = "مستويات مستحيلة",
            ["MENU_PLAY"] = "ابدأ اللعب",
            ["MENU_LEVEL_MAP"] = "خريطة المستويات",
            ["MENU_PROFILE"] = "اللاعب",
            ["MENU_SETTINGS"] = "الإعدادات",
            ["MENU_BACK"] = "رجوع",
            ["MENU_TAGLINE"] = "تبدو سهلة. فكّر مرة أخرى.",
            ["MENU_SUBTITLE"] = "30 لغزًا. قاعدة واحدة: شكّك في كل شيء.",
            ["MENU_FOOTER"] = "13+  |  30 لغزًا حتميًا  |  تقدّم دون اتصال",
            ["MAP_TITLE"] = "خريطة المستويات",
            ["MAP_SUBTITLE"] = "اختر تحديًا وأكمله لفتح المستوى التالي.",
            ["MAP_CURRENT"] = "المستوى الحالي {0:00}",
            ["MAP_PROGRESS"] = "المكتمل {0} / {1}",
            ["MAP_STARS"] = "النجوم {0} / {1}",
            ["LEVEL_COMPLETED"] = "مكتمل",
            ["PROFILE_TITLE"] = "ملف اللاعب",
            ["PROFILE_PROGRESS"] = "التقدّم",
            ["PROFILE_COMPLETED"] = "المستويات المكتملة   {0}",
            ["PROFILE_TOTAL_LEVELS"] = "إجمالي المستويات   {0}",
            ["PROFILE_COMPLETION_PERCENT"] = "نسبة الإكمال   {0}%",
            ["PROFILE_STARS_LABEL"] = "النجوم   {0} / {1}",
            ["PROFILE_COINS_LABEL"] = "العملات   {0}",
            ["PROFILE_HINT"] = "استمر في الحل. اللوح يتذكر أفضل نجومك.",
            ["SETTINGS_TITLE"] = "الإعدادات",
            ["SETTINGS_AUDIO"] = "الصوت",
            ["SETTINGS_FEEDBACK"] = "التفاعل",
            ["SETTINGS_MUSIC"] = "الموسيقى",
            ["SETTINGS_SFX"] = "المؤثرات",
            ["SETTINGS_HAPTICS"] = "الاهتزاز",
            ["SETTINGS_LANGUAGE"] = "اللغة",
            ["SETTINGS_RESET"] = "إعادة ضبط التقدم المحلي",
            ["SETTINGS_RESET_BUTTON"] = "إعادة الضبط",
            ["SETTINGS_PROGRESS"] = "التقدم",
            ["SETTINGS_VERSION"] = "الإصدار {0}",
            ["SETTINGS_STATE"] = "{0}  {1}",
            ["SETTINGS_ON"] = "تشغيل",
            ["SETTINGS_OFF"] = "إيقاف",
            ["SETTINGS_ENGLISH"] = "English",
            ["SETTINGS_ARABIC"] = "العربية",
            ["GAME_LEVEL"] = "المستوى {0:00}",
            ["GAME_LEVEL_SHORT"] = "المستوى",
            ["GAME_OBJECTIVE"] = "اعثر على المفتاح وافتح الباب.",
            ["GAME_HINT"] = "انظر إلى الشيء الذي لا يتصرف كما تتوقع.",
            ["TUTORIAL_TITLE"] = "بداية سريعة",
            ["TUTORIAL_RULE_TITLE"] = "قاعدة جديدة",
            ["TUTORIAL_BODY"] = "المس المفتاح، ثم المس الباب.",
            ["TUTORIAL_DRAG_BODY"] = "اسحب الكتلة إلى مكانها، ثم افتح الباب.",
            ["TUTORIAL_SWITCH_BODY"] = "فعّل المفتاح، ثم اجمع المفتاح وافتح الباب.",
            ["TUTORIAL_REVEAL_BODY"] = "افحص محفّز الكشف، ثم اجمع ما يظهر وافتح الباب.",
            ["TUTORIAL_SEQUENCE_BODY"] = "اضغط على العلامات بالترتيب الظاهر، ثم افتح الباب.",
            ["TUTORIAL_KEY"] = "المفتاح",
            ["TUTORIAL_DOOR"] = "الباب",
            ["TUTORIAL_BLOCK"] = "الكتلة",
            ["TUTORIAL_SWITCH"] = "المفتاح",
            ["TUTORIAL_REVEAL"] = "كشف",
            ["TUTORIAL_SEQUENCE"] = "الترتيب",
            ["HOOK_OPEN"] = "افتح الباب.",
            ["HOOK_NOT_YET"] = "ليس بعد.",
            ["HOOK_TITLE"] = "تبدو سهلة. فكّر مرة أخرى.",
            ["GAME_HINT_BUTTON"] = "تلميح  -5",
            ["GAME_HINT_NEED_COINS"] = "تحتاج إلى {0} عملات لاستخدام التلميح.",
            ["GAME_PAUSE"] = "متوقف مؤقتًا",
            ["GAME_PAUSE_HINT"] = "تم إيقاف اللغز بأمان. خذ لحظة ثم تابع.",
            ["GAME_RESUME"] = "متابعة",
            ["GAME_RESTART"] = "إعادة التشغيل",
            ["GAME_RETRY"] = "إعادة المحاولة",
            ["GAME_CONTINUE"] = "استمرار",
            ["GAME_SETTINGS"] = "الإعدادات",
            ["GAME_LEVEL_MAP"] = "خريطة المستويات",
            ["GAME_EXIT"] = "الخروج إلى الخريطة",
            ["GAME_COMPLETE"] = "اكتمل المستوى",
            ["GAME_COMPLETE_SUBTITLE"] = "تم حفظ النتيجة. اختر خطوتك التالية.",
            ["GAME_FAILED"] = "حاول مرة أخرى",
            ["GAME_FAILURE_SUBTITLE"] = "أعد اللوح وجرّب فكرة مختلفة.",
            ["GAME_NEXT"] = "المستوى التالي",
            ["GAME_REPLAY"] = "إعادة اللعب",
            ["GAME_MENU"] = "القائمة",
            ["GAME_STARS_EARNED"] = "النجوم المكتسبة  {0} / 3",
            ["GAME_STARS_THIS_RUN"] = "هذا التشغيل  {0} / 3",
            ["GAME_STARS_THIS_RUN_UNAVAILABLE"] = "هذا التشغيل  — / 3",
            ["GAME_BEST_STARS"] = "الأفضل  {0} / 3",
            ["GAME_COINS_EARNED"] = "العملات المكتسبة  +{0}",
            ["GAME_COINS_THIS_COMPLETION"] = "هذا الإكمال  +{0}",
            ["GAME_COINS_THIS_COMPLETION_UNAVAILABLE"] = "هذا الإكمال  —",
            ["GAME_COINS_TOTAL"] = "إجمالي العملات  {0}",
            ["GAME_PROGRESS_SUMMARY"] = "التقدّم  •  {0} / {1} مستوى  •  {2} / {3} نجمة",
            ["LEVEL_LOCKED"] = "مقفل",
            ["LEVEL_CURRENT"] = "الحالي",
            ["LEVEL_OBJECTIVE"] = "اعثر على المفتاح وافتح الباب.",
            ["LEVEL_IDENTITY"] = "{0}  •  {1}",
            ["IDENTITY_TYPE_LOGIC"] = "منطق",
            ["IDENTITY_TYPE_PHYSICS"] = "فيزياء",
            ["IDENTITY_TYPE_OBSERVATION"] = "ملاحظة",
            ["IDENTITY_TYPE_TIMING"] = "توقيت",
            ["IDENTITY_TYPE_HOLD"] = "ثبات",
            ["IDENTITY_TYPE_TRICK"] = "خدعة",
            ["IDENTITY_TYPE_INTERACTION"] = "تفاعل",
            ["IDENTITY_DIFFICULTY"] = "الفئة {0}",
            ["IDENTITY_TIER_1"] = "تمهيدي",
            ["IDENTITY_TIER_2"] = "إحماء",
            ["IDENTITY_TIER_3"] = "تحوّل",
            ["IDENTITY_TIER_4"] = "ضغط",
            ["IDENTITY_TIER_5"] = "خداع",
            ["IDENTITY_TIER_6"] = "إتقان",
            ["IDENTITY_TIER_7"] = "نهائي"
        };

        private static readonly string[] EnglishTitles =
        {
            "The Key Is Right There", "Drag the Wrong Box", "One Button", "The Quiet Switch", "Do Not Touch the Door",
            "Falling Up", "The Fake Exit", "Three Seconds", "The Heavy Key", "Behind the Text", "Red Means Wait",
            "Two Doors", "The Stubborn Lever", "Small Gap", "The Missing Floor", "Silent Alarm", "The Long Way",
            "Almost Symmetric", "Hold Your Breath", "The Third Tap", "Falling Key", "Wrong Layer", "The Locked Hint",
            "Four Corners", "The Impatient Door", "Mirror Room", "The One-Way Box", "The Last Coin", "Two-Step Reset", "Looks Impossible"
        };

        private static readonly string[] ArabicTitles =
        {
            "المفتاح هناك تمامًا", "اسحب الصندوق الخطأ", "زر واحد", "المفتاح الهادئ", "لا تلمس الباب",
            "السقوط إلى الأعلى", "المخرج الوهمي", "ثلاث ثوانٍ", "المفتاح الثقيل", "خلف النص", "الأحمر يعني الانتظار",
            "بابان", "الرافعة العنيدة", "فجوة صغيرة", "الأرضية المفقودة", "إنذار صامت", "الطريق الطويل",
            "شبه متماثل", "احبس أنفاسك", "النقرة الثالثة", "مفتاح ساقط", "الطبقة الخاطئة", "التلميح المقفل",
            "أربع زوايا", "الباب المستعجل", "غرفة المرايا", "الصندوق ذو الاتجاه الواحد", "العملة الأخيرة", "إعادة ضبط بخطوتين", "يبدو مستحيلًا"
        };

        private static readonly string[] EnglishObjectives =
        {
            "Collect the key, then open the door.",
            "Fit the block into its socket, then open the door.",
            "Collect the key and power the switch before opening the door.",
            "Inspect the room to reveal the hidden key, then escape.",
            "Collect the key and clear the three-step sequence before opening the door.",
            "Spot the real key among the decoys, then open the door.",
            "Place the block correctly while ignoring the decoy shapes, then escape.",
            "Power the switch, avoid the false targets, and open the door.",
            "Reveal the hidden key, collect it, and ignore the decoys on the way out.",
            "Collect the key, then solve the marked sequence despite the distractions.",
            "Find the key, activate the switch, and choose the safe route to the door.",
            "Place the block and activate the switch before making your escape.",
            "Power the switch, then complete the sequence and open the door.",
            "Reveal the key, fit the block into its socket, then escape.",
            "Place the block and finish the sequence before touching the door.",
            "Reveal the key, power the switch, then open the exit.",
            "Fit the block, activate the switch, and avoid three decoys.",
            "Reveal the key, then follow every marker in the correct order.",
            "Activate the switch and finish the sequence before opening the door.",
            "Reveal, collect, place, power, and complete the sequence before the final tap.",
            "Collect the key and complete the four-marker sequence through the decoys.",
            "Reveal the key, then place the block in the distant socket.",
            "Power the switch and complete the sequence without trusting the decoys.",
            "Reveal the key, place the block, and power the switch to escape.",
            "Reveal the key, power the switch, and complete the sequence at the exit.",
            "Reveal the key, place the block, and finish the sequence in order.",
            "Place the block, power the switch, and complete the five-marker sequence.",
            "Reveal the key and place the block while the decoys guard the route.",
            "Reveal, place, power, and sequence every requirement in this final test.",
            "Master the block, reveal, switch, and five-step sequence, then open the door."
        };

        private static readonly string[] ArabicObjectives =
        {
            "اجمع المفتاح، ثم افتح الباب.",
            "ثبّت الكتلة في مكانها، ثم افتح الباب.",
            "اجمع المفتاح وشغّل المفتاح قبل فتح الباب.",
            "افحص الغرفة لكشف المفتاح المخفي، ثم اهرب.",
            "اجمع المفتاح وأنهِ الترتيب ذي الخطوات الثلاث قبل فتح الباب.",
            "ميّز المفتاح الحقيقي بين الطُعم، ثم افتح الباب.",
            "ضع الكتلة بشكل صحيح وتجاهل الأشكال المضللة، ثم اهرب.",
            "شغّل المفتاح وتجنب الأهداف الوهمية وافتح الباب.",
            "اكشف المفتاح المخفي واجمعه وتجاهل الطُعم في طريق الخروج.",
            "اجمع المفتاح ثم حل العلامات بالترتيب رغم التشتيت.",
            "اعثر على المفتاح وفعّل المفتاح واختر الطريق الآمن إلى الباب.",
            "ضع الكتلة وفعّل المفتاح قبل الهروب.",
            "شغّل المفتاح ثم أكمل الترتيب وافتح الباب.",
            "اكشف المفتاح وثبّت الكتلة في مكانها، ثم اهرب.",
            "ضع الكتلة وأنهِ الترتيب قبل لمس الباب.",
            "اكشف المفتاح وشغّل المفتاح، ثم افتح المخرج.",
            "ثبّت الكتلة وفعّل المفتاح وتجنب ثلاثة طُعوم.",
            "اكشف المفتاح ثم اتبع كل علامة بالترتيب الصحيح.",
            "فعّل المفتاح وأنهِ الترتيب قبل فتح الباب.",
            "اكشف واجمع وثبّت وشغّل وأنهِ الترتيب قبل النقرة الأخيرة.",
            "اجمع المفتاح وأنهِ ترتيب العلامات الأربع بين الطُعوم.",
            "اكشف المفتاح ثم ضع الكتلة في المكان البعيد.",
            "شغّل المفتاح وأنهِ الترتيب دون الوثوق بالطُعوم.",
            "اكشف المفتاح وثبّت الكتلة وشغّل المفتاح للهروب.",
            "اكشف المفتاح وشغّل المفتاح وأنهِ الترتيب عند المخرج.",
            "اكشف المفتاح وثبّت الكتلة وأنهِ الترتيب بالترتيب الصحيح.",
            "ثبّت الكتلة وشغّل المفتاح وأنهِ ترتيب العلامات الخمس.",
            "اكشف المفتاح وثبّت الكتلة بينما تحرس الطُعوم الطريق.",
            "اكشف وثبّت وشغّل وأنهِ كل المتطلبات في الاختبار الأخير.",
            "أتقن الكتلة والكشف والمفتاح والترتيب ذي الخطوات الخمس، ثم افتح الباب."
        };

        private static readonly string[] EnglishHints =
        {
            "Tap the key first, then tap the door.",
            "Drag the block into the matching socket.",
            "Turn the switch on before trying the door.",
            "Inspect the reveal trigger to show what the room hides.",
            "Tap the three markers in the demonstrated order.",
            "Tap the real key, then head for the door.",
            "Drag the block into the socket and ignore the decoy shapes.",
            "Power the switch, then look for the safe route.",
            "Use the reveal marker before searching for the key.",
            "Collect the key, then start at the first sequence marker.",
            "Study the room, choose the key, then activate the switch.",
            "Finish the block placement before toggling the switch.",
            "After powering the switch, tap the markers in order.",
            "Reveal the key first, then move the block to its socket.",
            "Place the block, then begin with sequence marker one.",
            "Use the reveal marker and power the switch before the door.",
            "Place the block, activate the switch, and ignore the decoys.",
            "Reveal the key, then tap markers from first to last.",
            "Power the switch and do not skip a sequence marker.",
            "This room needs every tool; begin with the reveal marker.",
            "Collect the key, then follow four markers in order.",
            "Reveal the key, then drag the block to the distant socket.",
            "Power the switch before starting the sequence.",
            "Reveal, place, then power the switch in that order.",
            "After revealing and powering, start at marker one.",
            "Reveal the key, place the block, then follow the sequence.",
            "Place the block, power the switch, then clear five markers.",
            "The key is hidden; reveal it, place the block, and ignore decoys.",
            "Do not stop after one requirement; complete all four.",
            "Begin with reveal, then block, switch, and the final sequence."
        };

        private static readonly string[] ArabicHints =
        {
            "اضغط على المفتاح أولًا، ثم اضغط على الباب.",
            "اسحب الكتلة إلى المكان المطابق.",
            "فعّل المفتاح قبل محاولة فتح الباب.",
            "افحص محفّز الكشف لإظهار ما تخفيه الغرفة.",
            "اضغط على العلامات الثلاث بالترتيب الموضح.",
            "اضغط على المفتاح الحقيقي، ثم اتجه إلى الباب.",
            "اسحب الكتلة إلى الفتحة وتجاهل الأشكال المضللة.",
            "شغّل المفتاح، ثم ابحث عن الطريق الآمن.",
            "فعّل علامة الكشف قبل البحث عن المفتاح.",
            "اجمع المفتاح، ثم ابدأ بالعلامة الأولى في الترتيب.",
            "افحص العلامات واختر المفتاح، ثم فعّل المفتاح.",
            "أنهِ وضع الكتلة قبل تبديل المفتاح.",
            "بعد تشغيل المفتاح اضغط العلامات بالترتيب.",
            "اكشف المفتاح أولًا ثم حرّك الكتلة إلى الفتحة.",
            "ضع الكتلة ثم ابدأ الترتيب من العلامة الأولى.",
            "استخدم علامة الكشف ثم شغّل المفتاح قبل الباب.",
            "ضع الكتلة، ثم فعّل المفتاح وتجاهل الطُعوم.",
            "اكشف المفتاح ثم اضغط العلامات من الأولى إلى الأخيرة.",
            "شغّل المفتاح ثم لا تتخطَّ أي علامة.",
            "هذه الغرفة تحتاج كل الأدوات: ابدأ بعلامة الكشف.",
            "اجمع المفتاح ثم اتبع أربع علامات بالترتيب.",
            "اكشف المفتاح، ثم اسحب الكتلة إلى المكان البعيد.",
            "شغّل المفتاح قبل بدء ترتيب العلامات.",
            "نفّذ الكشف ثم وضع الكتلة ثم تشغيل المفتاح.",
            "بعد الكشف والتشغيل، ابدأ بالعلامة الأولى.",
            "اكشف المفتاح وضع الكتلة ثم اتبع الترتيب.",
            "ضع الكتلة وشغّل المفتاح ثم أكمل خمس علامات.",
            "المفتاح مخفي؛ اكشفه ثم ضع الكتلة وتجاهل الطُعوم.",
            "لا تتوقف بعد أول شرط؛ نفّذ المتطلبات الأربعة كلها.",
            "ابدأ بالكشف، ثم الكتلة، ثم المفتاح، ثم الترتيب الأخير."
        };

        public static string CurrentLanguage
        {
            get
            {
                var saved = PlayerPrefs.GetString(LanguageKey, "en").ToLowerInvariant();
                return saved == "ar" ? "ar" : "en";
            }
        }

        public static bool IsArabic => CurrentLanguage == "ar";

        public static void SetLanguage(string language)
        {
            var normalized = string.Equals(language, "ar", StringComparison.OrdinalIgnoreCase) ? "ar" : "en";
            PlayerPrefs.SetString(LanguageKey, normalized);
            PlayerPrefs.Save();
        }

        public static string Get(string key)
        {
            var table = IsArabic ? Arabic : English;
            if (table.TryGetValue(key, out var value)) return value;
            return English.TryGetValue(key, out value) ? value : key;
        }

        public static string Format(string key, params object[] args)
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, Get(key), args);
        }

        public static string GetPuzzleTypeLabel(PuzzleType type)
        {
            var key = type switch
            {
                PuzzleType.Logic => "IDENTITY_TYPE_LOGIC",
                PuzzleType.Physics => "IDENTITY_TYPE_PHYSICS",
                PuzzleType.Observation => "IDENTITY_TYPE_OBSERVATION",
                PuzzleType.Timing => "IDENTITY_TYPE_TIMING",
                PuzzleType.Hold => "IDENTITY_TYPE_HOLD",
                PuzzleType.Trick => "IDENTITY_TYPE_TRICK",
                PuzzleType.Interaction => "IDENTITY_TYPE_INTERACTION",
                _ => "IDENTITY_TYPE_LOGIC"
            };
            return Get(key);
        }

        public static string GetDifficultyLabel(int difficulty)
        {
            var safeDifficulty = Mathf.Clamp(difficulty, 1, 7);
            return Format("LEVEL_IDENTITY", Format("IDENTITY_DIFFICULTY", safeDifficulty), Get("IDENTITY_TIER_" + safeDifficulty));
        }

        public static string GetLevelIdentity(PuzzleType type, int difficulty)
        {
            return Format("LEVEL_IDENTITY", GetPuzzleTypeLabel(type), GetDifficultyLabel(difficulty));
        }

        public static string GetLevelTitle(int index, string fallback)
        {
            return GetLevelValue(index, IsArabic ? ArabicTitles : EnglishTitles, fallback);
        }

        public static string GetLevelObjective(int index, string fallback)
        {
            return GetLevelValue(index, IsArabic ? ArabicObjectives : EnglishObjectives, fallback);
        }

        public static string GetLevelHint(int index, string fallback)
        {
            return GetLevelValue(index, IsArabic ? ArabicHints : EnglishHints, fallback);
        }

        private static string GetLevelValue(int index, string[] values, string fallback)
        {
            var zeroBased = index - 1;
            return zeroBased >= 0 && zeroBased < values.Length ? values[zeroBased] : fallback;
        }

        public static void ApplyTo(TMP_Text text)
        {
            if (text == null) return;
            text.isRightToLeftText = IsArabic;
            if (IsArabic && text.alignment == TextAlignmentOptions.Left)
            {
                // ApplyTo may be called more than once; never toggle an already RTL-aligned label.
                text.alignment = TextAlignmentOptions.Right;
            }
        }
    }
}

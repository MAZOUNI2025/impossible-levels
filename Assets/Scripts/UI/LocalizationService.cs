using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
            ["PROFILE_TITLE"] = "PLAYER PROFILE",
            ["PROFILE_PROGRESS"] = "PROGRESS",
            ["PROFILE_COMPLETED"] = "COMPLETED   {0} / 30",
            ["PROFILE_HINT"] = "Keep solving. The board remembers your best stars.",
            ["SETTINGS_TITLE"] = "SETTINGS",
            ["SETTINGS_AUDIO"] = "AUDIO & FEEDBACK",
            ["SETTINGS_MUSIC"] = "MUSIC",
            ["SETTINGS_SFX"] = "SFX",
            ["SETTINGS_HAPTICS"] = "HAPTICS",
            ["SETTINGS_LANGUAGE"] = "LANGUAGE",
            ["SETTINGS_RESET"] = "RESET LOCAL PROGRESS",
            ["SETTINGS_RESET_BUTTON"] = "RESET",
            ["SETTINGS_ON"] = "ON",
            ["SETTINGS_OFF"] = "OFF",
            ["SETTINGS_ENGLISH"] = "ENGLISH",
            ["SETTINGS_ARABIC"] = "العربية",
            ["GAME_LEVEL"] = "LEVEL {0:00}",
            ["GAME_LEVEL_SHORT"] = "LEVEL",
            ["GAME_OBJECTIVE"] = "Find the key and open the door.",
            ["GAME_HINT"] = "Look at the object that does not behave as expected.",
            ["HOOK_OPEN"] = "Open the door.",
            ["HOOK_NOT_YET"] = "Not yet.",
            ["HOOK_TITLE"] = "Looks Easy. Think Again.",
            ["GAME_HINT_BUTTON"] = "HINT  -5",
            ["GAME_PAUSE"] = "PAUSED",
            ["GAME_RESUME"] = "RESUME",
            ["GAME_RESTART"] = "RESTART",
            ["GAME_RETRY"] = "RETRY",
            ["GAME_CONTINUE"] = "CONTINUE",
            ["GAME_SETTINGS"] = "SETTINGS",
            ["GAME_LEVEL_MAP"] = "LEVEL MAP",
            ["GAME_EXIT"] = "EXIT TO LEVEL MAP",
            ["GAME_COMPLETE"] = "LEVEL COMPLETE",
            ["GAME_FAILED"] = "TRY AGAIN",
            ["GAME_NEXT"] = "NEXT LEVEL",
            ["GAME_REPLAY"] = "REPLAY",
            ["GAME_MENU"] = "MENU",
            ["GAME_STARS_EARNED"] = "STARS EARNED  {0} / 3",
            ["GAME_COINS_EARNED"] = "COINS EARNED  +{0}",
            ["LEVEL_LOCKED"] = "LOCKED",
            ["LEVEL_CURRENT"] = "CURRENT",
            ["LEVEL_OBJECTIVE"] = "Find the key and open the door."
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
            ["PROFILE_TITLE"] = "ملف اللاعب",
            ["PROFILE_PROGRESS"] = "التقدّم",
            ["PROFILE_COMPLETED"] = "المكتمل   {0} / 30",
            ["PROFILE_HINT"] = "استمر في الحل. اللوح يتذكر أفضل نجومك.",
            ["SETTINGS_TITLE"] = "الإعدادات",
            ["SETTINGS_AUDIO"] = "الصوت والتفاعل",
            ["SETTINGS_MUSIC"] = "الموسيقى",
            ["SETTINGS_SFX"] = "المؤثرات",
            ["SETTINGS_HAPTICS"] = "الاهتزاز",
            ["SETTINGS_LANGUAGE"] = "اللغة",
            ["SETTINGS_RESET"] = "إعادة ضبط التقدم المحلي",
            ["SETTINGS_RESET_BUTTON"] = "إعادة الضبط",
            ["SETTINGS_ON"] = "تشغيل",
            ["SETTINGS_OFF"] = "إيقاف",
            ["SETTINGS_ENGLISH"] = "English",
            ["SETTINGS_ARABIC"] = "العربية",
            ["GAME_LEVEL"] = "المستوى {0:00}",
            ["GAME_LEVEL_SHORT"] = "المستوى",
            ["GAME_OBJECTIVE"] = "اعثر على المفتاح وافتح الباب.",
            ["GAME_HINT"] = "انظر إلى الشيء الذي لا يتصرف كما تتوقع.",
            ["HOOK_OPEN"] = "افتح الباب.",
            ["HOOK_NOT_YET"] = "ليس بعد.",
            ["HOOK_TITLE"] = "تبدو سهلة. فكّر مرة أخرى.",
            ["GAME_HINT_BUTTON"] = "تلميح  -5",
            ["GAME_PAUSE"] = "متوقف مؤقتًا",
            ["GAME_RESUME"] = "متابعة",
            ["GAME_RESTART"] = "إعادة التشغيل",
            ["GAME_RETRY"] = "إعادة المحاولة",
            ["GAME_CONTINUE"] = "استمرار",
            ["GAME_SETTINGS"] = "الإعدادات",
            ["GAME_LEVEL_MAP"] = "خريطة المستويات",
            ["GAME_EXIT"] = "الخروج إلى الخريطة",
            ["GAME_COMPLETE"] = "اكتمل المستوى",
            ["GAME_FAILED"] = "حاول مرة أخرى",
            ["GAME_NEXT"] = "المستوى التالي",
            ["GAME_REPLAY"] = "إعادة اللعب",
            ["GAME_MENU"] = "القائمة",
            ["GAME_STARS_EARNED"] = "النجوم المكتسبة  {0} / 3",
            ["GAME_COINS_EARNED"] = "العملات المكتسبة  +{0}",
            ["LEVEL_LOCKED"] = "مقفل",
            ["LEVEL_CURRENT"] = "الحالي",
            ["LEVEL_OBJECTIVE"] = "اعثر على المفتاح وافتح الباب."
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
            "Open the door.", "Reveal the real target.", "Activate the button.", "Find the hidden switch.", "Reach the goal without the door.",
            "Reach the exit.", "Find the real exit.", "Hold until the ring completes.", "Activate the pressure plate.", "Reveal the hidden control.",
            "Tap during the safe phase.", "Open the matching door.", "Pull the lever fully.", "Roll the ball to the goal.", "Cross the missing section.",
            "Disable the alarm.", "Activate both plates.", "Find the mismatch.", "Keep the character still.", "Complete the three-state switch.",
            "Catch the key.", "Tap the true target.", "Unlock the hint path.", "Tap the corners in order.", "Wait for the true opening.",
            "Recreate the pattern.", "Move the box around the obstacle.", "Reach the door.", "Reveal the second state.", "Open the final door."
        };

        private static readonly string[] ArabicObjectives =
        {
            "افتح الباب.", "اكشف الهدف الحقيقي.", "فعّل الزر.", "اعثر على المفتاح المخفي.", "بلغ الهدف دون استخدام الباب.",
            "بلغ المخرج.", "اعثر على المخرج الحقيقي.", "استمر حتى تكتمل الدائرة.", "فعّل لوحة الضغط.", "اكشف أداة التحكم المخفية.",
            "انقر أثناء المرحلة الآمنة.", "افتح الباب المطابق.", "اسحب الرافعة بالكامل.", "دحرج الكرة إلى الهدف.", "اعبر الجزء المفقود.",
            "عطّل الإنذار.", "فعّل اللوحتين.", "اعثر على الاختلاف.", "أبقِ الشخصية ساكنة.", "أكمل المفتاح ذي الحالات الثلاث.",
            "أمسك بالمفتاح.", "انقر على الهدف الحقيقي.", "افتح مسار التلميح.", "انقر على الزوايا بالترتيب.", "انتظر الفتح الحقيقي.",
            "أعد تشكيل النمط.", "حرّك الصندوق حول العائق.", "بلغ الباب.", "اكشف الحالة الثانية.", "افتح الباب الأخير."
        };

        private static readonly string[] EnglishHints =
        {
            "Notice the object that changes the goal.", "The smallest object may matter most.", "The first result is not the final state.", "Look at the room edges.", "The goal label can be misleading.",
            "Watch the direction of movement.", "Check which object has a collider.", "A short press is not enough.", "The key is not meant to be dragged.", "UI can hide part of the puzzle.",
            "Color is a timing signal.", "Compare symbols, not colors.", "Keep dragging after the first click.", "Use momentum rather than precision.", "The scenery is interactive.",
            "Find what is moving.", "The direct path is a trap.", "Look for the object that breaks symmetry.", "Sometimes no movement is the solution.", "Count the state changes.",
            "Release is the action.", "Shadows can reveal the real target.", "The hint is part of the puzzle.", "Order is hidden in the environment.", "Do not retry immediately.",
            "Symmetry is the instruction.", "Plan the complete path.", "The reward is a distraction.", "Retry can change the puzzle.", "Combine what the world taught you."
        };

        private static readonly string[] ArabicHints =
        {
            "لاحظ الشيء الذي يغيّر الهدف.", "قد يكون أصغر عنصر هو الأهم.", "النتيجة الأولى ليست الحالة النهائية.", "انظر إلى حواف الغرفة.", "قد يكون اسم الهدف مضللًا.",
            "راقب اتجاه الحركة.", "تحقق من الجسم الذي يملك مصادمًا.", "الضغط القصير لا يكفي.", "المفتاح ليس مخصصًا للسحب.", "قد تخفي الواجهة جزءًا من اللغز.",
            "اللون إشارة للتوقيت.", "قارن الرموز لا الألوان.", "واصل السحب بعد النقرة الأولى.", "استخدم الزخم بدل الدقة.", "المشهد قابل للتفاعل.",
            "اعثر على الشيء المتحرك.", "المسار المباشر فخ.", "ابحث عن العنصر الذي يكسر التماثل.", "أحيانًا يكون عدم الحركة هو الحل.", "عدّ تغيّرات الحالة.",
            "الإفلات هو الفعل المطلوب.", "قد تكشف الظلال الهدف الحقيقي.", "التلميح جزء من اللغز.", "الترتيب مخفي في البيئة.", "لا تعد المحاولة فورًا.",
            "التماثل هو التعليمات.", "خطط للمسار كاملًا.", "المكافأة مجرد تشتيت.", "قد تغيّر إعادة المحاولة اللغز.", "اجمع ما علّمك العالم إياه."
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
            if (IsArabic)
            {
                if (text.alignment == TextAlignmentOptions.Left) text.alignment = TextAlignmentOptions.Right;
                else if (text.alignment == TextAlignmentOptions.Right) text.alignment = TextAlignmentOptions.Left;
            }
        }
    }
}

from __future__ import annotations
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
checks = []

def check(name: str, ok: bool, detail: str):
    checks.append({"name": name, "ok": bool(ok), "detail": detail})

manifest = ROOT / "Packages" / "manifest.json"
try:
    json.loads(manifest.read_text(encoding="utf-8"))
    check("manifest_json", True, "Packages/manifest.json is valid JSON")
except Exception as exc:
    check("manifest_json", False, str(exc))

for rel in [
    "ProjectSettings/ProjectVersion.txt",
    "ProjectSettings/EditorBuildSettings.asset",
    "Assets/Scenes/MainMenu.unity",
    "Assets/Scenes/Gameplay.unity",
    "Assets/Scripts/Core/RuntimeSceneBootstrap.cs",
    "Assets/Scripts/Core/GameBootstrap.cs",
]:
    path = ROOT / rel
    check(rel, path.exists(), "present" if path.exists() else "missing")

cs = "\n".join(p.read_text(encoding="utf-8", errors="ignore") for p in (ROOT / "Assets" / "Scripts").rglob("*.cs"))
for symbol in [
    "public void Configure(LevelRuntime levelRuntime, Transform root, Camera camera)",
    "public void SetLevelIndex(int index)",
    "public void StartFirstLevel()",
    "public void LoadNextLevel()",
]:
    check("symbol:" + symbol.split("(")[0], symbol in cs, "found" if symbol in cs else "missing")

level_images = sorted((ROOT / "Assets" / "Art" / "Levels").glob("level_*.png"))
ui_images = sorted((ROOT / "Assets" / "Art" / "UI").glob("*.png"))
music = sorted((ROOT / "Assets" / "Audio" / "Music").glob("*.wav"))
sfx = sorted((ROOT / "Assets" / "Audio" / "SFX").glob("*.wav"))
check("level_thumbnails", len(level_images) == 30, f"{len(level_images)} found")
check("ui_assets", len(ui_images) >= 5, f"{len(ui_images)} found")
check("audio_assets", len(music) >= 2 and len(sfx) >= 7, f"{len(music)} music and {len(sfx)} SFX")

report = {
    "project": "IMPOSSIBLE LEVELS",
    "source_static_checks": checks,
    "passed": sum(1 for item in checks if item["ok"]),
    "total": len(checks),
    "unity_editor_build_verified": False,
    "notes": [
        "Static checks validate files and symbols only.",
        "Unity compilation and Android APK installation still require Unity Editor and Android SDK.",
    ],
}
(ROOT / "STATIC_PROJECT_CHECK.json").write_text(json.dumps(report, indent=2), encoding="utf-8")
print(json.dumps(report, indent=2))
raise SystemExit(0 if report["passed"] == report["total"] else 1)

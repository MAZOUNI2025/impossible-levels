import math
import os
import random
import struct
import wave

OUT = "/home/ubuntu/IMPOSSIBLE_LEVELS/audio/sfx"
RATE = 44100
random.seed(7)
os.makedirs(OUT, exist_ok=True)

def envelope(i, n, attack=0.02, release=0.12):
    t = i / max(1, n - 1)
    a = min(1.0, t / attack) if attack else 1.0
    r = min(1.0, (1.0 - t) / release) if release else 1.0
    return max(0.0, min(1.0, a, r))

def tone(freqs, duration, volume=0.25, noise=0.0, attack=0.02, release=0.12, vibrato=0.0):
    n = int(RATE * duration)
    samples = []
    phases = [0.0 for _ in freqs]
    for i in range(n):
        t = i / RATE
        vib = 1.0 + vibrato * math.sin(2 * math.pi * 5 * t)
        value = 0.0
        for j, freq in enumerate(freqs):
            value += math.sin(phases[j]) / len(freqs)
            phases[j] += 2 * math.pi * freq * vib / RATE
        value += noise * (random.random() * 2 - 1)
        value *= envelope(i, n, attack, release) * volume
        samples.append(max(-1.0, min(1.0, value)))
    return samples

def write(name, samples):
    path = os.path.join(OUT, name + ".wav")
    with wave.open(path, "wb") as wf:
        wf.setnchannels(1)
        wf.setsampwidth(2)
        wf.setframerate(RATE)
        frames = b"".join(struct.pack("<h", int(s * 32767)) for s in samples)
        wf.writeframes(frames)

write("ui_tap", tone([660, 990], 0.07, 0.22, attack=0.005, release=0.08))
write("ui_invalid", tone([180, 130], 0.16, 0.22, attack=0.005, release=0.12))
write("key_pickup", tone([660, 880, 1320], 0.32, 0.24, attack=0.01, release=0.22, vibrato=0.01))
write("door_unlock", tone([330, 495, 660, 990], 0.55, 0.24, attack=0.015, release=0.35, vibrato=0.008))
write("hint", tone([440, 554, 659], 0.28, 0.18, attack=0.01, release=0.2))
write("success", tone([523, 659, 784, 1047], 0.75, 0.22, attack=0.015, release=0.4))
write("failure", tone([330, 247, 196], 0.45, 0.2, attack=0.01, release=0.3))
write("pause", tone([523, 392], 0.18, 0.16, attack=0.01, release=0.16))

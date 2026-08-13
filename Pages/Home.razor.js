let audioContext;

const cueSequences = {
    activate: [
        { frequency: 420, duration: 0.06, offset: 0 },
        { frequency: 700, duration: 0.09, offset: 0.07 }
    ],
    deactivate: [
        { frequency: 620, duration: 0.06, offset: 0 },
        { frequency: 340, duration: 0.08, offset: 0.07 }
    ],
    navigate: [
        { frequency: 520, duration: 0.055, offset: 0 }
    ],
    target: [
        { frequency: 280, duration: 0.045, offset: 0 },
        { frequency: 560, duration: 0.07, offset: 0.055 }
    ],
    complete: [
        { frequency: 440, duration: 0.08, offset: 0 },
        { frequency: 660, duration: 0.1, offset: 0.09 },
        { frequency: 880, duration: 0.14, offset: 0.2 }
    ]
};

function getAudioContext() {
    const AudioContext = window.AudioContext || window.webkitAudioContext;
    if (!AudioContext) {
        return null;
    }

    audioContext ??= new AudioContext();
    return audioContext;
}

function playTone(context, tone, startTime) {
    const oscillator = context.createOscillator();
    const gain = context.createGain();
    const toneStart = startTime + tone.offset;
    const toneEnd = toneStart + tone.duration;

    oscillator.type = "sine";
    oscillator.frequency.setValueAtTime(tone.frequency, toneStart);
    gain.gain.setValueAtTime(0.0001, toneStart);
    gain.gain.exponentialRampToValueAtTime(0.045, toneStart + 0.012);
    gain.gain.exponentialRampToValueAtTime(0.0001, toneEnd);

    oscillator.connect(gain);
    gain.connect(context.destination);
    oscillator.start(toneStart);
    oscillator.stop(toneEnd + 0.015);
}

export async function play(cue) {
    const context = getAudioContext();
    const sequence = cueSequences[cue];
    if (!context || !sequence) {
        return;
    }

    if (context.state === "suspended") {
        await context.resume();
    }

    const startTime = context.currentTime + 0.01;
    sequence.forEach(tone => playTone(context, tone, startTime));
}

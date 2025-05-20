let video = document.getElementById("video");
let statusText = document.getElementById("statusText") || { innerHTML: "" };
let warningCountEl = document.getElementById("warningCount");
let failMessage = document.getElementById("failMessage");
let form = document.getElementById("examForm");

const warningKey = "warning_count";
let warningCount = parseInt(localStorage.getItem(warningKey) || "0");
warningCountEl.innerText = warningCount;

let faceModel, audioContext, micStream, streamRef;
let monitoring = true;
const warningLimit = 4;

let lastEyeContactTime = Date.now();
let warningIssuedForBreak = false;
const sustainedBreakThreshold = 5000; // 5 seconds

window.onload = async () => {
    await loadModel();
    await startMedia();
    monitorLoop();
};

async function loadModel() {
    faceModel = await blazeface.load();
}

async function startMedia() {
    streamRef = await navigator.mediaDevices.getUserMedia({ video: true, audio: true });
    video.srcObject = streamRef;

    audioContext = new (window.AudioContext || window.webkitAudioContext)();
    micStream = audioContext.createMediaStreamSource(streamRef);
}

function monitorLoop() {
    const analyser = audioContext.createAnalyser();
    micStream.connect(analyser);
    analyser.fftSize = 512;
    const dataArray = new Uint8Array(analyser.frequencyBinCount);

    setInterval(async () => {
        if (!monitoring) return;

        // Microphone volume check
        analyser.getByteFrequencyData(dataArray);
        const avgVolume = dataArray.reduce((a, b) => a + b, 0) / dataArray.length / 100;
        if (avgVolume > 0.08) {
            issueWarning("Loud background noise detected");
        }

        const predictions = await faceModel.estimateFaces(video, false);

        if (predictions.length !== 1 || predictions[0]?.probability?.[0] < 0.95) {
            handleEyeContactBreak("Face not detected");
            return;
        }

        const face = predictions[0];
        const [rightEye, leftEye, nose, mouth, rightEar, leftEar] = face.landmarks;

        const eyeAvgY = (rightEye[1] + leftEye[1]) / 2;
        const noseY = nose[1];
        const eyeToNoseY = noseY - eyeAvgY;

        const leftFaceWidth = Math.abs(leftEye[0] - leftEar[0]);
        const rightFaceWidth = Math.abs(rightEye[0] - rightEar[0]);
        const eyeLevelDiff = Math.abs(rightEye[1] - leftEye[1]);

        const isLookingDown = eyeToNoseY > 35;
        const isLookingSideways = Math.abs(leftFaceWidth - rightFaceWidth) > 40;
        const isHeadTilted = eyeLevelDiff > 20;

        const isBreakingEyeContact = isLookingDown || isLookingSideways || isHeadTilted;

        if (isBreakingEyeContact) {
            handleEyeContactBreak("Not maintaining eye contact with the screen");
        } else {
            clearEyeContactBreak();
            statusText.innerHTML = `<span class="text-success">🟢 Eye contact maintained</span>`;
        }
    }, 1000);
}

function handleEyeContactBreak(reason) {
    const now = Date.now();

    if (!warningIssuedForBreak && now - lastEyeContactTime >= sustainedBreakThreshold) {
        issueWarning(reason);
        warningIssuedForBreak = true;
    }

    statusText.innerHTML = `<span class="text-warning">⚠️ Eye contact lost</span>`;
}

function clearEyeContactBreak() {
    lastEyeContactTime = Date.now();
    warningIssuedForBreak = false;
}

function issueWarning(reason) {
    clearEyeContactBreak();

    warningCount++;
    localStorage.setItem(warningKey, warningCount);
    warningCountEl.innerText = warningCount;
    statusText.innerHTML = `<span class="text-danger">🚨 Warning #${warningCount}: ${reason}</span>`;

    fetch('/Assessment/LogViolation', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ reason })
    });

    if (warningCount >= warningLimit) {
        endExam();
    }
}

function endExam() {
    monitoring = false;
    localStorage.setItem(warningKey, warningLimit);

    if (failMessage) failMessage.classList.remove("d-none");
    if (streamRef) streamRef.getTracks().forEach(t => t.stop());
    if (audioContext && audioContext.state !== "closed") audioContext.close();

    setTimeout(() => {
        if (form) form.submit();
    }, 3000);
}
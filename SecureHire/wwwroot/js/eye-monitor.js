let video = document.getElementById("video");
let statusText = document.getElementById("statusText") || { innerHTML: "" };
let warningCountEl = document.getElementById("warningCount");
let failMessage = document.getElementById("failMessage");
let form = document.getElementById("examForm");

const warningKey = "warning_count";
const warningLimit = 4;
const sustainedBreakThreshold = 5000; // 5 seconds
const gracePeriod = 3000; // initial grace period

let warningCount = 0;
let lastEyeContactTime = Date.now();
let warningIssued = false;
let monitoring = true;
let faceModel = null;
let streamRef = null;
let startTime = Date.now();

// Clear old data
localStorage.removeItem(warningKey);
warningCountEl.innerText = "0";

// Optional debug overlay
const countdownOverlay = document.createElement('div');
countdownOverlay.style.position = 'fixed';
countdownOverlay.style.bottom = '90px';
countdownOverlay.style.right = '20px';
countdownOverlay.style.backgroundColor = 'rgba(255, 255, 0, 0.9)';
countdownOverlay.style.padding = '10px 15px';
countdownOverlay.style.borderRadius = '8px';
countdownOverlay.style.fontWeight = 'bold';
countdownOverlay.style.fontFamily = 'monospace';
countdownOverlay.style.display = 'none';
countdownOverlay.style.zIndex = '1000';
document.body.appendChild(countdownOverlay);

window.onload = async () => {
    faceModel = await blazeface.load();
    await startCamera();
    monitorLoop();
};

async function startCamera() {
    streamRef = await navigator.mediaDevices.getUserMedia({ video: true });
    video.srcObject = streamRef;
}

function monitorLoop() {
    setInterval(async () => {
        if (!monitoring || !faceModel) return;

        // Wait for grace period
        if (Date.now() - startTime < gracePeriod) {
            statusText.innerHTML = `<span class="text-muted">⏳ Initializing...</span>`;
            return;
        }

        const predictions = await faceModel.estimateFaces(video, false);
        if (predictions.length !== 1 || predictions[0]?.probability?.[0] < 0.60) {
            handleEyeContactLoss("Face not detected");
            return;
        }

        const face = predictions[0];
        const [rightEye, leftEye, nose] = face.landmarks;

        const eyeAvgY = (rightEye[1] + leftEye[1]) / 2;
        const noseY = nose[1];
        const eyeToNoseY = noseY - eyeAvgY;

        // Log raw values
        console.log("eyeToNoseY =", eyeToNoseY);

        const isLookingDown = eyeToNoseY > 40;

        if (isLookingDown) {
            handleEyeContactLoss("Not maintaining eye contact");
        } else {
            clearEyeContactLoss();
            statusText.innerHTML = `<span class="text-success">🟢 Eye contact maintained</span>`;
            countdownOverlay.style.display = 'none';
        }
    }, 1000);
}

function handleEyeContactLoss(reason) {
    const now = Date.now();
    const elapsed = now - lastEyeContactTime;

    console.log("Eye contact lost for", (elapsed / 1000).toFixed(1), "seconds");

    // Show countdown overlay
    countdownOverlay.innerText = `⚠️ Eye contact lost: ${(elapsed / 1000).toFixed(1)}s`;
    countdownOverlay.style.display = 'block';

    if (!warningIssued && elapsed >= sustainedBreakThreshold) {
        issueWarning(reason);
        warningIssued = true;
    }

    statusText.innerHTML = `<span class="text-warning">⚠️ Not maintaining eye contact</span>`;
}

function clearEyeContactLoss() {
    lastEyeContactTime = Date.now();
    warningIssued = false;
}

function issueWarning(reason) {
    clearEyeContactLoss();

    warningCount++;
    localStorage.setItem(warningKey, warningCount);
    warningCountEl.innerText = warningCount;
    statusText.innerHTML = `<span class="text-danger">🚨 Warning #${warningCount}: ${reason}</span>`;
    countdownOverlay.style.display = 'none';

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
    if (streamRef) streamRef.getTracks().forEach(track => track.stop());

    setTimeout(() => {
        if (form) form.submit();
    }, 3000);
}

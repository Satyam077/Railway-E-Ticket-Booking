window.cameraControl = {
    stream: null,

    start: async function () {
        try {
            this.stream = await navigator.mediaDevices.getUserMedia({ video: true });
            const video = document.getElementById('cameraPreview');
            video.srcObject = this.stream;
        } catch (err) {
            console.error("Camera error:", err);
        }
    },

    stop: function () {
        if (this.stream) {
            this.stream.getTracks().forEach(track => track.stop());
            this.stream = null;
        }
    },

    capture: function () {
        const video = document.getElementById('cameraPreview');
        const canvas = document.createElement('canvas');

        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;

        canvas.getContext('2d').drawImage(video, 0, 0);

        return canvas.toDataURL("image/png");
    }
};

// Global Alert System for Railway Ticket Booking
// Provides attractive, informative alert messages matching the design system

window.showAlert = function (message, type = 'info', duration = 4000) {
    // Remove any existing alerts
    const existingAlerts = document.querySelectorAll('.custom-alert-container');
    existingAlerts.forEach(alert => alert.remove());

    // Create alert container
    const alertContainer = document.createElement('div');
    alertContainer.className = 'custom-alert-container';
    
    // Set alert type and icon
    let iconClass = 'fas fa-info-circle';
    let alertClass = 'alert-info';
    
    switch (type.toLowerCase()) {
        case 'success':
            iconClass = 'fas fa-check-circle';
            alertClass = 'alert-success';
            break;
        case 'error':
        case 'danger':
            iconClass = 'fas fa-exclamation-circle';
            alertClass = 'alert-error';
            break;
        case 'warning':
            iconClass = 'fas fa-exclamation-triangle';
            alertClass = 'alert-warning';
            break;
        case 'info':
        default:
            iconClass = 'fas fa-info-circle';
            alertClass = 'alert-info';
            break;
    }

    // Create alert HTML
    alertContainer.innerHTML = `
        <div class="custom-alert ${alertClass}">
            <div class="alert-content">
                <div class="alert-icon">
                    <i class="${iconClass}"></i>
                </div>
                <div class="alert-message">
                    <strong>${getAlertTitle(type)}</strong>
                    <p>${message}</p>
                </div>
                <button class="alert-close" onclick="this.closest('.custom-alert-container').remove()">
                    <i class="fas fa-times"></i>
                </button>
            </div>
            <div class="alert-progress"></div>
        </div>
    `;

    // Append to body
    document.body.appendChild(alertContainer);

    // Trigger animation
    setTimeout(() => {
        alertContainer.classList.add('show');
    }, 10);

    // Auto-remove after duration
    if (duration > 0) {
        const progressBar = alertContainer.querySelector('.alert-progress');
        progressBar.style.animationDuration = duration + 'ms';
        
        setTimeout(() => {
            alertContainer.classList.remove('show');
            setTimeout(() => {
                if (alertContainer.parentNode) {
                    alertContainer.remove();
                }
            }, 300);
        }, duration);
    }
};

// Helper function to get alert title
function getAlertTitle(type) {
    switch (type.toLowerCase()) {
        case 'success':
            return 'Success!';
        case 'error':
        case 'danger':
            return 'Error!';
        case 'warning':
            return 'Warning!';
        case 'info':
        default:
            return 'Information';
    }
}

// Convenience functions
window.showSuccess = function (message, duration = 4000) {
    window.showAlert(message, 'success', duration);
};

window.showError = function (message, duration = 5000) {
    window.showAlert(message, 'error', duration);
};

window.showWarning = function (message, duration = 4000) {
    window.showAlert(message, 'warning', duration);
};

window.showInfo = function (message, duration = 4000) {
    window.showAlert(message, 'info', duration);
};

// Backward compatibility - replace default alert
window.alert = function (message) {
    window.showInfo(message, 4000);
};


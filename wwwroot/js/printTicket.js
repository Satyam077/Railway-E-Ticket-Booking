// Print Ticket Functionality for Railway Ticket Booking
// Provides a clean, printable ticket format with all booking details

window.printTicket = function (bookingDataJson) {
    // Parse JSON if it's a string
    let bookingData;
    if (typeof bookingDataJson === 'string') {
        try {
            bookingData = JSON.parse(bookingDataJson);
        } catch (e) {
            console.error('Error parsing booking data:', e);
            bookingData = {};
        }
    } else {
        bookingData = bookingDataJson;
    }
    
    // Create a new window for printing
    const printWindow = window.open('', '_blank', 'width=800,height=600');
    
    if (!printWindow) {
        console.error('Popup blocked. Please allow popups to print tickets.');
        alert('Please allow popups to print tickets.');
        return false;
    }

    // Build HTML content for the ticket
    const ticketHTML = `
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Railway Ticket - ${bookingData.pnr || 'N/A'}</title>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }
        
        body {
            font-family: 'Arial', sans-serif;
            background: white;
            color: #333;
            padding: 20px;
        }
        
        .ticket-container {
            max-width: 800px;
            margin: 0 auto;
            border: 3px solid #1a73e8;
            border-radius: 10px;
            overflow: hidden;
            box-shadow: 0 4px 20px rgba(0,0,0,0.1);
        }
        
        .ticket-header {
            background: linear-gradient(135deg, #1a73e8 0%, #0d47a1 100%);
            color: white;
            padding: 20px;
            text-align: center;
        }
        
        .ticket-header h1 {
            font-size: 28px;
            margin-bottom: 10px;
            text-transform: uppercase;
            letter-spacing: 2px;
        }
        
        .ticket-header .subtitle {
            font-size: 14px;
            opacity: 0.9;
        }
        
        .ticket-body {
            padding: 30px;
            background: white;
        }
        
        .ticket-section {
            margin-bottom: 30px;
            border-bottom: 2px dashed #e0e0e0;
            padding-bottom: 20px;
        }
        
        .ticket-section:last-child {
            border-bottom: none;
        }
        
        .section-title {
            font-size: 18px;
            font-weight: bold;
            color: #1a73e8;
            margin-bottom: 15px;
            text-transform: uppercase;
            border-bottom: 2px solid #1a73e8;
            padding-bottom: 5px;
        }
        
        .info-grid {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 15px;
            margin-bottom: 15px;
        }
        
        .info-item {
            display: flex;
            flex-direction: column;
        }
        
        .info-label {
            font-size: 11px;
            color: #666;
            text-transform: uppercase;
            margin-bottom: 5px;
            font-weight: 600;
        }
        
        .info-value {
            font-size: 16px;
            font-weight: bold;
            color: #333;
        }
        
        .route-section {
            background: #f5f5f5;
            padding: 20px;
            border-radius: 8px;
            margin: 20px 0;
        }
        
        .route-display {
            display: flex;
            align-items: center;
            justify-content: space-between;
            margin: 15px 0;
        }
        
        .station-box {
            flex: 1;
            text-align: center;
            padding: 15px;
            background: white;
            border-radius: 5px;
            border: 2px solid #1a73e8;
        }
        
        .station-name {
            font-size: 20px;
            font-weight: bold;
            color: #1a73e8;
            margin-bottom: 5px;
        }
        
        .station-time {
            font-size: 14px;
            color: #666;
        }
        
        .arrow {
            font-size: 30px;
            color: #1a73e8;
            margin: 0 20px;
        }
        
        .passenger-table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 15px;
        }
        
        .passenger-table th {
            background: #1a73e8;
            color: white;
            padding: 12px;
            text-align: left;
            font-size: 12px;
            text-transform: uppercase;
        }
        
        .passenger-table td {
            padding: 12px;
            border-bottom: 1px solid #e0e0e0;
            font-size: 14px;
        }
        
        .passenger-table tr:hover {
            background: #f5f5f5;
        }
        
        .payment-summary {
            background: #f9f9f9;
            padding: 20px;
            border-radius: 8px;
            margin-top: 20px;
        }
        
        .payment-row {
            display: flex;
            justify-content: space-between;
            padding: 8px 0;
            border-bottom: 1px solid #e0e0e0;
        }
        
        .payment-row:last-child {
            border-bottom: none;
            border-top: 2px solid #1a73e8;
            margin-top: 10px;
            padding-top: 15px;
        }
        
        .payment-label {
            font-size: 14px;
            color: #666;
        }
        
        .payment-value {
            font-size: 16px;
            font-weight: bold;
            color: #333;
        }
        
        .total-amount {
            font-size: 24px;
            color: #1a73e8;
        }
        
        .footer {
            text-align: center;
            padding: 20px;
            background: #f5f5f5;
            color: #666;
            font-size: 12px;
        }
        
        .status-badge {
            display: inline-block;
            padding: 5px 15px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: bold;
            text-transform: uppercase;
        }
        
        .status-confirmed {
            background: #28a745;
            color: white;
        }
        
        .status-pending {
            background: #ffc107;
            color: #333;
        }
        
        .status-cancelled {
            background: #dc3545;
            color: white;
        }
        
        .barcode {
            text-align: center;
            padding: 20px;
            font-family: 'Courier New', monospace;
            font-size: 24px;
            letter-spacing: 3px;
            background: #f5f5f5;
            margin: 20px 0;
        }
        
        @media print {
            body {
                padding: 0;
            }
            
            .ticket-container {
                border: none;
                box-shadow: none;
            }
            
            .no-print {
                display: none;
            }
            
            @page {
                margin: 0.5cm;
                size: A4;
            }
        }
        
        .print-actions {
            text-align: center;
            padding: 20px;
            background: #f5f5f5;
        }
        
        .print-btn {
            background: #1a73e8;
            color: white;
            border: none;
            padding: 12px 30px;
            font-size: 16px;
            border-radius: 5px;
            cursor: pointer;
            margin: 0 10px;
        }
        
        .print-btn:hover {
            background: #0d47a1;
        }
    </style>
</head>
<body>
    <div class="ticket-container">
        <div class="ticket-header">
            <h1>🛤️ Indian Railways</h1>
            <div class="subtitle">E-Ticket / Electronic Reservation Slip</div>
        </div>
        
        <div class="ticket-body">
            <!-- PNR and Status -->
            <div class="ticket-section">
                <div class="info-grid">
                    <div class="info-item">
                        <span class="info-label">PNR Number</span>
                        <span class="info-value">${bookingData.pnr || 'N/A'}</span>
                    </div>
                    <div class="info-item">
                        <span class="info-label">Booking Status</span>
                        <span class="info-value">
                            <span class="status-badge status-${(bookingData.status || '').toLowerCase()}">
                                ${bookingData.status || 'N/A'}
                            </span>
                        </span>
                    </div>
                    <div class="info-item">
                        <span class="info-label">Booking Date</span>
                        <span class="info-value">${bookingData.bookingDate || 'N/A'}</span>
                    </div>
                    <div class="info-item">
                        <span class="info-label">Journey Date</span>
                        <span class="info-value">${bookingData.journeyDate || 'N/A'}</span>
                    </div>
                </div>
                <div class="barcode">
                    ${bookingData.pnr || 'N/A'}
                </div>
            </div>
            
            <!-- Train Details -->
            <div class="ticket-section">
                <div class="section-title">Train Details</div>
                <div class="info-grid">
                    <div class="info-item">
                        <span class="info-label">Train Name</span>
                        <span class="info-value">${bookingData.trainName || 'N/A'}</span>
                    </div>
                    <div class="info-item">
                        <span class="info-label">Train Number</span>
                        <span class="info-value">${bookingData.trainNumber || 'N/A'}</span>
                    </div>
                    <div class="info-item">
                        <span class="info-label">Train Type</span>
                        <span class="info-value">${bookingData.trainType || 'N/A'}</span>
                    </div>
                    <div class="info-item">
                        <span class="info-label">Class</span>
                        <span class="info-value">${bookingData.selectedClass || 'N/A'}</span>
                    </div>
                </div>
            </div>
            
            <!-- Route Details -->
            <div class="ticket-section">
                <div class="section-title">Journey Details</div>
                <div class="route-section">
                    <div class="route-display">
                        <div class="station-box">
                            <div class="station-name">${bookingData.sourceStation || 'N/A'}</div>
                            <div class="station-time">${bookingData.departureTime || 'N/A'}</div>
                        </div>
                        <div class="arrow">→</div>
                        <div class="station-box">
                            <div class="station-name">${bookingData.destinationStation || 'N/A'}</div>
                            <div class="station-time">${bookingData.arrivalTime || 'N/A'}</div>
                        </div>
                    </div>
                </div>
            </div>
            
            <!-- Passenger Details -->
            <div class="ticket-section">
                <div class="section-title">Passenger Details</div>
                <table class="passenger-table">
                    <thead>
                        <tr>
                            <th>#</th>
                            <th>Name</th>
                            <th>Age</th>
                            <th>Gender</th>
                            <th>Seat/Coach</th>
                            <th>Berth</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${bookingData.passengers ? bookingData.passengers.map((p, index) => `
                            <tr>
                                <td>${index + 1}</td>
                                <td><strong>${p.fullName || p.firstName + ' ' + p.lastName}</strong></td>
                                <td>${p.age || 'N/A'}</td>
                                <td>${p.gender || 'N/A'}</td>
                                <td>${p.seatNumber || 'N/A'}</td>
                                <td>${p.berthPreference || 'N/A'}</td>
                            </tr>
                        `).join('') : '<tr><td colspan="6" style="text-align: center;">No passengers</td></tr>'}
                    </tbody>
                </table>
            </div>
            
            <!-- Payment Details -->
            <div class="ticket-section">
                <div class="section-title">Payment Summary</div>
                <div class="payment-summary">
                    <div class="payment-row">
                        <span class="payment-label">Base Fare</span>
                        <span class="payment-value">₹${bookingData.baseFare || '0.00'}</span>
                    </div>
                    ${bookingData.superfastCharge > 0 ? `
                    <div class="payment-row">
                        <span class="payment-label">Superfast Charge</span>
                        <span class="payment-value">₹${bookingData.superfastCharge || '0.00'}</span>
                    </div>
                    ` : ''}
                    ${bookingData.discountAmount > 0 ? `
                    <div class="payment-row">
                        <span class="payment-label">Discount</span>
                        <span class="payment-value" style="color: #28a745;">-₹${bookingData.discountAmount || '0.00'}</span>
                    </div>
                    ` : ''}
                    <div class="payment-row">
                        <span class="payment-label">Tax (18% GST)</span>
                        <span class="payment-value">₹${bookingData.taxAmount || '0.00'}</span>
                    </div>
                    <div class="payment-row">
                        <span class="payment-label">Service Charge</span>
                        <span class="payment-value">₹${bookingData.serviceCharge || '0.00'}</span>
                    </div>
                    <div class="payment-row">
                        <span class="payment-label total-amount">Total Amount</span>
                        <span class="payment-value total-amount">₹${bookingData.totalAmount || '0.00'}</span>
                    </div>
                    ${bookingData.paymentStatus ? `
                    <div class="payment-row" style="margin-top: 15px; padding-top: 15px; border-top: 1px solid #e0e0e0;">
                        <span class="payment-label">Payment Status</span>
                        <span class="payment-value">
                            <span class="status-badge status-${(bookingData.paymentStatus || '').toLowerCase()}">
                                ${bookingData.paymentStatus || 'N/A'}
                            </span>
                        </span>
                    </div>
                    ` : ''}
                </div>
            </div>
            
            <!-- Contact Information -->
            <div class="ticket-section">
                <div class="section-title">Contact Information</div>
                <div class="info-grid">
                    <div class="info-item">
                        <span class="info-label">Email</span>
                        <span class="info-value">${bookingData.contactEmail || 'N/A'}</span>
                    </div>
                    <div class="info-item">
                        <span class="info-label">Phone</span>
                        <span class="info-value">${bookingData.contactPhone || 'N/A'}</span>
                    </div>
                </div>
            </div>
        </div>
        
        <div class="footer">
            <p><strong>Important Instructions:</strong></p>
            <p>1. This is an E-Ticket. Please carry a valid ID proof while traveling.</p>
            <p>2. Please arrive at the station at least 30 minutes before departure.</p>
            <p>3. In case of cancellation, refund will be processed as per railway rules.</p>
            <p style="margin-top: 15px; font-size: 10px;">This is a computer-generated ticket. No signature required.</p>
        </div>
    </div>
    
    <div class="print-actions no-print">
        <button class="print-btn" onclick="window.print()">
            🖨️ Print Ticket
        </button>
        <button class="print-btn" onclick="window.close()" style="background: #6c757d;">
            ✕ Close
        </button>
    </div>
    
    <script>
        // Auto-print when window loads (optional - can be disabled)
        // window.onload = function() {
        //     setTimeout(function() {
        //         window.print();
        //     }, 500);
        // };
    </script>
</body>
</html>
    `;

    // Write content to print window
    printWindow.document.write(ticketHTML);
    printWindow.document.close();
    
    // Wait for content to load, then trigger print dialog
    printWindow.onload = function() {
        setTimeout(function() {
            printWindow.print();
        }, 250);
    };
    
    return true;
};

// Alternative function to print current page (simpler approach)
window.printCurrentTicket = function() {
    // Hide elements that shouldn't be printed
    const style = document.createElement('style');
    style.textContent = \`
        @media print {
            .no-print,
            button,
            .btn,
            nav,
            .navbar,
            .card-header:has(button) {
                display: none !important;
            }
            
            .ticket-container {
                border: 2px solid #1a73e8 !important;
            }
            
            body {
                background: white !important;
            }
            
            @page {
                margin: 1cm;
                size: A4;
            }
        }
    \`;
    document.head.appendChild(style);
    
    // Trigger print
    window.print();
    
    // Remove style after printing
    setTimeout(() => {
        document.head.removeChild(style);
    }, 1000);
};

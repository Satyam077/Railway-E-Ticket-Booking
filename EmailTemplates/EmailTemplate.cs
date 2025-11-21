using Railway_Ticket_Booking.Domain.Entities;
using Railway_Ticket_Booking.Domain.Enums;

namespace Railway_Ticket_Booking.EmailTemplates
{
    public static class EmailTemplate
    {
        public static string GenerateRegistrationEmail(User user, UserRole role)
        {
            var roleName = role switch
            {
                UserRole.SuperAdmin => "Super Administrator",
                UserRole.Admin => "Administrator",
                UserRole.ZonalManager => "Zonal Manager",
                UserRole.Customer => "Customer",
                _ => "User"
            };

            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #0066cc; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border: 1px solid #ddd; }}
        .footer {{ background-color: #333; color: white; padding: 15px; text-align: center; border-radius: 0 0 5px 5px; font-size: 12px; }}
        .info-table {{ width: 100%; margin: 20px 0; }}
        .info-table td {{ padding: 8px; border-bottom: 1px solid #eee; }}
        .info-table td:first-child {{ font-weight: bold; width: 40%; }}
        .button {{ display: inline-block; padding: 12px 30px; background-color: #0066cc; color: white; text-decoration: none; border-radius: 5px; margin-top: 20px; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Welcome to Railway Ticket Booking System</h1>
        </div>
        <div class=""content"">
            <h2>Hello {user.FirstName}!</h2>
            <p>Your account has been successfully created. We're excited to have you on board!</p>
            
            <h3>Account Details:</h3>
            <table class=""info-table"">
                <tr>
                    <td>Full Name:</td>
                    <td>{user.FullName}</td>
                </tr>
                <tr>
                    <td>Email:</td>
                    <td>{user.Email}</td>
                </tr>
                <tr>
                    <td>Phone Number:</td>
                    <td>{user.PhoneNumber}</td>
                </tr>
                <tr>
                    <td>Role:</td>
                    <td>{roleName}</td>
                </tr>
                <tr>
                    <td>Account Status:</td>
                    <td>{(user.IsActive ? "Active" : "Inactive")}</td>
                </tr>
                <tr>
                    <td>Date of Birth:</td>
                    <td>{user.DateOfBirth:dd MMM, yyyy}</td>
                </tr>
                <tr>
                    <td>Gender:</td>
                    <td>{user.Gender ?? "Not specified"}</td>
                </tr>
                <tr>
                    <td>Address:</td>
                    <td>{user.Address ?? "Not provided"}, {user.City ?? ""}, {user.State ?? ""} - {user.PinCode ?? ""}, {user.Country ?? "India"}</td>
                </tr>
                <tr>
                    <td>Account Created:</td>
                    <td>{user.CreatedAt:dd MMM, yyyy 'at' HH:mm} UTC</td>
                </tr>
            </table>

            <p><strong>Important:</strong> Please keep your login credentials secure and do not share them with anyone.</p>
            
            <p>You can now log in to your account and start booking railway tickets.</p>
            
            <a href=""#"" class=""button"">Login to Your Account</a>
            
            <p style=""margin-top: 30px; padding-top: 20px; border-top: 1px solid #ddd; font-size: 12px; color: #666;"">
                If you did not create this account, please contact our support team immediately.
            </p>
        </div>
        <div class=""footer"">
            <p>&copy; {DateTime.Now.Year} Railway Ticket Booking System. All rights reserved.</p>
            <p>This is an automated email, please do not reply.</p>
        </div>
    </div>
</body>
</html>";
        }

        public static string GenerateBookingConfirmationEmail(
            Booking booking,
            Payment payment,
            Train train,
            TrainSchedule schedule,
            Station sourceStation,
            Station destinationStation)
        {
            var statusBadgeColor = booking.Status switch
            {
                BookingStatus.Confirmed => "#28a745",
                BookingStatus.Pending => "#ffc107",
                BookingStatus.Cancelled => "#dc3545",
                BookingStatus.WaitListed => "#17a2b8",
                _ => "#6c757d"
            };

            var paymentStatusBadgeColor = payment?.Status switch
            {
                PaymentStatus.Completed => "#28a745",
                PaymentStatus.Pending => "#ffc107",
                PaymentStatus.Failed => "#dc3545",
                _ => "#6c757d"
            };

            var sourceStationName = sourceStation != null 
                ? $"{sourceStation.StationName} ({sourceStation.StationCode})" 
                : booking.SourceStationId;
            
            var destinationStationName = destinationStation != null 
                ? $"{destinationStation.StationName} ({destinationStation.StationCode})" 
                : booking.DestinationStationId;

            var trainName = train != null 
                ? $"{train.TrainNumber} - {train.Name}" 
                : booking.TrainId;

            var departureTime = "";
            var arrivalTime = "";

            if (schedule != null)
            {
                var sourceScheduleStation = schedule.Stations?.FirstOrDefault(s => s.StationId == booking.SourceStationId);
                var destScheduleStation = schedule.Stations?.FirstOrDefault(s => s.StationId == booking.DestinationStationId);
                
                if (sourceScheduleStation != null)
                {
                    departureTime = $"{(int)sourceScheduleStation.DepartureTime.TotalHours:D2}:{sourceScheduleStation.DepartureTime.Minutes:D2}";
                }
                
                if (destScheduleStation != null)
                {
                    arrivalTime = $"{(int)destScheduleStation.ArrivalTime.TotalHours:D2}:{destScheduleStation.ArrivalTime.Minutes:D2}";
                }
            }

            var passengersHtml = "";
            if (booking.Passengers != null && booking.Passengers.Any())
            {
                passengersHtml = string.Join("", booking.Passengers.Select((p, index) =>
                {
                    var seat = booking.Seats?.FirstOrDefault(s => s.PassengerId == p.PassengerId.ToString());
                    var seatInfo = seat != null ? $"{seat.FullSeatNumber}" : "Not Allocated";
                    
                    var badges = "";
                    if (p.IsChild) badges += "<span style='background: #ffc107; color: #000; padding: 2px 6px; border-radius: 3px; font-size: 10px; margin-left: 5px;'>Child</span>";
                    if (p.IsSenior) badges += "<span style='background: #17a2b8; color: #fff; padding: 2px 6px; border-radius: 3px; font-size: 10px; margin-left: 5px;'>Senior</span>";

                    return $@"
                    <tr style='border-bottom: 1px solid #eee;'>
                        <td style='padding: 12px;'><strong>{index + 1}</strong></td>
                        <td style='padding: 12px;'><strong>{p.FullName}</strong>{badges}</td>
                        <td style='padding: 12px;'>{p.Age}</td>
                        <td style='padding: 12px;'>{p.Gender ?? "-"}</td>
                        <td style='padding: 12px;'>
                            {(string.IsNullOrEmpty(p.IdProofType) ? "-" : $"{p.IdProofType}<br/><small>{p.IdProofNumber}</small>")}
                        </td>
                        <td style='padding: 12px;'>
                            {(string.IsNullOrEmpty(p.BerthPreference) ? "-" : $"<span style='background: #6c757d; color: #fff; padding: 4px 8px; border-radius: 3px; font-size: 11px;'>{p.BerthPreference}</span>")}
                        </td>
                        <td style='padding: 12px;'>
                            <span style='background: #007bff; color: #fff; padding: 4px 8px; border-radius: 3px; font-size: 11px;'>{seatInfo}</span>
                        </td>
                    </tr>";
                }));
            }

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; background-color: #f4f4f4; }}
        .container {{ max-width: 800px; margin: 20px auto; background-color: #ffffff; }}
        .header {{ background: linear-gradient(135deg, #28a745 0%, #20c997 100%); color: white; padding: 30px; text-align: center; }}
        .header h1 {{ margin: 0; font-size: 28px; }}
        .header p {{ margin: 10px 0 0 0; font-size: 16px; opacity: 0.9; }}
        .content {{ padding: 30px; }}
        .section {{ margin-bottom: 30px; }}
        .section-title {{ background-color: #f8f9fa; padding: 15px; border-left: 4px solid #007bff; margin-bottom: 20px; }}
        .section-title h2 {{ margin: 0; color: #007bff; font-size: 20px; }}
        .info-grid {{ display: grid; grid-template-columns: 1fr 1fr; gap: 15px; margin-bottom: 20px; }}
        .info-item {{ background-color: #f8f9fa; padding: 15px; border-radius: 5px; }}
        .info-item label {{ display: block; font-size: 12px; color: #666; margin-bottom: 5px; text-transform: uppercase; }}
        .info-item strong {{ font-size: 16px; color: #333; }}
        .badge {{ display: inline-block; padding: 6px 12px; border-radius: 4px; font-size: 12px; font-weight: bold; }}
        .table {{ width: 100%; border-collapse: collapse; margin-top: 15px; }}
        .table th {{ background-color: #007bff; color: white; padding: 12px; text-align: left; font-size: 12px; text-transform: uppercase; }}
        .table td {{ padding: 12px; border-bottom: 1px solid #eee; }}
        .amount-box {{ background-color: #f8f9fa; padding: 20px; border-radius: 5px; margin-top: 20px; }}
        .amount-row {{ display: flex; justify-content: space-between; padding: 8px 0; border-bottom: 1px solid #ddd; }}
        .amount-row.total {{ border-bottom: 2px solid #007bff; font-size: 18px; font-weight: bold; margin-top: 10px; }}
        .footer {{ background-color: #333; color: white; padding: 20px; text-align: center; font-size: 12px; }}
        .pnr-box {{ background: linear-gradient(135deg, #007bff 0%, #0056b3 100%); color: white; padding: 20px; text-align: center; border-radius: 5px; margin: 20px 0; }}
        .pnr-box h2 {{ margin: 0; font-size: 24px; }}
        .pnr-box p {{ margin: 5px 0 0 0; opacity: 0.9; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>🎉 Booking Confirmed!</h1>
            <p>Your railway ticket has been successfully booked</p>
        </div>

        <div class=""content"">
            <!-- PNR Box -->
            <div class=""pnr-box"">
                <p style=""margin: 0; font-size: 14px; opacity: 0.9;"">PNR Number</p>
                <h2 style=""margin: 5px 0; font-size: 32px; letter-spacing: 2px;"">{booking.PNR}</h2>
                <p style=""margin: 5px 0 0 0; font-size: 12px; opacity: 0.8;"">Please save this PNR for future reference</p>
            </div>

            <!-- Booking Summary -->
            <div class=""section"">
                <div class=""section-title"">
                    <h2>📋 Booking Summary</h2>
                </div>
                <div class=""info-grid"">
                    <div class=""info-item"">
                        <label>Booking Status</label>
                        <strong><span class=""badge"" style=""background-color: {statusBadgeColor}; color: white;"">{booking.Status}</span></strong>
                    </div>
                    <div class=""info-item"">
                        <label>Booking Date</label>
                        <strong>{booking.BookingDate:dd MMM, yyyy HH:mm}</strong>
                    </div>
                    <div class=""info-item"">
                        <label>Journey Date</label>
                        <strong>{booking.JourneyDate:dd MMM, yyyy}</strong>
                    </div>
                    <div class=""info-item"">
                        <label>Passengers</label>
                        <strong>{booking.Passengers?.Count ?? 0}</strong>
                    </div>
                </div>
            </div>

            <!-- Train & Journey Details -->
            <div class=""section"">
                <div class=""section-title"">
                    <h2>🚂 Train & Journey Details</h2>
                </div>
                <div class=""info-grid"">
                    <div class=""info-item"" style=""grid-column: 1 / -1;"">
                        <label>Train</label>
                        <strong>{trainName}</strong>
                        {(train != null ? $"<br/><small style='color: #666;'>{train.TrainType}</small>" : "")}
                    </div>
                    <div class=""info-item"">
                        <label>From Station</label>
                        <strong>{sourceStationName}</strong>
                    </div>
                    <div class=""info-item"">
                        <label>To Station</label>
                        <strong>{destinationStationName}</strong>
                    </div>
                    {(string.IsNullOrEmpty(departureTime) ? "" : $@"
                    <div class=""info-item"">
                        <label>Departure Time</label>
                        <strong>{departureTime}</strong>
                    </div>")}
                    {(string.IsNullOrEmpty(arrivalTime) ? "" : $@"
                    <div class=""info-item"">
                        <label>Arrival Time</label>
                        <strong>{arrivalTime}</strong>
                    </div>")}
                </div>
            </div>

            <!-- Passenger Details -->
            <div class=""section"">
                <div class=""section-title"">
                    <h2>👥 Passenger Details</h2>
                </div>
                <table class=""table"">
                    <thead>
                        <tr>
                            <th>#</th>
                            <th>Name</th>
                            <th>Age</th>
                            <th>Gender</th>
                            <th>ID Proof</th>
                            <th>Berth</th>
                            <th>Seat</th>
                        </tr>
                    </thead>
                    <tbody>
                        {passengersHtml}
                    </tbody>
                </table>
            </div>

            <!-- Payment Details -->
            <div class=""section"">
                <div class=""section-title"">
                    <h2>💳 Payment Details</h2>
                </div>
                {(payment != null ? $@"
                <div class=""info-grid"">
                    <div class=""info-item"">
                        <label>Transaction ID</label>
                        <strong>{payment.TransactionId}</strong>
                    </div>
                    <div class=""info-item"">
                        <label>Payment Gateway</label>
                        <strong>{payment.GatewayName}</strong>
                    </div>
                    <div class=""info-item"">
                        <label>Payment Status</label>
                        <strong><span class=""badge"" style=""background-color: {paymentStatusBadgeColor}; color: white;"">{payment.Status}</span></strong>
                    </div>
                    {(payment.CompletedAt.HasValue ? $@"
                    <div class=""info-item"">
                        <label>Paid On</label>
                        <strong>{payment.CompletedAt.Value:dd MMM, yyyy HH:mm}</strong>
                    </div>" : "")}
                </div>" : "")}
                
                <div class=""amount-box"">
                    <div class=""amount-row"">
                        <span>Base Fare</span>
                        <strong>₹{booking.TotalAmount:N2}</strong>
                    </div>
                    <div class=""amount-row"">
                        <span>Tax (18%)</span>
                        <strong>₹{booking.TaxAmount:N2}</strong>
                    </div>
                    <div class=""amount-row"">
                        <span>Service Charge</span>
                        <strong>₹{booking.ServiceCharge:N2}</strong>
                    </div>
                    <div class=""amount-row total"">
                        <span>Total Amount</span>
                        <span style=""color: #007bff;"">₹{booking.FinalAmount:N2}</span>
                    </div>
                </div>
            </div>

            <!-- Contact Information -->
            <div class=""section"">
                <div class=""section-title"">
                    <h2>📞 Contact Information</h2>
                </div>
                <div class=""info-grid"">
                    <div class=""info-item"">
                        <label>Email</label>
                        <strong>{booking.ContactEmail}</strong>
                    </div>
                    <div class=""info-item"">
                        <label>Phone</label>
                        <strong>{booking.ContactPhone}</strong>
                    </div>
                </div>
            </div>

            <!-- Important Notes -->
            <div class=""section"" style=""background-color: #fff3cd; padding: 20px; border-radius: 5px; border-left: 4px solid #ffc107;"">
                <h3 style=""margin-top: 0; color: #856404;"">⚠️ Important Information</h3>
                <ul style=""margin: 10px 0; padding-left: 20px; color: #856404;"">
                    <li>Please carry a valid ID proof while traveling</li>
                    <li>Arrive at the station at least 30 minutes before departure</li>
                    <li>Keep this email and PNR number for reference</li>
                    <li>In case of any issues, contact customer support with your PNR</li>
                    <li>Seat numbers are subject to change in case of train composition changes</li>
                </ul>
            </div>
        </div>

        <div class=""footer"">
            <p>&copy; {DateTime.Now.Year} Railway Ticket Booking System. All rights reserved.</p>
            <p>This is an automated email, please do not reply.</p>
            <p style=""margin-top: 10px; opacity: 0.8;"">For support, please contact us at support@railwaybooking.com</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}

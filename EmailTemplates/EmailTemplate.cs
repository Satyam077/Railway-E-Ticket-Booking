using RailwayTicketBooking.Domain.Entities;
using RailwayTicketBooking.Domain.Enums;

namespace RailwayTicketBooking.EmailTemplates
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
            <h1>Booking Confirmed!</h1>
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
                    <h2>Booking Summary</h2>
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
                    <h2>Train & Journey Details</h2>
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
                    <h2>Passenger Details</h2>
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
                    <h2>Payment Details</h2>
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
                        <strong>&#8377; {booking.TotalAmount:N2}</strong>
                    </div>
                    <div class=""amount-row"">
                        <span>Tax (18%)</span>
                        <strong>&#8377; {booking.TaxAmount:N2}</strong>
                    </div>
                    <div class=""amount-row"">
                        <span>Service Charge</span>
                        <strong>&#8377; {booking.ServiceCharge:N2}</strong>
                    </div>
                    <div class=""amount-row total"">
                        <span>Total Amount</span>
                        <span style=""color: #007bff;"">&#8377; {booking.FinalAmount:N2}</span>
                    </div>
                </div>
            </div>

            <!-- Contact Information -->
            <div class=""section"">
                <div class=""section-title"">
                    <h2>Contact Information</h2>
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
                <h3 style=""margin-top: 0; color: #856404;"">Important Information</h3>
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
            <p style=""margin-top: 10px; opacity: 0.8;"">For support, please contact us at uniquextech7@gmail.com</p>
        </div>
    </div>
</body>
</html>";
        }

        public static string GenerateCancellationEmail(
            Booking booking,
            Payment payment,
            Train train,
            TrainSchedule schedule,
            Station sourceStation,
            Station destinationStation,
            decimal refundAmount,
            decimal cancellationCharge)
        {
            var sourceStationName = sourceStation != null 
                ? $"{sourceStation.StationName} ({sourceStation.StationCode})" 
                : booking.SourceStationId;
            
            var destinationStationName = destinationStation != null 
                ? $"{destinationStation.StationName} ({destinationStation.StationCode})" 
                : booking.DestinationStationId;

            var trainName = train != null 
                ? $"{train.TrainNumber} - {train.Name}" 
                : booking.TrainId;

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; background-color: #f4f4f4; }}
        .container {{ max-width: 800px; margin: 20px auto; background-color: #ffffff; }}
        .header {{ background: linear-gradient(135deg, #dc3545 0%, #c82333 100%); color: white; padding: 30px; text-align: center; }}
        .header h1 {{ margin: 0; font-size: 28px; }}
        .header p {{ margin: 10px 0 0 0; font-size: 16px; opacity: 0.9; }}
        .content {{ padding: 30px; }}
        .section {{ margin-bottom: 30px; }}
        .section-title {{ background-color: #f8f9fa; padding: 15px; border-left: 4px solid #dc3545; margin-bottom: 20px; }}
        .section-title h2 {{ margin: 0; color: #dc3545; font-size: 20px; }}
        .info-grid {{ display: grid; grid-template-columns: 1fr 1fr; gap: 15px; margin-bottom: 20px; }}
        .info-item {{ background-color: #f8f9fa; padding: 15px; border-radius: 5px; }}
        .info-item label {{ display: block; font-size: 12px; color: #666; margin-bottom: 5px; text-transform: uppercase; }}
        .info-item strong {{ font-size: 16px; color: #333; }}
        .pnr-box {{ background: linear-gradient(135deg, #dc3545 0%, #c82333 100%); color: white; padding: 20px; text-align: center; border-radius: 5px; margin: 20px 0; }}
        .pnr-box h2 {{ margin: 0; font-size: 24px; }}
        .refund-box {{ background-color: #d4edda; border: 2px solid #28a745; padding: 20px; border-radius: 5px; margin: 20px 0; }}
        .refund-box h3 {{ margin: 0 0 10px 0; color: #155724; }}
        .amount-row {{ display: flex; justify-content: space-between; padding: 8px 0; border-bottom: 1px solid #ddd; }}
        .amount-row.total {{ border-bottom: 2px solid #28a745; font-size: 18px; font-weight: bold; margin-top: 10px; color: #155724; }}
        .footer {{ background-color: #333; color: white; padding: 20px; text-align: center; font-size: 12px; }}
        .warning-box {{ background-color: #fff3cd; padding: 20px; border-radius: 5px; border-left: 4px solid #ffc107; margin: 20px 0; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Booking Cancelled</h1>
            <p>Your booking has been successfully cancelled</p>
        </div>

        <div class=""content"">
            <!-- PNR Box -->
            <div class=""pnr-box"">
                <p style=""margin: 0; font-size: 14px; opacity: 0.9;"">PNR Number</p>
                <h2 style=""margin: 5px 0; font-size: 32px; letter-spacing: 2px;"">{booking.PNR}</h2>
                <p style=""margin: 5px 0 0 0; font-size: 12px; opacity: 0.8;"">Cancelled on: {booking.CancellationDate?.ToString("dd MMM, yyyy HH:mm") ?? DateTime.UtcNow.ToString("dd MMM, yyyy HH:mm")}</p>
            </div>

            <!-- Cancellation Details -->
            <div class=""section"">
                <div class=""section-title"">
                    <h2>Cancellation Details</h2>
                </div>
                <div class=""info-grid"">
                    <div class=""info-item"">
                        <label>Original Booking Amount</label>
                        <strong>?{booking.FinalAmount:N2}</strong>
                    </div>
                    <div class=""info-item"">
                        <label>Cancellation Charges</label>
                        <strong style=""color: #dc3545;"">?{cancellationCharge:N2}</strong>
                    </div>
                    <div class=""info-item"">
                        <label>Refund Amount</label>
                        <strong style=""color: #28a745; font-size: 20px;"">?{refundAmount:N2}</strong>
                    </div>
                    <div class=""info-item"">
                        <label>Refund Processing Time</label>
                        <strong>5-7 business days</strong>
                    </div>
                </div>
                {(string.IsNullOrEmpty(booking.CancellationReason) ? "" : $@"
                    <div class=""info-item"" style=""grid-column: 1 / -1; margin-top: 15px;"">
                        <label>Reason for Cancellation</label>
                        <strong>{booking.CancellationReason}</strong>
                    </div>")}
            </div>

            <!-- Booking Information -->
            <div class=""section"">
                <div class=""section-title"">
                    <h2>Booking Information</h2>
                </div>
                <div class=""info-grid"">
                    <div class=""info-item"" style=""grid-column: 1 / -1;"">
                        <label>Train</label>
                        <strong>{trainName}</strong>
                    </div>
                    <div class=""info-item"">
                        <label>From Station</label>
                        <strong>{sourceStationName}</strong>
                    </div>
                    <div class=""info-item"">
                        <label>To Station</label>
                        <strong>{destinationStationName}</strong>
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

            <!-- Refund Information -->
            <div class=""refund-box"">
                <h3>Refund Information</h3>
                <div class=""amount-row"">
                    <span>Original Amount Paid</span>
                    <strong>?{booking.FinalAmount:N2}</strong>
                </div>
                <div class=""amount-row"">
                    <span>Cancellation Charges</span>
                    <strong style=""color: #dc3545;"">-?{cancellationCharge:N2}</strong>
                </div>
                <div class=""amount-row total"">
                    <span>Refund Amount</span>
                    <span style=""color: #28a745; font-size: 22px;"">?{refundAmount:N2}</span>
                </div>
                <p style=""margin-top: 15px; color: #155724; font-size: 14px;"">
                    <strong>Note:</strong> The refund will be processed to your original payment method within 5-7 business days. 
                    You will receive a confirmation email once the refund is processed.
                </p>
            </div>

            <!-- Important Notes -->
            <div class=""warning-box"">
                <h3 style=""margin-top: 0; color: #856404;"">?? Important Information</h3>
                <ul style=""margin: 10px 0; padding-left: 20px; color: #856404;"">
                    <li>Your booking has been cancelled successfully</li>
                    <li>Refund will be credited to your original payment method</li>
                    <li>Refund processing time: 5-7 business days</li>
                    <li>You will receive a refund confirmation email once processed</li>
                    <li>For any queries, contact support with your PNR: <strong>{booking.PNR}</strong></li>
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

        public static string GenerateRefundEmail(
            Booking booking,
            Payment payment,
            Train train,
            Station sourceStation,
            Station destinationStation,
            decimal refundAmount,
            string refundTransactionId)
        {
            var sourceStationName = sourceStation != null 
                ? $"{sourceStation.StationName} ({sourceStation.StationCode})" 
                : booking.SourceStationId;
            
            var destinationStationName = destinationStation != null 
                ? $"{destinationStation.StationName} ({destinationStation.StationCode})" 
                : booking.DestinationStationId;

            var trainName = train != null 
                ? $"{train.TrainNumber} - {train.Name}" 
                : booking.TrainId;

            var refundCompletedDate = payment?.Refund?.RefundCompletedAt?.ToString("dd MMM, yyyy HH:mm") 
                ?? DateTime.UtcNow.ToString("dd MMM, yyyy HH:mm");

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
        .section-title {{ background-color: #f8f9fa; padding: 15px; border-left: 4px solid #28a745; margin-bottom: 20px; }}
        .section-title h2 {{ margin: 0; color: #28a745; font-size: 20px; }}
        .info-grid {{ display: grid; grid-template-columns: 1fr 1fr; gap: 15px; margin-bottom: 20px; }}
        .info-item {{ background-color: #f8f9fa; padding: 15px; border-radius: 5px; }}
        .info-item label {{ display: block; font-size: 12px; color: #666; margin-bottom: 5px; text-transform: uppercase; }}
        .info-item strong {{ font-size: 16px; color: #333; }}
        .pnr-box {{ background: linear-gradient(135deg, #28a745 0%, #20c997 100%); color: white; padding: 20px; text-align: center; border-radius: 5px; margin: 20px 0; }}
        .pnr-box h2 {{ margin: 0; font-size: 24px; }}
        .refund-box {{ background-color: #d4edda; border: 2px solid #28a745; padding: 25px; border-radius: 5px; margin: 20px 0; text-align: center; }}
        .refund-box h3 {{ margin: 0 0 15px 0; color: #155724; font-size: 24px; }}
        .refund-amount {{ font-size: 36px; font-weight: bold; color: #28a745; margin: 15px 0; }}
        .footer {{ background-color: #333; color: white; padding: 20px; text-align: center; font-size: 12px; }}
        .success-box {{ background-color: #d1ecf1; padding: 20px; border-radius: 5px; border-left: 4px solid #17a2b8; margin: 20px 0; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Refund Processed Successfully!</h1>
            <p>Your refund has been credited to your account</p>
        </div>

        <div class=""content"">
            <!-- PNR Box -->
            <div class=""pnr-box"">
                <p style=""margin: 0; font-size: 14px; opacity: 0.9;"">PNR Number</p>
                <h2 style=""margin: 5px 0; font-size: 32px; letter-spacing: 2px;"">{booking.PNR}</h2>
                <p style=""margin: 5px 0 0 0; font-size: 12px; opacity: 0.8;"">Refund Completed: {refundCompletedDate}</p>
            </div>

            <!-- Refund Amount Box -->
            <div class=""refund-box"">
                <h3>Refund Amount</h3>
                <div class=""refund-amount"">?{refundAmount:N2}</div>
                <p style=""color: #155724; margin: 10px 0 0 0;"">
                    This amount has been credited to your original payment method
                </p>
            </div>

            <!-- Refund Details -->
            <div class=""section"">
                <div class=""section-title"">
                    <h2>Refund Details</h2>
                </div>
                <div class=""info-grid"">
                    <div class=""info-item"">
                        <label>Refund Transaction ID</label>
                        <strong>{refundTransactionId}</strong>
                    </div>
                    <div class=""info-item"">
                        <label>Refund Status</label>
                        <strong style=""color: #28a745;"">Completed</strong>
                    </div>
                    <div class=""info-item"">
                        <label>Refund Amount</label>
                        <strong style=""color: #28a745; font-size: 18px;"">?{refundAmount:N2}</strong>
                    </div>
                    <div class=""info-item"">
                        <label>Refund Completed On</label>
                        <strong>{refundCompletedDate}</strong>
                    </div>
                    {(payment?.Refund?.RefundInitiatedAt != null ? $@"
                    <div class=""info-item"">
                        <label>Refund Initiated On</label>
                        <strong>{payment.Refund.RefundInitiatedAt:dd MMM, yyyy HH:mm}</strong>
                    </div>" : "")}
                    <div class=""info-item"">
                        <label>Payment Gateway</label>
                        <strong>{payment?.GatewayName ?? "PayU"}</strong>
                    </div>
                </div>
            </div>

            <!-- Booking Information -->
            <div class=""section"">
                <div class=""section-title"">
                    <h2>Cancelled Booking Details</h2>
                </div>
                <div class=""info-grid"">
                    <div class=""info-item"" style=""grid-column: 1 / -1;"">
                        <label>Train</label>
                        <strong>{trainName}</strong>
                    </div>
                    <div class=""info-item"">
                        <label>From Station</label>
                        <strong>{sourceStationName}</strong>
                    </div>
                    <div class=""info-item"">
                        <label>To Station</label>
                        <strong>{destinationStationName}</strong>
                    </div>
                    <div class=""info-item"">
                        <label>Journey Date</label>
                        <strong>{booking.JourneyDate:dd MMM, yyyy}</strong>
                    </div>
                    <div class=""info-item"">
                        <label>Cancellation Date</label>
                        <strong>{booking.CancellationDate?.ToString("dd MMM, yyyy HH:mm") ?? "N/A"}</strong>
                    </div>
                </div>
            </div>

            <!-- Success Message -->
            <div class=""success-box"">
                <h3 style=""margin-top: 0; color: #0c5460;"">? Refund Credited</h3>
                <p style=""color: #0c5460; margin: 10px 0;"">
                    Your refund of <strong>?{refundAmount:N2}</strong> has been successfully processed and credited to your original payment method.
                    The amount should reflect in your account within 1-2 business days depending on your bank.
                </p>
                <p style=""color: #0c5460; margin: 10px 0 0 0;"">
                    <strong>Refund Transaction ID:</strong> {refundTransactionId}
                </p>
            </div>

            <!-- Important Notes -->
            <div class=""section"" style=""background-color: #fff3cd; padding: 20px; border-radius: 5px; border-left: 4px solid #ffc107;"">
                <h3 style=""margin-top: 0; color: #856404;"">?? Important Information</h3>
                <ul style=""margin: 10px 0; padding-left: 20px; color: #856404;"">
                    <li>The refund has been processed to your original payment method</li>
                    <li>It may take 1-2 business days for the amount to reflect in your account</li>
                    <li>Keep this email and refund transaction ID for your records</li>
                    <li>For any queries, contact support with your PNR: <strong>{booking.PNR}</strong></li>
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

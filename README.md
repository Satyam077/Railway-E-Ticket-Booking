# 🚂 Railway Ticket Booking System

A comprehensive, full-featured railway ticket booking system built with **Blazor Server** and **.NET 8**. This system provides a complete solution for managing train bookings, payments, user authentication, and administrative operations.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![Blazor](https://img.shields.io/badge/Blazor-Server-512BD4?logo=blazor)
![MongoDB](https://img.shields.io/badge/MongoDB-4.4-47A248?logo=mongodb)
![License](https://img.shields.io/badge/license-MIT-blue)

## 📋 Table of Contents

- [Features](#-features)
- [Technology Stack](#-technology-stack)
- [Architecture](#-architecture)
- [Prerequisites](#-prerequisites)
- [Installation](#-installation)
- [Configuration](#-configuration)
- [Usage](#-usage)
- [Project Structure](#-project-structure)
- [API Endpoints](#-api-endpoints)
- [Authentication & Authorization](#-authentication--authorization)
- [Payment Integration](#-payment-integration)
- [Contributing](#-contributing)
- [License](#-license)

## ✨ Features

### 🎫 **Booking Management**
- **Train Search**: Advanced search with filters (source, destination, date, class)
- **Real-time Seat Availability**: Dynamic seat availability calculation based on confirmed bookings
- **Multi-Passenger Booking**: Book tickets for multiple passengers in a single transaction
- **Class Selection**: Support for multiple classes (Sleeper, AC 2-Tier, AC 3-Tier, AC First Class, etc.)
- **PNR Status Check**: Check booking status using PNR number
- **Booking History**: View and manage all past and current bookings
- **Ticket Printing**: Professional ticket printing with all booking details

### 💰 **Fare Calculation**
- **Distance-Based Pricing**: Per-kilometer fare calculation for each class
- **Minimum Fare Protection**: Ensures minimum base fare for short distances
- **Superfast Charges**: Automatic superfast train surcharge calculation
- **Tax Calculation**: 18% GST on base fare
- **Coupon/Discount System**: Support for promotional codes and manual discounts
- **Service Charges**: Fixed service charge per booking

### 💳 **Payment Integration**
- **PayU Gateway Integration**: Secure payment processing through PayU
- **Multiple Payment Methods**: Support for various payment options
- **Refund Processing**: Automated refund handling for cancellations
- **Payment Status Tracking**: Real-time payment status updates
- **Transaction History**: Complete payment transaction records

### 👥 **User Management**
- **User Registration**: Secure user registration with email verification
- **JWT Authentication**: Token-based authentication system
- **Role-Based Access Control**: Multiple user roles (SuperAdmin, Admin, ZonalManager, Customer)
- **User Profile Management**: Edit profile, view booking history
- **Password Security**: BCrypt password hashing

### 🛠️ **Admin Panel**
- **Train Management**: CRUD operations for trains and classes
- **Route Management**: Create and manage train routes with stations
- **Station Management**: Add, edit, and manage railway stations
- **Schedule Management**: Create and update train schedules
- **Booking Management**: View and manage all bookings
- **User Management**: Admin interface for user management with search, filter, and pagination

### 📧 **Email Notifications**
- **Booking Confirmation**: Automated email on successful booking
- **Payment Confirmation**: Email notifications for payment status
- **Cancellation Notifications**: Refund and cancellation emails
- **Customizable Templates**: HTML email templates

### 🎨 **User Interface**
- **Modern Design**: Beautiful, responsive UI with gradient backgrounds
- **Glassmorphism Effects**: Modern glassmorphism design elements
- **Custom Alerts**: Attractive, informative alert system
- **Mobile Responsive**: Fully responsive design for all devices
- **Accessibility**: WCAG-compliant design

## 🛠️ Technology Stack

### **Backend**
- **.NET 8.0**: Latest .NET framework
- **Blazor Server**: Server-side Blazor for real-time UI updates
- **MongoDB**: NoSQL database for flexible data storage
- **MediatR**: CQRS pattern implementation
- **JWT Authentication**: Secure token-based authentication
- **BCrypt**: Password hashing

### **Frontend**
- **Blazor Components**: Component-based UI framework
- **Bootstrap 5.3**: Responsive CSS framework
- **Font Awesome 6.4**: Icon library
- **Custom CSS**: Modern design with gradients and animations
- **JavaScript Interop**: Client-side functionality

### **Third-Party Integrations**
- **PayU Payment Gateway**: Payment processing
- **MailKit**: Email service integration
- **MongoDB Driver**: Database connectivity

## 🏗️ Architecture

The project follows **Clean Architecture** principles with clear separation of concerns:

```
Railway Ticket Booking/
├── Domain/              # Domain entities and business logic
│   ├── Entities/        # Core domain entities
│   ├── Enums/           # Enumerations
│   └── DTOs/            # Data Transfer Objects
├── Application/         # Application layer (CQRS with MediatR)
│   ├── Commands/        # Write operations
│   ├── Queries/         # Read operations
│   └── Services/        # Application services
├── Infrastructure/      # Infrastructure layer
│   ├── Services/        # External services (JWT, Email, PayU)
│   └── MongoDbContext.cs
├── Pages/               # Blazor pages
│   ├── Admin/           # Admin panel pages
│   ├── Users/           # User-facing pages
│   └── Account/         # Authentication pages
├── Controller/          # API controllers
├── Components/          # Reusable Blazor components
└── wwwroot/             # Static files (CSS, JS, images)
```

### **Design Patterns**
- **CQRS (Command Query Responsibility Segregation)**: Separate read and write operations
- **Repository Pattern**: Data access abstraction
- **Mediator Pattern**: Decoupled communication between components
- **Dependency Injection**: Loose coupling and testability

## 📦 Prerequisites

Before you begin, ensure you have the following installed:

- **.NET 8.0 SDK** or later
- **MongoDB** (local installation or MongoDB Atlas account)
- **Visual Studio 2022** or **VS Code** with C# extension
- **Git** for version control

## 🚀 Installation

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/railway-ticket-booking.git
cd railway-ticket-booking
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Configure MongoDB

Update `appsettings.json` with your MongoDB connection string:

```json
{
  "MongoDb": {
    "ConnectionString": "mongodb://localhost:27017",
    "Database": "RailwayTicket"
  }
}
```

For MongoDB Atlas, use:
```json
{
  "MongoDb": {
    "ConnectionString": "mongodb+srv://username:password@cluster.mongodb.net/",
    "Database": "RailwayTicket"
  }
}
```

### 4. Configure Application Settings

Update the following in `appsettings.json`:

- **JWT Settings**: Secret key, issuer, audience
- **Email Settings**: SMTP configuration for email notifications
- **PayU Settings**: Payment gateway credentials (for production)

### 5. Run the Application

```bash
dotnet run
```

The application will be available at `https://localhost:5001` or `http://localhost:5000`

## ⚙️ Configuration

### **JWT Configuration**

```json
{
  "Jwt": {
    "SecretKey": "your-secret-key-here",
    "Issuer": "RailwayTicketBooking",
    "Audience": "RailwayTicketBookingUsers",
    "ExpirationMinutes": "60"
  }
}
```

### **Email Configuration**

```json
{
  "EmailSettings": {
    "FromName": "Railway Booking",
    "FromEmail": "your-email@gmail.com",
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUser": "your-email@gmail.com",
    "SmtpPass": "your-app-password",
    "UseSsl": true
  }
}
```

### **PayU Configuration**

```json
{
  "PayU": {
    "Key": "your-payu-key",
    "Salt": "your-payu-salt",
    "TestMode": true,
    "SuccessUrl": "http://localhost:5038/api/payu/response",
    "FailureUrl": "http://localhost:5038/api/payu/response",
    "RefundCallbackUrl": "http://localhost:5038/api/payu/refund-callback"
  }
}
```

## 📖 Usage

### **For Users**

1. **Register/Login**: Create an account or login with existing credentials
2. **Search Trains**: Enter source, destination, and travel date
3. **Select Class**: Choose your preferred class (Sleeper, AC, etc.)
4. **Add Passengers**: Enter passenger details
5. **Apply Coupon** (Optional): Enter coupon code for discounts
6. **Make Payment**: Complete payment through PayU gateway
7. **View Booking**: Check booking confirmation and print ticket
8. **PNR Status**: Check booking status using PNR number

### **For Administrators**

1. **Login** as Admin/SuperAdmin
2. **Manage Trains**: Add, edit, or delete trains
3. **Manage Routes**: Create routes with stations
4. **Manage Stations**: Add or update station information
5. **Manage Schedules**: Create train schedules
6. **View Bookings**: Monitor all bookings
7. **Manage Users**: View and manage user accounts

## 📁 Project Structure

```
Railway Ticket Booking/
├── Application/
│   ├── Account/              # User authentication commands/queries
│   ├── Bookings/             # Booking management
│   │   ├── Commands/         # Create, Cancel booking
│   │   ├── Queries/          # Get bookings, PNR status
│   │   └── Services/         # Fare calculation service
│   ├── Routes/               # Route management
│   ├── Stations/             # Station management
│   └── TrainSchedules/       # Schedule management
├── Controller/
│   └── PayuController.cs     # Payment gateway endpoints
├── Domain/
│   ├── Entities/             # Domain models
│   ├── Enums/                # Enumerations
│   └── DTOs/                 # Data transfer objects
├── Infrastructure/
│   ├── Services/             # External services
│   │   ├── JwtService.cs    # JWT token generation
│   │   ├── EmailService.cs   # Email sending
│   │   └── PayuService.cs    # Payment processing
│   └── MongoDbContext.cs     # Database context
├── Pages/
│   ├── Admin/                # Admin panel pages
│   ├── Users/                # User pages
│   └── Account/              # Auth pages
├── Components/               # Reusable components
├── wwwroot/
│   ├── css/                  # Stylesheets
│   └── js/                   # JavaScript files
└── Program.cs                # Application entry point
```

## 🔌 API Endpoints

### **Payment Endpoints**

- `POST /api/payu/create` - Create payment request
- `POST /api/payu/response` - Payment response callback
- `POST /api/payu/refund-callback` - Refund callback

### **Authentication**

- JWT tokens are used for authentication
- Tokens stored in browser localStorage
- Automatic token validation on each request

## 🔐 Authentication & Authorization

### **User Roles**

- **SuperAdmin**: Full system access
- **Admin**: Administrative operations
- **ZonalManager**: Regional management access
- **Customer**: Standard user access

### **Protecting Pages**

Add authorization attributes to pages:

```razor
@page "/admin-page"
@using Microsoft.AspNetCore.Authorization
@attribute [Authorize(Roles = "Admin,SuperAdmin")]
```

### **Available Policies**

- `SuperAdminOnly` - Only SuperAdmin
- `AdminOnly` - Admin or SuperAdmin
- `ZonalManagerOnly` - ZonalManager, Admin, or SuperAdmin
- `CustomerOnly` - Only Customer
- `AuthenticatedUsers` - Any authenticated user

See [AuthorizationGuide.md](AuthorizationGuide.md) for detailed information.

## 💳 Payment Integration

The system integrates with **PayU** payment gateway:

- **Payment Processing**: Secure payment handling
- **Refund Processing**: Automated refunds for cancellations
- **Transaction Tracking**: Complete payment history
- **Status Updates**: Real-time payment status

### **Fare Calculation**

The system calculates fares based on:

- **Distance**: Per-kilometer rates for each class
- **Minimum Fare**: Class-specific minimum charges
- **Superfast Charge**: Additional charges for superfast trains
- **Taxes**: 18% GST on base fare
- **Service Charge**: Fixed ₹20 per booking
- **Discounts**: Coupon codes and manual discounts

## 🧪 Testing

```bash
# Run tests (if available)
dotnet test
```

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

### **Development Guidelines**

- Follow C# coding conventions
- Use meaningful variable and method names
- Add comments for complex logic
- Write unit tests for new features
- Update documentation as needed

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👨‍💻 Author

**Your Name**
- GitHub: [@yourusername](https://github.com/yourusername)
- Email: your.email@example.com

## 🙏 Acknowledgments

- **Blazor** team for the amazing framework
- **MongoDB** for the flexible database solution
- **PayU** for payment gateway integration
- All contributors and open-source libraries used in this project

## 📞 Support

For support, email your.email@example.com or create an issue in the repository.

## 🗺️ Roadmap

- [ ] Mobile app development
- [ ] Real-time notifications
- [ ] Advanced analytics dashboard
- [ ] Multi-language support
- [ ] Social media login integration
- [ ] Booking modification feature
- [ ] Group booking enhancements
- [ ] API documentation with Swagger

---

⭐ If you find this project helpful, please give it a star!


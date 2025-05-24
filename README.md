# Hospital Management System

A comprehensive hospital management application built with C# and .NET.

## Features

### Core Features
- User Authentication System
  - Login/Logout functionality
  - Different user roles (Admin, Doctor, Nurse, Patient)
  - Password recovery
- Patient Management
  - Patient registration
  - Patient profiles with medical history
  - Search/filter patients
  - Patient admission/discharge system
- Appointment Scheduling
  - Doctor availability calendar
  - Appointment booking interface
  - Appointment reminders
  - Rescheduling/cancellation
- Medical Records
  - Digital health records
  - Prescription management
  - Lab test results tracking
  - Medical history viewer

### Advanced Features
- Billing System
- Inventory Management
- Reporting

## Technical Stack
- C# / .NET 6.0
- WPF for desktop UI
- Entity Framework Core for database access
- SQL Server for data storage
- MVVM architecture

## Getting Started

1. Clone this repository
2. Open the solution in Visual Studio
3. Restore NuGet packages
4. Update the connection string in `appsettings.json`
5. Run the application

## Project Structure

- `HospitalApp.Core` - Core business logic and models
- `HospitalApp.Data` - Data access layer with Entity Framework
- `HospitalApp.WPF` - WPF UI application
- `HospitalApp.Tests` - Unit and integration tests


CHANGE
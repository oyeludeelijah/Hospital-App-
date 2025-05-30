# Simple Hospital Management System

A lightweight hospital management application built with C# and Windows Forms.

## Features

- **Patient Management**
  - Add, edit, and delete patient records
  - Search patients by name
  - Store basic patient information and medical history

- **Doctor Management**
  - Add, edit, and delete doctor records
  - Search doctors by name or specialization
  - Manage doctor specializations

- **Appointment Scheduling**
  - Schedule appointments between patients and doctors
  - Filter appointments by date
  - Manage appointment status (Scheduled, Completed, Canceled)

## Technical Details

- Built with C# and .NET 6.0
- Uses Windows Forms for the user interface
- In-memory data storage (no database required)
- Simple and lightweight architecture

## How to Run the Application

1. Open the solution in Visual Studio
2. Build the solution
3. Run the application

## Project Structure

- `Models/` - Contains data model classes (Patient, Doctor, Appointment)
- `Services/` - Contains the DataService for in-memory data management
- `Controls/` - Contains user controls for the different management screens
- `Forms/` - Contains forms for adding/editing records

## Notes

This is a simplified version of the hospital management system, designed to be lightweight and avoid any performance issues or Visual Studio crashes. It uses in-memory storage rather than a database, making it easier to run without any configuration.

The application includes sample data to demonstrate functionality.

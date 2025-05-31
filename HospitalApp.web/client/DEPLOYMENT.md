# Hospital Management System - Deployment Guide

## Mock API Implementation

Your Hospital Management System has been updated with a mock API implementation that allows the frontend to work on Vercel without requiring a backend connection. This solves the "Server did not respond" error you were experiencing.

### How It Works

1. **Environment Detection**
   - The app automatically detects whether it's running in development (localhost) or production (Vercel)
   - In development: Attempts to connect to your local API at http://localhost:5005
   - In production: Uses mock data built into the frontend

2. **API Status Indicator**
   - A small indicator in the bottom-right corner shows the current API status:
   - "Connected to API" (green): Successfully connected to your local backend
   - "API Connection Error" (red): Failed to connect to your local backend
   - "Using Demo Data" (orange): Using mock data (this will always show on Vercel)

3. **Mock Data**
   - The app includes predefined mock data for all modules:
     - Patients
     - Doctors
     - Appointments
     - Departments
     - Billing
   - Authentication works with any credentials in mock mode

## Deployment Steps

1. **Build Your React App**
   ```
   cd HospitalApp.web/client
   npm run build
   ```

2. **Deploy to Vercel**
   - Push your changes to GitHub
   - Vercel will automatically deploy the updated version
   - The app will use mock data in the deployed environment

## Local Development

For local development, you still need to run both the backend and frontend:

1. **Start the Backend (Terminal 1)**
   ```
   cd HospitalApp.web/HospitalApi
   dotnet run
   ```

2. **Start the Frontend (Terminal 2)**
   ```
   cd HospitalApp.web/client
   npm start
   ```

This setup gives you flexibility:
- You can develop locally with real API connections
- Your deployed app works without a backend API
- You can deploy the backend API later when needed

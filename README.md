# EventEase 🎉  
A Cloud-Based Event Management System using ASP.NET Core MVC and Microsoft Azure

## Overview

EventEase is a fully-featured cloud-hosted web application that allows users to manage venues, events, and bookings efficiently. Built with ASP.NET Core MVC and backed by Azure services, EventEase demonstrates modern web development practices, including database normalization, cloud storage, and responsive design.

## Features

✅ **Venue Management**  
- Add, update, delete, and view venues  
- Store venue images in Azure Blob Storage  
- Filter venues by availability

✅ **Event Management**  
- Add, update, delete, and view events  
- Associate events with venues and classify by event type  

✅ **Booking Management**  
- Book venues for events  
- Prevent double bookings on the same date and time  
- Prevent deletion of booked venues or events  
- Consolidated booking view with venue and event details  

✅ **Search & Filtering**  
- Search bookings by Booking ID or Event Name  
- Filter bookings by Event Type, Date Range, and Venue Availability  

✅ **Validation & Error Handling**  
- Alerts for missing required fields  
- Graceful handling of user input errors  

✅ **Cloud Integration**  
- Hosted via Azure Web App  
- Azure SQL Database for relational data  
- Azure Blob Storage for image files  

## Technologies Used

- **Frontend:** HTML5, CSS3, Bootstrap 5, jQuery  
- **Backend:** ASP.NET Core MVC, C#  
- **Database:** Azure SQL Database (with EF Core ORM)  
- **Cloud Services:**  
  - Azure Web App (PaaS)  
  - Azure SQL Database  
  - Azure Blob Storage  
- **IDE:** Visual Studio 2022  

## Getting Started

### Prerequisites
- Visual Studio 2022 with ASP.NET Core support
- Azure subscription
- SQL Server Management Studio (optional)

### Clone the Repository
```bash
git clone https://github.com/your-username/eventease.git

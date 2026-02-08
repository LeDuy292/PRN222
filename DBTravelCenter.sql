-- Tạo database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'DBTravelCenter')
BEGIN
    CREATE DATABASE DBTravelCenter;
END
GO
USE DBTravelCenter;
GO

-- Bảng Trip
IF OBJECT_ID('Trip', 'U') IS NULL
BEGIN
    CREATE TABLE Trip (
        TripID INT IDENTITY(1,1) PRIMARY KEY,
        Code VARCHAR(30) NOT NULL UNIQUE,
        Destination NVARCHAR(200) NOT NULL,
        Price DECIMAL(12,2) NOT NULL CHECK (Price >= 0),
        Status VARCHAR(20) NOT NULL CHECK (Status IN ('Available','Booked'))
    );
END
GO

-- Bảng Customer (thêm Password)
IF OBJECT_ID('Customer', 'U') IS NULL
BEGIN
    CREATE TABLE Customer (
        CustomerID INT IDENTITY(1,1) PRIMARY KEY,
        Code VARCHAR(30) NOT NULL UNIQUE,
        FullName NVARCHAR(150) NOT NULL,
        Email VARCHAR(200) UNIQUE,
        Age INT CHECK (Age >= 0),
        Password NVARCHAR(100) NOT NULL,   -- thêm cột Password
        Role VARCHAR(20) NOT NULL DEFAULT 'User' CHECK (Role IN ('User', 'Admin'))
    );
END
GO

-- Bảng Booking
IF OBJECT_ID('Booking', 'U') IS NULL
BEGIN
    CREATE TABLE Booking (
        BookingID INT IDENTITY(1,1) PRIMARY KEY,
        TripID INT NOT NULL,
        CustomerID INT NOT NULL,
        BookingDate DATE NOT NULL DEFAULT (GETDATE()),
        Status VARCHAR(20) NOT NULL CHECK (Status IN ('Pending','Confirmed','Cancelled')),
        FOREIGN KEY (TripID) REFERENCES Trip(TripID),
        FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID)
    );
END
GO

-- Dữ liệu mẫu
-- Only insert if empty to avoid duplicates on re-run
IF NOT EXISTS (SELECT 1 FROM Trip)
BEGIN
    INSERT INTO Trip (Code, Destination, Price, Status) VALUES
    ('TRP-PAR-01', N'Paris', 2150.00, 'Available'),
    ('TRP-TYO-02', N'Tokyo', 1890.00, 'Available'),
    ('TRP-NYC-03', N'New York', 1590.00, 'Booked'),
    ('TRP-PAR-04', N'Paris', 1990.00, 'Available');
END

IF NOT EXISTS (SELECT 1 FROM Customer)
BEGIN
    INSERT INTO Customer (Code, FullName, Email, Age, Password, Role) VALUES
    ('CUS-001', N'Nguyen Van A', 'a.nguyen@example.com', 28, '123456', 'User'),
    ('CUS-002', N'Tran Thi B',   'b.tran@example.com',   24, 'password1', 'User'),
    ('CUS-003', N'Le Van C',     'c.le@example.com',     31, 'abc@123', 'User'),
    ('CUS-ADM', N'Administrator', 'admin@travel.com',    30, 'admin@123', 'Admin');
END

IF NOT EXISTS (SELECT 1 FROM Booking)
BEGIN
    INSERT INTO Booking (TripID, CustomerID, Status) VALUES
    (1, 1, 'Pending'),
    (2, 2, 'Confirmed'),
    (3, 3, 'Cancelled');
END
GO

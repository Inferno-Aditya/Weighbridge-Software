# VinayakEnterprises — Master Specification
> This file is the single source of truth for this project.
> Always refer back to this file before making any decisions.
> Do not deviate from this spec without explicit user instruction.


Build a desktop weighbridge management application called "VinayakEnterprises" using C# .NET 8 with WPF (MVVM pattern). The app must run on Windows 10 and 11 with a minimum of 4 GB RAM.

--- BRANDING ---
App name: VinayakEnterprises
Company name: VinayakEnterprises
Dark mode accent: saffron #FF6F00. Light mode accent: navy #1A237E.

--- TECH STACK ---
- Framework: C# .NET 8, WPF, MVVM (CommunityToolkit.Mvvm)
- UI library: MaterialDesignThemes for WPF
- Database: SQLite via Microsoft.Data.Sqlite (single .db file at %APPDATA%\VinayakEnterprises\)
- ANPR: Tesseract OCR (offline, background thread)
- Camera: AForge.NET or OpenCvSharp4 (DirectShow)
- PDF: QuestPDF for ticket generation
- Excel: ClosedXML for report exports
- WhatsApp: Selenium WebDriver + ChromeDriver (WA-Web automation, no API key)
- Serial/COM: System.IO.Ports
- Logging: Serilog append-only audit log
- Passwords: BCrypt.Net-Next
- License crypto: System.Security.Cryptography (built-in HMAC-SHA256)
- Performance target: under 150 MB idle RAM, camera at 480p/15fps max

--- DATABASE ---
SQLite with auto-migration on startup. Index on: Date, VehicleNo, TicketNo.
Tables: Users, Roles, Customers, Suppliers, Items, Vehicles, Field01, Field02, Field03,
WBLocations, SlipEntries, AuditLog, CompanyMaster, SystemSettings, HelpCodes,
GoodsDispatch, LicenseInfo.

--- AUTHENTICATION ---
Login screen on startup: Username + Password.
Passwords stored as bcrypt hash. Session auto-lock after 30 min inactivity (configurable).
Log failed attempts to AuditLog. Default admin: admin / admin123 (force password change on first login).
Login screen shows a small license status indicator: "Licensed", "Expiring in X days", or "Expired".

--- USER ROLES (3-tier) ---
OPERATOR: Slip Entry, capture weight, print ticket, view own day tickets, send WhatsApp, Ticket Search (read-only). Cannot see License Info.
TECHNICIAN: All Operator + all Masters (add/edit/delete), COM Port Setup, System Setting, hardware config, Database Backup Config, Help Code management. Cannot access financial reports, delete tickets, or License Info.
OWNER/ADMIN: Full access — User Master, all Reports, Delete Slip Data, Ticket Edit, SAP Export, Data Export, Event Log, Company Master, Server Setting, License Info (view + activate new key).

--- MODULES ---

FILE MENU:
- Server Setting: configure DB path (local or UNC network path)
- Company Master: name, address, logo upload, GST number, phone (used in PDF letterhead)
- System Setting: theme (dark/light), language (EN/HI), camera device index, default printer, session timeout, stable weight threshold
- User Master: create, edit, deactivate users; assign role
- License Info (Owner/Admin only): Machine ID, license type, issue date, expiry date, days remaining, activated by, activation date, vendor contact info, "Enter New License Key" button
- Log Out, Exit

MASTERS (all with full CRUD, keyboard navigation, auto-complete):
- Customer_Name: Code No, Name, Address, City, Phone, Mobile, VAT No, Email, W_Charges, Rate Type, GST No, Is Blacklist
- Supplier_Name: Code No, Name, Address, City, Phone, Mobile, VAT No, Email, W_Charges, Website, Is Blacklist
- Item_Name: Code No, Name, Price, Weight, Unit
- Vehicle_No: Vehicle No, Tare Wt Kg (Get Weight button), RTO Wt Kg (Get Weight button), Tare Date, Max Tare Allow, Min Tare Allow, Is Blacklist
- Field01_Name / Field02_Name / Field03_Name: Code No, Name, Remarks
- WB Location: location name and code
- Com Port Setup: port name, baud rate, data bits, stop bits, parity; USB mode; TCP/IP mode (IP + port); auto-detect option
- Field Setting: rename Field01/02/03 labels globally

TRANSACTION — SLIP ENTRY:
- Large digital weight display (7-segment style font) at top centre; green when stable, amber when unstable
- Fields: Ticket No (auto), Customer Name (auto-complete), Supplier Name, Vehicle No (auto-complete, ANPR-filled), Item Name, Field01/02/03
- Gross Weight / Tare Weight / Net Weight display
- Gross Date + Time, Tare Date + Time
- First Weighment [F2], Second Weighment [F3]
- Capture Weight [F5]: triggers camera frame grab, ANPR OCR on background thread, CCTV screenshot saved as JPEG named {TicketNo}_{datetime}.jpg
- Embedded live camera preview in the form (small, 240p, top-right corner)
- Save [F10]: saves slip, generates PDF ticket, triggers WhatsApp send to party mobile, shows toast notification
- Clear [F6], Close [Esc]
- Keyboard shortcut bar always visible at bottom
- NONE / OK weight indicator

REPORTS (Query Report tree):
Item Master, Customer Master, Supplier Master, Vehicle No Master,
Slip Register (filter: date range, customer, supplier, item, vehicle, ticket status, user, slip number range),
Item Wise Summary, Customer Name Wise Summary (Party Wise), Vehicle No Wise Summary,
Supplier Name Wise Summary, Item Name Party Wise Summary, Weight Wise Summary,
Audit Edited Report, Audit Deleted Report, Deleted Slip Register, Daily Report.
All reports: View in-app grid, Export to PDF (QuestPDF), Export to Excel (ClosedXML).

UTILITY:
- SAP File Export: date-wise or serial-no export to CSV/XML
- Delete Slip Data: list of slips, select all, delete with confirmation and audit entry
- Change User Password: old password, new password, confirm password
- Ticket Search: search by date, vehicle, ticket number; print and Net Printing options
- Data Export: export slip data to CSV/Excel
- Event Log: system events (must be activated in System Setting)
- Ticket Edit: every change logged to AuditLog (old value, new value, user, timestamp)
- Help Code: pre-fill combinations (Customer, Supplier, Vehicle, Item, Tare Wt) for fast slip entry
- Database Backup Config: active toggle, auto-delete after N days, backup folder path, schedule (daily/weekly/monthly); Backup Now button
- Goods Dispatch: party name, vehicle no, ticket lookup [F5], gross/tare/net/item display; New [F2], Save [F10], Edit [F3], Clear [F6], Close [Esc], Report button
- Send WhatsApp Message: ticket list by date, search, Refresh, Send, Setting (template config), Close

HELP:
- Operation Manual Help: opens CHM or HTML help file

--- HARDWARE INTEGRATION ---
Support all three simultaneously configurable:
1. Serial/COM (RS232): System.IO.Ports.SerialPort, all standard baud rates
2. USB: HID detection, fallback to virtual COM
3. Network/TCP: TCP client to configurable IP:port
Weight polling: every 500ms. Stable: 3 consecutive equal readings within ±2kg tolerance.
If disconnected: show "NOT CONNECTED" in weight display.

Camera (ANPR + CCTV):
- Any DirectShow USB or IP camera; index configurable in System Settings
- Live 240p preview embedded in Slip Entry
- On F5: grab frame, save JPEG, run Tesseract OCR on background thread
- OCR result auto-fills Vehicle No (user can manually override; amber highlight if confidence below 70%)
- Tessdata: include eng + hin language packs in publish output

--- UI / UX ---
Use MaterialDesignThemes. Modern card-based layout — not old grey WinForms style.
Dark theme: bg #1A1A2E, surface #16213E, accent saffron #FF6F00, text white.
Light theme: bg #F5F5F5, surface #FFFFFF, accent navy #1A237E, text #212121.
Theme toggle and language toggle (EN|HI) always in title bar.
Languages: full English and Hindi ResourceDictionary XAML files for all labels, buttons, messages, reports.
Dashboard on login: today's ticket count, total net weight today, last 5 tickets table.
Toast notifications (bottom-right) for: save success, WhatsApp sent, errors, license warnings.
Auto-complete on all master dropdowns.
Weight display uses large 7-segment font (Segment7 or Digital-7).

--- WHATSAPP INTEGRATION ---
Method: WA-Web automation via Selenium ChromeDriver.
First run: open WhatsApp Web, scan QR, save session cookies.
On ticket save: compose message from configurable template using fields:
{TicketNo}, {VehicleNo}, {GrossWt}, {TareWt}, {NetWt}, {ItemName}, {DateTime}.
Send to party mobile number from Customer or Supplier master.
Run on background thread; toast on success or failure.
Manual send available from Utility > Send WhatsApp Message.

--- PDF TICKET ---
Generate with QuestPDF. Layout:
- Header: company logo, name, address, GST, phone (from Company Master)
- Title: WEIGHMENT SLIP / तौल पर्ची
- All slip fields: Ticket No, dates/times, Vehicle No, Customer, Supplier, Item, Field01/02/03
- Weights: Gross, Tare, Net (in Kg, bold)
- QR code containing Ticket No
- Footer: authorised signature line, operator name, software name

--- TAMPER-PROOF AUDIT LOG ---
Serilog append-only file sink at %APPDATA%\VinayakEnterprises\audit.log.
AuditLog SQLite table: Id, Timestamp, UserId, UserName, Action, EntityType, EntityId,
OldValue (JSON), NewValue (JSON), IPAddress, PCName.
Log: login, logout, slip save, slip edit (old+new), slip delete, master changes,
password change, settings change, failed login, license activation, license check failures.

--- SOFTWARE LICENSE SYSTEM ---

TYPE: Machine-locked, fully offline, algorithm-based (no license server required).
AVAILABLE PLANS: 1 Year (365 days), 2 Years (730 days), Perpetual (no expiry, encoded as year 9999).

MACHINE ID GENERATION:
- Collect on first run: CPU ID + Motherboard Serial + Windows Machine GUID
- Combine and hash with SHA-256, truncate to 16 uppercase hex characters
- Format displayed to user: VE-XXXX-XXXX-XXXX-XXXX
- Store in encrypted registry key: HKLM\SOFTWARE\VinayakEnterprises\MachineID
- Also shown on License Info screen and full-screen block screen

LICENSE KEY FORMAT:
- Format: VE1-XXXX-XXXX-XXXX-XXXX-XXXX (25 alphanumeric chars + dashes)
- Key encodes: Machine ID hash + license type code (1Y / 2Y / PP) + expiry date (YYYYMMDD) + HMAC-SHA256 signature
- Shared HMAC secret is hardcoded in both the app and the vendor keygen tool; never exposed to users
- App validates key fully offline by recomputing HMAC and comparing

ACTIVATION FLOW:
1. First run with no valid license: show Activation Screen with Machine ID + license key entry field
2. Customer shares Machine ID with vendor
3. Vendor runs keygen tool (separate app, see below) → generates license key
4. Customer enters key in Activation Screen → app validates offline
5. If valid: store encrypted license data in registry + %APPDATA%\VinayakEnterprises\license.dat
6. Log activation event to AuditLog (timestamp, machine ID, license type, expiry date)
7. Re-activation for renewals: Owner/Admin → File → License Info → "Enter New License Key"

RUNTIME LICENSE CHECKS:
- Check on every app startup (before login screen shown)
- Check every 24 hours silently on background thread
- Anti-tamper: verify machine ID on each check; if mismatch → treat as invalid
- Clock rollback detection: compare current date with last-run date stored in registry;
  if current date is earlier than last recorded date → flag as tampered, block app
- Store last-run date in encrypted registry key on every successful check

EXPIRY BEHAVIOUR:
- 15 days before expiry: show amber warning banner on dashboard every session; include days remaining count
- On expiry date: begin 7-day grace period. Red banner shown on every screen every session. Full app functionality retained during grace period.
- After 7-day grace period ends: full-screen lock screen. All app functions blocked.
  Lock screen shows: "License Expired", Machine ID, vendor contact details (phone/email from Company Master), "Enter License Key" button.
- Perpetual licenses (PP): no expiry check, no warnings, year 9999 in expiry field.

LICENSE INFO SCREEN (Owner/Admin only, File → License Info):
- Machine ID, License Type (1 Year / 2 Years / Perpetual), Issue Date, Expiry Date,
  Days Remaining (or "Perpetual"), Activated By (username), Activation Date
- Vendor contact info for renewal
- "Enter New License Key" button
- Menu item hidden entirely for Operator and Technician roles

VENDOR KEYGEN TOOL (separate app, not distributed to customers):
- Lightweight C# WinForms or console application
- Inputs: Machine ID, license type (1Y / 2Y / PP), activation date
- Output: license key string ready to give to customer
- Uses the same shared HMAC secret as the main app
- Vendor keeps this tool privately; never share with customers

SECURITY NOTES:
- Obfuscate the HMAC secret string in the compiled binary using a simple XOR mask or split-string technique to make it harder to extract via decompilation
- Consider using .NET obfuscation (e.g. ConfuserEx, free) on the published executable
- License data stored in registry uses DPAPI encryption (ProtectedData.Protect with CurrentMachine scope)

--- PROJECT STRUCTURE ---
VinayakEnterprises.sln
├── VinayakEnterprises.App        (WPF startup, views, XAML)
├── VinayakEnterprises.Core       (ViewModels, Services, Models)
├── VinayakEnterprises.Data       (SQLite repositories, migrations)
├── VinayakEnterprises.Hardware   (SerialPort, USB, TCP weighbridge)
├── VinayakEnterprises.Imaging    (Camera capture, Tesseract ANPR)
├── VinayakEnterprises.Reports    (QuestPDF tickets, ClosedXML Excel)
├── VinayakEnterprises.Notifications (WhatsApp Selenium, audit logging)
├── VinayakEnterprises.Licensing  (Machine ID, key validation, DPAPI storage, expiry logic)
└── VinayakEnterprises.KeygenTool (Vendor-only keygen — separate solution or project, not in release build)

Build main app as self-contained single-exe publish (win-x64).
Include Tesseract tessdata (eng + hin) in output.
Target .NET 8, minimum Windows 10 1809.
Do NOT include KeygenTool in the customer-facing publish output.
# open-einvoice (ERechnung.Web)

An open-source .NET-based web application for creating, importing, and validating
**XRechnung (EN 16931)** compliant electronic invoices.
This project is not affiliated with or endorsed by any governmental authority.

## Open e-Invoice Web Application

open-einvoice is a lightweight, privacy-friendly web application that allows users
to create and import XRechnung XML invoices directly in the browser.
It is designed to run locally or on self-hosted environments without any external dependencies.

The application focuses on transparency, correctness, and compliance with European
e-invoicing standards.

---

## ✨ Features

- Create XRechnung invoices (EN 16931 compliant)
- Import and validate existing XRechnung XML files
- Automatic recalculation of:
  - Net amounts
  - VAT
  - Gross totals
- Invoice preview before export
- Export options:
  - XRechnung XML
  - (Planned) ZUGFeRD / Factur-X (PDF/A-3)
- No registration, no database, no tracking
- PathBase support (e.g. `/xrechnung`) for sub-folder hosting
- Clean Razor Pages UI using Bootstrap 5

---

## 🧾 Supported Standards

- XRechnung 3.x
- EN 16931
- PEPPOL BIS Billing
- (Planned) ZUGFeRD / Factur-X

---

## 🛠 Technologies

- .NET (ASP.NET Core Razor Pages)
- .NET 8 / .NET 9 compatible
- Bootstrap 5
- XML-based invoice processing
- Clean separation of:
  - Web UI
  - Business logic
  - Invoice generation

---

## ⚙️ How It Works

This application allows users to:

1. Enter invoice data via a web-based form
2. Import existing XRechnung XML files
3. Validate and recalculate invoice totals automatically
4. Preview invoice data before export
5. Download the resulting XRechnung XML file

All processing is performed locally on the hosting server.
No data is transmitted to third-party services.

---

## 🚀 Getting Started

### Requirements

- .NET SDK 8 or newer
- Windows, Linux, or macOS
- Optional: IIS, Nginx, or reverse proxy

### Run locally

```bash
dotnet run --project src/ERechnung.Web

## ⚠️ Disclaimer

This software is provided "as is", without warranty of any kind.
Use it at your own risk.

The authors and contributors are not responsible for any damages,
data loss, or legal issues resulting from the use of this software.

This project does not constitute legal or tax advice.

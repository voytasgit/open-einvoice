# ERechnung.Web

A free, open-source web application to create and import **XRechnung (EN 16931)** invoices  
with preview and export support.

✔ No registration  
✔ No database  
✔ No tracking  
✔ Runs locally or on IIS / Linux  

---

## ✨ Features

- Create XRechnung (EN 16931 compliant)
- Import existing XRechnung XML
- Automatic recalculation (net / VAT / gross)
- Invoice preview before download
- Export as:
  - XRechnung XML
  - (planned) ZUGFeRD / Factur-X
- PathBase support (`/xrechnung`) for sub-folder hosting
- Clean Razor Pages UI (Bootstrap 5)

---

## 🧾 Supported Standards

- **XRechnung 3.x**
- **EN 16931**
- **PEPPOL BIS Billing**
- (Planned) ZUGFeRD / Factur-X PDF/A-3

---

## 🚀 Getting Started

### Requirements

- .NET 8 SDK
- Windows / Linux / macOS
- Optional: IIS or reverse proxy

### Run locally

```bash
dotnet run --project src/ERechnung.Web

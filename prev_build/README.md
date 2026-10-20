# 🏠 NiSource Smart Move Assistant
### AI-Driven Conversational Utility Concierge — Hackathon Prototype

**Smart Move Assistant** replaces cumbersome, multi-step "Start, Stop, Move" service web forms with an intuitive, conversational **intent-to-action** interface. Instead of navigating 15+ form fields, customers describe their move in plain English. The AI automatically parses service addresses, schedules dates, enforces mandatory identity verification rules, calculates dual-service overlap, and renders an **interactive, fully editable summary card** on the right.

---

## ⚡ Tech Stack

cd backend\SmartMoveAssistant.Api
dotnet run --no-build --launch-profile http

cd frontend
npm run dev

* **Frontend:** Vue.js 3 (Composition API), Tailwind CSS (Utility & Eco theme), Vite
* **Backend:** .NET 8 Core Web API (C#), ASP.NET Core Controllers, Swagger / OpenAPI
* **AI / LLM Engine:** Live enterprise LLM integration (`https://genailab.tcs.in` with `azure/genailab-maas-gpt-4o-mini`) + resilient fallback extractor

---

## 📋 Mandatory Field Verification Matrix

| Field | Move Service | Stop Service | Start Service | Notes |
| :--- | :---: | :---: | :---: | :--- |
| **First Name** | ✅ Mandatory | ✅ Mandatory | ✅ Mandatory | Identity Verification |
| **Last Name** | ✅ Mandatory | ✅ Mandatory | ✅ Mandatory | Identity Verification |
| **SSN** | ✅ Mandatory | ✅ Mandatory | ✅ Mandatory | Masked with Show/Hide toggle |
| **Date of Birth (DOB)** | ✅ Mandatory | ✅ Mandatory | ✅ Mandatory | Identity Verification |
| **Phone Number** | ✅ Mandatory | ✅ Mandatory | ✅ Mandatory | Technician SMS updates |
| **Email Address** | ✅ Mandatory | ✅ Mandatory | ✅ Mandatory | Digital confirmation receipt |
| **Account Number** | ✅ Mandatory | ✅ Mandatory | ⚪ Optional / New | Required for Move & Stop |
| **Current Address** | ✅ Mandatory (from) | ✅ Mandatory (stop) | ✅ Mandatory (start) | Service address |
| **New Address** | ✅ Mandatory (to) | ❌ Not Needed | ❌ Not Needed | Destination address |
| **Stop Date (`DisconnectDate`)**| ✅ Mandatory | ✅ Mandatory | ❌ Not Needed | When to stop service |
| **Start Date (`ConnectDate`)** | ✅ Mandatory | ❌ Not Needed | ✅ Mandatory | When to start service |
| **Forwarding Address** | ⚪ Optional | ⚪ Optional | ❌ Not Needed | For final bill |
| **Dual-Service Overlap Days** | ⚡ Auto-Calculated | ❌ Not Needed | ❌ Not Needed | Calculated when dates overlap |

---

## 🏗️ Architecture & Component Overview

```
┌────────────────────────────────────────────────────────┐
│               Smart Move Assistant UI                  │
├──────────────────────────┬─────────────────────────────┤
│   Left Panel (Chat)      │   Right Panel (Summary)     │
│  • Conversational Stream │  • Fully Editable Inputs    │
│  • Multi-turn questions  │  • Live Validation Badges   │
│  • 1-Click Demo Chips    │  • Overlap Calculator (4d)  │
│  • Real-time typing dots │  • Quick-Fill Identity Demo │
│  • Input & Send Button   │  • Confirm & Schedule Move  │
└────────────┬─────────────┴──────────────▲──────────────┘
             │ (POST /api/transfer/process)│
             ▼                             │ (JSON DTO)
┌──────────────────────────────────────────┴─────────────┐
│             .NET 8 Core Web API (C#)                   │
│  • TransferController.cs                               │
│  • LlmService.cs (HttpClient with SSL Bypass Handler)  │
│  • Models: ActionIntent (Enum), TransferResponseDto    │
│  • Multi-turn state merging & mandatory verification   │
└──────────────────────────┬─────────────────────────────┘
                           │
                           ▼
┌────────────────────────────────────────────────────────┐
│             Enterprise LLM Gateway                     │
│  https://genailab.tcs.in/chat/completions              │
│  Model: azure/genailab-maas-gpt-4o-mini                │
└────────────────────────────────────────────────────────┘
```

---

## 🚀 Step-by-Step Instructions: How to Run Locally

### Prerequisites
* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (verified: `8.0.414`)
* [Node.js](https://nodejs.org/) (verified: `v22.15.1`)

---

### Step 1: Start the .NET Core Backend API

1. Open your first terminal (PowerShell or Command Prompt).
2. Navigate to the backend directory:
   ```powershell
   cd backend\SmartMoveAssistant.Api
   ```
3. Run the application:
   ```powershell
   dotnet run --no-build --launch-profile http
   ```
   > 💡 **Tip:** Adding `--no-build` starts the server in under **0.5 seconds** and skips repetitive NuGet network checks.

4. **Verify Backend is Running:**
   * Open: **[http://localhost:5000/api/transfer/status](http://localhost:5000/api/transfer/status)** (should return `status: "Online"`).
   * Open: **[http://localhost:5000/swagger](http://localhost:5000/swagger)** for interactive Swagger UI testing.

---

### Step 2: Start the Vue.js Frontend

1. Open a **second terminal window** (keep the first terminal running).
2. Navigate to the frontend directory:
   ```powershell
   cd frontend
   ```
3. (If not already installed) Install dependencies:
   ```powershell
   npm install
   ```
4. Start the Vite dev server:
   ```powershell
   npm run dev
   ```
5. **Open in Browser:**
   👉 **[http://localhost:5173](http://localhost:5173)**

---

## 🎯 Hackathon Presentation Flow (Multi-Turn Conversational Demo)

Follow this presentation flow to demonstrate the solution to judges:

### 1. Multi-Turn Conversational Slot-Filling Demo
1. In the chat on the left, click the chip:
   👉 **`1️⃣ Move Request (Missing Identity)`**
   > *"I am moving out of 104 Main St on Oct 28th. I bought a house at 450 Oak Drive and need the gas on by the 25th so I can paint before moving in. Send my final bill to 777 Pine Ave."*
2. Click **Send** (or press Enter).
3. **Show Judges:**
   - The AI extracts the addresses (`104 Main St`, `450 Oak Drive`), dates (`Oct 28`, `Oct 25`), and intent (`Move`).
   - The AI recognizes mandatory verification fields are missing and **explicitly prompts**:
     *"I've staged your Move request, but utility regulations require verification before scheduling. Please provide: First Name, Last Name, SSN, Date of Birth, Phone Number, Email Address, and Account Number."*
   - On the **Right Panel**, the addresses and dates are pre-filled, and the missing fields are highlighted with amber warning badges.

### 2. Resolving Missing Details (Two Ways)
Show the judges that the customer has **two flexible options**:
* **Option A (Via Chat):** Click chip **`2️⃣ Reply with Missing Details`** (*"My account number is ACT-8492019, my name is Sarah Connor, SSN is 123-45-6789, DOB is 1985-06-15, phone (555) 234-5678, email sarah.connor@example.com."*) and click **Send**. The AI merges the new fields into the active order without losing existing addresses!
* **Option B (Direct In-Place Editing):** Or click **`⚡ Quick-Fill Identity`** at the top right of the panel (or type directly into any input field on the right).

### 3. Dynamic Overlap Calculation & Live In-Place Editing
- Highlight the **Continuous Dual-Service Overlap: 4 Days** card (Oct 25 to Oct 28).
- **Interactive Test:** Change the Start Date on the right panel from `2026-10-25` to `2026-10-24`. Notice the overlap days dynamically recalculate to **5 Days** live on screen without needing an extra API call!

### 4. 1-Click Confirmation & Order Dispatch
- Once all mandatory fields are verified, the large green button activates: **"Confirm & Schedule Move"**.
- Click it to view the dispatch modal with confirmation order reference (e.g. `#SMA-849201`) and printable receipt.

---

## 🛠️ API Endpoints Reference

| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `POST` | `/api/transfer/process` | Accepts natural language text & optional existing state; returns merged DTO with validation and overlap days. |
| `POST` | `/api/transfer/confirm` | Confirms booking and generates order reference ID. |
| `GET` | `/api/transfer/models` | Queries LLM gateway for available models. |
| `GET` | `/api/transfer/status` | Returns backend health, model name, and endpoint. |

### Sample Direct API Test (PowerShell):
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/transfer/process" `
  -Method Post `
  -Body '{"message":"I am moving out of 104 Main St on Oct 28th. I bought a house at 450 Oak Drive and need the gas on by the 25th."}' `
  -ContentType "application/json" | ConvertTo-Json -Depth 5
```

---

## ⚙️ Configuration (`appsettings.json`)

To change models, endpoints, or keys, edit `backend/SmartMoveAssistant.Api/appsettings.json`:

```json
{
  "LlmSettings": {
    "ApiKey": "sk-w0AxDs1W5AFg07WGpRTYPw",
    "BaseUrl": "https://genailab.tcs.in",
    "Model": "azure/genailab-maas-gpt-4o-mini",
    "Temperature": 0.1,
    "TimeoutSeconds": 30
  }
}
```

---

## ❓ Troubleshooting & FAQs

### Q1: `dotnet run` is taking a long time to start.
**Fix:** Run `dotnet run --no-build --launch-profile http`. 
* By default, `dotnet run` contacts `nuget.org` over the network to check package updates. Adding `--no-build` skips the network restore and starts the server in under 0.5 seconds.

### Q2: Error "The process cannot access the file because it is being used by another process" (Port 5000 locked).
**Fix:** An existing `SmartMoveAssistant.Api.exe` process is still running. In PowerShell, terminate it:
```powershell
taskkill /IM SmartMoveAssistant.Api.exe /F
```
Then start the backend again.

### Q3: Frontend shows "Connecting API..." in the header.
**Fix:** Ensure your backend is running in Terminal 1 on `http://localhost:5000`. You can test if it's reachable by opening `http://localhost:5000/api/transfer/status` in your browser.

---

## 📁 Related Handoff Documents
* For starting a new AI conversation with complete technical handoff context, see: **[`PROJECT_CONTEXT.md`](PROJECT_CONTEXT.md)**.
* For the design and architecture log, see: **[`walkthrough.md`](file:///C:/Users/GenAIKOCVISUSR18/.gemini/antigravity/brain/4e284109-a58d-476f-ac25-2a47152963ad/walkthrough.md)**.

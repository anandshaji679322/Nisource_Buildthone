# 📋 Smart Move Assistant — Complete Project Context & Conversation Handoff

> **Use this document to initialize a new AI chat session with full context, code state, architecture, decisions, and execution instructions.**

---

## 1. Project Overview & Business Goal
* **Project Name:** NiSource "Smart Move Assistant"
* **Event:** Utility Company Innovation Hackathon
* **Core Problem:** Traditional utility "Start, Stop, Move" web workflows require customers to navigate cumbersome 4-page, 15-field forms. Customers frequently get confused when they need **overlapping service** (e.g., getting gas turned on at a new home a few days early to paint and clean before vacating their old apartment).
* **Solution:** A split-screen, AI-driven concierge that replaces multi-step forms with a single **intent-to-action conversational interface**.
  - **Left Panel (Chat):** Natural language conversation where the customer describes their move. If any mandatory details are missing, the AI explicitly prompts for them across multiple conversational turns.
  - **Right Panel (Interactive Transfer Card):** Dynamically populates with all extracted details, displays an auto-calculated **Dual-Service Overlap Badge** (e.g. 4 days), allows **in-place editing of every single field**, and provides a large green **"Confirm & Schedule Move"** button.

---

## 2. Tech Stack & Environment Configuration
* **Frontend:**
  - Framework: **Vue.js 3** (Composition API, `<script setup>`)
  - Build Tool: **Vite 8**
  - Styling: **Tailwind CSS 3.4** (Utility / Eco color themes)
  - Port: `http://localhost:5173` (Vite dev server proxies `/api` to port `5000`)
* **Backend:**
  - Framework: **.NET 8 Core Web API (C#)**
  - SDK Version: `8.0.414`
  - Documentation: **Swagger / OpenAPI** enabled at `http://localhost:5000/swagger`
  - Port: `http://localhost:5000` (`http` launch profile in `launchSettings.json`)
* **Live Enterprise LLM Gateway:**
  - Endpoint: `https://genailab.tcs.in/chat/completions`
  - Model: `azure/genailab-maas-gpt-4o-mini` *(tested & verified; 26 models available on `/models`)*
  - API Key: `sk-w0AxDs1W5AFg07WGpRTYPw`
  - Special Network Requirement: Internal lab self-signed SSL certificates require bypassing certificate validation in .NET `HttpClientHandler` (`DangerousAcceptAnyServerCertificateValidator`).
  - Resilient Fallback: Built-in heuristic regex parser in `LlmService.cs` ensures the hackathon demo never fails even during Wi-Fi or API gateway outages.

---

## 3. Mandatory Field Matrix & Business Rules

| Field | Move Service | Stop Service | Start Service | Notes |
| :--- | :---: | :---: | :---: | :--- |
| **First Name** | ✅ Mandatory | ✅ Mandatory | ✅ Mandatory | Customer Identity Verification |
| **Last Name** | ✅ Mandatory | ✅ Mandatory | ✅ Mandatory | Customer Identity Verification |
| **SSN** | ✅ Mandatory | ✅ Mandatory | ✅ Mandatory | Masked input with Show/Hide toggle |
| **Date of Birth (DOB)** | ✅ Mandatory | ✅ Mandatory | ✅ Mandatory | Identity Verification |
| **Phone Number** | ✅ Mandatory | ✅ Mandatory | ✅ Mandatory | For confirmation SMS & technician alerts |
| **Email Address** | ✅ Mandatory | ✅ Mandatory | ✅ Mandatory | For digital confirmation receipt |
| **Account Number** | ✅ Mandatory | ✅ Mandatory | ⚪ Optional / New | Mandatory for Move & Stop existing accounts |
| **Current / Service Address** | ✅ Mandatory (from) | ✅ Mandatory (stop) | ✅ Mandatory (start) | Primary physical address |
| **New Address** | ✅ Mandatory (to) | ❌ N/A | ❌ N/A | Destination residence |
| **Stop Date (`DisconnectDate`)**| ✅ Mandatory | ✅ Mandatory | ❌ N/A | When service terminates at old residence |
| **Start Date (`ConnectDate`)** | ✅ Mandatory | ❌ N/A | ✅ Mandatory | When service activates at new residence |
| **Forwarding Address** | ⚪ Optional | ⚪ Optional | ❌ N/A | Destination for final closing bill |
| **Dual-Service Overlap Days** | ⚡ Calculated | ❌ N/A | ❌ N/A | `(DisconnectDate - ConnectDate) + 1` |

---

## 4. Multi-Turn Conversational & Verification Logic

1. **Turn 1 (Partial Prompt):**
   - User types: *"I am moving out of 104 Main St on Oct 28th. I bought a house at 450 Oak Drive and need the gas on by the 25th so I can paint before moving in. Send my final bill to 777 Pine Ave."*
   - Backend extracts addresses, dates, intent (`Move`), and calculates `overlapDays: 4`.
   - Backend checks mandatory rules: flags that `FirstName`, `LastName`, `SSN`, `DOB`, `PhoneNumber`, `Email`, and `AccountNumber` are missing (`isComplete = false`).
   - AI responds: *"I've staged your Move request, but utility regulations require verification before scheduling. Please provide: First Name, Last Name, SSN, Date of Birth, Phone Number, Email Address, and Account Number. You can reply here or enter them directly on the right."*
   - Right panel displays addresses & dates filled, and marks identity fields with amber warning tags.

2. **Turn 2 (Follow-Up Reply or In-Place Edit):**
   - User replies: *"My account number is ACT-8492019, my name is Sarah Connor, SSN is 123-45-6789, DOB is 1985-06-15, phone (555) 234-5678, email sarah.connor@example.com."*
   - Backend merges the new identity fields into the existing staged order (`existingData`) without losing addresses or flipping the intent.
   - All mandatory fields satisfied (`isComplete = true`).
   - AI confirms: *"All required details for your Move service request are verified! Please review the transfer card on the right..."*
   - Large green **"Confirm & Schedule Move"** button unlocks.

3. **In-Place Right Panel Editing:**
   - Every field on the right is bound to reactive state.
   - Changing the start or stop date on the right panel dynamically recalculates the **Overlap Days** live on screen without needing an extra API call.
   - The user can click **"⚡ Quick-Fill Identity"** in the top-right header for rapid 1-click completion during demos.

---

## 5. Repository File Structure

```
c:\Users\GenAIKOCVISUSR18\Desktop\Nisource-cdr-1\
│
├── README.md                                    # Complete project overview & demo presentation script
├── PROJECT_CONTEXT.md                           # This handoff file for new chats
│
├── backend\SmartMoveAssistant.Api\              # .NET 8 Core Web API
│   ├── Controllers\
│   │   └── TransferController.cs                # /api/transfer/process, /confirm, /status, /models
│   ├── Models\
│   │   ├── Enums.cs                             # ActionIntent enum (Start, Stop, Move)
│   │   └── TransferDtos.cs                      # TransferResponseDto, ProcessTransferRequest, etc.
│   ├── Services\
│   │   ├── ILlmService.cs                       # Interface for LLM extraction
│   │   └── LlmService.cs                        # Live LLM client, SSL handler, multi-turn merge, fallback
│   ├── Configuration\
│   │   └── LlmOptions.cs                        # Strongly-typed options for ApiKey, BaseUrl, Model
│   ├── Properties\
│   │   └── launchSettings.json                  # Port 5000 configuration
│   ├── appsettings.json                         # Pre-configured with genailab.tcs.in & gpt-4o-mini
│   ├── Program.cs                               # CORS, HttpClient with SSL bypass, Swagger, controllers
│   └── SmartMoveAssistant.Api.csproj
│
└── frontend\                                    # Vue.js 3 + Tailwind CSS + Vite
    ├── src\
    │   ├── components\
    │   │   ├── HeaderNav.vue                    # Brand banner, live API status badge, model name, reset button
    │   │   ├── ChatPanel.vue                    # Conversational stream, quick demo chips (1, 2, 3), input
    │   │   ├── SummaryPanel.vue                 # Interactive editable form, overlap card, live validation meter
    │   │   └── ConfirmationModal.vue            # Dispatch order ID receipt, print/save dialog
    │   ├── services\
    │   │   └── api.js                           # Frontend fetch client connecting to /api/transfer/*
    │   ├── App.vue                              # Split-screen orchestration & reactive state sync
    │   ├── main.js                              # Vue app entry
    │   └── style.css                            # Tailwind directives & custom scrollbars
    ├── index.html                               # Plus Jakarta Sans typography
    ├── vite.config.js                           # Proxy /api to http://localhost:5000
    ├── tailwind.config.js                       # Utility & Eco theme extensions
    ├── postcss.config.js
    └── package.json
```

---

## 6. How to Run & Verify Locally

### Terminal 1: Backend (.NET 8 Web API)
```powershell
cd backend\SmartMoveAssistant.Api
dotnet run --no-build --launch-profile http
```
* **URL:** `http://localhost:5000`
* **Swagger Documentation:** `http://localhost:5000/swagger`
* **Health Check:** `http://localhost:5000/api/transfer/status`

> **Pro Tip:** Using `--no-build` starts the server in under 0.5s and skips redundant remote NuGet package checks.

### Terminal 2: Frontend (Vue 3 + Vite)
```powershell
cd frontend
npm run dev
```
* **URL:** `http://localhost:5173`

---

## 7. Hackathon Judge Presentation Script (2 Minutes)

1. **The Hook:**
   > *"Utilities lose thousands of customers during moving because 15-field web forms are confusing—especially when customers need gas at both homes to paint before moving. Smart Move Assistant replaces all forms with conversational AI."*
2. **Action 1 (Click Demo Chip 1):** Click `1️⃣ Move Request (Missing Identity)`.
   > *"The customer explains their move in natural language. The AI extracts the origin (104 Main St on Oct 28) and destination (450 Oak Drive on Oct 25), but correctly enforces utility KYC rules: it pauses and asks for First/Last Name, SSN, DOB, Phone, Email, and Account Number."*
3. **Action 2 (Click Demo Chip 2 or Quick-Fill):** Click `2️⃣ Reply with Missing Details` (or click `⚡ Quick-Fill Identity` on the right).
   > *"The AI merges the identity details across conversation turns. The right panel validates all fields and updates the live completion bar."*
4. **Action 3 (Show Dual-Service Overlap):**
   > *"Notice the auto-calculated '4 Days Dual Service Overlap' from Oct 25 to Oct 28. If the user edits the Start Date to Oct 24 directly in the right panel, the overlap instantly recalculates to 5 Days live."*
5. **Action 4 (Click Confirm & Schedule):**
   > *"Click 'Confirm & Schedule Move' to dispatch order #SMA-849201 with instant confirmation receipt."*

---

## 8. Continuation Prompt for Next Chat Session

If opening a new chat window, simply paste the following:

> *"I am working on the NiSource Smart Move Assistant prototype located at `c:\Users\GenAIKOCVISUSR18\Desktop\Nisource-cdr-1`. Please read `PROJECT_CONTEXT.md` for full context on our Vue 3 + Tailwind frontend and .NET 8 Web API backend with live DeepSeek/GPT-4o integration. I want to continue developing or extending this application."*

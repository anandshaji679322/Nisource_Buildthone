<template>
  <div class="h-full bg-white rounded-2xl border border-slate-200/80 shadow-sm overflow-hidden flex flex-col">
    <!-- Header with Intent Tabs and Progress -->
    <div class="px-5 py-3.5 border-b border-slate-100 bg-slate-50/80 flex flex-col sm:flex-row sm:items-center justify-between gap-3">
      <div class="flex items-center space-x-2.5">
        <div class="w-8 h-8 rounded-lg bg-emerald-600 text-white flex items-center justify-center shadow-sm">
          <svg class="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"/>
            <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"/>
          </svg>
        </div>
        <div>
          <div class="flex items-center space-x-2">
            <h2 class="text-sm font-bold text-slate-800">Transfer Summary & Order Form</h2>
            <span class="text-[10px] font-semibold px-2 py-0.5 rounded-full bg-emerald-100 text-emerald-800">
              Live & Editable
            </span>
          </div>
          <p class="text-xs text-slate-500">Auto-extracted from chat • Edit any field below</p>
        </div>
      </div>

      <!-- Quick Fill Demo Data Button -->
      <button 
        @click="fillDemoData" 
        type="button"
        class="inline-flex items-center justify-center px-3 py-1.5 text-xs font-semibold rounded-lg bg-sky-50 text-sky-700 hover:bg-sky-100 border border-sky-200 transition active:scale-95 shadow-2xs whitespace-nowrap self-start sm:self-auto cursor-pointer"
        title="Fill remaining fields with demo customer data"
      >
        <span class="mr-1">⚡</span> Quick-Fill Identity
      </button>
    </div>

    <!-- Intent Tabs & Live Progress Bar -->
    <div class="px-5 py-2.5 bg-white border-b border-slate-100">
      <div class="flex items-center justify-between flex-wrap gap-2 mb-2">
        <!-- Intent Buttons -->
        <div class="inline-flex p-1 bg-slate-100 rounded-xl space-x-1 text-xs font-semibold">
          <button 
            @click="setIntent('Move')" 
            :class="[formData.actionIntent === 'Move' ? 'bg-white text-sky-700 shadow-2xs' : 'text-slate-600 hover:text-slate-900']"
            class="px-3 py-1 rounded-lg transition cursor-pointer"
          >
            🚚 Move
          </button>
          <button 
            @click="setIntent('Stop')" 
            :class="[formData.actionIntent === 'Stop' ? 'bg-white text-rose-700 shadow-2xs' : 'text-slate-600 hover:text-slate-900']"
            class="px-3 py-1 rounded-lg transition cursor-pointer"
          >
            🛑 Stop
          </button>
          <button 
            @click="setIntent('Start')" 
            :class="[formData.actionIntent === 'Start' ? 'bg-white text-emerald-700 shadow-2xs' : 'text-slate-600 hover:text-slate-900']"
            class="px-3 py-1 rounded-lg transition cursor-pointer"
          >
            ⚡ Start
          </button>
        </div>

        <!-- Completion Indicator Badge -->
        <div class="flex items-center space-x-1.5 text-xs font-semibold">
          <span 
            :class="[missingList.length === 0 ? 'bg-emerald-100 text-emerald-800 border-emerald-200' : 'bg-amber-100 text-amber-800 border-amber-200']"
            class="px-2.5 py-0.5 rounded-full border flex items-center space-x-1"
          >
            <span v-if="missingList.length === 0">✓ Verified & Ready to Schedule</span>
            <span v-else>⚠️ {{ missingList.length }} Field{{ missingList.length > 1 ? 's' : '' }} Missing</span>
          </span>
        </div>
      </div>

      <!-- Progress Meter -->
      <div class="w-full bg-slate-100 h-1.5 rounded-full overflow-hidden">
        <div 
          class="h-full transition-all duration-300 rounded-full"
          :class="[missingList.length === 0 ? 'bg-emerald-500' : 'bg-sky-500']"
          :style="{ width: `${completionPercentage}%` }"
        ></div>
      </div>
    </div>

    <!-- Body Scrollable Content -->
    <div class="flex-1 overflow-y-auto p-4 sm:p-5 space-y-4">
      
      <!-- PROMINENT ADDRESS & ROUTE SUMMARY CARD (VISIBLE AT THE VERY TOP!) -->
      <div class="bg-gradient-to-r from-sky-50 via-slate-50 to-emerald-50 rounded-2xl p-4 border border-sky-100 shadow-2xs">
        <div class="flex items-center justify-between mb-2">
          <span class="text-[11px] font-bold uppercase tracking-wider text-slate-500">Service Route Overview</span>
          <span v-if="calculatedOverlap > 0" class="text-[10px] font-bold px-2 py-0.5 rounded-full bg-amber-100 text-amber-900 border border-amber-200 animate-pulse">
            {{ calculatedOverlap }} Days Dual-Service Overlap
          </span>
        </div>

        <div class="grid grid-cols-1 sm:grid-cols-2 gap-3">
          <!-- Origin / Current Address Display Card -->
          <div class="bg-white rounded-xl p-3 border border-slate-200 shadow-2xs">
            <div class="flex items-center justify-between text-[11px] mb-1">
              <span class="font-bold uppercase tracking-wide text-rose-600 flex items-center">
                <span class="w-2 h-2 rounded-full bg-rose-500 mr-1.5"></span>
                {{ formData.actionIntent === 'Start' ? 'Service Address' : 'From (Current)' }}
              </span>
              <span class="text-[10px] font-medium text-slate-400" v-if="formData.disconnectDate">
                Stop: {{ formData.disconnectDate }}
              </span>
            </div>
            <div class="text-xs font-bold text-slate-800 truncate" :title="formData.currentAddress || 'Not specified yet'">
              {{ formData.currentAddress || '— Awaiting address from chat —' }}
            </div>
          </div>

          <!-- Destination / New Address Display Card -->
          <div v-if="formData.actionIntent === 'Move'" class="bg-white rounded-xl p-3 border border-emerald-200 shadow-2xs">
            <div class="flex items-center justify-between text-[11px] mb-1">
              <span class="font-bold uppercase tracking-wide text-emerald-700 flex items-center">
                <span class="w-2 h-2 rounded-full bg-emerald-500 mr-1.5"></span>
                To (New Residence)
              </span>
              <span class="text-[10px] font-medium text-slate-400" v-if="formData.connectDate">
                Start: {{ formData.connectDate }}
              </span>
            </div>
            <div class="text-xs font-bold text-slate-800 truncate" :title="formData.newAddress || 'Not specified yet'">
              {{ formData.newAddress || '— Awaiting address from chat —' }}
            </div>
          </div>
        </div>

        <!-- Dual-Service Notice -->
        <div v-if="formData.actionIntent === 'Move' && calculatedOverlap > 0" class="mt-2.5 p-2 bg-amber-50/80 rounded-lg border border-amber-200/60 text-[11px] text-amber-800 flex items-center space-x-1.5">
          <span>💡</span>
          <span><strong>Dual Service Active (Oct {{ getDay(formData.connectDate) }} – Oct {{ getDay(formData.disconnectDate) }}):</strong> Continuous utility service at both homes for painting & transition.</span>
        </div>
      </div>

      <!-- SECTION 1: SERVICE LOCATIONS & DATES (EDITABLE INPUTS) -->
      <div class="bg-slate-50/70 rounded-2xl p-4 border border-slate-200/80 space-y-3">
        <div class="flex items-center justify-between pb-1 border-b border-slate-200/60">
          <span class="text-xs font-bold uppercase tracking-wider text-slate-700">1. Service Locations & Move Schedule</span>
          <span class="text-[10px] text-slate-400 font-medium">Click any field to edit</span>
        </div>

        <div class="space-y-3">
          <!-- Current Address Input -->
          <div class="p-3 bg-white rounded-xl border border-slate-200 space-y-1.5">
            <div class="flex items-center justify-between">
              <label class="text-xs font-bold text-slate-800 flex items-center">
                <span class="w-2 h-2 rounded-full bg-rose-500 mr-1.5"></span>
                {{ formData.actionIntent === 'Start' ? 'Service Address (Connect)' : 'Current Address (Disconnect)' }}
              </label>
              <span class="text-[10px] text-rose-600 font-bold" v-if="!formData.currentAddress">Required</span>
              <span class="text-[10px] text-emerald-600 font-bold" v-else>✓ Recorded</span>
            </div>
            <input
              v-model="formData.currentAddress"
              @input="onFieldChange"
              type="text"
              placeholder="e.g. 104 Main St, Springfield"
              class="w-full px-3 py-2 text-xs rounded-xl border transition focus:outline-none focus:ring-2 focus:ring-sky-500"
              :class="[!formData.currentAddress ? 'border-amber-300 bg-amber-50/20' : 'border-slate-300 bg-white font-medium']"
            />

            <!-- Disconnect Date (Stop Date) -->
            <div v-if="formData.actionIntent !== 'Start'" class="pt-1 flex items-center justify-between">
              <label class="text-xs font-medium text-slate-600">Stop Date (Disconnect):</label>
              <input
                v-model="formData.disconnectDate"
                @input="onFieldChange"
                type="date"
                class="px-2.5 py-1 text-xs rounded-lg border border-slate-300 font-medium focus:outline-none focus:ring-2 focus:ring-sky-500"
                :class="[!formData.disconnectDate ? 'border-amber-300 bg-amber-50/20' : '']"
              />
            </div>
          </div>

          <!-- New Address Input (for Move) -->
          <div v-if="formData.actionIntent === 'Move'" class="p-3 bg-white rounded-xl border border-emerald-200 space-y-1.5 ring-1 ring-emerald-500/10">
            <div class="flex items-center justify-between">
              <label class="text-xs font-bold text-slate-800 flex items-center">
                <span class="w-2 h-2 rounded-full bg-emerald-500 mr-1.5"></span>
                New Address (Connect Service)
              </label>
              <span class="text-[10px] text-emerald-700 font-bold" v-if="!formData.newAddress">Required for Move</span>
              <span class="text-[10px] text-emerald-600 font-bold" v-else>✓ Recorded</span>
            </div>
            <input
              v-model="formData.newAddress"
              @input="onFieldChange"
              type="text"
              placeholder="e.g. 450 Oak Drive, Columbus"
              class="w-full px-3 py-2 text-xs rounded-xl border transition focus:outline-none focus:ring-2 focus:ring-sky-500"
              :class="[!formData.newAddress ? 'border-amber-300 bg-amber-50/20' : 'border-slate-300 bg-white font-medium']"
            />

            <!-- Connect Date (Start Date) -->
            <div class="pt-1 flex items-center justify-between">
              <label class="text-xs font-medium text-slate-600">Start Date (Connect):</label>
              <input
                v-model="formData.connectDate"
                @input="onFieldChange"
                type="date"
                class="px-2.5 py-1 text-xs rounded-lg border border-slate-300 font-medium focus:outline-none focus:ring-2 focus:ring-sky-500"
                :class="[!formData.connectDate ? 'border-amber-300 bg-amber-50/20' : '']"
              />
            </div>
          </div>

          <!-- Start Date (for Start Service only) -->
          <div v-if="formData.actionIntent === 'Start'" class="p-3 bg-white rounded-xl border border-slate-200 flex items-center justify-between">
            <label class="text-xs font-bold text-slate-700">Start Date (When to connect):</label>
            <input
              v-model="formData.connectDate"
              @input="onFieldChange"
              type="date"
              class="px-2.5 py-1 text-xs rounded-lg border border-slate-300 font-medium focus:outline-none focus:ring-2 focus:ring-sky-500"
            />
          </div>

          <!-- Forwarding Address -->
          <div class="p-3 bg-white rounded-xl border border-slate-200 space-y-1">
            <div class="flex items-center justify-between">
              <span class="text-xs font-semibold text-slate-700">Forwarding Address (Final Bill)</span>
              <span class="text-[10px] text-slate-400">Optional</span>
            </div>
            <input
              v-model="formData.forwardingAddress"
              @input="onFieldChange"
              type="text"
              placeholder="e.g. 777 Pine Ave (Defaults to new address if empty)"
              class="w-full px-3 py-2 text-xs rounded-xl border border-slate-300 transition focus:outline-none focus:ring-2 focus:ring-sky-500"
            />
          </div>
        </div>
      </div>

      <!-- SECTION 2: CUSTOMER IDENTITY & ACCOUNT VERIFICATION -->
      <div class="bg-slate-50/70 rounded-2xl p-4 border border-slate-200/80 space-y-3">
        <div class="flex items-center justify-between pb-1 border-b border-slate-200/60">
          <span class="text-xs font-bold uppercase tracking-wider text-slate-700">2. Customer Account & Identity</span>
          <span class="text-[10px] text-slate-400 font-medium">Utility identity verification</span>
        </div>

        <div class="grid grid-cols-1 sm:grid-cols-2 gap-3">
          <!-- Account Number -->
          <div class="sm:col-span-2">
            <label class="flex items-center justify-between text-xs font-semibold text-slate-700 mb-1">
              <span>Account Number</span>
              <span v-if="formData.actionIntent !== 'Start'" class="text-[10px] text-rose-600 font-bold">
                {{ formData.accountNumber ? '✓ Verified' : 'Required for ' + formData.actionIntent }}
              </span>
              <span v-else class="text-[10px] text-slate-400 font-normal">Optional (New Customer)</span>
            </label>
            <input
              v-model="formData.accountNumber"
              @input="onFieldChange"
              type="text"
              placeholder="e.g. ACT-8492019"
              class="w-full px-3 py-2 text-xs rounded-xl border font-mono transition focus:outline-none focus:ring-2 focus:ring-sky-500"
              :class="[!formData.accountNumber && formData.actionIntent !== 'Start' ? 'border-amber-300 bg-amber-50/20' : 'border-slate-300 bg-white']"
            />
          </div>

          <!-- First Name -->
          <div>
            <label class="flex items-center justify-between text-xs font-semibold text-slate-700 mb-1">
              <span>First Name</span>
              <span class="text-[10px]" :class="[formData.firstName ? 'text-emerald-600 font-bold' : 'text-rose-600 font-bold']">
                {{ formData.firstName ? '✓' : 'Required' }}
              </span>
            </label>
            <input
              v-model="formData.firstName"
              @input="onFieldChange"
              type="text"
              placeholder="e.g. Sarah"
              class="w-full px-3 py-2 text-xs rounded-xl border transition focus:outline-none focus:ring-2 focus:ring-sky-500"
              :class="[!formData.firstName ? 'border-amber-300 bg-amber-50/20' : 'border-slate-300 bg-white']"
            />
          </div>

          <!-- Last Name -->
          <div>
            <label class="flex items-center justify-between text-xs font-semibold text-slate-700 mb-1">
              <span>Last Name</span>
              <span class="text-[10px]" :class="[formData.lastName ? 'text-emerald-600 font-bold' : 'text-rose-600 font-bold']">
                {{ formData.lastName ? '✓' : 'Required' }}
              </span>
            </label>
            <input
              v-model="formData.lastName"
              @input="onFieldChange"
              type="text"
              placeholder="e.g. Connor"
              class="w-full px-3 py-2 text-xs rounded-xl border transition focus:outline-none focus:ring-2 focus:ring-sky-500"
              :class="[!formData.lastName ? 'border-amber-300 bg-amber-50/20' : 'border-slate-300 bg-white']"
            />
          </div>

          <!-- SSN -->
          <div>
            <label class="flex items-center justify-between text-xs font-semibold text-slate-700 mb-1">
              <span>SSN</span>
              <span class="text-[10px]" :class="[formData.ssn ? 'text-emerald-600 font-bold' : 'text-rose-600 font-bold']">
                {{ formData.ssn ? '✓' : 'Required' }}
              </span>
            </label>
            <div class="relative">
              <input
                v-model="formData.ssn"
                @input="onFieldChange"
                :type="showSsn ? 'text' : 'password'"
                placeholder="XXX-XX-XXXX"
                class="w-full px-3 py-2 text-xs rounded-xl border font-mono transition focus:outline-none focus:ring-2 focus:ring-sky-500 pr-14"
                :class="[!formData.ssn ? 'border-amber-300 bg-amber-50/20' : 'border-slate-300 bg-white']"
              />
              <button 
                type="button" 
                @click="showSsn = !showSsn"
                class="absolute right-2 top-2 text-[10px] font-semibold text-slate-500 hover:text-slate-800 px-1.5 py-0.5 rounded bg-slate-100 border border-slate-200 cursor-pointer"
              >
                {{ showSsn ? 'Hide' : 'Show' }}
              </button>
            </div>
          </div>

          <!-- DOB -->
          <div>
            <label class="flex items-center justify-between text-xs font-semibold text-slate-700 mb-1">
              <span>Date of Birth (DOB)</span>
              <span class="text-[10px]" :class="[formData.dob ? 'text-emerald-600 font-bold' : 'text-rose-600 font-bold']">
                {{ formData.dob ? '✓' : 'Required' }}
              </span>
            </label>
            <input
              v-model="formData.dob"
              @input="onFieldChange"
              type="date"
              class="w-full px-3 py-2 text-xs rounded-xl border transition focus:outline-none focus:ring-2 focus:ring-sky-500"
              :class="[!formData.dob ? 'border-amber-300 bg-amber-50/20' : 'border-slate-300 bg-white']"
            />
          </div>

          <!-- Phone Number -->
          <div>
            <label class="flex items-center justify-between text-xs font-semibold text-slate-700 mb-1">
              <span>Phone Number</span>
              <span class="text-[10px]" :class="[formData.phoneNumber ? 'text-emerald-600 font-bold' : 'text-rose-600 font-bold']">
                {{ formData.phoneNumber ? '✓' : 'Required' }}
              </span>
            </label>
            <input
              v-model="formData.phoneNumber"
              @input="onFieldChange"
              type="tel"
              placeholder="(555) 000-0000"
              class="w-full px-3 py-2 text-xs rounded-xl border transition focus:outline-none focus:ring-2 focus:ring-sky-500"
              :class="[!formData.phoneNumber ? 'border-amber-300 bg-amber-50/20' : 'border-slate-300 bg-white']"
            />
          </div>

          <!-- Email Address -->
          <div>
            <label class="flex items-center justify-between text-xs font-semibold text-slate-700 mb-1">
              <span>Email Address</span>
              <span class="text-[10px]" :class="[formData.email ? 'text-emerald-600 font-bold' : 'text-rose-600 font-bold']">
                {{ formData.email ? '✓' : 'Required' }}
              </span>
            </label>
            <input
              v-model="formData.email"
              @input="onFieldChange"
              type="email"
              placeholder="customer@example.com"
              class="w-full px-3 py-2 text-xs rounded-xl border transition focus:outline-none focus:ring-2 focus:ring-sky-500"
              :class="[!formData.email ? 'border-amber-300 bg-amber-50/20' : 'border-slate-300 bg-white']"
            />
          </div>
        </div>
      </div>

      <!-- MISSING FIELDS HELPER (ONLY MENTIONS WHAT IS ACTUALLY MISSING) -->
      <div v-if="missingList.length > 0" class="p-3 bg-amber-50/80 rounded-xl border border-amber-200 text-xs">
        <span class="font-bold text-amber-900 block mb-1">Missing {{ missingList.length }} details to schedule:</span>
        <div class="flex flex-wrap gap-1.5">
          <span v-for="item in missingList" :key="item" class="px-2 py-0.5 rounded-md bg-white border border-amber-300 text-amber-900 text-[11px] font-semibold">
            • {{ item }}
          </span>
        </div>
      </div>

      <!-- CONFIRM & SCHEDULE BUTTON -->
      <div class="pt-1">
        <button
          @click="submitTransfer"
          :disabled="isScheduling || missingList.length > 0"
          class="w-full py-3.5 px-6 rounded-xl font-bold text-sm text-white transition-all duration-200 flex items-center justify-center space-x-2 shadow-lg"
          :class="[
            missingList.length === 0 && !isScheduling
              ? 'bg-emerald-600 hover:bg-emerald-700 active:bg-emerald-800 shadow-emerald-600/25 cursor-pointer transform hover:-translate-y-0.5' 
              : 'bg-slate-300 text-slate-500 cursor-not-allowed shadow-none'
          ]"
        >
          <svg v-if="!isScheduling && missingList.length === 0" class="w-5 h-5" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
            <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/>
            <polyline points="22 4 12 14.01 9 11.01"/>
          </svg>
          <svg v-else-if="isScheduling" class="animate-spin w-5 h-5 text-white" viewBox="0 0 24 24" fill="none">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8H4z"></path>
          </svg>
          <span>
            {{ isScheduling ? 'Scheduling Move with Field Ops...' : (missingList.length === 0 ? 'Confirm & Schedule ' + formData.actionIntent : 'Complete Missing Fields to Schedule') }}
          </span>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { reactive, ref, computed, watch } from 'vue'

const props = defineProps({
  transferData: {
    type: Object,
    default: () => ({})
  },
  isScheduling: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits(['confirm', 'update:transferData'])

const showSsn = ref(false)

const formData = reactive({
  actionIntent: 'Move',
  accountNumber: '',
  firstName: '',
  lastName: '',
  ssn: '',
  dob: '',
  phoneNumber: '',
  email: '',
  currentAddress: '',
  newAddress: '',
  disconnectDate: '',
  connectDate: '',
  forwardingAddress: '',
  overlapDays: 0,
  confidenceScore: 0.98
})

// Sync incoming props to local formData
watch(() => props.transferData, (newData) => {
  if (newData) {
    if (newData.actionIntent) {
      formData.actionIntent = newData.actionIntent
      if (newData.actionIntent === 'Start') {
        formData.newAddress = ''
        formData.disconnectDate = ''
      } else if (newData.actionIntent === 'Stop') {
        formData.newAddress = ''
        formData.connectDate = ''
      }
    }
    if (newData.accountNumber !== undefined && newData.accountNumber !== '') formData.accountNumber = newData.accountNumber
    if (newData.firstName !== undefined && newData.firstName !== '') formData.firstName = newData.firstName
    if (newData.lastName !== undefined && newData.lastName !== '') formData.lastName = newData.lastName
    if (newData.ssn !== undefined && newData.ssn !== '') formData.ssn = newData.ssn
    if (newData.dob !== undefined && newData.dob !== '') formData.dob = normalizeDate(newData.dob)
    if (newData.phoneNumber !== undefined && newData.phoneNumber !== '') formData.phoneNumber = newData.phoneNumber
    if (newData.email !== undefined && newData.email !== '') formData.email = newData.email
    if (newData.currentAddress !== undefined && newData.currentAddress !== '') formData.currentAddress = newData.currentAddress
    if (newData.newAddress !== undefined && newData.newAddress !== '') formData.newAddress = newData.newAddress
    if (newData.disconnectDate !== undefined && newData.disconnectDate !== '') formData.disconnectDate = normalizeDate(newData.disconnectDate)
    if (newData.connectDate !== undefined && newData.connectDate !== '') formData.connectDate = normalizeDate(newData.connectDate)
    if (newData.forwardingAddress !== undefined && newData.forwardingAddress !== '') formData.forwardingAddress = newData.forwardingAddress
  }
}, { immediate: true, deep: true })

function normalizeDate(d) {
  if (!d) return ''
  if (/^\d{4}-\d{2}-\d{2}$/.test(d)) return d
  try {
    const parsed = new Date(d)
    if (!isNaN(parsed.getTime())) {
      return parsed.toISOString().split('T')[0]
    }
  } catch (e) {}
  return d
}

function getDay(dateStr) {
  if (!dateStr) return ''
  try {
    const d = new Date(dateStr)
    return d.getDate()
  } catch (e) {}
  return dateStr
}

function setIntent(intent) {
  formData.actionIntent = intent
  onFieldChange()
}

const calculatedOverlap = computed(() => {
  if (formData.actionIntent !== 'Move') return 0
  if (!formData.connectDate || !formData.disconnectDate) return 0

  const start = new Date(formData.connectDate)
  const end = new Date(formData.disconnectDate)
  if (!isNaN(start.getTime()) && !isNaN(end.getTime()) && start <= end) {
    return Math.floor((end - start) / (1000 * 60 * 60 * 24)) + 1
  }
  return 0
})

const missingList = computed(() => {
  const missing = []

  // Customer identity
  if (!formData.firstName?.trim()) missing.push('First Name')
  if (!formData.lastName?.trim()) missing.push('Last Name')
  if (!formData.ssn?.trim()) missing.push('SSN')
  if (!formData.dob?.trim()) missing.push('Date of Birth')
  if (!formData.phoneNumber?.trim()) missing.push('Phone Number')
  if (!formData.email?.trim()) missing.push('Email Address')

  // Intent-specific rules
  if (formData.actionIntent === 'Move') {
    if (!formData.accountNumber?.trim()) missing.push('Account Number')
    if (!formData.currentAddress?.trim()) missing.push('Current Address')
    if (!formData.disconnectDate?.trim()) missing.push('Stop Date')
    if (!formData.newAddress?.trim()) missing.push('New Address')
    if (!formData.connectDate?.trim()) missing.push('Start Date')
  } else if (formData.actionIntent === 'Stop') {
    if (!formData.accountNumber?.trim()) missing.push('Account Number')
    if (!formData.currentAddress?.trim()) missing.push('Current Service Address')
    if (!formData.disconnectDate?.trim()) missing.push('Stop Date')
  } else if (formData.actionIntent === 'Start') {
    if (!formData.currentAddress?.trim()) missing.push('Service Address')
    if (!formData.connectDate?.trim()) missing.push('Start Date')
  }

  return missing
})

const totalRequiredCount = computed(() => {
  if (formData.actionIntent === 'Move') return 11
  if (formData.actionIntent === 'Stop') return 9
  return 8 // Start
})

const completionPercentage = computed(() => {
  const filled = totalRequiredCount.value - missingList.value.length
  return Math.max(5, Math.min(100, Math.round((filled / totalRequiredCount.value) * 100)))
})

function onFieldChange() {
  formData.overlapDays = calculatedOverlap.value
  emit('update:transferData', { ...formData, isComplete: missingList.value.length === 0, missingFields: missingList.value })
}

function fillDemoData() {
  if (!formData.accountNumber) formData.accountNumber = 'ACT-8492019'
  if (!formData.firstName) formData.firstName = 'Sarah'
  if (!formData.lastName) formData.lastName = 'Connor'
  if (!formData.ssn) formData.ssn = '123-45-6789'
  if (!formData.dob) formData.dob = '1985-06-15'
  if (!formData.phoneNumber) formData.phoneNumber = '(555) 234-5678'
  if (!formData.email) formData.email = 'sarah.connor@example.com'
  if (!formData.currentAddress) formData.currentAddress = '104 Main St'
  if (formData.actionIntent === 'Move' && !formData.newAddress) formData.newAddress = '450 Oak Drive'
  if (!formData.disconnectDate && formData.actionIntent !== 'Start') formData.disconnectDate = '2026-10-28'
  if (!formData.connectDate && formData.actionIntent !== 'Stop') formData.connectDate = '2026-10-25'
  if (!formData.forwardingAddress) formData.forwardingAddress = '777 Pine Ave'
  onFieldChange()
}

function submitTransfer() {
  if (missingList.value.length > 0) return
  formData.overlapDays = calculatedOverlap.value
  emit('confirm', { ...formData })
}
</script>

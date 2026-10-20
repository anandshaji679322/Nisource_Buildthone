<template>
  <div class="flex flex-col h-full bg-white rounded-2xl border border-slate-200/80 shadow-sm overflow-hidden">
    <!-- Chat Header -->
    <div class="px-5 py-4 border-b border-slate-100 bg-slate-50/50 flex items-center justify-between">
      <div class="flex items-center space-x-2.5">
        <div class="w-8 h-8 rounded-lg bg-sky-600 text-white flex items-center justify-center shadow-sm">
          <svg class="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z"/>
          </svg>
        </div>
        <div>
          <h2 class="text-sm font-bold text-slate-800">Move Concierge Chat</h2>
          <p class="text-xs text-slate-500">Conversational intent & entity extraction</p>
        </div>
      </div>

      <span class="inline-flex items-center text-[11px] font-medium px-2 py-0.5 rounded-md bg-emerald-50 text-emerald-700 border border-emerald-200">
        AI Ready
      </span>
    </div>

    <!-- Suggested Test Prompts Bar -->
    <div class="px-4 py-2.5 bg-sky-50/60 border-b border-sky-100 flex items-center space-x-2 overflow-x-auto text-xs">
      <span class="text-slate-500 font-medium whitespace-nowrap flex items-center">
        <svg class="w-3.5 h-3.5 mr-1 text-sky-600" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <polygon points="13 2 3 14 12 14 11 22 21 10 12 10 13 2"></polygon>
        </svg>
        Demo Flow:
      </span>
      <!-- Step 1: Prompt without identity (triggers missing field questions) -->
      <button 
        @click="loadPrompt(1)" 
        :disabled="isLoading"
        class="whitespace-nowrap px-2.5 py-1 bg-white hover:bg-sky-50 text-sky-700 font-medium rounded-lg border border-sky-200 shadow-2xs hover:border-sky-300 transition text-[11px] active:scale-95 disabled:opacity-50"
        title="Send move request with only addresses & dates. Demonstrates AI asking for missing mandatory fields."
      >
        1️⃣ Move Request (Missing Identity)
      </button>

      <!-- Step 2: Supply missing identity details -->
      <button 
        @click="loadPrompt(2)" 
        :disabled="isLoading"
        class="whitespace-nowrap px-2.5 py-1 bg-white hover:bg-emerald-50 text-emerald-700 font-medium rounded-lg border border-emerald-200 shadow-2xs hover:border-emerald-300 transition text-[11px] active:scale-95 disabled:opacity-50"
        title="Follow-up reply supplying Account Number, SSN, DOB, Phone, and Email."
      >
        2️⃣ Reply with Missing Details
      </button>

      <!-- Step 3: Complete move in one prompt -->
      <button 
        @click="loadPrompt(3)" 
        :disabled="isLoading"
        class="whitespace-nowrap px-2.5 py-1 bg-white hover:bg-slate-50 text-slate-700 font-medium rounded-lg border border-slate-200 shadow-2xs transition text-[11px] active:scale-95 disabled:opacity-50"
        title="Send entire complete move in one single message."
      >
        ✨ Complete Move (All-in-One)
      </button>
    </div>

    <!-- Message History Stream -->
    <div ref="chatContainer" class="flex-1 overflow-y-auto p-4 sm:p-5 space-y-4">
      <div v-for="(msg, index) in messages" :key="index" class="flex flex-col">
        <!-- AI Message -->
        <div v-if="msg.sender === 'ai'" class="flex items-start space-x-3 max-w-[92%]">
          <div class="w-7 h-7 rounded-full bg-gradient-to-tr from-sky-600 to-emerald-500 text-white flex-shrink-0 flex items-center justify-center text-xs shadow-sm mt-0.5">
            AI
          </div>
          <div class="space-y-1">
            <div class="bg-slate-100 text-slate-800 rounded-2xl rounded-tl-sm px-4 py-3 text-sm shadow-2xs leading-relaxed whitespace-pre-line">
              {{ msg.text }}
            </div>
            <span class="text-[10px] text-slate-400 pl-1">{{ msg.time }}</span>
          </div>
        </div>

        <!-- User Message -->
        <div v-else class="flex items-end justify-end space-x-2 self-end max-w-[90%]">
          <div class="space-y-1 text-right">
            <div class="bg-sky-600 text-white rounded-2xl rounded-tr-sm px-4 py-3 text-sm shadow-sm leading-relaxed text-left whitespace-pre-line">
              {{ msg.text }}
            </div>
            <span class="text-[10px] text-slate-400 pr-1">{{ msg.time }}</span>
          </div>
          <div class="w-7 h-7 rounded-full bg-slate-300 text-slate-700 flex-shrink-0 flex items-center justify-center text-xs font-semibold mt-0.5">
            You
          </div>
        </div>
      </div>

      <!-- Thinking / Loading Indicator -->
      <div v-if="isLoading" class="flex items-start space-x-3 max-w-[85%]">
        <div class="w-7 h-7 rounded-full bg-gradient-to-tr from-sky-600 to-emerald-500 text-white flex-shrink-0 flex items-center justify-center text-xs shadow-sm">
          AI
        </div>
        <div class="bg-slate-100 text-slate-600 rounded-2xl rounded-tl-sm px-4 py-3 text-sm shadow-2xs flex items-center space-x-2">
          <span class="text-xs font-medium text-slate-600">Processing move request with LLM...</span>
          <div class="flex space-x-1 items-center">
            <span class="w-1.5 h-1.5 bg-sky-500 rounded-full animate-bounce [animation-delay:-0.3s]"></span>
            <span class="w-1.5 h-1.5 bg-sky-500 rounded-full animate-bounce [animation-delay:-0.15s]"></span>
            <span class="w-1.5 h-1.5 bg-sky-500 rounded-full animate-bounce"></span>
          </div>
        </div>
      </div>
    </div>

    <!-- Input Footer -->
    <div class="p-3.5 sm:p-4 border-t border-slate-200 bg-white">
      <form @submit.prevent="handleSend" class="relative flex items-center">
        <textarea
          v-model="inputMessage"
          @keydown.enter.exact.prevent="handleSend"
          rows="2"
          placeholder="Describe your move, or provide missing details (Name, SSN, DOB, Account #...)"
          class="w-full resize-none rounded-xl border border-slate-300 pr-24 pl-3.5 py-2.5 text-sm text-slate-800 placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-sky-500 focus:border-transparent transition bg-slate-50/50 hover:bg-white"
          :disabled="isLoading"
        ></textarea>
        
        <div class="absolute right-2 bottom-2.5 flex items-center space-x-1">
          <button
            type="submit"
            :disabled="!inputMessage.trim() || isLoading"
            class="inline-flex items-center justify-center px-3.5 py-1.5 rounded-lg bg-sky-600 hover:bg-sky-700 disabled:bg-slate-300 text-white text-xs font-semibold shadow-sm transition active:scale-95 disabled:active:scale-100"
          >
            <span>Send</span>
            <svg class="w-3.5 h-3.5 ml-1" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="m22 2-7 20-4-9-9-4Z"/>
              <path d="M22 2 11 13"/>
            </svg>
          </button>
        </div>
      </form>
      <div class="flex justify-between items-center mt-1.5 px-1 text-[11px] text-slate-400">
        <span>Press <kbd class="px-1 py-0.5 bg-slate-100 rounded text-[10px] text-slate-600 border border-slate-200 font-mono">Enter</kbd> to send</span>
        <span>Multi-Turn Verified Concierge</span>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, nextTick, watch } from 'vue'

const props = defineProps({
  messages: {
    type: Array,
    required: true
  },
  isLoading: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits(['send'])

const inputMessage = ref('')
const chatContainer = ref(null)

const PROMPTS = {
  1: "I am moving out of 104 Main St on Oct 28th. I bought a house at 450 Oak Drive and need the gas on by the 25th so I can paint before moving in. Send my final bill to 777 Pine Ave.",
  2: "My account number is ACT-8492019, my name is Sarah Connor, SSN is 123-45-6789, DOB is 1985-06-15, phone (555) 234-5678, email sarah.connor@example.com.",
  3: "I am moving from 104 Main St on Oct 28th to 450 Oak Drive on Oct 25th. My utility account number is ACT-8492019, name Sarah Connor, SSN 123-45-6789, DOB 1985-06-15, phone 555-234-5678, email sarah@example.com, forward final bill to 777 Pine Ave."
}

function loadPrompt(step) {
  inputMessage.value = PROMPTS[step] || ''
}

function handleSend() {
  const text = inputMessage.value.trim()
  if (!text || props.isLoading) return
  emit('send', text)
  inputMessage.value = ''
  scrollToBottom()
}

function scrollToBottom() {
  nextTick(() => {
    if (chatContainer.value) {
      chatContainer.value.scrollTop = chatContainer.value.scrollHeight
    }
  })
}

watch(() => props.messages.length, () => {
  scrollToBottom()
})
</script>

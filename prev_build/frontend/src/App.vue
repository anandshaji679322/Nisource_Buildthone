<template>
  <div class="min-h-screen bg-slate-100 flex flex-col font-sans">
    <!-- Top Navigation Header -->
    <HeaderNav 
      :backendOnline="backendOnline" 
      :modelName="modelName"
      @reset="resetState" 
    />

    <!-- Main Content: Split-Screen Layout -->
    <main class="flex-1 max-w-7xl w-full mx-auto p-4 sm:p-6 lg:p-8 flex flex-col">
      <div class="grid grid-cols-1 lg:grid-cols-12 gap-6 flex-1 min-h-[640px]">
        <!-- LEFT PANEL: Conversational Chat Interface (5.5 cols on lg) -->
        <section class="lg:col-span-5 flex flex-col h-full min-h-[480px]">
          <ChatPanel 
            :messages="messages" 
            :isLoading="isLoading" 
            @send="onSendMessage" 
          />
        </section>

        <!-- RIGHT PANEL: Dynamic & Editable Transfer Summary (6.5 cols on lg) -->
        <section class="lg:col-span-7 flex flex-col h-full min-h-[480px]">
          <SummaryPanel 
            :transferData="transferData" 
            :isScheduling="isScheduling" 
            @update:transferData="onUpdateTransferData"
            @confirm="onConfirmTransfer" 
          />
        </section>
      </div>
    </main>

    <!-- Confirmation Modal -->
    <ConfirmationModal 
      :isOpen="showConfirmation" 
      :confirmation="confirmedOrder" 
      :transferDetails="transferData"
      @close="showConfirmation = false" 
    />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import HeaderNav from './components/HeaderNav.vue'
import ChatPanel from './components/ChatPanel.vue'
import SummaryPanel from './components/SummaryPanel.vue'
import ConfirmationModal from './components/ConfirmationModal.vue'
import { processTransfer, confirmTransfer, checkApiStatus } from './services/api'

// Application State
const backendOnline = ref(true)
const modelName = ref('azure/genailab-maas-gpt-4o-mini')
const isLoading = ref(false)
const isScheduling = ref(false)
const showConfirmation = ref(false)
const confirmedOrder = ref(null)

const INITIAL_WELCOME = {
  sender: 'ai',
  text: "Hello! I'm your NiSource Smart Move Assistant. Instead of filling out lengthy forms, tell me about your move in everyday language.\n\nTo schedule your order, utility regulations require: First & Last Name, SSN, Date of Birth, Phone, Email, and your Account Number (for Move & Stop). You can provide them in chat or edit directly in the form on the right!",
  time: 'Just now'
}

const messages = ref([INITIAL_WELCOME])

// Default transfer state
function getDefaultTransferData() {
  return {
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
    isComplete: false,
    missingFields: []
  }
}

const transferData = ref(getDefaultTransferData())

// Check backend status on mount
onMounted(async () => {
  const status = await checkApiStatus()
  if (status) {
    backendOnline.value = true
    if (status.model) {
      modelName.value = status.model
    }
  } else {
    backendOnline.value = false
  }
})

// Sync updates from editable fields on the right
function onUpdateTransferData(updated) {
  transferData.value = { ...transferData.value, ...updated }
}

// Handle sending a message to backend .NET API
async function onSendMessage(userText) {
  const now = new Date()
  const timeStr = now.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })

  // 1. Add User Message
  messages.value.push({
    sender: 'user',
    text: userText,
    time: timeStr
  })

  isLoading.value = true

  try {
    // 2. Call .NET Backend POST /api/transfer/process with existing state
    const result = await processTransfer(userText, transferData.value)

    // 3. Update reactive transfer data (dynamically populates Right Panel)
    transferData.value = { ...transferData.value, ...result }

    // 4. Add AI Response Message
    messages.value.push({
      sender: 'ai',
      text: result.aiExplanation || "I have updated your service transfer details. Please review the form on the right.",
      time: new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
    })

  } catch (err) {
    console.error('Error processing transfer:', err)
    messages.value.push({
      sender: 'ai',
      text: `⚠️ Notice: We ran into an issue connecting to the AI service (${err.message}). Please verify that the .NET API is running.`,
      time: new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
    })
  } finally {
    isLoading.value = false
  }
}

// Handle confirming and scheduling the move
async function onConfirmTransfer(dto) {
  isScheduling.value = true
  try {
    const confirmation = await confirmTransfer(dto)
    confirmedOrder.value = confirmation
    showConfirmation.value = true

    messages.value.push({
      sender: 'ai',
      text: `🎉 Success! Your ${dto.actionIntent} service order has been confirmed under reference #${confirmation.confirmationNumber}. Service scheduled for ${confirmation.scheduledDate}.`,
      time: new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
    })
  } catch (err) {
    alert(`Could not confirm transfer: ${err.message}`)
  } finally {
    isScheduling.value = false
  }
}

// Reset state
function resetState() {
  messages.value = [INITIAL_WELCOME]
  transferData.value = getDefaultTransferData()
  showConfirmation.value = false
  confirmedOrder.value = null
}
</script>

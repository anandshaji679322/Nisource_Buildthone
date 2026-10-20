<template>
  <div v-if="isOpen" class="fixed inset-0 z-50 overflow-y-auto flex items-center justify-center p-4 bg-slate-900/60 backdrop-blur-xs animate-fade-in">
    <div class="relative bg-white rounded-3xl max-w-lg w-full p-6 sm:p-8 shadow-2xl border border-slate-100 text-center transform transition-all">
      <!-- Success Icon -->
      <div class="w-16 h-16 rounded-2xl bg-emerald-100 text-emerald-600 mx-auto flex items-center justify-center mb-4 shadow-sm ring-8 ring-emerald-50">
        <svg class="w-9 h-9" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
          <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/>
          <polyline points="22 4 12 14.01 9 11.01"/>
        </svg>
      </div>

      <h3 class="text-xl font-extrabold text-slate-900 mb-1">Service Order Confirmed & Scheduled!</h3>
      <p class="text-xs text-slate-500 mb-5">
        Your {{ transferDetails?.actionIntent || 'Move' }} order has been dispatched to NiSource field operations.
      </p>

      <!-- Confirmation Card -->
      <div class="bg-slate-50 rounded-2xl p-4 border border-slate-200/80 mb-5 text-left space-y-3">
        <!-- Header Reference -->
        <div class="flex items-center justify-between pb-2.5 border-b border-slate-200">
          <div>
            <div class="text-[10px] font-bold uppercase tracking-wider text-slate-400">Order Reference #</div>
            <div class="text-base font-mono font-bold text-sky-700">{{ confirmation?.confirmationNumber || 'SMA-948102' }}</div>
          </div>
          <span class="inline-flex items-center px-2.5 py-1 rounded-full text-xs font-semibold bg-emerald-100 text-emerald-800">
            ● Dispatched to Crew
          </span>
        </div>

        <!-- Customer Identity & Account -->
        <div class="grid grid-cols-2 gap-2 text-xs pb-2 border-b border-slate-200">
          <div>
            <span class="text-slate-400 block text-[10px] uppercase font-semibold">Account Holder</span>
            <span class="font-bold text-slate-800">{{ transferDetails?.firstName }} {{ transferDetails?.lastName }}</span>
          </div>
          <div>
            <span class="text-slate-400 block text-[10px] uppercase font-semibold">Account Number</span>
            <span class="font-mono font-semibold text-slate-700">{{ transferDetails?.accountNumber || 'Pending Setup' }}</span>
          </div>
        </div>

        <!-- Locations -->
        <div class="grid grid-cols-1 sm:grid-cols-2 gap-3 text-xs">
          <div>
            <span class="text-slate-400 block text-[10px] uppercase font-semibold">
              {{ transferDetails?.actionIntent === 'Start' ? 'Service Location' : 'Current (Disconnect)' }}
            </span>
            <span class="font-semibold text-slate-800 block truncate" :title="transferDetails?.currentAddress">{{ transferDetails?.currentAddress }}</span>
            <span class="text-[11px] text-rose-600 font-medium" v-if="transferDetails?.disconnectDate">Stop: {{ transferDetails?.disconnectDate }}</span>
          </div>
          <div v-if="transferDetails?.actionIntent === 'Move'">
            <span class="text-slate-400 block text-[10px] uppercase font-semibold">New Residence (Connect)</span>
            <span class="font-semibold text-slate-800 block truncate" :title="transferDetails?.newAddress">{{ transferDetails?.newAddress }}</span>
            <span class="text-[11px] text-emerald-600 font-medium">Start: {{ transferDetails?.connectDate }}</span>
          </div>
        </div>

        <!-- Overlap -->
        <div v-if="transferDetails?.actionIntent === 'Move' && transferDetails?.overlapDays > 0" class="pt-2 border-t border-slate-200 flex items-center justify-between text-xs">
          <span class="text-slate-500">Dual-Service Window:</span>
          <span class="font-bold text-amber-700 bg-amber-50 px-2 py-0.5 rounded border border-amber-200">
            {{ transferDetails.overlapDays }} Days Simultaneous Active
          </span>
        </div>

        <!-- Notice -->
        <div class="pt-1 text-[11px] text-slate-500 flex items-center justify-between">
          <span>Confirmation dispatched to:</span>
          <span class="font-medium text-slate-700">{{ transferDetails?.email || 'customer email' }}</span>
        </div>
      </div>

      <!-- Action Buttons -->
      <div class="space-y-2.5">
        <button
          @click="printReceipt"
          class="w-full py-2.5 px-4 rounded-xl border border-slate-300 hover:bg-slate-50 text-slate-700 font-semibold text-xs flex items-center justify-center space-x-1.5 transition active:scale-95 shadow-2xs cursor-pointer"
        >
          <svg class="w-4 h-4 text-slate-500" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <polyline points="6 9 6 2 18 2 18 9"/>
            <path d="M6 18H4a2 2 0 0 1-2-2v-5a2 2 0 0 1 2-2h16a2 2 0 0 1 2 2v5a2 2 0 0 1-2 2h-2"/>
            <rect width="12" height="8" x="6" y="14"/>
          </svg>
          <span>Print / Save Confirmation Receipt</span>
        </button>

        <button
          @click="$emit('close')"
          class="w-full py-3 px-4 rounded-xl bg-slate-900 hover:bg-slate-800 text-white font-bold text-xs tracking-wide transition active:scale-95 shadow-sm cursor-pointer"
        >
          Done
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
const props = defineProps({
  isOpen: {
    type: Boolean,
    default: false
  },
  confirmation: {
    type: Object,
    default: null
  },
  transferDetails: {
    type: Object,
    default: null
  }
})

defineEmits(['close'])

function printReceipt() {
  window.print()
}
</script>

<style scoped>
@keyframes fadeIn {
  from { opacity: 0; transform: scale(0.96); }
  to { opacity: 1; transform: scale(1); }
}
.animate-fade-in {
  animation: fadeIn 0.2s cubic-bezier(0.16, 1, 0.3, 1) forwards;
}
</style>

<script setup>
import { ref, computed } from 'vue';
import { 
  X, 
  ArrowLeftRight, 
  TrendingUp, 
  TrendingDown, 
  Calendar, 
  DollarSign, 
  Zap, 
  Thermometer, 
  CheckCircle 
} from 'lucide-vue-next';

const props = defineProps({
  isOpen: {
    type: Boolean,
    default: false
  },
  history: {
    type: Array,
    required: true
  },
  initialPeriodA: {
    type: String,
    default: '2026-07'
  },
  initialPeriodB: {
    type: String,
    default: '2026-06'
  }
});

const emit = defineEmits(['close']);

const selectedA = ref(props.initialPeriodA);
const selectedB = ref(props.initialPeriodB);

const recordA = computed(() => {
  return props.history.find(x => x.periodId === selectedA.value) || props.history[0];
});

const recordB = computed(() => {
  return props.history.find(x => x.periodId === selectedB.value) || props.history[1] || props.history[0];
});

// Dynamic calculations directly from the two selected records
const billDelta = computed(() => {
  return Math.round((recordA.value.totalBillAmount - recordB.value.totalBillAmount) * 100) / 100;
});

const billPercent = computed(() => {
  if (recordB.value.totalBillAmount <= 0) return 0;
  return Math.round((billDelta.value / recordB.value.totalBillAmount) * 1000) / 10;
});

const kwhDelta = computed(() => {
  return Math.round((recordA.value.consumptionKwh - recordB.value.consumptionKwh) * 10) / 10;
});

const kwhPercent = computed(() => {
  if (recordB.value.consumptionKwh <= 0) return 0;
  return Math.round((kwhDelta.value / recordB.value.consumptionKwh) * 1000) / 10;
});

const daysDelta = computed(() => {
  return recordA.value.billingDays - recordB.value.billingDays;
});

const dailyDelta = computed(() => {
  return Math.round((recordA.value.avgDailyKwh - recordB.value.avgDailyKwh) * 100) / 100;
});

const cddDelta = computed(() => {
  return Math.round((recordA.value.coolingDegreeDays - recordB.value.coolingDegreeDays) * 10) / 10;
});
</script>

<template>
  <div v-if="isOpen" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/80 backdrop-blur-sm">
    <div class="bg-slate-900 border border-slate-700 w-full max-w-3xl rounded-2xl shadow-2xl overflow-hidden flex flex-col max-h-[90vh]">
      
      <!-- Modal Header -->
      <div class="flex items-center justify-between px-6 py-4 border-b border-slate-800 bg-slate-950/70">
        <div class="flex items-center space-x-2.5">
          <ArrowLeftRight class="w-5 h-5 text-emerald-400" />
          <h3 class="text-base font-bold text-white">Compare Two Billing Periods</h3>
        </div>
        <button 
          @click="emit('close')"
          class="text-slate-400 hover:text-white p-1 rounded-lg hover:bg-slate-800 transition-colors"
        >
          <X class="w-5 h-5" />
        </button>
      </div>

      <!-- Modal Body -->
      <div class="p-6 overflow-y-auto space-y-6">
        
        <!-- Period Selectors -->
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div class="bg-slate-950 p-3.5 rounded-xl border border-slate-800">
            <label class="block text-xs font-semibold text-emerald-400 uppercase tracking-wider mb-1.5">
              Target Period (Period A)
            </label>
            <select 
              v-model="selectedA"
              class="w-full bg-slate-900 border border-slate-700 text-white text-sm rounded-lg px-3 py-2 focus:ring-2 focus:ring-emerald-500 focus:outline-none"
            >
              <option v-for="p in history" :key="p.periodId" :value="p.periodId">
                {{ p.monthName }} (${{ p.totalBillAmount.toFixed(2) }} - {{ p.consumptionKwh }} kWh)
              </option>
            </select>
          </div>

          <div class="bg-slate-950 p-3.5 rounded-xl border border-slate-800">
            <label class="block text-xs font-semibold text-cyan-400 uppercase tracking-wider mb-1.5">
              Comparison Baseline (Period B)
            </label>
            <select 
              v-model="selectedB"
              class="w-full bg-slate-900 border border-slate-700 text-white text-sm rounded-lg px-3 py-2 focus:ring-2 focus:ring-cyan-500 focus:outline-none"
            >
              <option v-for="p in history" :key="p.periodId" :value="p.periodId">
                {{ p.monthName }} (${{ p.totalBillAmount.toFixed(2) }} - {{ p.consumptionKwh }} kWh)
              </option>
            </select>
          </div>
        </div>

        <!-- Metric Delta Cards -->
        <div class="grid grid-cols-2 sm:grid-cols-3 gap-3.5">
          
          <!-- Bill Amount Delta -->
          <div class="bg-slate-950/80 border border-slate-800 p-3.5 rounded-xl">
            <span class="text-[11px] text-slate-400 block">Bill Difference</span>
            <div class="flex items-baseline space-x-1 mt-1">
              <span :class="['text-lg font-bold font-mono', billDelta >= 0 ? 'text-rose-400' : 'text-emerald-400']">
                {{ billDelta >= 0 ? '+' : '' }}${{ billDelta.toFixed(2) }}
              </span>
              <span class="text-xs text-slate-400 font-mono">({{ billPercent >= 0 ? '+' : '' }}{{ billPercent }}%)</span>
            </div>
          </div>

          <!-- Consumption Delta -->
          <div class="bg-slate-950/80 border border-slate-800 p-3.5 rounded-xl">
            <span class="text-[11px] text-slate-400 block">Consumption Difference</span>
            <div class="flex items-baseline space-x-1 mt-1">
              <span :class="['text-lg font-bold font-mono', kwhDelta >= 0 ? 'text-amber-400' : 'text-emerald-400']">
                {{ kwhDelta >= 0 ? '+' : '' }}{{ kwhDelta.toFixed(1) }} kWh
              </span>
              <span class="text-xs text-slate-400 font-mono">({{ kwhPercent >= 0 ? '+' : '' }}{{ kwhPercent }}%)</span>
            </div>
          </div>

          <!-- Daily Average Delta -->
          <div class="bg-slate-950/80 border border-slate-800 p-3.5 rounded-xl">
            <span class="text-[11px] text-slate-400 block">Avg Daily Difference</span>
            <div class="flex items-baseline space-x-1 mt-1">
              <span class="text-lg font-bold font-mono text-white">
                {{ dailyDelta >= 0 ? '+' : '' }}{{ dailyDelta.toFixed(2) }}
              </span>
              <span class="text-xs text-slate-400">kWh/day</span>
            </div>
          </div>

          <!-- Days in Cycle Delta -->
          <div class="bg-slate-950/80 border border-slate-800 p-3.5 rounded-xl">
            <span class="text-[11px] text-slate-400 block">Billing Days Difference</span>
            <div class="flex items-baseline space-x-1 mt-1">
              <span class="text-lg font-bold font-mono text-cyan-300">
                {{ daysDelta >= 0 ? '+' : '' }}{{ daysDelta }}
              </span>
              <span class="text-xs text-slate-400">days ({{ recordA.billingDays }} vs {{ recordB.billingDays }})</span>
            </div>
          </div>

          <!-- Weather CDD Delta -->
          <div class="bg-slate-950/80 border border-slate-800 p-3.5 rounded-xl">
            <span class="text-[11px] text-slate-400 block">Cooling Degree Days (CDD)</span>
            <div class="flex items-baseline space-x-1 mt-1">
              <span class="text-lg font-bold font-mono text-amber-300">
                {{ cddDelta >= 0 ? '+' : '' }}{{ cddDelta.toFixed(0) }}
              </span>
              <span class="text-xs text-slate-400">CDD</span>
            </div>
          </div>

          <!-- Avg Temperature Delta -->
          <div class="bg-slate-950/80 border border-slate-800 p-3.5 rounded-xl">
            <span class="text-[11px] text-slate-400 block">Average Temperature</span>
            <div class="flex items-baseline space-x-1 mt-1">
              <span class="text-lg font-bold font-mono text-white">
                {{ (recordA.avgTemperatureF - recordB.avgTemperatureF) >= 0 ? '+' : '' }}{{ (recordA.avgTemperatureF - recordB.avgTemperatureF).toFixed(1) }}°F
              </span>
              <span class="text-xs text-slate-400">({{ recordA.avgTemperatureF }}°F vs {{ recordB.avgTemperatureF }}°F)</span>
            </div>
          </div>

        </div>

        <!-- Detailed Side-by-Side Breakdown Table -->
        <div class="bg-slate-950 rounded-xl border border-slate-800 overflow-hidden">
          <table class="w-full text-left text-xs">
            <thead class="bg-slate-900/80 text-slate-400 border-b border-slate-800">
              <tr>
                <th class="py-2.5 px-4 font-semibold">Component</th>
                <th class="py-2.5 px-4 font-semibold text-emerald-400">{{ recordA.monthName }}</th>
                <th class="py-2.5 px-4 font-semibold text-cyan-400">{{ recordB.monthName }}</th>
                <th class="py-2.5 px-4 font-semibold text-right">Variance</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-slate-800/60 font-mono text-slate-200">
              <tr>
                <td class="py-2.5 px-4 font-sans text-slate-300">Total Billed</td>
                <td class="py-2.5 px-4">${{ recordA.totalBillAmount.toFixed(2) }}</td>
                <td class="py-2.5 px-4">${{ recordB.totalBillAmount.toFixed(2) }}</td>
                <td :class="['py-2.5 px-4 text-right font-bold', billDelta >= 0 ? 'text-rose-400' : 'text-emerald-400']">
                  {{ billDelta >= 0 ? '+' : '' }}${{ billDelta.toFixed(2) }}
                </td>
              </tr>
              <tr>
                <td class="py-2.5 px-4 font-sans text-slate-300">Electricity Usage</td>
                <td class="py-2.5 px-4">{{ recordA.consumptionKwh }} kWh</td>
                <td class="py-2.5 px-4">{{ recordB.consumptionKwh }} kWh</td>
                <td class="py-2.5 px-4 text-right">{{ kwhDelta >= 0 ? '+' : '' }}{{ kwhDelta.toFixed(1) }} kWh</td>
              </tr>
              <tr>
                <td class="py-2.5 px-4 font-sans text-slate-300">Daily Average Rate</td>
                <td class="py-2.5 px-4">{{ recordA.avgDailyKwh.toFixed(2) }} kWh/d</td>
                <td class="py-2.5 px-4">{{ recordB.avgDailyKwh.toFixed(2) }} kWh/d</td>
                <td class="py-2.5 px-4 text-right">{{ dailyDelta >= 0 ? '+' : '' }}{{ dailyDelta.toFixed(2) }} kWh/d</td>
              </tr>
              <tr>
                <td class="py-2.5 px-4 font-sans text-slate-300">Energy Charges</td>
                <td class="py-2.5 px-4">${{ recordA.energyCharges.toFixed(2) }}</td>
                <td class="py-2.5 px-4">${{ recordB.energyCharges.toFixed(2) }}</td>
                <td class="py-2.5 px-4 text-right">${{ (recordA.energyCharges - recordB.energyCharges).toFixed(2) }}</td>
              </tr>
              <tr>
                <td class="py-2.5 px-4 font-sans text-slate-300">Fixed Customer Charge</td>
                <td class="py-2.5 px-4">${{ recordA.fixedCharges.toFixed(2) }}</td>
                <td class="py-2.5 px-4">${{ recordB.fixedCharges.toFixed(2) }}</td>
                <td class="py-2.5 px-4 text-right text-slate-500">$0.00 (Fixed)</td>
              </tr>
              <tr>
                <td class="py-2.5 px-4 font-sans text-slate-300">Taxes & State Surcharges</td>
                <td class="py-2.5 px-4">${{ recordA.taxesAndFees.toFixed(2) }}</td>
                <td class="py-2.5 px-4">${{ recordB.taxesAndFees.toFixed(2) }}</td>
                <td class="py-2.5 px-4 text-right">${{ (recordA.taxesAndFees - recordB.taxesAndFees).toFixed(2) }}</td>
              </tr>
            </tbody>
          </table>
        </div>

      </div>

      <!-- Modal Footer -->
      <div class="px-6 py-3.5 border-t border-slate-800 bg-slate-950/70 flex justify-end">
        <button
          @click="emit('close')"
          class="px-4 py-2 rounded-xl bg-slate-800 hover:bg-slate-700 text-white text-xs font-semibold transition-colors"
        >
          Close Comparison
        </button>
      </div>

    </div>
  </div>
</template>

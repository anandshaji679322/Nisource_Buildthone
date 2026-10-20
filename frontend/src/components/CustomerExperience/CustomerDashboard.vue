<script setup>
import { ref, onMounted } from 'vue';
import { 
  DollarSign, 
  Zap, 
  TrendingUp, 
  TrendingDown, 
  Calendar, 
  MapPin, 
  User, 
  ShieldCheck, 
  ArrowLeftRight, 
  Sparkles,
  Info,
  Clock,
  Gauge
} from 'lucide-vue-next';
import { api } from '../../services/api';
import HistoricalUsageChart from './HistoricalUsageChart.vue';
import AiExplanationPanel from './AiExplanationPanel.vue';
import BillComparisonModal from './BillComparisonModal.vue';

const summary = ref(null);
const loading = ref(true);
const explaining = ref(false);
const explanation = ref(null);
const isCompareOpen = ref(false);
const selectedPeriod = ref('2026-07');

async function loadData(periodId = '2026-07') {
  loading.value = true;
  selectedPeriod.value = periodId;
  try {
    const data = await api.getDashboardSummary('ACT-7492-EAST', periodId);
    summary.value = data;
    explanation.value = null; // Reset explanation for new period until user clicks
  } catch (err) {
    console.error('Error loading summary:', err);
  } finally {
    loading.value = false;
  }
}

async function handleExplainBill() {
  if (!summary.value) return;
  explaining.value = true;
  try {
    const result = await api.explainBill(
      summary.value.profile.accountNumber,
      summary.value.selectedBill.periodId,
      summary.value.previousBill.periodId
    );
    explanation.value = result;
  } catch (err) {
    console.error('Error explaining bill:', err);
  } finally {
    explaining.value = false;
  }
}

onMounted(() => {
  loadData('2026-07');
});
</script>

<template>
  <div class="space-y-6">
    
    <!-- Customer Header Profile & Billing Period Selector -->
    <div class="bg-slate-900/90 rounded-2xl border border-slate-800 p-6 shadow-xl backdrop-blur-sm">
      <div class="flex flex-col lg:flex-row lg:items-center justify-between gap-4">
        
        <!-- Customer Identity Info -->
        <div class="flex items-start sm:items-center space-x-4">
          <div class="w-12 h-12 rounded-2xl bg-emerald-600/20 border border-emerald-500/30 flex items-center justify-center text-emerald-400 font-bold text-lg shadow-inner">
            <User class="w-6 h-6" />
          </div>
          <div>
            <div class="flex items-center space-x-2.5">
              <h2 class="text-xl font-extrabold text-white">
                {{ summary?.profile?.customerName || 'Eleanor Vance' }}
              </h2>
              <span class="text-xs font-mono font-semibold px-2 py-0.5 rounded-md bg-slate-800 text-slate-300 border border-slate-700">
                #{{ summary?.profile?.accountNumber || 'ACT-7492-EAST' }}
              </span>
              <span class="text-[11px] font-bold px-2 py-0.5 rounded-full bg-emerald-500/10 text-emerald-400 border border-emerald-500/20">
                {{ summary?.selectedBill?.status || 'Current' }}
              </span>
            </div>
            <div class="flex flex-wrap items-center gap-x-4 gap-y-1 text-xs text-slate-400 mt-1">
              <span class="flex items-center space-x-1">
                <MapPin class="w-3.5 h-3.5 text-slate-500" />
                <span>{{ summary?.profile?.serviceAddress || '4420 Sycamore Crest Way, Columbus, OH 43215' }}</span>
              </span>
              <span class="flex items-center space-x-1">
                <Gauge class="w-3.5 h-3.5 text-slate-500" />
                <span>Meter: {{ summary?.profile?.meterId || 'MTR-OH-9021' }}</span>
              </span>
              <span class="text-slate-500">|</span>
              <span class="text-slate-300 font-medium">{{ summary?.profile?.ratePlan || 'Residential Standard Electric' }}</span>
            </div>
          </div>
        </div>

        <!-- Period Selector & Compare Trigger -->
        <div class="flex items-center space-x-3">
          <div class="flex items-center space-x-2 bg-slate-950 px-3 py-1.5 rounded-xl border border-slate-800">
            <Calendar class="w-4 h-4 text-emerald-400" />
            <span class="text-xs text-slate-400 font-medium">Billing Period:</span>
            <select
              :value="selectedPeriod"
              @change="loadData($event.target.value)"
              class="bg-transparent text-sm font-bold text-white focus:outline-none cursor-pointer"
            >
              <option 
                v-for="b in summary?.profile?.billingHistory || []" 
                :key="b.periodId" 
                :value="b.periodId"
                class="bg-slate-900 text-white"
              >
                {{ b.monthName }}
              </option>
            </select>
          </div>

          <!-- Compare Button -->
          <button
            @click="isCompareOpen = true"
            class="flex items-center space-x-1.5 px-3.5 py-2 rounded-xl bg-slate-800 hover:bg-slate-700 text-white text-xs font-semibold border border-slate-700 transition-colors shadow-sm"
          >
            <ArrowLeftRight class="w-4 h-4 text-cyan-400" />
            <span>Compare Periods</span>
          </button>
        </div>

      </div>

      <!-- Simulated Data Warning Banner -->
      <div class="mt-4 pt-3 border-t border-slate-800/80 flex items-center justify-between text-[11px] text-slate-400">
        <div class="flex items-center space-x-2">
          <Info class="w-3.5 h-3.5 text-purple-400" />
          <span>Fictional demo account for utility buildathon prototype. All records, meters, and rates are simulated.</span>
        </div>
        <span class="font-mono text-[10px] text-slate-500">Cycle Duration: {{ summary?.selectedBill?.billingDays }} Days ({{ summary?.selectedBill?.startDate }} to {{ summary?.selectedBill?.endDate }})</span>
      </div>
    </div>

    <!-- KPI Summary Metric Cards -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
      
      <!-- Current Bill Amount -->
      <div class="bg-slate-900/90 rounded-2xl border border-slate-800 p-5 shadow-lg relative overflow-hidden">
        <div class="flex items-center justify-between text-slate-400 text-xs font-medium">
          <span>Current Bill Amount</span>
          <DollarSign class="w-4 h-4 text-emerald-400" />
        </div>
        <div class="mt-2 flex items-baseline space-x-2">
          <span class="text-3xl font-extrabold text-white font-mono tracking-tight">
            ${{ summary?.selectedBill?.totalBillAmount?.toFixed(2) || '0.00' }}
          </span>
          <span 
            :class="[
              'text-xs font-bold font-mono flex items-center',
              (summary?.billAmountDelta || 0) >= 0 ? 'text-rose-400' : 'text-emerald-400'
            ]"
          >
            <component :is="(summary?.billAmountDelta || 0) >= 0 ? TrendingUp : TrendingDown" class="w-3.5 h-3.5 mr-0.5 inline" />
            {{ (summary?.billAmountPercentChange || 0) >= 0 ? '+' : '' }}{{ summary?.billAmountPercentChange || 0 }}%
          </span>
        </div>
        <div class="mt-2 text-[11px] text-slate-400 flex items-center justify-between">
          <span>vs Previous ({{ summary?.previousBill?.monthName }}):</span>
          <span class="font-mono text-slate-300 font-semibold">${{ summary?.previousBill?.totalBillAmount?.toFixed(2) }}</span>
        </div>
      </div>

      <!-- Electricity Consumption (kWh) -->
      <div class="bg-slate-900/90 rounded-2xl border border-slate-800 p-5 shadow-lg relative overflow-hidden">
        <div class="flex items-center justify-between text-slate-400 text-xs font-medium">
          <span>Total Consumption</span>
          <Zap class="w-4 h-4 text-amber-400" />
        </div>
        <div class="mt-2 flex items-baseline space-x-2">
          <span class="text-3xl font-extrabold text-white font-mono tracking-tight">
            {{ summary?.selectedBill?.consumptionKwh || 0 }}
          </span>
          <span class="text-xs text-slate-400 font-bold">kWh</span>
          <span 
            :class="[
              'text-xs font-bold font-mono ml-auto',
              (summary?.consumptionDeltaKwh || 0) >= 0 ? 'text-amber-400' : 'text-emerald-400'
            ]"
          >
            {{ (summary?.consumptionPercentChange || 0) >= 0 ? '+' : '' }}{{ summary?.consumptionPercentChange || 0 }}%
          </span>
        </div>
        <div class="mt-2 text-[11px] text-slate-400 flex items-center justify-between">
          <span>vs Previous ({{ summary?.previousBill?.monthName }}):</span>
          <span class="font-mono text-slate-300 font-semibold">{{ summary?.previousBill?.consumptionKwh }} kWh</span>
        </div>
      </div>

      <!-- Average Daily Consumption -->
      <div class="bg-slate-900/90 rounded-2xl border border-slate-800 p-5 shadow-lg relative overflow-hidden">
        <div class="flex items-center justify-between text-slate-400 text-xs font-medium">
          <span>Average Daily Usage</span>
          <Clock class="w-4 h-4 text-cyan-400" />
        </div>
        <div class="mt-2 flex items-baseline space-x-2">
          <span class="text-3xl font-extrabold text-white font-mono tracking-tight">
            {{ summary?.selectedBill?.avgDailyKwh?.toFixed(2) || '0.00' }}
          </span>
          <span class="text-xs text-slate-400 font-bold">kWh/day</span>
        </div>
        <div class="mt-2 text-[11px] text-slate-400 flex items-center justify-between">
          <span>Delta from previous cycle:</span>
          <span class="font-mono text-slate-300 font-semibold">
            {{ (summary?.avgDailyDeltaKwh || 0) >= 0 ? '+' : '' }}{{ summary?.avgDailyDeltaKwh?.toFixed(2) }} kWh/d
          </span>
        </div>
      </div>

      <!-- Billing Cycle Details & Rate -->
      <div class="bg-slate-900/90 rounded-2xl border border-slate-800 p-5 shadow-lg relative overflow-hidden">
        <div class="flex items-center justify-between text-slate-400 text-xs font-medium">
          <span>Effective Rate & Climate</span>
          <Calendar class="w-4 h-4 text-purple-400" />
        </div>
        <div class="mt-2 flex items-baseline space-x-2">
          <span class="text-3xl font-extrabold text-white font-mono tracking-tight">
            ${{ summary?.selectedBill?.ratePerKwh?.toFixed(3) }}
          </span>
          <span class="text-xs text-slate-400 font-bold">/ kWh</span>
        </div>
        <div class="mt-2 text-[11px] text-slate-400 flex items-center justify-between">
          <span>Avg Temperature / CDD:</span>
          <span class="font-mono text-amber-300 font-semibold">
            {{ summary?.selectedBill?.avgTemperatureF }}°F ({{ summary?.selectedBill?.coolingDegreeDays }} CDD)
          </span>
        </div>
      </div>

    </div>

    <!-- AI Bill Explanation Panel (Prominent Feature) -->
    <AiExplanationPanel
      :explanation="explanation"
      :loading="explaining"
      :selected-month-name="summary?.selectedBill?.monthName"
      @request-explanation="handleExplainBill"
    />

    <!-- Historical Usage & Load Profile Visualization -->
    <HistoricalUsageChart
      v-if="summary?.profile?.billingHistory"
      :history="summary.profile.billingHistory"
      :selected-period-id="selectedPeriod"
      :daily-usage="summary?.selectedBill?.dailyUsage || []"
      @select-period="loadData"
    />

    <!-- Bill Comparison Modal -->
    <BillComparisonModal
      v-if="summary?.profile?.billingHistory"
      :is-open="isCompareOpen"
      :history="summary.profile.billingHistory"
      :initial-period-a="selectedPeriod"
      :initial-period-b="summary?.previousBill?.periodId"
      @close="isCompareOpen = false"
    />

  </div>
</template>

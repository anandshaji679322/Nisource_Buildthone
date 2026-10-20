<script setup>
import { ref, computed } from 'vue';
import { 
  Building2, 
  MapPin, 
  Gauge, 
  User, 
  Zap, 
  CheckCircle2, 
  AlertTriangle, 
  RotateCcw, 
  Calendar, 
  Sun, 
  CloudSun, 
  ShieldAlert, 
  TrendingUp, 
  HelpCircle,
  FileCheck,
  DollarSign,
  Activity,
  Layers,
  Thermometer
} from 'lucide-vue-next';
import { api } from '../../services/api';

const props = defineProps({
  detail: {
    type: Object,
    required: true
  },
  loading: {
    type: Boolean,
    default: false
  }
});

const emit = defineEmits(['statusUpdated', 'close']);

const actionLoading = ref(false);
const actionFeedback = ref('');

async function performAction(action) {
  if (!props.detail?.record) return;
  actionLoading.value = true;
  actionFeedback.value = '';
  try {
    const res = await api.updateReviewStatus(props.detail.record.customerId, action);
    actionFeedback.value = res.message;
    emit('statusUpdated', { customerId: props.detail.record.customerId, newStatus: res.newStatus });
  } catch (err) {
    console.error('Action error:', err);
    actionFeedback.value = 'Failed to update review status.';
  } finally {
    actionLoading.value = false;
  }
}

const maxChartKwh = computed(() => {
  if (!props.detail?.record?.historicalReadings) return 800;
  const historyVals = props.detail.record.historicalReadings
    .filter(x => x.actualKwh)
    .map(x => x.actualKwh);
  const estimateVals = [
    props.detail.record.baseline3MonthAvgKwh || 0,
    props.detail.record.smartBillAiEstimateKwh || 0
  ];
  return Math.max(...historyVals, ...estimateVals, 800);
});
</script>

<template>
  <div class="bg-slate-900 rounded-2xl border border-slate-800 p-6 shadow-2xl space-y-6">
    
    <!-- Top Workspace Header -->
    <div class="flex flex-col lg:flex-row lg:items-center justify-between pb-6 border-b border-slate-800 gap-4">
      <div>
        <div class="flex items-center space-x-3">
          <div class="w-10 h-10 rounded-xl bg-cyan-600/20 border border-cyan-500/30 flex items-center justify-center text-cyan-400">
            <Gauge class="w-5 h-5" />
          </div>
          <div>
            <div class="flex items-center space-x-2">
              <h3 class="text-lg font-extrabold text-white">
                {{ detail.record.customerName }}
              </h3>
              <span class="text-xs font-mono font-bold px-2 py-0.5 rounded bg-slate-800 text-slate-300 border border-slate-700">
                {{ detail.record.customerId }}
              </span>
              <span 
                :class="[
                  'text-[10px] font-bold px-2 py-0.5 rounded-full uppercase tracking-wider',
                  detail.record.reviewStatus === 'Accepted'
                    ? 'bg-emerald-500/20 text-emerald-300 border border-emerald-500/30'
                    : (detail.record.reviewStatus === 'Flagged for Inspection'
                      ? 'bg-rose-500/20 text-rose-300 border border-rose-500/30'
                      : 'bg-amber-500/20 text-amber-300 border border-amber-500/30')
                ]"
              >
                {{ detail.record.reviewStatus }}
              </span>
            </div>
            <div class="flex items-center space-x-3 text-xs text-slate-400 mt-1">
              <span>Meter: <strong class="text-slate-200 font-mono">{{ detail.record.meterId }}</strong></span>
              <span>•</span>
              <span>{{ detail.record.serviceAddress }}</span>
              <span>•</span>
              <span class="text-cyan-400">{{ detail.record.region }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Action Workflow Buttons -->
      <div class="flex flex-wrap items-center gap-2">
        <!-- Accept Button -->
        <button
          @click="performAction('accept')"
          :disabled="actionLoading"
          class="flex items-center space-x-1.5 px-4 py-2 rounded-xl bg-emerald-600 hover:bg-emerald-500 text-white text-xs font-semibold shadow-md shadow-emerald-600/20 transition-all cursor-pointer disabled:opacity-50"
        >
          <CheckCircle2 class="w-4 h-4" />
          <span>Accept Estimate for Demo</span>
        </button>

        <!-- Flag for Review -->
        <button
          @click="performAction('flag')"
          :disabled="actionLoading"
          class="flex items-center space-x-1.5 px-4 py-2 rounded-xl bg-rose-600/20 hover:bg-rose-600/30 border border-rose-500/40 text-rose-300 text-xs font-semibold transition-all cursor-pointer disabled:opacity-50"
        >
          <AlertTriangle class="w-4 h-4" />
          <span>Flag for Manual Inspection</span>
        </button>

        <!-- Reset Record -->
        <button
          @click="performAction('reset')"
          :disabled="actionLoading"
          class="flex items-center space-x-1.5 px-3 py-2 rounded-xl bg-slate-800 hover:bg-slate-700 text-slate-300 text-xs font-semibold border border-slate-700 transition-all cursor-pointer disabled:opacity-50"
          title="Reset back to pending"
        >
          <RotateCcw class="w-3.5 h-3.5" />
          <span>Reset</span>
        </button>
      </div>
    </div>

    <!-- Feedback Message if Action clicked -->
    <div v-if="actionFeedback" class="p-3 rounded-lg bg-emerald-950/60 border border-emerald-500/30 text-emerald-200 text-xs flex items-center justify-between">
      <span>{{ actionFeedback }}</span>
      <span class="text-[10px] text-emerald-400 font-mono">Simulated Workflow State Updated</span>
    </div>

    <!-- Comparative Estimation Summary Header (Baseline vs. SmartBill AI) -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
      
      <!-- Box 1: Traditional 3-Month Average Baseline -->
      <div class="bg-slate-950 rounded-xl border border-slate-800 p-4 relative overflow-hidden">
        <div class="flex items-center justify-between text-slate-400 text-xs font-semibold">
          <span>Traditional Baseline (3-Mo Avg)</span>
          <span class="text-[10px] px-1.5 py-0.5 rounded bg-slate-800 text-slate-400 font-mono">Normalized by Days</span>
        </div>
        <div class="mt-2 flex items-baseline space-x-2">
          <span class="text-3xl font-extrabold font-mono text-slate-300">
            {{ detail.record.baseline3MonthAvgKwh }}
          </span>
          <span class="text-xs text-slate-400 font-bold">kWh</span>
          <span class="text-sm font-mono text-slate-400 ml-auto font-semibold">
            ${{ detail.record.estimatedDollarBaseline.toFixed(2) }}
          </span>
        </div>
        <p class="text-[11px] text-slate-400 mt-2 font-mono">
          Method: Simple rolling average of April, May & June. Ignores upcoming July heatwave.
        </p>
      </div>

      <!-- Box 2: SmartBill AI Estimate -->
      <div class="bg-slate-950 rounded-xl border border-cyan-500/40 p-4 relative overflow-hidden ring-1 ring-cyan-500/20">
        <div class="flex items-center justify-between text-cyan-400 text-xs font-semibold">
          <div class="flex items-center space-x-1.5">
            <Zap class="w-3.5 h-3.5 text-cyan-400" />
            <span>SmartBill AI Estimate</span>
          </div>
          <span class="text-[10px] px-2 py-0.5 rounded-full bg-cyan-500/10 text-cyan-300 font-mono font-bold">
            Weather & Seasonal Aware
          </span>
        </div>
        <div class="mt-2 flex items-baseline space-x-2">
          <span class="text-3xl font-extrabold font-mono text-cyan-300">
            {{ detail.record.smartBillAiEstimateKwh }}
          </span>
          <span class="text-xs text-cyan-400 font-bold">kWh</span>
          <span class="text-sm font-mono text-emerald-400 ml-auto font-semibold">
            ${{ detail.record.estimatedDollarSmartBill.toFixed(2) }}
          </span>
        </div>
        <div class="text-[11px] text-cyan-200/80 mt-2 flex items-center justify-between">
          <span>Uncertainty Range:</span>
          <span class="font-mono font-bold text-white">{{ detail.record.confidenceRange }}</span>
        </div>
      </div>

      <!-- Box 3: Variance & Risk Impact -->
      <div class="bg-slate-950 rounded-xl border border-amber-500/40 p-4 relative overflow-hidden">
        <div class="flex items-center justify-between text-amber-400 text-xs font-semibold">
          <span>Variance & Financial Risk</span>
          <span 
            :class="[
              'text-[10px] px-2 py-0.5 rounded-full font-bold uppercase',
              detail.record.riskLevel === 'High' ? 'bg-rose-500/20 text-rose-300' : 'bg-amber-500/20 text-amber-300'
            ]"
          >
            {{ detail.record.riskLevel }} Risk
          </span>
        </div>
        <div class="mt-2 flex items-baseline space-x-2">
          <span :class="['text-3xl font-extrabold font-mono', detail.record.varianceKwh >= 0 ? 'text-amber-300' : 'text-cyan-300']">
            {{ detail.record.varianceKwh >= 0 ? '+' : '' }}{{ detail.record.varianceKwh }}
          </span>
          <span class="text-xs text-slate-400 font-bold">kWh</span>
          <span class="text-xs font-mono font-bold ml-1 text-slate-300">
            ({{ detail.record.variancePercent >= 0 ? '+' : '' }}{{ detail.record.variancePercent }}%)
          </span>
          <span class="text-sm font-mono text-amber-400 ml-auto font-bold">
            {{ detail.record.financialVarianceAmount >= 0 ? '+' : '' }}${{ detail.record.financialVarianceAmount.toFixed(2) }}
          </span>
        </div>
        <p class="text-[11px] text-amber-200/80 mt-2">
          Flag: <strong class="text-white">{{ detail.record.potentialErrorDirection }}</strong> if estimated with baseline.
        </p>
      </div>

    </div>

    <!-- Comparative Load Curve Visualization (Actuals vs Baseline vs SmartBill) -->
    <div class="bg-slate-950 rounded-xl border border-slate-800 p-5">
      <div class="flex items-center justify-between mb-4">
        <div>
          <h4 class="text-xs font-bold uppercase tracking-wider text-slate-200">
            Historical Actuals & Dual-Estimate Projection
          </h4>
          <p class="text-[11px] text-slate-400">
            Comparing verified meter history (Aug 2025 – Jun 2026) with the competing July 2026 estimates
          </p>
        </div>
        <div class="flex items-center space-x-4 text-xs">
          <span class="flex items-center space-x-1">
            <span class="w-3 h-3 rounded-sm bg-slate-600"></span>
            <span class="text-slate-400 text-[11px]">Actual Meter Reads</span>
          </span>
          <span class="flex items-center space-x-1">
            <span class="w-3 h-3 rounded-sm bg-slate-400 border border-dashed border-white"></span>
            <span class="text-slate-400 text-[11px]">3-Mo Avg ({{ detail.record.baseline3MonthAvgKwh }} kWh)</span>
          </span>
          <span class="flex items-center space-x-1">
            <span class="w-3 h-3 rounded-sm bg-cyan-400 shadow-sm shadow-cyan-400"></span>
            <span class="text-cyan-300 text-[11px] font-bold">SmartBill AI ({{ detail.record.smartBillAiEstimateKwh }} kWh)</span>
          </span>
        </div>
      </div>

      <!-- Bar Chart -->
      <div class="h-56 flex items-end justify-between gap-2 pt-6 px-1 relative">
        <div class="absolute inset-x-0 top-6 border-b border-slate-800/60 pointer-events-none"></div>
        <div class="absolute inset-x-0 top-1/2 border-b border-slate-800/40 pointer-events-none"></div>

        <!-- Historical Bars -->
        <div
          v-for="h in detail.record.historicalReadings"
          :key="h.periodId"
          class="flex-1 flex flex-col items-center justify-end h-full group"
        >
          <!-- Value Label -->
          <span class="text-[9px] font-mono text-slate-400 mb-1 opacity-0 group-hover:opacity-100 transition-opacity">
            {{ h.actualKwh || 'Missing' }}
          </span>

          <!-- Bar -->
          <div
            v-if="h.actualKwh"
            :style="{ height: `${(h.actualKwh / maxChartKwh) * 100}%` }"
            class="w-full max-w-[34px] rounded-t-md bg-slate-700/80 hover:bg-slate-600 transition-all"
            :title="`${h.monthLabel}: ${h.actualKwh} kWh (${h.avgTempF}°F)`"
          ></div>

          <!-- If July Missing Column, Show Side-by-side or stacked projection comparison -->
          <div v-else class="w-full max-w-[48px] flex items-end justify-center space-x-1 h-full">
            <!-- Baseline Bar -->
            <div
              :style="{ height: `${(detail.record.baseline3MonthAvgKwh / maxChartKwh) * 100}%` }"
              class="w-1/2 rounded-t bg-slate-500 border border-dashed border-slate-300"
              :title="`Baseline 3-Month Average: ${detail.record.baseline3MonthAvgKwh} kWh`"
            ></div>
            <!-- SmartBill Bar -->
            <div
              :style="{ height: `${(detail.record.smartBillAiEstimateKwh / maxChartKwh) * 100}%` }"
              class="w-1/2 rounded-t bg-gradient-to-t from-cyan-600 to-cyan-400 shadow-md shadow-cyan-500/30"
              :title="`SmartBill AI: ${detail.record.smartBillAiEstimateKwh} kWh`"
            ></div>
          </div>

          <!-- Label -->
          <span 
            :class="[
              'text-[10px] mt-2 font-mono',
              !h.actualKwh ? 'text-cyan-400 font-bold' : 'text-slate-400'
            ]"
          >
            {{ h.monthLabel.split(' ')[0] }}
          </span>
        </div>
      </div>
    </div>

    <!-- Factor-by-Factor Explainability Breakdown & Weather Integration -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-5">
      
      <!-- Left: Weather Integration Panel (Open-Meteo Provenance) -->
      <div class="bg-slate-950 rounded-xl border border-slate-800 p-4 space-y-3">
        <div class="flex items-center justify-between border-b border-slate-800/80 pb-2">
          <div class="flex items-center space-x-2">
            <Sun class="w-4 h-4 text-amber-400" />
            <h4 class="text-xs font-bold uppercase tracking-wider text-slate-200">
              Weather Integration & Climate Analytics
            </h4>
          </div>
          <span class="text-[10px] px-2 py-0.5 rounded bg-emerald-500/10 text-emerald-400 font-mono border border-emerald-500/20">
            {{ detail.weatherDetails.weatherSource }}
          </span>
        </div>

        <div class="grid grid-cols-3 gap-2.5 text-center text-xs">
          <div class="bg-slate-900 p-2.5 rounded-lg border border-slate-800">
            <span class="text-[10px] text-slate-400 block">Avg Temperature</span>
            <span class="text-sm font-bold text-amber-300 font-mono mt-0.5 block">
              {{ detail.weatherDetails.avgTemperatureF }}°F
            </span>
          </div>
          <div class="bg-slate-900 p-2.5 rounded-lg border border-slate-800">
            <span class="text-[10px] text-slate-400 block">Cooling Degree Days</span>
            <span class="text-sm font-bold text-rose-300 font-mono mt-0.5 block">
              {{ detail.weatherDetails.coolingDegreeDays }} CDD
            </span>
          </div>
          <div class="bg-slate-900 p-2.5 rounded-lg border border-slate-800">
            <span class="text-[10px] text-slate-400 block">Peak Max Temp</span>
            <span class="text-sm font-bold text-white font-mono mt-0.5 block">
              {{ detail.weatherDetails.maxTemperatureF }}°F
            </span>
          </div>
        </div>

        <div class="p-3 rounded-lg bg-slate-900 border border-slate-800 text-xs space-y-1">
          <div class="flex items-center justify-between text-slate-300 font-medium">
            <span>Customer Heat Sensitivity (β):</span>
            <span class="font-mono text-cyan-300">{{ detail.record.baseCoolingSensitivityKwhPerCdd }} kWh / CDD</span>
          </div>
          <div class="flex items-center justify-between text-slate-300 font-medium">
            <span>Base Non-Weather Load:</span>
            <span class="font-mono text-slate-300">{{ detail.record.baseLoadDailyKwh }} kWh / day</span>
          </div>
          <div class="flex items-center justify-between text-slate-400 text-[11px] pt-1 border-t border-slate-800">
            <span>Climate Condition:</span>
            <span class="text-amber-400 font-semibold">{{ detail.weatherDetails.weatherConditionSummary }}</span>
          </div>
        </div>
      </div>

      <!-- Right: Factor Attribution Breakdown -->
      <div class="bg-slate-950 rounded-xl border border-slate-800 p-4 space-y-3">
        <div class="flex items-center justify-between border-b border-slate-800/80 pb-2">
          <div class="flex items-center space-x-2">
            <Layers class="w-4 h-4 text-cyan-400" />
            <h4 class="text-xs font-bold uppercase tracking-wider text-slate-200">
              Factor-by-Factor Estimate Attribution
            </h4>
          </div>
          <span class="text-[10px] font-mono text-slate-400">Deterministic Formulation</span>
        </div>

        <div class="space-y-2">
          <div 
            v-for="(f, idx) in detail.explanationFactors" 
            :key="idx"
            class="p-2.5 rounded-lg bg-slate-900 border border-slate-800 text-xs"
          >
            <div class="flex items-center justify-between mb-0.5">
              <span class="font-bold text-slate-200">{{ f.factor }}</span>
              <span class="font-mono font-bold text-emerald-400">{{ f.contributionKwh }}</span>
            </div>
            <p class="text-[11px] text-slate-400 leading-snug">{{ f.description }}</p>
          </div>
        </div>
      </div>

    </div>

    <!-- Bottom Operational Recommendation & Disclaimer -->
    <div class="p-4 rounded-xl bg-slate-950 border border-slate-800 flex flex-col sm:flex-row sm:items-center justify-between gap-3 text-xs">
      <div class="flex items-start space-x-2.5">
        <ShieldAlert class="w-5 h-5 text-cyan-400 flex-shrink-0 mt-0.5" />
        <div>
          <span class="font-bold text-white block mb-0.5">Operational Guidance</span>
          <p class="text-slate-300">{{ detail.operationalRecommendation }}</p>
        </div>
      </div>
      <span class="text-[11px] text-slate-500 italic flex-shrink-0">
        Recommendation requires operator review and approval under utility policy.
      </span>
    </div>

  </div>
</template>

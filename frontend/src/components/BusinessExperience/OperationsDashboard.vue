<script setup>
import { ref, computed, onMounted } from 'vue';
import { 
  Building2, 
  AlertTriangle, 
  CheckCircle2, 
  Clock, 
  Filter, 
  Search, 
  Gauge, 
  MapPin, 
  TrendingUp, 
  TrendingDown, 
  BarChart3, 
  ArrowUpRight,
  ShieldAlert,
  RotateCcw,
  Sparkles
} from 'lucide-vue-next';
import { api } from '../../services/api';
import EstimationWorkspace from './EstimationWorkspace.vue';
import AccuracyEvaluationModal from './AccuracyEvaluationModal.vue';

const dashboardData = ref(null);
const loading = ref(true);
const selectedCustomer = ref(null);
const detailData = ref(null);
const detailLoading = ref(false);
const isBacktestOpen = ref(false);

// Filters
const filterRegion = ref('All');
const filterRisk = ref('All');
const filterStatus = ref('All');
const filterDirection = ref('All');
const searchQuery = ref('');

async function loadDashboard() {
  loading.value = true;
  try {
    const data = await api.getEstimatorDashboard();
    dashboardData.value = data;
    if (data.records && data.records.length > 0 && !selectedCustomer.value) {
      selectCustomer(data.records[0]);
    }
  } catch (err) {
    console.error('Error loading estimator dashboard:', err);
  } finally {
    loading.value = false;
  }
}

async function selectCustomer(record) {
  selectedCustomer.value = record;
  detailLoading.value = true;
  try {
    const detail = await api.getMeterDetail(record.customerId);
    detailData.value = detail;
  } catch (err) {
    console.error('Error fetching meter detail:', err);
  } finally {
    detailLoading.value = false;
  }
}

function handleStatusUpdated({ customerId, newStatus }) {
  if (dashboardData.value?.records) {
    const rec = dashboardData.value.records.find(r => r.customerId === customerId);
    if (rec) rec.reviewStatus = newStatus;
  }
  if (detailData.value?.record) {
    detailData.value.record.reviewStatus = newStatus;
  }
}

const filteredRecords = computed(() => {
  if (!dashboardData.value?.records) return [];
  return dashboardData.value.records.filter(r => {
    if (filterRegion.value !== 'All' && !r.region.includes(filterRegion.value)) return false;
    if (filterRisk.value !== 'All' && r.riskLevel !== filterRisk.value) return false;
    if (filterStatus.value !== 'All' && r.reviewStatus !== filterStatus.value) return false;
    if (filterDirection.value !== 'All' && r.potentialErrorDirection !== filterDirection.value) return false;
    if (searchQuery.value) {
      const q = searchQuery.value.toLowerCase();
      const matchName = r.customerName.toLowerCase().includes(q);
      const matchId = r.customerId.toLowerCase().includes(q);
      const matchMeter = r.meterId.toLowerCase().includes(q);
      if (!matchName && !matchId && !matchMeter) return false;
    }
    return true;
  });
});

onMounted(() => {
  loadDashboard();
});

defineExpose({
  openBacktest: () => { isBacktestOpen.value = true; }
});
</script>

<template>
  <div class="space-y-6">
    
    <!-- Top Operations Header -->
    <div class="bg-slate-900/90 rounded-2xl border border-slate-800 p-6 shadow-xl backdrop-blur-sm">
      <div class="flex flex-col lg:flex-row lg:items-center justify-between gap-4">
        <div>
          <div class="flex items-center space-x-3">
            <div class="w-10 h-10 rounded-xl bg-cyan-600/20 border border-cyan-500/30 flex items-center justify-center text-cyan-400">
              <Building2 class="w-5 h-5" />
            </div>
            <div>
              <div class="flex items-center space-x-2">
                <h2 class="text-xl font-extrabold text-white">Billing Operations & Meter Estimator</h2>
                <span class="text-[10px] font-mono px-2 py-0.5 rounded-full bg-cyan-500/10 text-cyan-300 border border-cyan-500/20 font-bold">
                  Weather-Aware Estimation
                </span>
              </div>
              <p class="text-xs text-slate-400 mt-0.5">
                Replace uncalibrated 3-month historical averages with multi-factor weather sensitivity regression
              </p>
            </div>
          </div>
        </div>

        <div class="flex items-center space-x-3">
          <button
            @click="isBacktestOpen = true"
            class="flex items-center space-x-2 px-4 py-2 rounded-xl bg-gradient-to-r from-indigo-600 to-indigo-500 hover:from-indigo-500 hover:to-indigo-400 text-white text-xs font-semibold shadow-lg shadow-indigo-600/20 transition-all cursor-pointer"
          >
            <BarChart3 class="w-4 h-4" />
            <span>Accuracy Backtest Benchmark</span>
          </button>
        </div>
      </div>
    </div>

    <!-- Operations Dashboard KPI Cards -->
    <div class="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-6 gap-3.5">
      
      <!-- Total Missing Readings -->
      <div class="bg-slate-900/80 rounded-xl border border-slate-800 p-4">
        <span class="text-[11px] text-slate-400 block font-medium">Missing Meter Reads</span>
        <div class="text-2xl font-extrabold font-mono text-white mt-1">
          {{ dashboardData?.totalMissingReadings || 16 }}
        </div>
        <span class="text-[10px] text-slate-500 mt-0.5 block">Cycle: July 2026</span>
      </div>

      <!-- Requiring Estimated Bill -->
      <div class="bg-slate-900/80 rounded-xl border border-slate-800 p-4">
        <span class="text-[11px] text-slate-400 block font-medium">Require Estimate</span>
        <div class="text-2xl font-extrabold font-mono text-cyan-300 mt-1">
          {{ dashboardData?.requiringEstimatedBill || 12 }}
        </div>
        <span class="text-[10px] text-slate-500 mt-0.5 block">Billing exceptions</span>
      </div>

      <!-- Awaiting Review -->
      <div class="bg-slate-900/80 rounded-xl border border-amber-500/30 p-4">
        <span class="text-[11px] text-slate-400 block font-medium">Awaiting Review</span>
        <div class="text-2xl font-extrabold font-mono text-amber-400 mt-1">
          {{ dashboardData?.awaitingReview || 8 }}
        </div>
        <span class="text-[10px] text-amber-300/80 mt-0.5 block">Action required</span>
      </div>

      <!-- High Risk Cases -->
      <div class="bg-slate-900/80 rounded-xl border border-rose-500/30 p-4">
        <span class="text-[11px] text-slate-400 block font-medium">High Risk Cases</span>
        <div class="text-2xl font-extrabold font-mono text-rose-400 mt-1">
          {{ dashboardData?.highRiskCases || 4 }}
        </div>
        <span class="text-[10px] text-rose-300/80 mt-0.5 block">Large Δ variance</span>
      </div>

      <!-- Underestimation Flags -->
      <div class="bg-slate-900/80 rounded-xl border border-slate-800 p-4">
        <span class="text-[11px] text-slate-400 block font-medium">Underestimation Flags</span>
        <div class="text-2xl font-extrabold font-mono text-amber-300 mt-1">
          {{ dashboardData?.underestimationFlags || 9 }}
        </div>
        <span class="text-[10px] text-slate-500 mt-0.5 block">Heatwave cooling load</span>
      </div>

      <!-- Overestimation Flags -->
      <div class="bg-slate-900/80 rounded-xl border border-slate-800 p-4">
        <span class="text-[11px] text-slate-400 block font-medium">Overestimation Flags</span>
        <div class="text-2xl font-extrabold font-mono text-teal-300 mt-1">
          {{ dashboardData?.overestimationFlags || 3 }}
        </div>
        <span class="text-[10px] text-slate-500 mt-0.5 block">Reduced seasonal load</span>
      </div>

    </div>

    <!-- Filter Bar -->
    <div class="bg-slate-900/80 p-4 rounded-xl border border-slate-800 flex flex-wrap items-center justify-between gap-3 text-xs">
      
      <!-- Left Filters -->
      <div class="flex flex-wrap items-center gap-2.5">
        <div class="flex items-center space-x-1 text-slate-400 font-semibold mr-1">
          <Filter class="w-3.5 h-3.5" />
          <span>Filters:</span>
        </div>

        <!-- Region Filter -->
        <select 
          v-model="filterRegion"
          class="bg-slate-950 border border-slate-700 text-slate-200 text-xs rounded-lg px-2.5 py-1.5 focus:outline-none"
        >
          <option value="All">All Regions</option>
          <option value="Columbus">Central Ohio</option>
          <option value="Merrillville">Northern Indiana</option>
          <option value="Pittsburgh">Western Pennsylvania</option>
          <option value="Richmond">Eastern Virginia</option>
          <option value="Cincinnati">Southern Ohio</option>
        </select>

        <!-- Risk Filter -->
        <select 
          v-model="filterRisk"
          class="bg-slate-950 border border-slate-700 text-slate-200 text-xs rounded-lg px-2.5 py-1.5 focus:outline-none"
        >
          <option value="All">All Risk Levels</option>
          <option value="High">High Risk</option>
          <option value="Medium">Medium Risk</option>
          <option value="Low">Low Risk</option>
        </select>

        <!-- Status Filter -->
        <select 
          v-model="filterStatus"
          class="bg-slate-950 border border-slate-700 text-slate-200 text-xs rounded-lg px-2.5 py-1.5 focus:outline-none"
        >
          <option value="All">All Review Statuses</option>
          <option value="Pending Review">Pending Review</option>
          <option value="Accepted">Accepted</option>
          <option value="Flagged for Inspection">Flagged for Inspection</option>
        </select>

        <!-- Direction Filter -->
        <select 
          v-model="filterDirection"
          class="bg-slate-950 border border-slate-700 text-slate-200 text-xs rounded-lg px-2.5 py-1.5 focus:outline-none"
        >
          <option value="All">All Error Directions</option>
          <option value="Underestimation Risk">Underestimation Risk</option>
          <option value="Overestimation Risk">Overestimation Risk</option>
        </select>
      </div>

      <!-- Right Search Box -->
      <div class="relative w-full sm:w-64">
        <Search class="w-3.5 h-3.5 absolute left-3 top-1/2 -translate-y-1/2 text-slate-500" />
        <input 
          v-model="searchQuery"
          type="text"
          placeholder="Search customer, meter or ID..."
          class="w-full bg-slate-950 border border-slate-700 rounded-lg pl-8 pr-3 py-1.5 text-xs text-white placeholder-slate-500 focus:outline-none focus:ring-1 focus:ring-cyan-500"
        />
      </div>

    </div>

    <!-- Split-Screen or Stack: Exceptions Table & Detailed Workspace -->
    <div class="space-y-6">
      
      <!-- Detailed Estimation Workspace for Selected Meter -->
      <div v-if="detailData">
        <EstimationWorkspace
          :detail="detailData"
          :loading="detailLoading"
          @status-updated="handleStatusUpdated"
        />
      </div>

      <!-- Meter Exceptions Table -->
      <div class="bg-slate-900 rounded-2xl border border-slate-800 overflow-hidden shadow-xl">
        <div class="px-6 py-4 border-b border-slate-800 bg-slate-950/60 flex items-center justify-between">
          <div>
            <h3 class="text-sm font-bold text-white">Missing Meter Reading Exception Queue</h3>
            <p class="text-xs text-slate-400">Select any record to inspect historical load profile and weather correlation</p>
          </div>
          <span class="text-xs font-mono text-slate-400">
            Showing {{ filteredRecords.length }} of {{ dashboardData?.records?.length || 0 }} exceptions
          </span>
        </div>

        <div class="overflow-x-auto">
          <table class="w-full text-left text-xs">
            <thead class="bg-slate-950 text-slate-400 border-b border-slate-800 font-semibold">
              <tr>
                <th class="py-3 px-4">Customer & Account</th>
                <th class="py-3 px-4">Meter & Location</th>
                <th class="py-3 px-4">Missing Period</th>
                <th class="py-3 px-4 text-slate-300">Baseline (3-Mo)</th>
                <th class="py-3 px-4 text-cyan-300">SmartBill AI</th>
                <th class="py-3 px-4">Variance Δ</th>
                <th class="py-3 px-4">Weather / Demand Flag</th>
                <th class="py-3 px-4">Risk</th>
                <th class="py-3 px-4">Status</th>
                <th class="py-3 px-4 text-right">Action</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-slate-800 font-mono text-slate-200">
              <tr 
                v-for="r in filteredRecords" 
                :key="r.customerId"
                @click="selectCustomer(r)"
                :class="[
                  'cursor-pointer transition-colors',
                  selectedCustomer?.customerId === r.customerId 
                    ? 'bg-cyan-950/40 ring-1 ring-inset ring-cyan-500/30' 
                    : 'hover:bg-slate-800/50'
                ]"
              >
                <!-- Customer Name -->
                <td class="py-3 px-4 font-sans font-medium text-white">
                  <div>{{ r.customerName }}</div>
                  <div class="text-[10px] text-slate-400 font-mono">{{ r.customerId }}</div>
                </td>

                <!-- Meter & Region -->
                <td class="py-3 px-4">
                  <div>{{ r.meterId }}</div>
                  <div class="text-[10px] text-slate-400 font-sans truncate max-w-[140px]">{{ r.region.split('(')[0] }}</div>
                </td>

                <!-- Missing Period -->
                <td class="py-3 px-4 font-sans text-slate-300">
                  <div>{{ r.missingPeriod }}</div>
                  <div class="text-[10px] text-slate-500">{{ r.missingPeriodDays }} billing days</div>
                </td>

                <!-- 3-Mo Baseline -->
                <td class="py-3 px-4 text-slate-300 font-bold">
                  {{ r.baseline3MonthAvgKwh }} kWh
                  <div class="text-[10px] text-slate-500 font-normal">${{ r.estimatedDollarBaseline.toFixed(2) }}</div>
                </td>

                <!-- SmartBill AI Estimate -->
                <td class="py-3 px-4 text-cyan-300 font-bold">
                  {{ r.smartBillAiEstimateKwh }} kWh
                  <div class="text-[10px] text-emerald-400 font-normal">${{ r.estimatedDollarSmartBill.toFixed(2) }}</div>
                </td>

                <!-- Variance -->
                <td class="py-3 px-4">
                  <span :class="[r.varianceKwh >= 0 ? 'text-amber-400 font-bold' : 'text-cyan-300 font-bold']">
                    {{ r.varianceKwh >= 0 ? '+' : '' }}{{ r.varianceKwh }} kWh
                  </span>
                  <div class="text-[10px] text-slate-400 font-sans">
                    ({{ r.variancePercent >= 0 ? '+' : '' }}{{ r.variancePercent }}%)
                  </div>
                </td>

                <!-- Weather Indicator -->
                <td class="py-3 px-4 font-sans text-[11px] text-slate-300 max-w-[160px] truncate" :title="r.weatherIndicator">
                  {{ r.weatherIndicator }}
                </td>

                <!-- Risk Badge -->
                <td class="py-3 px-4 font-sans">
                  <span 
                    :class="[
                      'text-[10px] px-2 py-0.5 rounded-full font-bold uppercase',
                      r.riskLevel === 'High' ? 'bg-rose-500/20 text-rose-300' : 'bg-amber-500/20 text-amber-300'
                    ]"
                  >
                    {{ r.riskLevel }}
                  </span>
                </td>

                <!-- Status Badge -->
                <td class="py-3 px-4 font-sans">
                  <span 
                    :class="[
                      'text-[10px] px-2 py-0.5 rounded-full font-semibold',
                      r.reviewStatus === 'Accepted'
                        ? 'bg-emerald-500/20 text-emerald-300'
                        : (r.reviewStatus === 'Flagged for Inspection'
                          ? 'bg-rose-500/20 text-rose-300'
                          : 'bg-amber-500/20 text-amber-300')
                    ]"
                  >
                    {{ r.reviewStatus }}
                  </span>
                </td>

                <!-- Action Button -->
                <td class="py-3 px-4 text-right font-sans">
                  <button 
                    class="text-xs text-cyan-400 hover:text-cyan-300 font-semibold inline-flex items-center space-x-1"
                  >
                    <span>Inspect</span>
                    <ArrowUpRight class="w-3.5 h-3.5" />
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

    </div>

    <!-- Accuracy Evaluation Modal -->
    <AccuracyEvaluationModal
      :is-open="isBacktestOpen"
      @close="isBacktestOpen = false"
    />

  </div>
</template>

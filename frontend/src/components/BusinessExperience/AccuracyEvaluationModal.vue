<script setup>
import { ref, onMounted } from 'vue';
import { 
  X, 
  BarChart3, 
  CheckCircle2, 
  ShieldCheck, 
  TrendingUp, 
  Zap, 
  Sparkles,
  ArrowDownRight,
  Activity
} from 'lucide-vue-next';
import { api } from '../../services/api';

const props = defineProps({
  isOpen: {
    type: Boolean,
    default: false
  }
});

const emit = defineEmits(['close']);

const result = ref(null);
const loading = ref(true);

async function fetchBacktest() {
  loading.value = true;
  try {
    const data = await api.runBacktest();
    result.value = data;
  } catch (err) {
    console.error('Error running backtest:', err);
  } finally {
    loading.value = false;
  }
}

onMounted(() => {
  fetchBacktest();
});
</script>

<template>
  <div v-if="isOpen" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/80 backdrop-blur-sm">
    <div class="bg-slate-900 border border-slate-700 w-full max-w-4xl rounded-2xl shadow-2xl overflow-hidden flex flex-col max-h-[92vh]">
      
      <!-- Header -->
      <div class="flex items-center justify-between px-6 py-4 border-b border-slate-800 bg-slate-950/70">
        <div class="flex items-center space-x-2.5">
          <div class="w-8 h-8 rounded-lg bg-indigo-500/20 border border-indigo-500/30 flex items-center justify-center text-indigo-400">
            <BarChart3 class="w-4 h-4" />
          </div>
          <div>
            <h3 class="text-base font-bold text-white">Estimation Accuracy Backtest Evaluation</h3>
            <p class="text-xs text-slate-400">Chronological out-of-sample benchmarking: 3-Month Average vs. SmartBill AI</p>
          </div>
        </div>
        <button 
          @click="emit('close')"
          class="text-slate-400 hover:text-white p-1 rounded-lg hover:bg-slate-800 transition-colors"
        >
          <X class="w-5 h-5" />
        </button>
      </div>

      <!-- Content -->
      <div class="p-6 overflow-y-auto space-y-6">
        
        <div v-if="loading" class="py-12 text-center">
          <div class="inline-block animate-spin w-8 h-8 border-4 border-indigo-500 border-t-transparent rounded-full mb-3"></div>
          <p class="text-sm text-slate-300">Executing chronological backtesting against ground-truth meter readings...</p>
        </div>

        <div v-else-if="result" class="space-y-6">
          
          <!-- Executive Summary Banner -->
          <div class="p-4 rounded-xl bg-gradient-to-r from-indigo-950/60 via-slate-900 to-emerald-950/40 border border-indigo-500/30">
            <div class="flex items-start space-x-3">
              <Sparkles class="w-5 h-5 text-indigo-400 flex-shrink-0 mt-0.5" />
              <div>
                <span class="text-xs font-bold uppercase tracking-wider text-indigo-300 block mb-1">
                  Benchmarking Executive Findings
                </span>
                <p class="text-sm text-slate-200 leading-relaxed">{{ result.executiveSummary }}</p>
              </div>
            </div>
          </div>

          <!-- Comparative KPI Grid -->
          <div class="grid grid-cols-2 sm:grid-cols-4 gap-3.5">
            
            <!-- SmartBill MAPE -->
            <div class="bg-slate-950 p-4 rounded-xl border border-emerald-500/30">
              <span class="text-[11px] text-slate-400 block font-medium">SmartBill AI MAPE</span>
              <div class="text-2xl font-bold font-mono text-emerald-400 mt-1">
                {{ result.smartBillMapePercent }}%
              </div>
              <span class="text-[10px] text-emerald-300/80 mt-1 block">Mean Absolute % Error</span>
            </div>

            <!-- Baseline MAPE -->
            <div class="bg-slate-950 p-4 rounded-xl border border-rose-500/30">
              <span class="text-[11px] text-slate-400 block font-medium">3-Month Baseline MAPE</span>
              <div class="text-2xl font-bold font-mono text-rose-400 mt-1">
                {{ result.baselineMapePercent }}%
              </div>
              <span class="text-[10px] text-rose-300/80 mt-1 block">Severe lag during transitions</span>
            </div>

            <!-- Total Error Reduction -->
            <div class="bg-slate-950 p-4 rounded-xl border border-indigo-500/30">
              <span class="text-[11px] text-slate-400 block font-medium">Accuracy Improvement</span>
              <div class="text-2xl font-bold font-mono text-cyan-300 mt-1 flex items-center">
                <ArrowDownRight class="w-5 h-5 mr-0.5 text-emerald-400 inline" />
                {{ result.accuracyImprovementPercent }}%
              </div>
              <span class="text-[10px] text-slate-400 mt-1 block">Error reduction rate</span>
            </div>

            <!-- Total Abs Error Saved -->
            <div class="bg-slate-950 p-4 rounded-xl border border-slate-800">
              <span class="text-[11px] text-slate-400 block font-medium">MAE Comparison</span>
              <div class="text-lg font-bold font-mono text-white mt-1">
                {{ result.smartBillMaeKwh }} vs {{ result.baselineMaeKwh }}
              </div>
              <span class="text-[10px] text-slate-400 mt-1 block">kWh Mean Absolute Error</span>
            </div>

          </div>

          <!-- Monthly Historical Breakdown Table -->
          <div class="bg-slate-950 rounded-xl border border-slate-800 overflow-hidden">
            <div class="px-4 py-3 border-b border-slate-800 bg-slate-900/50 flex items-center justify-between">
              <h4 class="text-xs font-bold uppercase tracking-wider text-slate-300">
                Month-by-Month Out-of-Sample Performance
              </h4>
              <span class="text-[11px] font-mono text-slate-400">Ground-truth actuals temporarily blinded during prediction</span>
            </div>

            <div class="overflow-x-auto">
              <table class="w-full text-left text-xs">
                <thead class="bg-slate-900/80 text-slate-400 border-b border-slate-800 font-semibold">
                  <tr>
                    <th class="py-2.5 px-3.5">Historical Month</th>
                    <th class="py-2.5 px-3.5 text-white">Actual (kWh)</th>
                    <th class="py-2.5 px-3.5 text-rose-300">3-Mo Baseline</th>
                    <th class="py-2.5 px-3.5 text-emerald-300">SmartBill AI</th>
                    <th class="py-2.5 px-3.5 text-rose-400">Baseline Error</th>
                    <th class="py-2.5 px-3.5 text-emerald-400">SmartBill Error</th>
                    <th class="py-2.5 px-3.5 text-right">Advantage</th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-slate-800/60 font-mono text-slate-200">
                  <tr v-for="m in result.monthlyBreakdown" :key="m.monthLabel" class="hover:bg-slate-900/40">
                    <td class="py-2.5 px-3.5 font-sans font-medium text-slate-300">{{ m.monthLabel }}</td>
                    <td class="py-2.5 px-3.5 font-bold text-white">{{ m.actualKwh }}</td>
                    <td class="py-2.5 px-3.5 text-slate-300">{{ m.baselineEstimateKwh }}</td>
                    <td class="py-2.5 px-3.5 text-emerald-300 font-semibold">{{ m.smartBillAiEstimateKwh }}</td>
                    <td class="py-2.5 px-3.5 text-rose-400">
                      ±{{ m.baselineErrorKwh }} kWh ({{ m.baselineAbsPercentError }}%)
                    </td>
                    <td class="py-2.5 px-3.5 text-emerald-400">
                      ±{{ m.smartBillErrorKwh }} kWh ({{ m.smartBillAbsPercentError }}%)
                    </td>
                    <td class="py-2.5 px-3.5 text-right text-emerald-400 font-bold font-sans">
                      +{{ Math.round(((m.baselineErrorKwh - m.smartBillErrorKwh) / m.baselineErrorKwh) * 100) }}%
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

        </div>

      </div>

      <!-- Footer -->
      <div class="px-6 py-3.5 border-t border-slate-800 bg-slate-950/70 flex justify-end">
        <button
          @click="emit('close')"
          class="px-4 py-2 rounded-xl bg-slate-800 hover:bg-slate-700 text-white text-xs font-semibold transition-colors"
        >
          Close Backtest Report
        </button>
      </div>

    </div>
  </div>
</template>

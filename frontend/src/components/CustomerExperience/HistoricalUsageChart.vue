<script setup>
import { ref, computed } from 'vue';
import { 
  BarChart2, 
  DollarSign, 
  Calendar, 
  SunMedium, 
  TrendingUp, 
  Layers,
  Thermometer
} from 'lucide-vue-next';

const props = defineProps({
  history: {
    type: Array,
    required: true
  },
  selectedPeriodId: {
    type: String,
    required: true
  },
  dailyUsage: {
    type: Array,
    default: () => []
  }
});

const emit = defineEmits(['selectPeriod']);

// View mode: 'kwh', 'cost', or 'daily'
const activeMode = ref('kwh');
const hoveredBar = ref(null);

// Chronological order (oldest to newest for charts)
const chronologicalHistory = computed(() => {
  return [...props.history].reverse();
});

const maxKwh = computed(() => {
  return Math.max(...props.history.map(x => x.consumptionKwh), 800);
});

const maxCost = computed(() => {
  return Math.max(...props.history.map(x => x.totalBillAmount), 200);
});

const maxDailyKwh = computed(() => {
  if (!props.dailyUsage || props.dailyUsage.length === 0) return 30;
  return Math.max(...props.dailyUsage.map(x => x.kwh), 30);
});
</script>

<template>
  <div class="bg-slate-900/80 rounded-2xl border border-slate-800 p-6 shadow-xl backdrop-blur-sm">
    <!-- Header with controls -->
    <div class="flex flex-col sm:flex-row sm:items-center justify-between pb-6 border-b border-slate-800 gap-4">
      <div>
        <div class="flex items-center space-x-2">
          <BarChart2 class="w-5 h-5 text-emerald-400" />
          <h3 class="text-base font-bold text-white">Historical Usage & Seasonal Patterns</h3>
        </div>
        <p class="text-xs text-slate-400 mt-0.5">
          Interactive load profile across 12 consecutive billing cycles with weather correlation
        </p>
      </div>

      <!-- Toggle View Controls -->
      <div class="flex items-center bg-slate-950 p-1 rounded-xl border border-slate-800 text-xs font-semibold">
        <button
          @click="activeMode = 'kwh'"
          :class="[
            'px-3 py-1.5 rounded-lg transition-all flex items-center space-x-1.5',
            activeMode === 'kwh'
              ? 'bg-emerald-600 text-white shadow-sm'
              : 'text-slate-400 hover:text-slate-200'
          ]"
        >
          <TrendingUp class="w-3.5 h-3.5" />
          <span>Consumption (kWh)</span>
        </button>

        <button
          @click="activeMode = 'cost'"
          :class="[
            'px-3 py-1.5 rounded-lg transition-all flex items-center space-x-1.5',
            activeMode === 'cost'
              ? 'bg-teal-600 text-white shadow-sm'
              : 'text-slate-400 hover:text-slate-200'
          ]"
        >
          <DollarSign class="w-3.5 h-3.5" />
          <span>Bill Amount ($)</span>
        </button>

        <button
          @click="activeMode = 'daily'"
          :class="[
            'px-3 py-1.5 rounded-lg transition-all flex items-center space-x-1.5',
            activeMode === 'daily'
              ? 'bg-cyan-600 text-white shadow-sm'
              : 'text-slate-400 hover:text-slate-200'
          ]"
        >
          <Calendar class="w-3.5 h-3.5" />
          <span>Daily Cycle Breakdown</span>
        </button>
      </div>
    </div>

    <!-- Chart Body -->
    <div class="mt-6">
      
      <!-- MODE: Monthly Consumption or Cost -->
      <div v-if="activeMode === 'kwh' || activeMode === 'cost'" class="space-y-4">
        
        <!-- Bar Grid Display -->
        <div class="h-64 flex items-end justify-between gap-2 pt-6 px-2 relative">
          <!-- Horizontal reference guide lines -->
          <div class="absolute inset-x-0 top-6 border-b border-slate-800/60 pointer-events-none"></div>
          <div class="absolute inset-x-0 top-1/2 border-b border-slate-800/40 pointer-events-none"></div>

          <div
            v-for="(item, idx) in chronologicalHistory"
            :key="item.periodId"
            class="flex-1 flex flex-col items-center h-full justify-end group cursor-pointer"
            @click="emit('selectPeriod', item.periodId)"
            @mouseenter="hoveredBar = item"
            @mouseleave="hoveredBar = null"
          >
            <!-- Hover Tooltip -->
            <div 
              v-if="hoveredBar?.periodId === item.periodId"
              class="absolute -top-12 z-20 bg-slate-950 border border-slate-700 text-white text-[11px] py-1 px-2.5 rounded-lg shadow-xl pointer-events-none whitespace-nowrap"
            >
              <div class="font-bold text-emerald-400">{{ item.monthName }}</div>
              <div>{{ item.consumptionKwh }} kWh | ${{ item.totalBillAmount.toFixed(2) }}</div>
              <div class="text-[10px] text-slate-400">{{ item.avgTemperatureF }}°F ({{ item.coolingDegreeDays }} CDD)</div>
            </div>

            <!-- Bar Column -->
            <div class="w-full max-w-[42px] flex flex-col items-center justify-end h-full">
              <!-- Value Label -->
              <span 
                :class="[
                  'text-[10px] font-mono mb-1 transition-opacity',
                  item.periodId === selectedPeriodId ? 'text-emerald-300 font-bold opacity-100' : 'text-slate-400 opacity-0 group-hover:opacity-100'
                ]"
              >
                {{ activeMode === 'kwh' ? Math.round(item.consumptionKwh) : '$' + Math.round(item.totalBillAmount) }}
              </span>

              <!-- The actual Bar -->
              <div
                :style="{
                  height: activeMode === 'kwh'
                    ? `${(item.consumptionKwh / maxKwh) * 100}%`
                    : `${(item.totalBillAmount / maxCost) * 100}%`
                }"
                :class="[
                  'w-full rounded-t-lg transition-all duration-300 relative',
                  item.periodId === selectedPeriodId
                    ? 'bg-gradient-to-t from-emerald-600 via-teal-500 to-emerald-400 shadow-lg shadow-emerald-500/30 ring-2 ring-emerald-400/50'
                    : 'bg-slate-700/60 hover:bg-slate-600'
                ]"
              >
                <!-- Temperature indicator dot on top of bar for summer months -->
                <div 
                  v-if="item.coolingDegreeDays > 100" 
                  class="absolute -top-2 left-1/2 -translate-x-1/2 w-1.5 h-1.5 rounded-full bg-amber-400 ring-2 ring-amber-400/20"
                  title="Summer Cooling Demand"
                ></div>
              </div>
            </div>

            <!-- Month Label -->
            <span 
              :class="[
                'text-[11px] mt-2 font-medium transition-colors',
                item.periodId === selectedPeriodId ? 'text-emerald-400 font-bold' : 'text-slate-400 group-hover:text-slate-200'
              ]"
            >
              {{ item.monthName.split(' ')[0].substring(0, 3) }}
            </span>
          </div>
        </div>

        <!-- Chart Legend & Summary Notes -->
        <div class="flex flex-wrap items-center justify-between pt-4 border-t border-slate-800 text-xs text-slate-400">
          <div class="flex items-center space-x-4">
            <div class="flex items-center space-x-1.5">
              <span class="w-3 h-3 rounded-sm bg-gradient-to-t from-emerald-600 to-teal-400"></span>
              <span>Selected Period</span>
            </div>
            <div class="flex items-center space-x-1.5">
              <span class="w-3 h-3 rounded-sm bg-slate-700"></span>
              <span>Historical Cycles</span>
            </div>
            <div class="flex items-center space-x-1.5">
              <span class="w-2 h-2 rounded-full bg-amber-400"></span>
              <span>Cooling Spike (CDD > 100)</span>
            </div>
          </div>
          <span class="italic text-[11px] text-slate-500">
            Click any bar to change active billing period
          </span>
        </div>

      </div>

      <!-- MODE: Daily Usage Breakdown for Selected Period -->
      <div v-else class="space-y-4">
        <div class="flex items-center justify-between text-xs text-slate-400 pb-2">
          <span>Daily consumption profile for selected cycle ({{ dailyUsage.length }} days)</span>
          <span class="text-emerald-400 font-mono font-medium">Avg: {{ (dailyUsage.reduce((a,b)=>a+b.kwh,0)/Math.max(1, dailyUsage.length)).toFixed(1) }} kWh/day</span>
        </div>

        <div class="h-64 flex items-end justify-between gap-1 pt-6 px-1 relative">
          <div
            v-for="d in dailyUsage"
            :key="d.date"
            class="flex-1 flex flex-col items-center justify-end h-full group cursor-pointer"
          >
            <!-- Bar -->
            <div
              :style="{ height: `${(d.kwh / maxDailyKwh) * 100}%` }"
              :class="[
                'w-full max-w-[14px] rounded-t-sm transition-all',
                d.isWeekend 
                  ? 'bg-purple-500/70 hover:bg-purple-400' 
                  : 'bg-emerald-500/70 hover:bg-emerald-400'
              ]"
              :title="`${d.date}: ${d.kwh} kWh (${d.avgTempF}°F)`"
            ></div>
            <span class="text-[9px] text-slate-500 mt-1 font-mono">
              {{ d.dayNumber % 5 === 0 || d.dayNumber === 1 ? d.dayNumber : '' }}
            </span>
          </div>
        </div>

        <div class="flex items-center space-x-4 pt-3 border-t border-slate-800 text-xs text-slate-400">
          <div class="flex items-center space-x-1.5">
            <span class="w-3 h-3 rounded-sm bg-emerald-500/70"></span>
            <span>Weekday Usage</span>
          </div>
          <div class="flex items-center space-x-1.5">
            <span class="w-3 h-3 rounded-sm bg-purple-500/70"></span>
            <span>Weekend Usage (+15% load)</span>
          </div>
        </div>
      </div>

    </div>
  </div>
</template>

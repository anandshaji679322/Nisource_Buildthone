<script setup>
import { defineProps, defineEmits } from 'vue';
import { 
  Zap, 
  UserCheck, 
  Building2, 
  Activity, 
  FlaskConical, 
  Sparkles,
  BarChart3
} from 'lucide-vue-next';

const props = defineProps({
  currentView: {
    type: String,
    required: true, // 'customer' or 'business'
  },
  apiHealthy: {
    type: Boolean,
    default: true
  }
});

const emit = defineEmits(['navigate', 'openBacktest']);
</script>

<template>
  <header class="bg-slate-900/90 backdrop-blur-md border-b border-slate-800 sticky top-0 z-40 transition-colors">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex items-center justify-between h-16">
        
        <!-- Brand & Logo -->
        <div class="flex items-center space-x-3 cursor-pointer" @click="emit('navigate', 'customer')">
          <div class="w-10 h-10 rounded-xl bg-gradient-to-tr from-emerald-600 via-teal-500 to-cyan-500 flex items-center justify-center shadow-lg shadow-emerald-500/20 ring-1 ring-white/20">
            <Zap class="w-5 h-5 text-white" />
          </div>
          <div>
            <div class="flex items-center space-x-2">
              <span class="text-xl font-extrabold tracking-tight bg-gradient-to-r from-white via-slate-100 to-emerald-300 bg-clip-text text-transparent">
                SmartBill AI
              </span>
              <span class="text-[10px] font-bold uppercase tracking-wider px-2 py-0.5 rounded-full bg-emerald-500/10 text-emerald-400 border border-emerald-500/20">
                Buildathon Prototype
              </span>
            </div>
            <p class="text-xs text-slate-400 font-medium">Intelligent Billing & Usage Advisor</p>
          </div>
        </div>

        <!-- Navigation Switcher -->
        <nav class="flex items-center bg-slate-950/70 p-1 rounded-xl border border-slate-800 shadow-inner">
          <button
            @click="emit('navigate', 'customer')"
            :class="[
              'flex items-center space-x-2 px-4 py-2 rounded-lg text-sm font-semibold transition-all duration-200',
              currentView === 'customer'
                ? 'bg-gradient-to-r from-emerald-600 to-teal-600 text-white shadow-md shadow-emerald-600/20'
                : 'text-slate-400 hover:text-slate-200 hover:bg-slate-800/50'
            ]"
          >
            <UserCheck class="w-4 h-4" />
            <span>Customer Experience</span>
            <span class="text-[10px] px-1.5 py-0.2 rounded bg-black/20 text-emerald-200 font-mono">/bill</span>
          </button>

          <button
            @click="emit('navigate', 'business')"
            :class="[
              'flex items-center space-x-2 px-4 py-2 rounded-lg text-sm font-semibold transition-all duration-200',
              currentView === 'business'
                ? 'bg-gradient-to-r from-cyan-600 to-blue-600 text-white shadow-md shadow-blue-600/20'
                : 'text-slate-400 hover:text-slate-200 hover:bg-slate-800/50'
            ]"
          >
            <Building2 class="w-4 h-4" />
            <span>Billing Operations</span>
            <span class="text-[10px] px-1.5 py-0.2 rounded bg-black/20 text-cyan-200 font-mono">/business/billing-estimator</span>
          </button>
        </nav>

        <!-- Right System Indicators & Action -->
        <div class="flex items-center space-x-3">
          <!-- Backtesting Quick Modal Trigger -->
          <button
            v-if="currentView === 'business'"
            @click="emit('openBacktest')"
            class="flex items-center space-x-1.5 px-3 py-1.5 rounded-lg bg-indigo-500/10 hover:bg-indigo-500/20 border border-indigo-500/30 text-indigo-300 text-xs font-semibold transition-colors"
          >
            <BarChart3 class="w-3.5 h-3.5 text-indigo-400" />
            <span>Accuracy Backtest</span>
          </button>

          <!-- API Status Pill -->
          <div class="flex items-center space-x-1.5 px-3 py-1 rounded-full bg-slate-800/80 border border-slate-700/60 text-xs">
            <span 
              :class="['w-2 h-2 rounded-full animate-pulse', apiHealthy ? 'bg-emerald-400 shadow-sm shadow-emerald-400' : 'bg-amber-400']"
            ></span>
            <span class="text-slate-300 font-mono text-[11px]">
              {{ apiHealthy ? '.NET 8 API Live' : 'Connecting...' }}
            </span>
          </div>

          <!-- Simulation Badge -->
          <div class="hidden sm:flex items-center space-x-1 px-2.5 py-1 rounded-md bg-purple-500/10 border border-purple-500/20 text-purple-300 text-xs">
            <FlaskConical class="w-3.5 h-3.5 text-purple-400" />
            <span class="font-medium text-[11px]">Simulated Data Mode</span>
          </div>
        </div>

      </div>
    </div>
  </header>
</template>

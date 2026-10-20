<script setup>
import { ref, onMounted, watch } from 'vue';
import HeaderNav from './components/HeaderNav.vue';
import CustomerDashboard from './components/CustomerExperience/CustomerDashboard.vue';
import OperationsDashboard from './components/BusinessExperience/OperationsDashboard.vue';
import { api } from './services/api';
import { 
  Sparkles, 
  HelpCircle, 
  ExternalLink, 
  ShieldCheck, 
  Layers,
  ArrowRight,
  Zap,
  Info
} from 'lucide-vue-next';

// View state: 'customer' (/bill) or 'business' (/business/billing-estimator)
const currentView = ref('customer');
const apiHealthy = ref(true);
const opsDashboardRef = ref(null);

function syncFromLocation() {
  const path = window.location.pathname.toLowerCase();
  if (path.includes('business') || path.includes('estimator')) {
    currentView.value = 'business';
  } else {
    currentView.value = 'customer';
  }
}

function navigateTo(view) {
  currentView.value = view;
  const targetPath = view === 'business' ? '/business/billing-estimator' : '/bill';
  window.history.pushState(null, '', targetPath);
}

// Handle browser popstate
window.addEventListener('popstate', () => {
  syncFromLocation();
});

async function checkSystemHealth() {
  try {
    apiHealthy.value = await api.checkHealth();
  } catch {
    apiHealthy.value = false;
  }
}

function openBacktestModal() {
  if (opsDashboardRef.value?.openBacktest) {
    opsDashboardRef.value.openBacktest();
  }
}

onMounted(() => {
  syncFromLocation();
  checkSystemHealth();
  setInterval(checkSystemHealth, 15000);
});
</script>

<template>
  <div class="min-h-screen bg-slate-950 text-slate-100 flex flex-col font-sans selection:bg-emerald-500 selection:text-white">
    
    <!-- Shared Top Navigation Bar -->
    <HeaderNav 
      :current-view="currentView" 
      :api-healthy="apiHealthy"
      @navigate="navigateTo"
      @open-backtest="openBacktestModal"
    />

    <!-- Quick Demo Scenario Banner -->
    <div class="bg-gradient-to-r from-emerald-950/40 via-slate-900 to-cyan-950/40 border-b border-slate-800/80 px-4 py-2">
      <div class="max-w-7xl mx-auto flex flex-wrap items-center justify-between text-xs gap-2">
        <div class="flex items-center space-x-2">
          <span class="px-2 py-0.5 rounded bg-emerald-500/20 text-emerald-300 font-bold font-mono text-[10px]">
            DEMO SCENARIO
          </span>
          <span class="text-slate-300">
            <template v-if="currentView === 'customer'">
              Customer Sarah/Eleanor notices July bill jumped from <strong>$96.40 (June)</strong> to <strong>$184.20 (July)</strong>. Click <strong>"Explain My Bill"</strong> to see AI decomposition.
            </template>
            <template v-else>
              Meter <strong>#MTR-COL-8821</strong> missed July reading. 3-Month Average predicts <strong>375 kWh</strong> ($58), but SmartBill AI estimates <strong>728 kWh</strong> ($113) preventing severe customer underbilling.
            </template>
          </span>
        </div>

        <div class="flex items-center space-x-2">
          <button 
            v-if="currentView === 'customer'"
            @click="navigateTo('business')"
            class="text-[11px] font-semibold text-cyan-400 hover:text-cyan-300 flex items-center space-x-1"
          >
            <span>Switch to Billing Ops</span>
            <ArrowRight class="w-3 h-3" />
          </button>
          <button 
            v-else
            @click="navigateTo('customer')"
            class="text-[11px] font-semibold text-emerald-400 hover:text-emerald-300 flex items-center space-x-1"
          >
            <span>Switch to Customer Advisor</span>
            <ArrowRight class="w-3 h-3" />
          </button>
        </div>
      </div>
    </div>

    <!-- Main View Content Area -->
    <main class="flex-1 max-w-7xl w-full mx-auto px-4 sm:px-6 lg:px-8 py-6">
      
      <!-- Route 1: Customer Bill & Usage Advisor (/bill) -->
      <CustomerDashboard 
        v-if="currentView === 'customer'"
      />

      <!-- Route 2: Missing Meter Reading Estimator (/business/billing-estimator) -->
      <OperationsDashboard 
        v-else-if="currentView === 'business'"
        ref="opsDashboardRef"
      />

    </main>

    <!-- Global Footer -->
    <footer class="bg-slate-900/60 border-t border-slate-800 py-6 text-xs text-slate-500 mt-auto">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 flex flex-col sm:flex-row items-center justify-between gap-3">
        <div class="flex items-center space-x-2">
          <Zap class="w-4 h-4 text-emerald-500" />
          <span class="font-bold text-slate-300">SmartBill AI</span>
          <span>— Utility Billing Intelligence & Weather-Aware Meter Estimation</span>
        </div>
        <div class="flex items-center space-x-4 text-[11px]">
          <span>.NET 8 Core Web API + Vue 3 + Tailwind CSS</span>
          <span>•</span>
          <span>GenAI Lab gpt-4o-mini & Open-Meteo Integration</span>
        </div>
      </div>
    </footer>

  </div>
</template>

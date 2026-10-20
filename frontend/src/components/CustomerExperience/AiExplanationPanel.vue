<script setup>
import { ref } from 'vue';
import { 
  Sparkles, 
  CheckCircle2, 
  HelpCircle, 
  TrendingUp, 
  ShieldCheck, 
  Calendar, 
  Zap, 
  ThermometerSun, 
  Cpu, 
  ArrowRight,
  Lightbulb,
  AlertCircle
} from 'lucide-vue-next';

const props = defineProps({
  explanation: {
    type: Object,
    default: null
  },
  loading: {
    type: Boolean,
    default: false
  },
  selectedMonthName: {
    type: String,
    default: 'Selected Period'
  }
});

const emit = defineEmits(['requestExplanation']);
</script>

<template>
  <div class="bg-gradient-to-b from-slate-900 to-slate-950 rounded-2xl border border-emerald-500/30 p-6 shadow-2xl relative overflow-hidden">
    <!-- Subtle background glow -->
    <div class="absolute -top-24 -right-24 w-72 h-72 bg-emerald-500/10 rounded-full blur-3xl pointer-events-none"></div>

    <!-- Header & Action Button -->
    <div class="flex flex-col sm:flex-row sm:items-center justify-between pb-5 border-b border-slate-800 gap-4 relative z-10">
      <div class="flex items-center space-x-3">
        <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-emerald-500 to-teal-600 flex items-center justify-center shadow-lg shadow-emerald-500/20">
          <Sparkles class="w-5 h-5 text-white animate-pulse" />
        </div>
        <div>
          <div class="flex items-center space-x-2">
            <h3 class="text-lg font-bold text-white">AI Bill Explanation</h3>
            <span class="text-[10px] font-mono px-2 py-0.5 rounded-full bg-emerald-400/10 text-emerald-300 border border-emerald-400/20">
              Explainable AI
            </span>
          </div>
          <p class="text-xs text-slate-400">
            Plain-language breakdown distinguishing measured facts from contributing factors
          </p>
        </div>
      </div>

      <!-- Prominent "Explain My Bill" Button -->
      <button
        @click="emit('requestExplanation')"
        :disabled="loading"
        class="inline-flex items-center justify-center space-x-2 px-5 py-2.5 rounded-xl bg-gradient-to-r from-emerald-500 to-teal-600 hover:from-emerald-400 hover:to-teal-500 text-white font-semibold text-sm shadow-lg shadow-emerald-500/25 transition-all transform active:scale-95 disabled:opacity-50 disabled:cursor-not-allowed cursor-pointer"
      >
        <Sparkles class="w-4 h-4" :class="{ 'animate-spin': loading }" />
        <span>{{ loading ? 'Analyzing Tariff & Weather...' : 'Explain My Bill' }}</span>
      </button>
    </div>

    <!-- Initial State when not yet explained -->
    <div v-if="!explanation && !loading" class="py-12 text-center relative z-10">
      <div class="w-14 h-14 rounded-2xl bg-slate-800/80 border border-slate-700 mx-auto flex items-center justify-center text-emerald-400 mb-3 shadow-inner">
        <Lightbulb class="w-7 h-7" />
      </div>
      <h4 class="text-base font-bold text-white">Understand What Drove Your Bill Fluctuation</h4>
      <p class="text-sm text-slate-400 max-w-md mx-auto mt-1">
        Click <span class="text-emerald-400 font-semibold">"Explain My Bill"</span> to compare {{ selectedMonthName }} against historical usage, weather cooling degree days, and tariff rate structures.
      </p>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="py-14 text-center relative z-10">
      <div class="inline-block animate-spin w-10 h-10 border-4 border-emerald-500 border-t-transparent rounded-full mb-3"></div>
      <p class="text-sm font-semibold text-slate-200">Evaluating historical consumption & weather sensitivity...</p>
      <p class="text-xs text-slate-400 mt-1">Running GenAI Lab model with deterministic tariff verification</p>
    </div>

    <!-- Structured Explanation Panel -->
    <div v-if="explanation && !loading" class="mt-6 space-y-6 relative z-10">
      
      <!-- 1. Plain-Language Summary Banner -->
      <div class="p-4 rounded-xl bg-emerald-950/40 border border-emerald-500/30 text-emerald-100 text-sm leading-relaxed shadow-sm">
        <div class="flex items-start space-x-2.5">
          <Sparkles class="w-5 h-5 text-emerald-400 flex-shrink-0 mt-0.5" />
          <div>
            <span class="font-bold text-white block mb-1">Executive Summary</span>
            <p>{{ explanation.plainLanguageSummary }}</p>
          </div>
        </div>
      </div>

      <!-- 2. Observed Facts vs. Contributing Factors Grid -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-5">
        
        <!-- Column 1: Observed Facts (Facts Only) -->
        <div class="bg-slate-950/60 rounded-xl border border-slate-800 p-4">
          <div class="flex items-center space-x-2 text-cyan-400 mb-3">
            <CheckCircle2 class="w-4 h-4" />
            <h4 class="text-xs font-bold uppercase tracking-wider">Observed Facts (Measured Data)</h4>
          </div>
          <ul class="space-y-2.5 text-xs text-slate-300">
            <li 
              v-for="(fact, fIdx) in explanation.observedChanges" 
              :key="fIdx"
              class="flex items-start space-x-2"
            >
              <span class="w-1.5 h-1.5 rounded-full bg-cyan-400 mt-1.5 flex-shrink-0"></span>
              <span>{{ fact }}</span>
            </li>
          </ul>
        </div>

        <!-- Column 2: Potential Contributing Factors (Inferences) -->
        <div class="bg-slate-950/60 rounded-xl border border-slate-800 p-4">
          <div class="flex items-center space-x-2 text-amber-400 mb-3">
            <HelpCircle class="w-4 h-4" />
            <h4 class="text-xs font-bold uppercase tracking-wider">Contributing Factors (Analytics & Inferences)</h4>
          </div>
          <div class="space-y-3">
            <div 
              v-for="(factor, cIdx) in explanation.contributingFactors" 
              :key="cIdx"
              class="p-2.5 rounded-lg bg-slate-900 border border-slate-800 text-xs"
            >
              <div class="flex items-center justify-between mb-1">
                <span class="font-semibold text-slate-200">{{ factor.factorName }}</span>
                <span 
                  :class="[
                    'text-[10px] px-2 py-0.5 rounded-full font-bold',
                    factor.impactLevel === 'High' ? 'bg-rose-500/20 text-rose-300' : 'bg-amber-500/20 text-amber-300'
                  ]"
                >
                  {{ factor.impactLevel }} Impact
                </span>
              </div>
              <p class="text-slate-400 text-[11px] leading-relaxed">{{ factor.description }}</p>
              <div v-if="factor.estimatedDollarImpact > 0" class="mt-1 text-[11px] font-mono text-emerald-400">
                Estimated impact: ~${{ factor.estimatedDollarImpact.toFixed(2) }}
              </div>
            </div>
          </div>
        </div>

      </div>

      <!-- 3. Key Supporting Figures & Comparisons -->
      <div class="bg-slate-950/40 rounded-xl border border-slate-800 p-4">
        <h4 class="text-xs font-bold uppercase tracking-wider text-slate-400 mb-3">
          Supporting Figures & Metric Comparisons
        </h4>
        <div class="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-6 gap-3">
          <div 
            v-for="(val, key) in explanation.supportingFigures" 
            :key="key"
            class="bg-slate-900 p-2.5 rounded-lg border border-slate-800 text-center"
          >
            <span class="text-[10px] text-slate-400 block truncate">{{ key }}</span>
            <span class="text-xs font-bold text-white font-mono mt-0.5 block">{{ val }}</span>
          </div>
        </div>
      </div>

      <!-- 4. Practical Energy-Saving Recommendations -->
      <div>
        <div class="flex items-center space-x-2 mb-3">
          <Lightbulb class="w-4 h-4 text-emerald-400" />
          <h4 class="text-xs font-bold uppercase tracking-wider text-slate-200">
            Actionable Recommendations to Reduce Bill
          </h4>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-3 gap-3">
          <div 
            v-for="(rec, rIdx) in explanation.practicalRecommendations" 
            :key="rIdx"
            class="bg-slate-950/80 border border-slate-800 rounded-xl p-3.5 flex flex-col justify-between"
          >
            <div>
              <div class="flex items-center justify-between mb-1.5">
                <span class="text-[10px] uppercase font-bold text-emerald-400 tracking-wider">{{ rec.category }}</span>
                <span class="text-[10px] px-1.5 py-0.5 rounded bg-emerald-500/10 text-emerald-300 font-mono">
                  Save ~${{ rec.estimatedMonthlySavings.toFixed(2) }}/mo
                </span>
              </div>
              <h5 class="text-xs font-bold text-white mb-1">{{ rec.title }}</h5>
              <p class="text-[11px] text-slate-300 leading-snug mb-2">{{ rec.action }}</p>
              <p class="text-[10px] text-slate-400 italic">"{{ rec.rationale }}"</p>
            </div>
            <div class="mt-3 pt-2 border-t border-slate-800 flex items-center justify-between text-[10px] text-slate-400">
              <span>Feasibility: {{ rec.feasibility }}</span>
              <span class="text-emerald-400 font-semibold cursor-pointer hover:underline">Apply tip →</span>
            </div>
          </div>
        </div>
      </div>

      <!-- 5. Confidence, Evidence & Source Engine Footnote -->
      <div class="pt-3 border-t border-slate-800 flex flex-col sm:flex-row sm:items-center justify-between text-[11px] text-slate-400 gap-2">
        <div class="flex items-center space-x-2">
          <ShieldCheck class="w-4 h-4 text-emerald-400 flex-shrink-0" />
          <span>{{ explanation.confidenceScore }}</span>
        </div>
        <div class="flex items-center space-x-2 font-mono text-[10px] text-slate-500">
          <Cpu class="w-3.5 h-3.5" />
          <span>Engine: {{ explanation.sourceEngine }}</span>
        </div>
      </div>

    </div>
  </div>
</template>

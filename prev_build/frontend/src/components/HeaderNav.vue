<template>
  <header class="bg-white border-b border-slate-200 sticky top-0 z-30 shadow-sm">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex items-center justify-between h-16">
        <!-- Brand & Title -->
        <div class="flex items-center space-x-3">
          <div class="w-10 h-10 rounded-xl bg-gradient-to-tr from-sky-600 to-emerald-500 flex items-center justify-center text-white shadow-md shadow-sky-500/20">
            <!-- Flame / Energy Icon -->
            <svg class="w-6 h-6" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <path d="M8.5 14.5A2.5 2.5 0 0 0 11 12c0-1.38-.5-2-1-3-1.072-2.143-.224-4.054 2-6 .5 2.5 2 4.9 4 6.5 2 1.6 3 3.5 3 5.5a7 7 0 1 1-14 0c0-1.153.433-2.294 1-3a2.5 2.5 0 0 0 2.5 2.5z" />
            </svg>
          </div>
          <div>
            <div class="flex items-center space-x-2">
              <span class="font-bold text-lg text-slate-900 tracking-tight">NiSource</span>
              <span class="text-xs font-semibold px-2 py-0.5 rounded-full bg-sky-100 text-sky-800 uppercase tracking-wide">Smart Move</span>
            </div>
            <p class="text-xs text-slate-500">Conversational Intent-to-Action Utility Concierge</p>
          </div>
        </div>

        <!-- System Badges & Actions -->
        <div class="flex items-center space-x-3">
          <!-- Backend Status Badge -->
          <div class="hidden sm:flex items-center space-x-1.5 px-3 py-1 rounded-full text-xs font-medium bg-slate-100 text-slate-700 border border-slate-200">
            <span class="relative flex h-2 w-2">
              <span :class="[backendOnline ? 'bg-emerald-400' : 'bg-amber-400']" class="animate-ping absolute inline-flex h-full w-full rounded-full opacity-75"></span>
              <span :class="[backendOnline ? 'bg-emerald-500' : 'bg-amber-500']" class="relative inline-flex rounded-full h-2 w-2"></span>
            </span>
            <span>{{ backendOnline ? '.NET 8 API Online' : 'Connecting API...' }}</span>
          </div>

          <!-- AI Model Badge -->
          <div class="hidden md:flex items-center space-x-1.5 px-3 py-1 rounded-full text-xs font-medium bg-emerald-50 text-emerald-800 border border-emerald-200">
            <svg class="w-3.5 h-3.5 text-emerald-600" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="m12 3-1.912 5.813a2 2 0 0 1-1.275 1.275L3 12l5.813 1.912a2 2 0 0 1 1.275 1.275L12 21l1.912-5.813a2 2 0 0 1 1.275-1.275L21 12l-5.813-1.912a2 2 0 0 1-1.275-1.275L12 3Z"/>
            </svg>
            <span class="font-mono text-[11px] truncate max-w-[190px]" :title="modelName">{{ modelName }}</span>
          </div>

          <!-- Reset Button -->
          <button 
            @click="$emit('reset')" 
            class="inline-flex items-center px-3 py-1.5 border border-slate-300 text-xs font-medium rounded-lg text-slate-700 bg-white hover:bg-slate-50 transition shadow-sm active:scale-95"
            title="Reset conversation and demo data"
          >
            <svg class="w-3.5 h-3.5 mr-1.5 text-slate-400" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M3 12a9 9 0 1 0 9-9 9.75 9.75 0 0 0-6.74 2.74L3 8"/>
              <path d="M3 3v5h5"/>
            </svg>
            Reset Demo
          </button>
        </div>
      </div>
    </div>
  </header>
</template>

<script setup>
defineProps({
  backendOnline: {
    type: Boolean,
    default: true
  },
  modelName: {
    type: String,
    default: 'azure/genailab-maas-gpt-4o-mini'
  }
})

defineEmits(['reset'])
</script>

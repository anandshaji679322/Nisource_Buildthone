/**
 * SmartBill AI API Client
 * Proxied via Vite dev server to ASP.NET Core Web API on http://localhost:5000
 */

const API_BASE = '/api';

export const api = {
  // --- Customer Bill Advisor Endpoints ---
  async getDashboardSummary(accountNumber = 'ACT-7492-EAST', periodId = null) {
    const params = new URLSearchParams({ accountNumber });
    if (periodId) params.append('periodId', periodId);
    const res = await fetch(`${API_BASE}/bill/summary?${params.toString()}`);
    if (!res.ok) throw new Error(`Failed to fetch dashboard summary: ${res.statusText}`);
    return await res.json();
  },

  async explainBill(accountNumber, selectedPeriodId, baselinePeriodId = null) {
    const res = await fetch(`${API_BASE}/bill/explain`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ accountNumber, selectedPeriodId, baselinePeriodId })
    });
    if (!res.ok) throw new Error(`Failed to explain bill: ${res.statusText}`);
    return await res.json();
  },

  async compareBills(accountNumber, periodIdA, periodIdB) {
    const res = await fetch(`${API_BASE}/bill/compare`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ accountNumber, periodIdA, periodIdB })
    });
    if (!res.ok) throw new Error(`Failed to compare bills: ${res.statusText}`);
    return await res.json();
  },

  async getRecommendations(accountNumber = 'ACT-7492-EAST', periodId = '2026-07') {
    const res = await fetch(`${API_BASE}/bill/recommendations?accountNumber=${accountNumber}&periodId=${periodId}`);
    if (!res.ok) throw new Error(`Failed to fetch recommendations: ${res.statusText}`);
    return await res.json();
  },

  // --- Business Billing Estimator Endpoints ---
  async getEstimatorDashboard() {
    const res = await fetch(`${API_BASE}/estimator/dashboard`);
    if (!res.ok) throw new Error(`Failed to fetch estimator dashboard: ${res.statusText}`);
    return await res.json();
  },

  async getMeterDetail(customerId) {
    const res = await fetch(`${API_BASE}/estimator/meter/${customerId}`);
    if (!res.ok) throw new Error(`Failed to fetch meter detail: ${res.statusText}`);
    return await res.json();
  },

  async updateReviewStatus(customerId, action, notes = '') {
    const res = await fetch(`${API_BASE}/estimator/review`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ customerId, action, notes })
    });
    if (!res.ok) throw new Error(`Failed to update review status: ${res.statusText}`);
    return await res.json();
  },

  async runBacktest() {
    const res = await fetch(`${API_BASE}/estimator/backtest`);
    if (!res.ok) throw new Error(`Failed to execute backtest: ${res.statusText}`);
    return await res.json();
  },

  // --- Health Check ---
  async checkHealth() {
    try {
      const res = await fetch(`${API_BASE}/health`);
      return res.ok;
    } catch {
      return false;
    }
  }
};

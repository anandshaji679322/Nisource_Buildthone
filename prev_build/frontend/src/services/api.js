// API client service for Smart Move Assistant

const BASE_URL = import.meta.env.VITE_API_URL || '';

export async function processTransfer(message, existingData = null) {
  const response = await fetch(`${BASE_URL}/api/transfer/process`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ message, existingData }),
  });

  if (!response.ok) {
    const errorData = await response.json().catch(() => ({}));
    throw new Error(errorData.error || `Server responded with status ${response.status}`);
  }

  return await response.json();
}

export async function confirmTransfer(transferDetails, customerNotes = '') {
  const response = await fetch(`${BASE_URL}/api/transfer/confirm`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      transferDetails,
      customerNotes,
    }),
  });

  if (!response.ok) {
    throw new Error(`Failed to confirm transfer: ${response.statusText}`);
  }

  return await response.json();
}

export async function checkApiStatus() {
  try {
    const response = await fetch(`${BASE_URL}/api/transfer/status`, {
      method: 'GET',
    });
    if (response.ok) {
      return await response.json();
    }
  } catch (e) {
    console.warn('API status check failed:', e);
  }
  return null;
}

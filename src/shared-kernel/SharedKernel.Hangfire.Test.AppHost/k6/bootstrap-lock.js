import http from 'k6/http';
import { check, fail } from 'k6';

export const options = {
  vus: 1,
  iterations: 1,
};

const baseUrl = (__ENV.TARGET_URL || '').replace(/\/$/, '');
const headers = { 'Content-Type': 'application/json' };

if (!baseUrl) {
  fail('TARGET_URL is required.');
}

function createScenarioId(prefix) {
  return `${prefix}-${Date.now()}-${Math.floor(Math.random() * 100000)}`;
}

function getState(scenarioId) {
  const response = http.get(`${baseUrl}/tests/scenarios/${scenarioId}`);

  check(response, {
    'scenario state is available': (r) => r.status === 200,
  });

  return response.json();
}

export default function () {
  const scenarioId = createScenarioId('bootstrap');
  const requestCount = Number(__ENV.BOOTSTRAP_REQUEST_COUNT || 8);
  const holdMilliseconds = Number(__ENV.BOOTSTRAP_HOLD_MS || 750);
  const requestBody = JSON.stringify({ holdMilliseconds });
  const requests = [];

  for (let index = 0; index < requestCount; index += 1) {
    requests.push([
      'POST',
      `${baseUrl}/tests/scenarios/${scenarioId}/bootstrap/run`,
      requestBody,
      { headers },
    ]);
  }

  const responses = http.batch(requests);

  check(responses, {
    'all bootstrap requests succeeded': (allResponses) =>
      allResponses.every((response) => response.status === 200),
  });

  const state = getState(scenarioId);

  check(state, {
    'all bootstrap requests entered lock': (snapshot) =>
      snapshot.bootstrapEntryCount === requestCount,
    'bootstrap lock stayed serialized': (snapshot) =>
      snapshot.bootstrapMaxConcurrency === 1,
  });

  if (state.bootstrapMaxConcurrency !== 1) {
    fail(`bootstrapMaxConcurrency should be 1 but was ${state.bootstrapMaxConcurrency}.`);
  }
}

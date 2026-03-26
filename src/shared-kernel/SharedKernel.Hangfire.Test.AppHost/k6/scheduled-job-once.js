import http from 'k6/http';
import { check, fail, sleep } from 'k6';

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

function waitForSingleExecution(scenarioId, timeoutSeconds, fieldName) {
  const startedAt = Date.now();

  while ((Date.now() - startedAt) / 1000 < timeoutSeconds) {
    const state = getState(scenarioId);

    if (state[fieldName] > 1) {
      fail(`${fieldName} exceeded 1: ${JSON.stringify(state)}`);
    }

    if (state[fieldName] === 1) {
      return state;
    }

    sleep(1);
  }

  fail(`Timed out waiting for ${fieldName} to reach 1.`);
}

export default function () {
  const scenarioId = createScenarioId('scheduled');
  const scheduleResponse = http.post(
    `${baseUrl}/tests/scenarios/${scenarioId}/jobs/scheduled`,
    JSON.stringify({ delayMilliseconds: 1500 }),
    { headers }
  );

  check(scheduleResponse, {
    'scheduled job accepted': (r) => r.status === 202,
  });

  const state = waitForSingleExecution(
    scenarioId,
    Number(__ENV.SCHEDULED_TIMEOUT_SECONDS || 45),
    'scheduledExecutions'
  );

  check(state, {
    'scheduled job executed once': (snapshot) =>
      snapshot.scheduledExecutions === 1 && snapshot.scheduledInstances.length === 1,
  });

  sleep(3);

  const finalState = getState(scenarioId);

  check(finalState, {
    'scheduled job remained single': (snapshot) => snapshot.scheduledExecutions === 1,
  });
}

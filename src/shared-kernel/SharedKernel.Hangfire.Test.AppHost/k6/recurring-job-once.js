import http from 'k6/http';
import { check, fail, sleep } from 'k6';

export const options = {
  vus: 1,
  iterations: 1,
};

const baseUrl = (__ENV.TARGET_URL || '').replace(/\/$/, '');

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
  const scenarioId = createScenarioId('recurring');
  const registrationResponse = http.post(
    `${baseUrl}/tests/scenarios/${scenarioId}/jobs/recurring/next-minute`,
    null
  );

  check(registrationResponse, {
    'recurring job accepted': (r) => r.status === 202,
  });

  const registration = registrationResponse.json();
  const timeoutSeconds = Number(__ENV.RECURRING_TIMEOUT_SECONDS || 120);
  const startedAt = Date.now();

  console.log(`Waiting for recurring job ${registration.jobId} scheduled at ${registration.scheduledForUtc}.`);

  while ((Date.now() - startedAt) / 1000 < timeoutSeconds) {
    const state = getState(scenarioId);

    if (state.recurringExecutions > 1) {
      fail(`recurringExecutions exceeded 1: ${JSON.stringify(state)}`);
    }

    if (state.recurringExecutions === 1) {
      check(state, {
        'recurring job executed once': (snapshot) =>
          snapshot.recurringExecutions === 1 && snapshot.recurringInstances.length === 1,
      });

      sleep(5);

      const finalState = getState(scenarioId);
      check(finalState, {
        'recurring job remained single': (snapshot) => snapshot.recurringExecutions === 1,
      });
      return;
    }

    sleep(1);
  }

  fail('Timed out waiting for recurringExecutions to reach 1.');
}

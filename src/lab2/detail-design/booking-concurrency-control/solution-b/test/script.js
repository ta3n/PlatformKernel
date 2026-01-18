import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate } from 'k6/metrics';

// Custom metric to track error rate (requests with status != 200)
export const errorRate = new Rate('errors');

export const options = {
  vus: 10,           // Number of virtual users running concurrently
  iterations: 10,    // Total iterations per VU (each user sends 1 request)
  thresholds: {
    // 95% of requests must complete within 20 seconds
    http_req_duration: ['p(95)<20000'],

    // Expect error rate to be between 89% and 91%
    // Because only 1 out of 10 requests can succeed when 1 room is available
    http_req_failed: ['rate > 0.89', 'rate < 0.91'],
  },
};

// API endpoint to test
const url = 'http://localhost:7088/api/booking/plans/75/rooms/64?api-version=1';

// Load payload and headers from external JSON files for easy modification
const payloadRaw = open('payload.json');
const headersRaw = open('headers.json');

const payload = JSON.parse(payloadRaw);
const headers = JSON.parse(headersRaw);

export default function () {
  // Send POST request to the API with provided payload and headers
  let res = http.post(
    url,
    JSON.stringify(payload),
    { headers }
  );

  // Check if the response status is 200 OK
  // If not, mark this request as an error
  check(res, {
    'is status 200': (r) => r.status === 200,
  }) || errorRate.add(1);

  // Pause for 0.3 seconds between iterations to simulate realistic user behavior
  sleep(0.3);
}

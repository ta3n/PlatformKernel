import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate } from 'k6/metrics';

export const errorRate = new Rate('errors');

export const options = {
  vus: 500,
  duration: '1s',
};

const token = "...";
export default function () {
  const url = "http://localhost:7087/api/price-calendar";

  const payload = JSON.stringify({
  "calendars": [
    {
      "priceTypeId": 288,
      "dateCalendar": 20261013,
      "isDeleted": false
    }
  ]
});

  const params = {
    headers: {
        "accept": "*/*",
        "content-type": "application/json-patch+json",
        "x-facility-key": "facility-bbc7dea5-2324-4206-8212-cb0b092ca0ca",
        "Authorization": `Bearer ${token}`
    }
  };

  const res = http.post(url, payload, params);

  check(res, {
    "status is 204": (r) => r.status === 204,
    "500 error" : (r) => r.status !== 204,
  })|| errorRate.add(1);
  sleep(0.3)
}

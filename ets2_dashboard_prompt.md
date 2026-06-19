# ETS2/ATS 실시간 텔레메트리 대시보드 HTML 제작 요청 프롬프트

## 개요

Euro Truck Simulator 2 / American Truck Simulator 게임의 실시간 텔레메트리(주행 데이터)를
WebSocket으로 수신해서 표시하는 **HTML 대시보드**를 만들어 주세요.

백엔드(C# 서버)는 이미 완성되어 있습니다. HTML/CSS/JS만 작성하면 됩니다.

---

## WebSocket 연결 정보

| 항목 | 값 |
|------|-----|
| 주소 | `ws://localhost:25555/` |
| 프로토콜 | WebSocket (표준) |
| 메시지 형식 | JSON (UTF-8) |
| 전송 주기 | 게임 데이터 갱신 시마다 자동 Push (약 25ms) |

### 연결 코드 예시 (JavaScript)

```javascript
const ws = new WebSocket("ws://localhost:25555/");

ws.onopen  = () => console.log("연결됨");
ws.onclose = () => console.log("연결 끊김");
ws.onerror = (e) => console.error("오류", e);

ws.onmessage = (event) => {
    const data = JSON.parse(event.data);
    // data 객체를 사용하여 UI 업데이트
    // 예: data.truck.speed.kph, data.job.onJob, data.navigation.speedLimitKph 등
};
```

---

## 수신 JSON 데이터 구조 (전체 스펙)

서버에서 매 프레임 아래 구조의 JSON을 전송합니다.
`// 주석`은 설명용이며 실제 JSON에는 없습니다.

```
{
  "system": {
    "sdkActive":  true,       // bool   - SDK가 게임에 정상 로드되었는지. false면 나머지 데이터 무효
    "gamePaused": false,      // bool   - 게임 일시정지 여부
    "game":       "ETS2",     // string - "ETS2" 또는 "ATS"
    "dllVersion": 14,         // uint   - 플러그인 버전
    "timestamp":  123456789   // ulong  - 데이터 갱신 타임스탬프(ms)
  },

  "truck": {
    "electricEnabled": true,   // bool - 전기(배터리) 켜짐 여부
    "engineEnabled":   true,   // bool - 엔진 켜짐 여부

    "speed": {
      "ms":  27.77,            // float - 속도 m/s
      "kph": 100.0,            // float - 속도 km/h  ★ 주로 사용
      "mph": 62.1              // float - 속도 mph
    },

    "gear": {
      "selected":  4,          // int - 실제 엔진 기어. 양수=전진, 0=중립(N), 음수=후진(R)
      "displayed": 4           // int - 계기판 표시 기어 (오토미션일 때 selected와 다를 수 있음)
    },

    "rpm": 1450.0,             // float - 엔진 RPM

    "fuel": {
      "amount":         400.5, // float - 현재 연료 (리터)
      "capacity":       700.0, // float - 최대 용량 (리터)
      "avgConsumption": 0.35,  // float - 평균 연비 (리터/km)
      "range":          1144.0,// float - 현재 연료로 갈 수 있는 예상 거리 (km)
      "warning":        false  // bool  - 연료 부족 경고등
    },

    "adblue": {
      "amount":   40.0,        // float - 현재 요소수 (리터)
      "capacity": 60.0,        // float - 최대 용량 (리터)
      "warning":  false        // bool  - 요소수 부족 경고등
    },

    "brake": {
      "parkingBrake":                false, // bool  - 주차 브레이크 ON 여부
      "motorBrake":                  false, // bool  - 엔진 브레이크 ON 여부
      "retarderLevel":               0,     // uint  - 리타더 단계 (0=꺼짐)
      "airPressure":                 120.0, // float - 에어 탱크 압력 (psi)
      "airPressureWarning":          false, // bool  - 에어 압력 경고
      "airPressureEmergencyWarning": false, // bool  - 에어 압력 긴급 경고 (비상제동 발생)
      "brakeTemperature":            25.0   // float - 브레이크 온도 (°C)
    },

    "cruiseControl": {
      "active":   true,        // bool  - 크루즈 컨트롤 활성 여부
      "speedKph": 90.0         // float - 설정된 크루즈 속도 (km/h). 비활성시 0
    },

    "lights": {
      "parking":       false,  // bool   - 주차등 (미등)
      "beamLow":       true,   // bool   - 하향등 (기본 전조등)
      "beamHigh":      false,  // bool   - 상향등
      "beacon":        false,  // bool   - 경광등(비콘)
      "brakeLights":   false,  // bool   - 브레이크등
      "reverseLights": false,  // bool   - 후진등
      "blinkerLeft":  { "active": false, "on": false }, // 좌측 방향지시등. active=스위치ON, on=현재 전구켜짐(깜빡임)
      "blinkerRight": { "active": false, "on": false }, // 우측 방향지시등
      "hazard":        false,  // bool   - 비상등 (양쪽 깜빡이)
      "auxFront":      "None", // string - 보조 전방 라이트 ("None" / "Low" / "High")
      "auxRoof":       "None"  // string - 루프 라이트
    },

    "warnings": {
      "oilPressure":      false, // bool - 오일 압력 경고등
      "waterTemperature": false, // bool - 수온 경고등
      "batteryVoltage":   false  // bool - 배터리 전압 경고등
    },

    "engine": {
      "oilPressure":      45.0,  // float - 오일 압력 (psi)
      "oilTemperature":   90.0,  // float - 오일 온도 (°C)
      "waterTemperature": 82.0,  // float - 냉각수 온도 (°C)
      "batteryVoltage":   24.5   // float - 배터리 전압 (V)
    },

    "wipers":   false,           // bool  - 와이퍼 ON 여부
    "odometer": 12450.5,         // float - 주행 거리계 (km)

    "differentialLock":         false, // bool - 차동 잠금
    "liftAxle":                 false, // bool - 리프트 액슬 (들린 차축)
    "liftAxleIndicator":        false, // bool - 리프트 액슬 표시등
    "trailerLiftAxle":          false, // bool - 트레일러 리프트 액슬
    "trailerLiftAxleIndicator": false, // bool

    "damage": {
      "engine":       0.0,  // float - 엔진 손상도 (0.0=정상 ~ 1.0=완전파손)
      "transmission": 0.0,  // float - 변속기 손상도
      "cabin":        0.0,  // float - 캐빈 손상도
      "chassis":      0.0,  // float - 섀시 손상도
      "wheelsAvg":    0.05  // float - 전체 바퀴 평균 손상도
    },

    "position": {
      "x":       1234.5,   // double - 월드 X 좌표
      "y":       10.0,     // double - 월드 Y 좌표 (높이)
      "z":       -500.0,   // double - 월드 Z 좌표
      "heading": 0.75,     // double - 방위각 (0.0/1.0=북, 0.25=서, 0.5=남, 0.75=동)
      "pitch":   0.0,      // double - 앞뒤 기울기
      "roll":    0.0       // double - 좌우 기울기
    },

    "config": {
      "truckBrand":    "Scania",  // string - 트럭 브랜드
      "truckModel":    "R 500",   // string - 트럭 모델
      "licensePlate":  "AB-1234", // string - 번호판
      "maxRpm":        2100.0,    // float  - 최대 RPM
      "forwardGears":  12,        // uint   - 전진 기어 수
      "reverseGears":  2,         // uint   - 후진 기어 수
      "retarderSteps": 3,         // uint   - 리타더 최대 단계
      "wheelCount":    6          // uint   - 바퀴 수
    }
  },

  "trailers": [                  // 배열. 대부분 trailers[0]만 사용하면 됨
    {
      "attached":     true,      // bool   - 트레일러 연결 여부
      "id":           "scs.box", // string - 내부 ID
      "name":         "Box",     // string - 이름
      "licensePlate": "TR-9999", // string - 트레일러 번호판
      "damage": {
        "chassis": 0.0,          // float - 섀시 손상도
        "wheels":  0.0,          // float - 바퀴 손상도
        "body":    0.0           // float - 차체 손상도
      },
      "position": {
        "x": 1230.0,             // float - 훅 연결 X 좌표
        "y": 9.5,                // float - 훅 연결 Y 좌표
        "z": -498.0              // float - 훅 연결 Z 좌표
      }
    }
  ],

  "job": {
    "onJob":             true,          // bool   - 현재 운송 작업 중인지. ★ false면 나머지 값 무시
    "specialJob":        false,         // bool   - 특수 화물 작업 여부
    "cargoLoaded":       true,          // bool   - 화물 적재 여부
    "market":            "CargoMarket", // string - 화물 시장 종류
    "income":            4500,          // ulong  - 예상 수익 (게임 내 화폐)
    "plannedDistanceKm": 350,           // uint   - 계획된 운송 거리 (km)

    "cargo": {
      "name":      "Steel Coils",       // string - 화물 이름
      "id":        "steel_coils",       // string - 내부 ID
      "massKg":    24000.0,             // float  - 화물 무게 (kg)
      "unitCount": 1,                   // uint   - 화물 단위 수
      "damage":    0.0                  // float  - 화물 손상도 (0.0~1.0)
    },

    "source": {
      "city":      "Praha",             // string - 출발 도시
      "cityId":    "prague",            // string - 도시 내부 ID
      "company":   "Kögel",             // string - 출발 회사
      "companyId": "kogel.prague"       // string - 회사 내부 ID
    },

    "destination": {
      "city":      "Wien",              // string - 목적지 도시
      "cityId":    "vienna",            // string - 도시 내부 ID
      "company":   "Schmitz",           // string - 목적지 회사
      "companyId": "schmitz.wien"       // string - 회사 내부 ID
    },

    "deliveryTime": {
      "value":        30480,            // uint - 납품 마감 시각 (게임 내 분 단위)
      "remainingMin": 120               // int  - 납품까지 남은 시간(분). ★ 음수=기한 초과
    }
  },

  "navigation": {
    "speedLimitKph":    90.0,           // float - 현재 도로 제한 속도 (km/h). 0이면 제한 없음
    "distanceM":        124000,         // float - 목적지까지 남은 거리 (m)
    "timeRemainingSec": 5040            // float - 목적지까지 예상 소요 시간 (초)
  },

  "controls": {
    "input": {
      "steering": 0.05,  // float - 운전자 핸들 입력 (-1.0=최대좌 ~ 0 ~ 1.0=최대우)
      "throttle": 0.6,   // float - 운전자 가속 입력 (0.0~1.0)
      "brake":    0.0,   // float - 운전자 브레이크 입력 (0.0~1.0)
      "clutch":   0.0    // float - 운전자 클러치 입력 (0.0~1.0)
    },
    "game": {
      "steering": 0.05,  // float - 게임이 실제 적용한 핸들 (크루즈/AEB 등 포함)
      "throttle": 0.6,   // float - 게임이 실제 적용한 가속
      "brake":    0.0,   // float - 게임이 실제 적용한 브레이크
      "clutch":   0.0    // float - 게임이 실제 적용한 클러치
    }
  },

  "events": {
    "jobDelivered": false, // bool - 화물 배달 완료 이벤트 (해당 순간만 true)
    "jobCancelled": false, // bool - 작업 취소 이벤트
    "jobFinished":  false, // bool - 작업 종료 이벤트
    "fined":        false, // bool - 벌금 부과 이벤트
    "tollgate":     false, // bool - 톨게이트 통과 이벤트
    "ferry":        false, // bool - 페리 이용 이벤트
    "train":        false, // bool - 열차 이용 이벤트
    "refuel":       false, // bool - 주유 중
    "refuelPayed":  false  // bool - 주유 결제 완료 이벤트
  }
}
```

---

## 제작 요청 사항

위 WebSocket + JSON 스펙을 기반으로 **단일 HTML 파일**(`dashboard.html`)을 만들어 주세요.

### 필수 기능

1. **연결 상태 표시**
   - `system.sdkActive` — SDK 연결 여부 (false면 "게임 미실행" 표시)
   - `system.gamePaused` — 일시정지 상태 표시

2. **주행 정보 패널**
   - 속도: `truck.speed.kph` (크고 눈에 띄게)
   - RPM: `truck.rpm` / `truck.config.maxRpm` (게이지 바 형태)
   - 기어: `truck.gear.displayed` → 양수=숫자, 0=**N**, 음수=**R+숫자** 로 표시
   - 크루즈 컨트롤: `truck.cruiseControl.active` + 설정 속도 `truck.cruiseControl.speedKph`
   - 주차브레이크: `truck.brake.parkingBrake`

3. **연료 & 요소수 게이지**
   - 연료: `truck.fuel.amount` / `truck.fuel.capacity` (진행 바 + 수치 표시)
   - 요소수: `truck.adblue.amount` / `truck.adblue.capacity`
   - 경고(`truck.fuel.warning`, `truck.adblue.warning`) 시 색상 빨간색으로 변경

4. **라이트 상태 패널**
   - 표시 대상: 주차등/하향등/상향등/경광등/비상등/좌우방향지시등/와이퍼
   - ON 상태는 밝게, OFF 상태는 어둡게 표시

5. **경고등 패널**
   - 연료/요소수/오일압력/수온/배터리/에어압력 경고를 아이콘 또는 텍스트로 표시
   - 경고 발생 시 빨간/주황 강조

6. **작업 정보 패널** (`job.onJob === true` 일 때만 표시)
   - 출발지 도시 + 회사 → 목적지 도시 + 회사
   - 화물명, 무게, 예상 수익
   - 납품 남은 시간 (`job.deliveryTime.remainingMin`, 음수면 "기한 초과" 표시)
   - 화물 손상도 (`job.cargo.damage` → 0% ~ 100%)

7. **내비게이션 패널**
   - 현재 제한속도: `navigation.speedLimitKph` (0이면 "제한 없음")
   - 목적지까지 남은 거리: `navigation.distanceM` (m/km 자동 전환)
   - 예상 소요 시간: `navigation.timeRemainingSec` (시간/분으로 변환)

8. **WebSocket 자동 재연결**
   - 연결이 끊기면 3초 후 자동으로 재연결 시도

### 디자인 방향

- **어두운 배경**의 차량 계기판 스타일 (다크 테마)
- 속도/RPM 등 핵심 수치는 크고 명확하게
- 경고 상태일 때 빨간색/주황색으로 강조
- 실시간 업데이트가 자연스럽게 느껴지도록

### 기술 조건

- **외부 라이브러리 사용 금지** (CDN 포함) — 순수 HTML + CSS + Vanilla JS만 사용
- 단일 `.html` 파일로 완성
- `ws://localhost:25555/` 에 자동 연결

---

## 참고: JavaScript 헬퍼 함수

```javascript
// 기어 표시: 양수=숫자, 0=N, 음수=R+숫자
function formatGear(gear) {
    if (gear > 0) return String(gear);
    if (gear === 0) return "N";
    return "R" + Math.abs(gear);
}

// 남은 시간 표시 (분 단위 입력)
function formatRemainingTime(minutes) {
    if (minutes < 0) return "기한 초과 " + Math.abs(minutes) + "분";
    const h = Math.floor(minutes / 60);
    const m = minutes % 60;
    return h > 0 ? h + "시간 " + m + "분" : m + "분";
}

// 거리 표시 (미터 단위 입력)
function formatDistance(meters) {
    if (meters >= 1000) return (meters / 1000).toFixed(1) + " km";
    return Math.round(meters) + " m";
}

// 소요 시간 표시 (초 단위 입력)
function formatTime(seconds) {
    const h = Math.floor(seconds / 3600);
    const m = Math.floor((seconds % 3600) / 60);
    return h > 0 ? h + "시간 " + m + "분" : m + "분";
}

// 손상도 표시 (0.0~1.0 입력 → 0%~100%)
function formatDamage(value) {
    return (value * 100).toFixed(1) + "%";
}

// WebSocket 자동 재연결 패턴
function connect() {
    const ws = new WebSocket("ws://localhost:25555/");
    ws.onmessage = (e) => {
        const data = JSON.parse(e.data);
        updateUI(data);
    };
    ws.onclose = () => setTimeout(connect, 3000); // 3초 후 재연결
}
connect();
```

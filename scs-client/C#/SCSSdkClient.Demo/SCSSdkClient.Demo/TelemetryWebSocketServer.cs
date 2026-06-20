using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SCSSdkClient.Object;

namespace SCSSdkClient.Demo {

    /// <summary>
    /// WebSocket 서버 - ETS2/ATS 텔레메트리 데이터를 연결된 모든 HTML 클라이언트에 실시간으로 브로드캐스트합니다.
    /// 기본 주소: ws://localhost:25555/
    /// </summary>
    public class TelemetryWebSocketServer {

        private HttpListener _httpListener;
        private readonly List<WebSocket> _clients = new List<WebSocket>();
        private readonly object _clientsLock = new object();
        private bool _isRunning;

        /// <summary>HTML 클라이언트가 연결되면 발생. 인자: 현재 연결 수</summary>
        public event Action<int> ClientConnected;
        /// <summary>HTML 클라이언트가 연결 해제되면 발생. 인자: 현재 연결 수</summary>
        public event Action<int> ClientDisconnected;

        /// <summary>서버가 수신 대기 중인 포트</summary>
        public int Port { get; }

        public TelemetryWebSocketServer(int port = 25555) {
            Port = port;
        }

        /// <summary>서버를 시작합니다.</summary>
        public void Start() {
            _isRunning = true;
            _httpListener = new HttpListener();
            // 관리자 권한이 있으므로 모든 인터페이스(+)에서 수신 대기 (IPv4, IPv6 문제 방지)
            _httpListener.Prefixes.Add($"http://+:{Port}/");
            _httpListener.Start();
            Console.WriteLine($"[WebSocket] 서버 시작됨: http://127.0.0.1:{Port}/");
            Task.Run(AcceptLoop);
        }

        /// <summary>서버를 종료합니다.</summary>
        public void Stop() {
            _isRunning = false;
            _httpListener?.Stop();
            lock (_clientsLock) {
                foreach (var ws in _clients) {
                    try { ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "서버 종료", CancellationToken.None); }
                    catch { /* ignore */ }
                }
                _clients.Clear();
            }
        }

        /// <summary>
        /// 텔레메트리 데이터를 JSON으로 직렬화하여 모든 클라이언트에 전송합니다.
        /// SCSSdkClientDemo.cs 의 Telemetry_Data 이벤트에서 호출하세요.
        /// </summary>
        public void Broadcast(SCSTelemetry data) {
            // ─────────────────────────────────────────────────────────────────
            // JSON 페이로드 구조 정의
            // HTML 개발자는 이 구조를 참고하여 data.필드명 으로 접근합니다.
            // ─────────────────────────────────────────────────────────────────
            var payload = BuildPayload(data);
            string json = JsonConvert.SerializeObject(payload);
            byte[] bytes = Encoding.UTF8.GetBytes(json);

            lock (_clientsLock) {
                var deadClients = new List<WebSocket>();
                foreach (var ws in _clients) {
                    if (ws.State == WebSocketState.Open) {
                        try {
                            ws.SendAsync(new ArraySegment<byte>(bytes),
                                WebSocketMessageType.Text, true, CancellationToken.None);
                        } catch {
                            deadClients.Add(ws);
                        }
                    } else {
                        deadClients.Add(ws);
                    }
                }
                foreach (var dead in deadClients) _clients.Remove(dead);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // JSON 페이로드 빌더
        // 아래 구조가 HTML에서 수신하는 JSON의 전체 스펙입니다.
        // ─────────────────────────────────────────────────────────────────────
        private static object BuildPayload(SCSTelemetry d) {
            var truck = d.TruckValues;
            var job   = d.JobValues;
            var nav   = d.NavigationValues;
            var ctrl  = d.ControlValues;

            return new {

                // ── 1. 시스템 상태 ──────────────────────────────────────────
                // 게임과 SDK의 연결 상태입니다. HTML 로딩 후 가장 먼저 확인하세요.
                system = new {
                    sdkActive  = d.SdkActive,          // bool  - SDK 플러그인이 게임에 로드되어 정상 동작 중인지
                    gamePaused = d.Paused,              // bool  - 게임이 일시정지 상태인지
                    game       = d.Game.ToString(),     // string - "ETS2" 또는 "ATS"
                    dllVersion = d.DllVersion,          // uint  - 플러그인 DLL 버전
                    timestamp  = d.Timestamp,           // ulong - 텔레메트리 타임스탬프(ms). 이 값이 바뀔 때만 나머지 데이터가 갱신됩니다.
                },

                // ── 2. 트럭 기본 상태 ──────────────────────────────────────
                // 운전 중 가장 자주 사용하는 핵심 값들입니다.
                truck = new {

                    // 2-1. 전원 상태
                    electricEnabled = truck.CurrentValues.ElectricEnabled,  // bool - 전기(배터리) 켜짐 여부
                    engineEnabled   = truck.CurrentValues.EngineEnabled,    // bool - 엔진 켜짐 여부

                    // 2-2. 속도 & 기어
                    // speed.kph  를 주로 사용하면 됩니다.
                    speed = new {
                        ms  = truck.CurrentValues.DashboardValues.Speed.Value,  // float - 속도 (m/s)
                        kph = truck.CurrentValues.DashboardValues.Speed.Kph,    // float - 속도 (km/h) ★ 주로 사용
                        mph = truck.CurrentValues.DashboardValues.Speed.Mph,    // float - 속도 (mph)
                    },
                    // gear: 양수=전진, 0=중립(N), 음수=후진(R)
                    // 예) 1=1단, -1=후진1단, 0=N
                    gear = new {
                        selected  = truck.CurrentValues.MotorValues.GearValues.Selected,        // int  - 실제 엔진 기어
                        displayed = truck.CurrentValues.DashboardValues.GearDashboards,         // int  - 계기판에 표시되는 기어
                    },
                    rpm = truck.CurrentValues.DashboardValues.RPM,  // float - 엔진 RPM

                    // 2-3. 연료
                    fuel = new {
                        amount          = truck.CurrentValues.DashboardValues.FuelValue.Amount,              // float - 현재 연료량 (리터)
                        capacity        = truck.ConstantsValues.CapacityValues.Fuel,                         // float - 최대 연료 용량 (리터)
                        avgConsumption  = truck.CurrentValues.DashboardValues.FuelValue.AverageConsumption,  // float - 평균 연비 (L/km)
                        range           = truck.CurrentValues.DashboardValues.FuelValue.Range,               // float - 현재 연료로 갈 수 있는 예상 거리 (km)
                        warning         = truck.CurrentValues.DashboardValues.WarningValues.FuelW,           // bool  - 연료 부족 경고
                    },

                    // 2-4. AdBlue (요소수)
                    adblue = new {
                        amount  = truck.CurrentValues.DashboardValues.AdBlue,                    // float - 현재 요소수량 (리터)
                        capacity = truck.ConstantsValues.CapacityValues.AdBlue,                  // float - 최대 요소수 용량 (리터)
                        warning = truck.CurrentValues.DashboardValues.WarningValues.AdBlue,      // bool  - 요소수 부족 경고
                    },

                    // 2-5. 브레이크
                    brake = new {
                        parkingBrake  = truck.CurrentValues.MotorValues.BrakeValues.ParkingBrake,   // bool  - 주차 브레이크 작동 여부
                        motorBrake    = truck.CurrentValues.MotorValues.BrakeValues.MotorBrake,     // bool  - 엔진 브레이크 작동 여부
                        retarderLevel = truck.CurrentValues.MotorValues.BrakeValues.RetarderLevel,  // uint  - 리타더 레벨 (0=꺼짐, 최대값은 트럭 설정에 따라 다름)
                        airPressure   = truck.CurrentValues.MotorValues.BrakeValues.AirPressure,    // float - 브레이크 에어 탱크 압력 (psi)
                        airPressureWarning          = truck.CurrentValues.DashboardValues.WarningValues.AirPressure,          // bool - 에어 압력 경고
                        airPressureEmergencyWarning = truck.CurrentValues.DashboardValues.WarningValues.AirPressureEmergency, // bool - 에어 압력 긴급 경고 (비상 제동 발생)
                        brakeTemperature = truck.CurrentValues.MotorValues.BrakeValues.Temperature, // float - 브레이크 온도 (°C)
                    },

                    // 2-6. 크루즈 컨트롤
                    cruiseControl = new {
                        active = truck.CurrentValues.DashboardValues.CruiseControl,                    // bool  - 크루즈 컨트롤 활성 여부
                        speedKph = truck.CurrentValues.DashboardValues.CruiseControlSpeed.Kph,         // float - 설정된 크루즈 속도 (km/h). 비활성시 0
                    },

                    // 2-7. 라이트
                    lights = new {
                        parking        = truck.CurrentValues.LightsValues.Parking,           // bool - 주차등 (미등)
                        beamLow        = truck.CurrentValues.LightsValues.BeamLow,           // bool - 하향등 (기본 전조등)
                        beamHigh       = truck.CurrentValues.LightsValues.BeamHigh,          // bool - 상향등
                        beacon         = truck.CurrentValues.LightsValues.Beacon,            // bool - 경광등 (비콘)
                        brakeLights    = truck.CurrentValues.LightsValues.Brake,             // bool - 브레이크등
                        reverseLights  = truck.CurrentValues.LightsValues.Reverse,           // bool - 후진등
                        // blinker: Active=방향지시등 스위치 ON 상태, On=현재 전구가 켜진 상태(깜빡임 주기)
                        blinkerLeft    = new { active = truck.CurrentValues.LightsValues.BlinkerLeftActive,  on = truck.CurrentValues.LightsValues.BlinkerLeftOn  }, // 좌 방향지시등
                        blinkerRight   = new { active = truck.CurrentValues.LightsValues.BlinkerRightActive, on = truck.CurrentValues.LightsValues.BlinkerRightOn }, // 우 방향지시등
                        hazard         = truck.CurrentValues.LightsValues.HazardWarningLights, // bool - 비상등 (양쪽 깜빡이)
                        auxFront       = truck.CurrentValues.LightsValues.AuxFront.ToString(), // string - 보조 전방 라이트 레벨 ("None"/"Low"/"High")
                        auxRoof        = truck.CurrentValues.LightsValues.AuxRoof.ToString(),  // string - 루프 라이트 레벨
                    },

                    // 2-8. 계기판 경고등
                    warnings = new {
                        oilPressure      = truck.CurrentValues.DashboardValues.WarningValues.OilPressure,      // bool - 오일 압력 경고
                        waterTemperature = truck.CurrentValues.DashboardValues.WarningValues.WaterTemperature, // bool - 수온 경고
                        batteryVoltage   = truck.CurrentValues.DashboardValues.WarningValues.BatteryVoltage,   // bool - 배터리 전압 경고
                    },

                    // 2-9. 엔진 및 차량 상태
                    engine = new {
                        oilPressure      = truck.CurrentValues.DashboardValues.OilPressure,      // float - 오일 압력 (psi)
                        oilTemperature   = truck.CurrentValues.DashboardValues.OilTemperature,   // float - 오일 온도 (°C)
                        waterTemperature = truck.CurrentValues.DashboardValues.WaterTemperature, // float - 냉각수 온도 (°C)
                        batteryVoltage   = truck.CurrentValues.DashboardValues.BatteryVoltage,   // float - 배터리 전압 (V)
                    },

                    // 2-10. 기타 상태
                    wipers        = truck.CurrentValues.DashboardValues.Wipers,       // bool  - 와이퍼 작동 여부
                    odometer      = truck.CurrentValues.DashboardValues.Odometer,     // float - 주행 거리계 (km)
                    differentialLock       = truck.CurrentValues.DifferentialLock,    // bool  - 차동 잠금 여부
                    liftAxle               = truck.CurrentValues.LiftAxle,            // bool  - 리프트 액슬(들림 차축) 작동 여부
                    liftAxleIndicator      = truck.CurrentValues.LiftAxleIndicator,   // bool  - 리프트 액슬 표시등
                    trailerLiftAxle        = truck.CurrentValues.TrailerLiftAxle,     // bool  - 트레일러 리프트 액슬
                    trailerLiftAxleIndicator = truck.CurrentValues.TrailerLiftAxleIndicator, // bool

                    // 2-11. 마모도 (손상도) 0.0 ~ 1.0 (0%=정상, 1.0=100% 손상)
                    damage = new {
                        engine       = truck.CurrentValues.DamageValues.Engine,       // float
                        transmission = truck.CurrentValues.DamageValues.Transmission, // float
                        cabin        = truck.CurrentValues.DamageValues.Cabin,        // float
                        chassis      = truck.CurrentValues.DamageValues.Chassis,      // float
                        wheelsAvg    = truck.CurrentValues.DamageValues.WheelsAvg,    // float - 전체 바퀴 평균 마모도
                    },

                    // 2-12. 트럭 세계 좌표 위치 & 방향
                    position = new {
                        x         = truck.CurrentValues.PositionValue.Position.X,         // double - 월드 X 좌표
                        y         = truck.CurrentValues.PositionValue.Position.Y,         // double - 월드 Y 좌표 (높이)
                        z         = truck.CurrentValues.PositionValue.Position.Z,         // double - 월드 Z 좌표
                        heading   = truck.CurrentValues.PositionValue.Orientation.Heading, // double - 방위각 (0.0~1.0, 0/1=북쪽, 0.25=서쪽, 0.5=남쪽, 0.75=동쪽)
                        pitch     = truck.CurrentValues.PositionValue.Orientation.Pitch,   // double - 피치 (앞/뒤 기울기)
                        roll      = truck.CurrentValues.PositionValue.Orientation.Roll,    // double - 롤 (좌/우 기울기)
                    },

                    // 2-13. 트럭 스펙 (거의 변하지 않는 설정값)
                    config = new {
                        truckBrand     = truck.ConstantsValues.Brand,           // string - 트럭 브랜드 이름 (예: "Scania")
                        truckModel     = truck.ConstantsValues.Name,            // string - 트럭 모델 이름
                        licensePlate   = truck.ConstantsValues.LicensePlate,    // string - 번호판
                        maxRpm         = truck.ConstantsValues.MotorValues.EngineRpmMax,        // float - 최대 RPM
                        forwardGears   = truck.ConstantsValues.MotorValues.ForwardGearCount,    // uint  - 전진 기어 수
                        reverseGears   = truck.ConstantsValues.MotorValues.ReverseGearCount,    // uint  - 후진 기어 수
                        retarderSteps  = truck.ConstantsValues.MotorValues.RetarderStepCount,   // uint  - 리타더 최대 단계
                        wheelCount     = truck.ConstantsValues.WheelsValues.Count,              // uint  - 바퀴 수
                    },
                },

                // ── 3. 트레일러 (최대 10개, [0]이 첫 번째 트레일러) ────────
                // 일반적으로 trailers[0] 만 사용합니다.
                trailers = BuildTrailersPayload(d.TrailerValues),

                // ── 4. 현재 운송 작업 정보 ─────────────────────────────────
                // onJob 이 false 이면 나머지 값들은 의미 없습니다.
                job = new {
                    onJob              = d.SpecialEventsValues.OnJob,              // bool   - 현재 운송 중인지 여부
                    specialJob         = job.SpecialJob,                           // bool   - 특수 화물 작업 여부
                    cargoLoaded        = job.CargoLoaded,                          // bool   - 화물이 적재되어 있는지
                    market             = job.Market.ToString(),                    // string - 화물 시장 종류 ("CargoMarket", "ManualParcel" 등)
                    income             = job.Income,                               // ulong  - 예상 수익 (게임 내 화폐)
                    plannedDistanceKm  = job.PlannedDistanceKm,                   // uint   - 계획된 운송 거리 (km)
                    cargo = new {
                        name        = job.CargoValues.Name,        // string - 화물 이름 (예: "Steel Coils")
                        id          = job.CargoValues.Id,          // string - 화물 내부 ID
                        massKg      = job.CargoValues.Mass,        // float  - 화물 무게 (kg)
                        unitCount   = job.CargoValues.UnitCount,   // uint   - 화물 단위 수
                        damage      = job.CargoValues.CargoDamage, // float  - 화물 손상도 (0.0~1.0)
                    },
                    source = new {
                        city        = job.CitySource,              // string - 출발 도시 이름
                        cityId      = job.CitySourceId,            // string - 출발 도시 ID
                        company     = job.CompanySource,           // string - 출발 회사 이름
                        companyId   = job.CompanySourceId,         // string - 출발 회사 ID
                    },
                    destination = new {
                        city        = job.CityDestination,         // string - 목적지 도시 이름
                        cityId      = job.CityDestinationId,       // string - 목적지 도시 ID
                        company     = job.CompanyDestination,      // string - 목적지 회사 이름
                        companyId   = job.CompanyDestinationId,    // string - 목적지 회사 ID
                    },
                    deliveryTime = new {
                        value           = job.DeliveryTime.Value,              // uint   - 납품 마감 시간 (게임 내 분 단위)
                        remainingMin    = job.RemainingDeliveryTime.Value,     // int    - 남은 시간 (분). 음수면 납품 기한 초과
                    },
                },

                // ── 5. 내비게이션 ──────────────────────────────────────────
                navigation = new {
                    speedLimitKph    = nav.SpeedLimit.Kph,          // float - 현재 구간 제한 속도 (km/h). 0이면 제한 없음
                    distanceM        = nav.NavigationDistance,       // float - 목적지까지 남은 거리 (m)
                    timeRemainingSec = nav.NavigationTime,           // float - 목적지까지 예상 소요 시간 (초)
                },

                // ── 6. 운전자 입력 & 게임 제어 ─────────────────────────────
                // 값 범위: -1.0 ~ 1.0 (또는 0.0 ~ 1.0)
                controls = new {
                    // 운전자가 실제로 입력한 값
                    input = new {
                        steering = ctrl.InputValues.Steering,  // float - 핸들 (-1.0=최대좌, 0=중앙, 1.0=최대우)
                        throttle = ctrl.InputValues.Throttle,  // float - 가속 (0.0~1.0)
                        brake    = ctrl.InputValues.Brake,     // float - 브레이크 (0.0~1.0)
                        clutch   = ctrl.InputValues.Clutch,    // float - 클러치 (0.0~1.0)
                    },
                    // 게임이 실제로 차량에 적용한 값 (크루즈컨트롤/AEB 등 보조장치 포함)
                    game = new {
                        steering = ctrl.GameValues.Steering,   // float
                        throttle = ctrl.GameValues.Throttle,   // float
                        brake    = ctrl.GameValues.Brake,      // float
                        clutch   = ctrl.GameValues.Clutch,     // float
                    },
                },

                // ── 7. 특수 이벤트 (해당 순간에만 true로 반짝임) ────────────
                events = new {
                    jobDelivered  = d.SpecialEventsValues.JobDelivered,  // bool - 화물 배달 완료
                    jobCancelled  = d.SpecialEventsValues.JobCancelled,  // bool - 작업 취소
                    jobFinished   = d.SpecialEventsValues.JobFinished,   // bool - 작업 종료
                    fined         = d.SpecialEventsValues.Fined,         // bool - 벌금 부과
                    tollgate      = d.SpecialEventsValues.Tollgate,      // bool - 톨게이트 통과
                    ferry         = d.SpecialEventsValues.Ferry,         // bool - 페리 이용
                    train         = d.SpecialEventsValues.Train,         // bool - 열차 이용
                    refuel        = d.SpecialEventsValues.Refuel,        // bool - 주유 중
                    refuelPayed   = d.SpecialEventsValues.RefuelPayed,   // bool - 주유 결제 완료
                },
            };
        }

        private static object[] BuildTrailersPayload(SCSTelemetry.Trailer[] trailers) {
            if (trailers == null) return new object[0];
            var result = new object[trailers.Length];
            for (int i = 0; i < trailers.Length; i++) {
                var t = trailers[i];
                result[i] = new {
                    attached     = t.Attached,           // bool   - 트레일러 연결 여부
                    id           = t.Id,                 // string - 트레일러 내부 ID
                    name         = t.Name,               // string - 트레일러 이름
                    licensePlate = t.LicensePlate,       // string - 트레일러 번호판
                    damage = new {
                        chassis   = t.DamageValues.Chassis,   // float - 섀시 손상도 (0.0~1.0)
                        wheels    = t.DamageValues.Wheels,    // float - 바퀴 손상도
                        body      = t.DamageValues.Body,      // float - 차체 손상도
                    },
                    position = new {
                        x       = t.Hook.X,   // float - 연결 훅의 X 좌표
                        y       = t.Hook.Y,   // float - 연결 훅의 Y 좌표
                        z       = t.Hook.Z,   // float - 연결 훅의 Z 좌표
                    },
                };
            }
            return result;
        }

        // ─── 내부: WebSocket 연결 수락 루프 ────────────────────────────────
        private async Task AcceptLoop() {
            while (_isRunning && _httpListener.IsListening) {
                try {
                    var ctx = await _httpListener.GetContextAsync();
                    if (ctx.Request.IsWebSocketRequest) {
                        var wsCtx = await ctx.AcceptWebSocketAsync(null);
                        var ws = wsCtx.WebSocket;
                        int cnt;
                        lock (_clientsLock) { _clients.Add(ws); cnt = _clients.Count; }
                        ClientConnected?.Invoke(cnt);
                        _ = Task.Run(() => KeepAliveLoop(ws));
                    } else {
                        ServeHtmlFile(ctx);
                    }
                } catch (Exception ex) when (_isRunning) {
                    Console.WriteLine($"[WebSocket] AcceptLoop 오류: {ex.Message}");
                }
            }
        }

        private async Task KeepAliveLoop(WebSocket ws) {
            var buf = new byte[256];
            while (ws.State == WebSocketState.Open) {
                try {
                    var result = await ws.ReceiveAsync(new ArraySegment<byte>(buf), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close) break;
                } catch { break; }
            }
            int count;
            lock (_clientsLock) { _clients.Remove(ws); count = _clients.Count; }
            ClientDisconnected?.Invoke(count);
        }

        private static void ServeHtmlFile(HttpListenerContext ctx) {
            try {
                string exeDir = AppDomain.CurrentDomain.BaseDirectory;
                string requestPath = ctx.Request.Url.AbsolutePath;
                if (requestPath == "/") requestPath = "/dashboard.html";
                
                string filePath = System.IO.Path.Combine(exeDir, "overlay", requestPath.TrimStart('/').Replace('/', '\\'));

                if (System.IO.File.Exists(filePath)) {
                    byte[] body = System.IO.File.ReadAllBytes(filePath);
                    string ext = System.IO.Path.GetExtension(filePath).ToLower();
                    
                    if (ext == ".png") ctx.Response.ContentType = "image/png";
                    else if (ext == ".js") ctx.Response.ContentType = "application/javascript";
                    else if (ext == ".css") ctx.Response.ContentType = "text/css";
                    else ctx.Response.ContentType = "text/html; charset=utf-8";
                    
                    ctx.Response.AddHeader("Access-Control-Allow-Origin", "*");
                    ctx.Response.ContentLength64 = body.Length;
                    ctx.Response.OutputStream.Write(body, 0, body.Length);
                    ctx.Response.OutputStream.Close();
                } else {
                    if (requestPath.EndsWith(".png")) {
                        ctx.Response.StatusCode = 404;
                        ctx.Response.Close();
                        return;
                    }
                    var sb = new System.Text.StringBuilder();
                    sb.Append("<html><body style='font-family:monospace;padding:30px;background:#111;color:#eee'>");
                    sb.Append("<h2 style='color:#f59e0b'>파일을 찾을 수 없습니다</h2>");
                    sb.Append($"<p>아래 경로에 파일을 배치해 주세요:</p>");
                    sb.Append($"<p style='color:#fff; background:#333; padding:10px; border-radius:5px;'>{filePath}</p>");
                    sb.Append("</body></html>");
                    byte[] body = Encoding.UTF8.GetBytes(sb.ToString());
                    ctx.Response.ContentType = "text/html; charset=utf-8";
                    ctx.Response.ContentLength64 = body.Length;
                    ctx.Response.OutputStream.Write(body, 0, body.Length);
                    ctx.Response.OutputStream.Close();
                }
            } catch {
                try { ctx.Response.StatusCode = 500; ctx.Response.Close(); } catch { }
            }
        }
    }
}

using System;
using System.Drawing;
using System.Windows.Forms;
using Newtonsoft.Json;
using SCSSdkClient.Object;

namespace SCSSdkClient.Demo {

    public partial class SCSSdkClientDemo : Form {

        // ── 핵심 객체 ─────────────────────────────────────────────────────
        public SCSSdkTelemetry Telemetry;
        private TelemetryWebSocketServer _wsServer;

        // ── 상태 추적 ─────────────────────────────────────────────────────
        private bool _connected   = false;   // 게임(공유메모리) 연결 여부
        private bool _sdkActive   = false;   // SDK 플러그인 활성 여부
        private bool _gamePaused  = false;   // 게임 일시정지 여부
        private int  _wsClients   = 0;       // 연결된 HTML 클라이언트 수
        private float _lastSpeed  = 0;
        private float _lastRpm    = 0;
        private int   _lastGear   = 0;
        private float _lastFuel   = 0;
        private string _lastUpdateInterval = "-";

        // ── 생성자 ────────────────────────────────────────────────────────
        public SCSSdkClientDemo() {
            InitializeComponent();
            timerUI.Start();
            Log("앱 시작됨. '연결 시작' 버튼을 눌러 게임 텔레메트리 수신을 시작하세요.", LogLevel.Info);
            Log($"WebSocket 서버 주소: ws://localhost:25555/", LogLevel.Info);
        }

        // ────────────────────────────────────────────────────────────────
        // 버튼 이벤트
        // ────────────────────────────────────────────────────────────────

        private void btnConnect_Click(object sender, EventArgs e) {
            if (_connected) return;

            try {
                // 1. WebSocket 서버 시작
                _wsServer = new TelemetryWebSocketServer(25555);
                _wsServer.ClientConnected    += OnWsClientConnected;
                _wsServer.ClientDisconnected += OnWsClientDisconnected;
                _wsServer.Start();
                Log("WebSocket 서버 시작: ws://localhost:25555/", LogLevel.Success);

                // 2. SCS 텔레메트리 연결
                Telemetry = new SCSSdkTelemetry();
                Telemetry.Data          += Telemetry_Data;
                Telemetry.JobStarted    += (s, a) => Log("이벤트: 작업 시작", LogLevel.Event);
                Telemetry.JobCancelled  += (s, a) => Log("이벤트: 작업 취소", LogLevel.Warning);
                Telemetry.JobDelivered  += (s, a) => Log("이벤트: 배달 완료", LogLevel.Success);
                Telemetry.Fined         += (s, a) => Log("이벤트: 벌금 부과", LogLevel.Warning);
                Telemetry.Tollgate      += (s, a) => Log("이벤트: 톨게이트 통과", LogLevel.Event);
                Telemetry.Ferry         += (s, a) => Log("이벤트: 페리 이용", LogLevel.Event);
                Telemetry.Train         += (s, a) => Log("이벤트: 열차 이용", LogLevel.Event);
                Telemetry.RefuelStart   += (s, a) => Log("이벤트: 주유 시작", LogLevel.Event);
                Telemetry.RefuelEnd     += (s, a) => Log("이벤트: 주유 완료", LogLevel.Event);

                if (Telemetry.Error != null) {
                    Log($"텔레메트리 오류: {Telemetry.Error.Message}", LogLevel.Error);
                    Log("게임이 실행 중인지, 그리고 scs-telemetry.dll 플러그인이 설치되어 있는지 확인하세요.", LogLevel.Warning);
                } else {
                    Log($"텔레메트리 연결 성공. 공유메모리: {Telemetry.Map}", LogLevel.Success);
                }

                _connected = true;
                btnConnect.Enabled    = false;
                btnDisconnect.Enabled = true;
                Log("게임 데이터 수신 대기 중...", LogLevel.Info);

                // 웹 브라우저 자동 실행
                OpenDashboardBrowser();

            } catch (Exception ex) {
                Log($"연결 실패: {ex.Message}", LogLevel.Error);
            }
        }

        private void btnOpenDashboard_Click(object sender, EventArgs e) {
            OpenDashboardBrowser();
        }

        private void OpenDashboardBrowser() {
            try {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo {
                    FileName = "chrome.exe",
                    Arguments = "--app=http://127.0.0.1:25555/",
                    UseShellExecute = true
                });
                Log("크롬(Chrome) 앱 모드로 대시보드를 열었습니다.", LogLevel.Success);
            } catch {
                try {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo {
                        FileName = "msedge.exe",
                        Arguments = "--app=http://127.0.0.1:25555/",
                        UseShellExecute = true
                    });
                    Log("엣지(Edge) 앱 모드로 대시보드를 열었습니다.", LogLevel.Success);
                } catch {
                    try {
                        // 둘 다 없으면 기본 브라우저 사용
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo {
                            FileName = "http://127.0.0.1:25555/",
                            UseShellExecute = true
                        });
                        Log("기본 브라우저로 대시보드를 열었습니다.", LogLevel.Success);
                    } catch {
                        Log("브라우저를 열 수 없습니다. 수동으로 http://127.0.0.1:25555/ 에 접속하세요.", LogLevel.Warning);
                    }
                }
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e) {
            Disconnect();
        }

        private void Disconnect(bool isClosing = false) {
            if (!_connected && _wsServer == null) return;

            try { Telemetry?.Dispose(); } catch { }
            try { _wsServer?.Stop(); } catch { }

            Telemetry  = null;
            _wsServer  = null;
            _connected = false;
            _sdkActive = false;
            _wsClients = 0;

            if (!isClosing) {
                btnConnect.Enabled    = true;
                btnDisconnect.Enabled = false;
                Log("연결이 해제되었습니다.", LogLevel.Warning);
            }
        }

        // ────────────────────────────────────────────────────────────────
        // 텔레메트리 데이터 수신
        // ────────────────────────────────────────────────────────────────

        private void Telemetry_Data(SCSTelemetry data, bool updated) {
            if (!updated) return;

            // HTML 클라이언트에 브로드캐스트
            _wsServer?.Broadcast(data);

            // 상태 값 업데이트 (UI 스레드 아님 → 저장만)
            _sdkActive  = data.SdkActive;
            _gamePaused = data.Paused;
            _lastSpeed  = data.TruckValues.CurrentValues.DashboardValues.Speed.Kph;
            _lastRpm    = data.TruckValues.CurrentValues.DashboardValues.RPM;
            _lastGear   = data.TruckValues.CurrentValues.DashboardValues.GearDashboards;
            _lastFuel   = data.TruckValues.CurrentValues.DashboardValues.FuelValue.Amount;
            _lastUpdateInterval = Telemetry?.UpdateInterval.ToString() ?? "-";

            // SDK 최초 활성화 감지 로그
            if (data.SdkActive && !_lastSdkActiveLogged) {
                _lastSdkActiveLogged = true;
                Log($"✅ 게임 감지! {data.Game} v{data.GameVersion} | SDK v{data.DllVersion}", LogLevel.Success);
            } else if (!data.SdkActive && _lastSdkActiveLogged) {
                _lastSdkActiveLogged = false;
                Log("⚠️ 게임 SDK 비활성화됨 (게임 종료?)", LogLevel.Warning);
            }
        }

        private bool _lastSdkActiveLogged = false;

        // ────────────────────────────────────────────────────────────────
        // WebSocket 클라이언트 이벤트
        // ────────────────────────────────────────────────────────────────

        private void OnWsClientConnected(int count) {
            _wsClients = count;
            Log($"🌐 HTML 클라이언트 연결됨 (현재 {count}개)", LogLevel.Success);
        }

        private void OnWsClientDisconnected(int count) {
            _wsClients = count;
            Log($"🌐 HTML 클라이언트 연결 해제 (현재 {count}개)", LogLevel.Info);
        }

        // ────────────────────────────────────────────────────────────────
        // UI 갱신 타이머 (500ms)
        // ────────────────────────────────────────────────────────────────

        private void timerUI_Tick(object sender, EventArgs e) {
            // LED 상태 업데이트
            SetLed(panelSdkLight,    lblSdkLight,    _connected && _sdkActive,  "SDK 플러그인 활성",     "SDK 플러그인 비활성");
            SetLed(panelGameLight,   lblGameLight,   _connected,                "게임 연결됨",           "게임 미연결");
            SetLed(panelWsLight,     lblWsLight,     _wsServer != null,         "WebSocket 실행 중",     "WebSocket 중지됨");
            SetLed(panelPausedLight, lblPausedLight, _gamePaused,               "일시정지",              "실행 중",
                   pauseColor: Color.FromArgb(210, 153, 34));   // 일시정지는 노란색

            // 수치 업데이트
            if (_connected && _sdkActive) {
                string gearStr = _lastGear > 0 ? _lastGear.ToString()
                               : _lastGear == 0 ? "N"
                               : "R" + Math.Abs(_lastGear);

                lblStatSpeed.Text       = $"속도:  {_lastSpeed:F0} km/h";
                lblStatRpm.Text         = $"RPM:  {_lastRpm:F0}";
                lblStatGear.Text        = $"기어:  {gearStr}";
                lblStatFuel.Text        = $"연료:  {_lastFuel:F0} L";
                lblStatClients.Text     = $"HTML 클라이언트:  {_wsClients} 개";
                lblStatUpdateRate.Text  = $"갱신 주기:  {_lastUpdateInterval} ms";
            } else {
                lblStatSpeed.Text      = "속도:  -";
                lblStatRpm.Text        = "RPM:  -";
                lblStatGear.Text       = "기어:  -";
                lblStatFuel.Text       = "연료:  -";
                lblStatClients.Text    = $"HTML 클라이언트:  {_wsClients} 개";
                lblStatUpdateRate.Text = "갱신 주기:  -";
            }
        }

        private static void SetLed(Panel led, Label lbl, bool on, string onText, string offText,
                                   Color? pauseColor = null) {
            if (on) {
                led.BackColor = pauseColor ?? Color.FromArgb(35, 134, 54);
                lbl.ForeColor = Color.FromArgb(201, 209, 217);
                lbl.Text      = onText;
            } else {
                led.BackColor = Color.FromArgb(68, 68, 68);
                lbl.ForeColor = Color.FromArgb(100, 110, 120);
                lbl.Text      = offText;
            }
        }

        // ────────────────────────────────────────────────────────────────
        // 로그
        // ────────────────────────────────────────────────────────────────

        private enum LogLevel { Info, Success, Warning, Error, Event }

        private void Log(string message, LogLevel level = LogLevel.Info) {
            if (rtbLog.IsDisposed) return;
            if (rtbLog.InvokeRequired) {
                try { rtbLog.Invoke(new Action(() => Log(message, level))); } catch { }
                return;
            }

            Color color = level switch {
                LogLevel.Success => Color.FromArgb(63, 185, 80),
                LogLevel.Warning => Color.FromArgb(210, 153, 34),
                LogLevel.Error   => Color.FromArgb(248, 81, 73),
                LogLevel.Event   => Color.FromArgb(88, 166, 255),
                _                => Color.FromArgb(139, 148, 158),
            };

            string line = $"[{DateTime.Now:HH:mm:ss}]  {message}\n";
            rtbLog.SelectionStart  = rtbLog.TextLength;
            rtbLog.SelectionLength = 0;
            rtbLog.SelectionColor  = color;
            rtbLog.AppendText(line);
            rtbLog.SelectionColor  = rtbLog.ForeColor;
            rtbLog.ScrollToCaret();

            // 로그가 너무 많아지면 위쪽 절반 제거
            if (rtbLog.Lines.Length > 500) {
                rtbLog.Select(0, rtbLog.GetFirstCharIndexFromLine(250));
                rtbLog.SelectedText = "";
            }
        }

        private void btnClearLog_Click(object sender, EventArgs e) {
            rtbLog.Clear();
            Log("로그 지워짐.", LogLevel.Info);
        }

        // ────────────────────────────────────────────────────────────────
        // 폼 닫기
        // ────────────────────────────────────────────────────────────────

        private void SCSSdkClientDemo_FormClosing(object sender, FormClosingEventArgs e) {
            Disconnect(isClosing: true);
        }

        // ── 이하: 기존 코드 호환용 빈 핸들러 (이전 Designer 참조) ─────────
        private delegate void TelemetryData(SCSTelemetry data, bool updated);
    }
}

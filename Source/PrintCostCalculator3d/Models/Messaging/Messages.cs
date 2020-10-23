using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintCostCalculator3d.Models.Messaging
{
    public enum Messages
    {
        //Host
        OnServerSettingsChanged,
        OnCheckServerConnection,
        OnSettingsChanged,
        //Dependencies
        OnRefreshData,
        OnRefreshSettings,
        OnRefreshPrintServerSettings,
        OnRefreshConnectionSettings,
        OnRefreshFiles,
        OnRestartTimer,
        OnServerOffline,
        OnServerNotConfigured,

        OnTimerStopped,
        OnTimerStarted,
        OnCurrentJobInfoChanged,
        OnPrinterStateChanged,
        OnFilesChanged,
        OnConnectionStatusChanged,
        OnPrintServerSettingsChanged,
        OnConnectionSettingsChanged,
        //Fitlers
        OnFilterChanged,

        //WebCam
        OnRestartWebCam,
        OnStopWebCam,

        //RepetierServer
        OnWebSocketReconnect,
        OnWebSocketDisconnected,
        OnWebSocketConnected,
        OnWebSocketError,
        OnRepetierOnlineStateChanged,
        OnSelectedPrinterChanged,
        OnRefreshPrinterState,
        OnRefreshPrinter,
        OnRefreshMessages,
        OnRefreshCurrentJobInfo,
        OnRefreshExternalCommands,
        OnRefreshWebCalls,

        OnJobsChanged,
        OnMessageReceived,
        OnMessagesChanged,
        OnSessionIdChanged,
        OnWebCallsChanged,
    }
}

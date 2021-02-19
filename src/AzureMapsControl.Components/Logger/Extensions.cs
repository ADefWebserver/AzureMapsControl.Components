﻿namespace AzureMapsControl.Components.Logger
{
    using Microsoft.Extensions.Logging;

    internal enum AzureMapLogEvent
    {
        AzureMap_OnInitialized = 1000,
        AzureMap_OnAfterRenderAsync = 1001,
        AzureMap_AttachDataSourcesCallback = 1005,
        AzureMap_DataSource_ImportDataFromUrlAsync = 1007,
        AzureMap_DataSource_AddAsync = 1008,
        AzureMap_DataSource_RemoveAsync = 1009,
        AzureMap_DataSource_ClearAsync = 1010,
        AzureMap_DrawingToolbarEvent = 1011,
        AzureMap_HtmlMarkerEventReceivedAsync = 1015,
        AzureMap_LayerEventReceivedAsync = 1020,
        AzureMap_MapEventReceivedAsync = 1024,
        AzureMap_ClearMapAsync = 1029,
        AzureMap_AddPopupAsync = 1030,
        AzureMap_Popup_OpenAsync = 1031,
        AzureMap_Popup_CloseAsync = 1032,
        AzureMap_Popup_RemoveAsync = 1033,
        AzureMap_Popup_UpdateAsync = 1034,
        AzureMap_ClearPopupsAsync = 1035,
        AzureMap_PopupEventReceivedAsync = 1036,
        MapService_AddMapAsync = 2000,
        AnimationService_Snakeline = 3000,
        Map_AddControlsAsync = 4000,
        Map_AddSourceAsync = 4001,
        Map_RemoveSourceAsync = 4002,
        Map_ClearSourcesAsync = 4003,
        Map_AddDrawingToolbarAsync = 4004,
        Map_UpdateDrawingToolbarAsync = 4005,
        Map_RemoveDrawingToolbarAsync = 4006,
        Map_AddHtmlMarkersAsync = 4007,
        Map_UpdateHtmlMarkersAsync = 4008,
        Map_RemoveHtmlMarkersAsync = 4009,
        Map_ClearHtmlMarkersAsync = 4010,
        Map_AddLayerAsync = 4011,
        Map_RemoveLayersAsync = 4012,
        Map_ClearLayersAsync = 4013,
        Map_SetCameraOptionsAsync = 4014,
        Map_SetStyleOptionsAsync = 4015,
        Map_SetUserInteractionAsync = 4016,
        Map_SetTrafficAsync = 4017,
        OverviewMapControl_UpdateAsync = 5000
    }

    internal static class Extensions
    {
        internal static void LogAzureMapsControlTrace(this ILogger logger, AzureMapLogEvent logEvent, string message, params object[] args) => logger.LogAzureMapsControl(LogLevel.Trace, logEvent, message, args);
        internal static void LogAzureMapsControlDebug(this ILogger logger, AzureMapLogEvent logEvent, string message, params object[] args) => logger.LogAzureMapsControl(LogLevel.Debug, logEvent, message, args);
        internal static void LogAzureMapsControlInfo(this ILogger logger, AzureMapLogEvent logEvent, string message, params object[] args) => logger.LogAzureMapsControl(LogLevel.Information, logEvent, message, args);
        internal static void LogAzureMapsControl(this ILogger logger, LogLevel logLevel, AzureMapLogEvent logEvent, string message, params object[] args) => logger.Log(logLevel, new EventId((int)logEvent), message, args);
    }
}

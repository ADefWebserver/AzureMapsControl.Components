﻿namespace AzureMapsControl.Components.Tests.Map
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    using AzureMapsControl.Components.Atlas;
    using AzureMapsControl.Components.Controls;
    using AzureMapsControl.Components.Data;
    using AzureMapsControl.Components.Drawing;
    using AzureMapsControl.Components.Exceptions;
    using AzureMapsControl.Components.Layers;
    using AzureMapsControl.Components.Map;
    using AzureMapsControl.Components.Markers;
    using AzureMapsControl.Components.Popups;
    using AzureMapsControl.Components.Runtime;
    using AzureMapsControl.Components.Traffic;

    using Microsoft.Extensions.Logging;
    using Microsoft.JSInterop;

    using Moq;

    using Xunit;

    public class MapTests
    {
        private readonly Mock<IMapJsRuntime> _jsRuntimeMock = new();
        private readonly Mock<ILogger> _loggerMock = new();

        [Fact]
        public void Should_BeInitialized()
        {
            const string id = "id";
            var map = new Map(id, _jsRuntimeMock.Object, _loggerMock.Object);
            Assert.Equal(id, map.Id);
            Assert.Null(map.Controls);
            Assert.Null(map.HtmlMarkers);
            Assert.Null(map.DrawingToolbarOptions);
            Assert.Null(map.Layers);
            Assert.Null(map.Sources);
            Assert.Null(map.Popups);
        }

        [Fact]
        public async void Should_AddControls_Async()
        {
            var controls = new List<Control> {
                new CompassControl()
            };

            const string id = "id";
            var map = new Map(id, _jsRuntimeMock.Object, _loggerMock.Object);

            await map.AddControlsAsync(controls);
            Assert.Equal(controls, map.Controls);

            _jsRuntimeMock.Verify(runtime => runtime.InvokeVoidAsync(Constants.JsConstants.Methods.Core.AddControls.ToCoreNamespace(), It.Is<IOrderedEnumerable<Control>>(
                ctrls => ctrls.Single() == controls.Single())), Times.Once);
            _jsRuntimeMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void Should_AddOrderedControls_Async()
        {
            var controls = new List<Control> {
                new OverviewMapControl(),
                new CompassControl()
            };

            const string id = "id";
            var map = new Map(id, _jsRuntimeMock.Object, _loggerMock.Object);

            await map.AddControlsAsync(controls);
            Assert.Equal(controls, map.Controls);

            _jsRuntimeMock.Verify(runtime => runtime.InvokeVoidAsync(Constants.JsConstants.Methods.Core.AddControls.ToCoreNamespace(), It.Is<IOrderedEnumerable<Control>>(
                ctrls => ctrls.First() == controls.ElementAt(1) && ctrls.ElementAt(1) == controls.First())), Times.Once);
            _jsRuntimeMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void Should_AddControls_ParamsVersion_Async()
        {
            var control = new CompassControl(position: ControlPosition.BottomLeft);
            const string id = "id";
            var map = new Map(id, _jsRuntimeMock.Object, _loggerMock.Object);

            await map.AddControlsAsync(control);
            Assert.Single(map.Controls);
            Assert.Contains(control, map.Controls);
            _jsRuntimeMock.Verify(runtime => runtime.InvokeVoidAsync(Constants.JsConstants.Methods.Core.AddControls.ToCoreNamespace(), It.Is<IOrderedEnumerable<Control>>(
                ctrls => ctrls.Single() == control)), Times.Once);
            _jsRuntimeMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void Should_AddHtmlMarkers_Async()
        {
            var markers = new List<HtmlMarker> { new HtmlMarker(null), new HtmlMarker(null) };
            var map = new Map("id", _jsRuntimeMock.Object, _loggerMock.Object, htmlMarkerInvokeHelper: new HtmlMarkerInvokeHelper(eventArgs => Task.CompletedTask));

            await map.AddHtmlMarkersAsync(markers);
            Assert.Contains(markers[0], map.HtmlMarkers);
            Assert.Contains(markers[1], map.HtmlMarkers);

            _jsRuntimeMock.Verify(runtime => runtime.InvokeVoidAsync(Constants.JsConstants.Methods.Core.AddHtmlMarkers.ToCoreNamespace(), It.Is<object[]>(parameters =>
                parameters[0] is IEnumerable<HtmlMarkerCreationOptions> && (parameters[0] as IEnumerable<HtmlMarkerCreationOptions>).Count() == 2
                && parameters[1] is DotNetObjectReference<HtmlMarkerInvokeHelper>
            )), Times.Once);
            _jsRuntimeMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void Should_AddHtmlMarkers_ParamsVersion_Async()
        {
            var marker1 = new HtmlMarker(null);
            var marker2 = new HtmlMarker(null);
            var map = new Map("id", _jsRuntimeMock.Object, _loggerMock.Object, htmlMarkerInvokeHelper: new HtmlMarkerInvokeHelper(eventArgs => Task.CompletedTask));

            await map.AddHtmlMarkersAsync(marker1, marker2);
            Assert.Contains(marker1, map.HtmlMarkers);
            Assert.Contains(marker2, map.HtmlMarkers);
            _jsRuntimeMock.Verify(runtime => runtime.InvokeVoidAsync(Constants.JsConstants.Methods.Core.AddHtmlMarkers.ToCoreNamespace(), It.Is<object[]>(parameters =>
                parameters[0] is IEnumerable<HtmlMarkerCreationOptions> && (parameters[0] as IEnumerable<HtmlMarkerCreationOptions>).Count() == 2
                && parameters[1] is DotNetObjectReference<HtmlMarkerInvokeHelper>
            )), Times.Once);
            _jsRuntimeMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void Should_UpdateHtmlMarkers_Async()
        {
            var updates = new List<HtmlMarkerUpdate> { new HtmlMarkerUpdate(new HtmlMarker(null, null), null), new HtmlMarkerUpdate(new HtmlMarker(null, null), null) };
            var map = new Map("id", _jsRuntimeMock.Object, _loggerMock.Object);

            await map.UpdateHtmlMarkersAsync(updates);
            _jsRuntimeMock.Verify(runtime => runtime.InvokeVoidAsync(Constants.JsConstants.Methods.Core.UpdateHtmlMarkers.ToCoreNamespace(), It.Is<object[]>(parameters =>
                parameters.Single() is IEnumerable<HtmlMarkerCreationOptions>
                && (parameters.Single() as IEnumerable<HtmlMarkerCreationOptions>).Count() == updates.Count)
            ), Times.Once);
            _jsRuntimeMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void Should_UpdateHtmlMarkers_ParamsVersion_Async()
        {
            var update1 = new HtmlMarkerUpdate(new HtmlMarker(null, null), null);
            var update2 = new HtmlMarkerUpdate(new HtmlMarker(null, null), null);
            var map = new Map("id", _jsRuntimeMock.Object, _loggerMock.Object);

            await map.UpdateHtmlMarkersAsync(update1, update2);
            _jsRuntimeMock.Verify(runtime => runtime.InvokeVoidAsync(Constants.JsConstants.Methods.Core.UpdateHtmlMarkers.ToCoreNamespace(), It.Is<object[]>(parameters =>
                            parameters.Single() is IEnumerable<HtmlMarkerCreationOptions>
                            && (parameters.Single() as IEnumerable<HtmlMarkerCreationOptions>).Count() == 2)
                        ), Times.Once);
            _jsRuntimeMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void Shoud_NotRemoveAnyHtmlMarkers_Async()
        {
            var map = new Map("id", _jsRuntimeMock.Object, _loggerMock.Object);

            await map.RemoveHtmlMarkersAsync(new List<HtmlMarker> { new HtmlMarker(null) });
            _jsRuntimeMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void Shoud_NotRemoveAnyHtmlMarkers_Null_Async()
        {
            var map = new Map("id", _jsRuntimeMock.Object, _loggerMock.Object, htmlMarkerInvokeHelper: new HtmlMarkerInvokeHelper(eventArgs => Task.CompletedTask));
            var htmlMarker = new HtmlMarker(null);
            await map.AddHtmlMarkersAsync(htmlMarker);

            await map.RemoveHtmlMarkersAsync(null);

            _jsRuntimeMock.Verify(runtime => runtime.InvokeVoidAsync(Constants.JsConstants.Methods.Core.AddHtmlMarkers.ToCoreNamespace(), It.Is<object[]>(parameters =>
                parameters[0] is IEnumerable<HtmlMarkerCreationOptions> && (parameters[0] as IEnumerable<HtmlMarkerCreationOptions>).Count() == 1
                && parameters[1] is DotNetObjectReference<HtmlMarkerInvokeHelper>
            )), Times.Once);
            _jsRuntimeMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void Shoud_RemoveAnyHtmlMarkers_Async()
        {
            var htmlMarker = new HtmlMarker(null);
            var htmlMarker2 = new HtmlMarker(null);

            var map = new Map("id", _jsRuntimeMock.Object, _loggerMock.Object, htmlMarkerInvokeHelper: new HtmlMarkerInvokeHelper(eventArgs => Task.CompletedTask));
            await map.AddHtmlMarkersAsync(new List<HtmlMarker> { htmlMarker, htmlMarker2 });

            await map.RemoveHtmlMarkersAsync(htmlMarker);
            Assert.DoesNotContain(htmlMarker, map.HtmlMarkers);
            Assert.Contains(htmlMarker2, map.HtmlMarkers);

            _jsRuntimeMock.Verify(runtime => runtime.InvokeVoidAsync(Constants.JsConstants.Methods.Core.AddHtmlMarkers.ToCoreNamespace(), It.Is<object[]>(parameters =>
                parameters[0] is IEnumerable<HtmlMarkerCreationOptions> && (parameters[0] as IEnumerable<HtmlMarkerCreationOptions>).Count() == 2
                && parameters[1] is DotNetObjectReference<HtmlMarkerInvokeHelper>
            )), Times.Once);
            _jsRuntimeMock.Verify(runtime => runtime.InvokeVoidAsync(Constants.JsConstants.Methods.Core.RemoveHtmlMarkers.ToCoreNamespace(), It.Is<object[]>(parameters => 
                parameters.Single() is IEnumerable<string> && (parameters[0] as IEnumerable<string>).Single() == htmlMarker.Id)
            ), Times.Once);
            _jsRuntimeMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void Shoud_RemoveAnyHtmlMarkers_ParamsVersion_Async()
        {
            var htmlMarker = new HtmlMarker(null);
            var htmlMarker2 = new HtmlMarker(null);

            var map = new Map("id", _jsRuntimeMock.Object, _loggerMock.Object, htmlMarkerInvokeHelper: new HtmlMarkerInvokeHelper(eventArgs => Task.CompletedTask));
            await map.AddHtmlMarkersAsync(htmlMarker, htmlMarker2);

            await map.RemoveHtmlMarkersAsync(htmlMarker);
            Assert.DoesNotContain(htmlMarker, map.HtmlMarkers);
            Assert.Contains(htmlMarker2, map.HtmlMarkers);

            _jsRuntimeMock.Verify(runtime => runtime.InvokeVoidAsync(Constants.JsConstants.Methods.Core.AddHtmlMarkers.ToCoreNamespace(), It.Is<object[]>(parameters =>
                parameters[0] is IEnumerable<HtmlMarkerCreationOptions> && (parameters[0] as IEnumerable<HtmlMarkerCreationOptions>).Count() == 2
                && parameters[1] is DotNetObjectReference<HtmlMarkerInvokeHelper>
            )), Times.Once);
            _jsRuntimeMock.Verify(runtime => runtime.InvokeVoidAsync(Constants.JsConstants.Methods.Core.RemoveHtmlMarkers.ToCoreNamespace(), It.Is<object[]>(parameters =>
                parameters.Single() is IEnumerable<string> && (parameters[0] as IEnumerable<string>).Single() == htmlMarker.Id)
            ), Times.Once);
            _jsRuntimeMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void Should_AddDrawingToolbar_Async()
        {
            var drawingToolbarOptions = new DrawingToolbarOptions();
            var map = new Map("id", _jsRuntimeMock.Object, _loggerMock.Object, new DrawingToolbarEventInvokeHelper(eventArgs => Task.CompletedTask));
            await map.AddDrawingToolbarAsync(drawingToolbarOptions);

            Assert.Equal(drawingToolbarOptions, map.DrawingToolbarOptions);

            _jsRuntimeMock.Verify(runtime => runtime.InvokeVoidAsync(Constants.JsConstants.Methods.Drawing.AddDrawingToolbar.ToDrawingNamespace(), It.Is<object[]>(parameters =>
                parameters[0] is DrawingToolbarCreationOptions
                && parameters[1] is DotNetObjectReference<DrawingToolbarEventInvokeHelper>
             )), Times.Once);
            _jsRuntimeMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void Should_UpdateDrawingToolbar_Async()
        {
            var drawingToolbarOptions = new DrawingToolbarOptions();
            var updateDrawingToolbarOptions = new DrawingToolbarUpdateOptions {
                Buttons = new List<DrawingButton>(),
                ContainerId = "containerId",
                NumColumns = 2,
                Position = ControlPosition.BottomLeft,
                Style = DrawingToolbarStyle.Dark,
                Visible = false
            };
            var map = new Map("id", _jsRuntimeMock.Object, _loggerMock.Object, new DrawingToolbarEventInvokeHelper(eventArgs => Task.CompletedTask));
            await map.AddDrawingToolbarAsync(drawingToolbarOptions);
            await map.UpdateDrawingToolbarAsync(updateDrawingToolbarOptions);

            Assert.Equal(updateDrawingToolbarOptions.Buttons, map.DrawingToolbarOptions.Buttons);
            Assert.Equal(updateDrawingToolbarOptions.ContainerId, map.DrawingToolbarOptions.ContainerId);
            Assert.Equal(updateDrawingToolbarOptions.NumColumns, map.DrawingToolbarOptions.NumColumns);
            Assert.Equal(updateDrawingToolbarOptions.Position, map.DrawingToolbarOptions.Position);
            Assert.Equal(updateDrawingToolbarOptions.Style, map.DrawingToolbarOptions.Style);
            Assert.Equal(updateDrawingToolbarOptions.Visible, map.DrawingToolbarOptions.Visible);

            _jsRuntimeMock.Verify(runtime => runtime.InvokeVoidAsync(Constants.JsConstants.Methods.Drawing.AddDrawingToolbar.ToDrawingNamespace(), It.Is<object[]>(parameters =>
                parameters[0] is DrawingToolbarCreationOptions
                && parameters[1] is DotNetObjectReference<DrawingToolbarEventInvokeHelper>
             )), Times.Once);
            _jsRuntimeMock.Verify(runtime => runtime.InvokeVoidAsync(Constants.JsConstants.Methods.Drawing.UpdateDrawingToolbar.ToDrawingNamespace(), It.Is<object[]>(parameters =>
                parameters.Single() is DrawingToolbarCreationOptions
             )), Times.Once);
            _jsRuntimeMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void Should_RemoveDrawingToolbar_Async()
        {
            var map = new Map("id", _jsRuntimeMock.Object, _loggerMock.Object, new DrawingToolbarEventInvokeHelper(eventArgs => Task.CompletedTask));

            await map.AddDrawingToolbarAsync(new DrawingToolbarOptions());
            await map.RemoveDrawingToolbarAsync();

            Assert.Null(map.DrawingToolbarOptions);
            _jsRuntimeMock.Verify(runtime => runtime.InvokeVoidAsync(Constants.JsConstants.Methods.Drawing.AddDrawingToolbar.ToDrawingNamespace(), It.Is<object[]>(parameters =>
                parameters[0] is DrawingToolbarCreationOptions
                && parameters[1] is DotNetObjectReference<DrawingToolbarEventInvokeHelper>
             )), Times.Once);
            _jsRuntimeMock.Verify(runtime => runtime.InvokeVoidAsync(Constants.JsConstants.Methods.Drawing.RemoveDrawingToolbar.ToDrawingNamespace()), Times.Once);
            _jsRuntimeMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void Should_NotRemoveDrawingToolbar_Async()
        {
            var map = new Map("id", _jsRuntimeMock.Object, _loggerMock.Object, new DrawingToolbarEventInvokeHelper(eventArgs => Task.CompletedTask));

            await map.RemoveDrawingToolbarAsync();

            Assert.Null(map.DrawingToolbarOptions);
            _jsRuntimeMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void Should_AddALayer_Async()
        {
            var assertAddLayerCallback = false;
            var layer = new BubbleLayer();
            var map = new Components.Map.Map("id", addLayerCallback: async (layerCallback, beforeCallback) => assertAddLayerCallback = layerCallback == layer && beforeCallback == null);

            await map.AddLayerAsync(layer);
            Assert.True(assertAddLayerCallback);
            Assert.Contains(layer, map.Layers);
        }

        [Fact]
        public async void Should_AddALayerWithBefore_Async()
        {
            var assertAddLayerCallback = false;
            var layer = new BubbleLayer();
            const string before = "before";
            var map = new Components.Map.Map("id", addLayerCallback: async (layerCallback, beforeCallback) => assertAddLayerCallback = layerCallback == layer && beforeCallback == before);

            await map.AddLayerAsync(layer, before);
            Assert.True(assertAddLayerCallback);
            Assert.Contains(layer, map.Layers);
        }

        [Fact]
        public async void Should_NotAddLayerWithSameId_Async()
        {
            var layer = new BubbleLayer();
            var map = new Components.Map.Map("id", addLayerCallback: async (layerCallback, beforeCallback) => { });

            await map.AddLayerAsync(layer);
            await Assert.ThrowsAnyAsync<LayerAlreadyAddedException>(async () => await map.AddLayerAsync(layer));
        }

        [Fact]
        public async void Should_RemoveOneLayer_Async()
        {
            var assertRemoveLayerCallback = false;
            var layer1 = new BubbleLayer();
            var layer2 = new BubbleLayer();

            var map = new Components.Map.Map("id", addLayerCallback: async (layerCallback, beforeCallback) => { }, removeLayersCallback: async layers => assertRemoveLayerCallback = layers.Single() == layer1.Id);
            await map.AddLayerAsync(layer1);
            await map.AddLayerAsync(layer2);
            await map.RemoveLayersAsync(layer1);

            Assert.True(assertRemoveLayerCallback);
            Assert.DoesNotContain(layer1, map.Layers);
            Assert.Contains(layer2, map.Layers);
        }

        [Fact]
        public async void Should_RemoveMultipleLayers_Async()
        {
            var assertRemoveLayerCallback = false;
            var layer1 = new BubbleLayer();
            var layer2 = new BubbleLayer();

            var map = new Components.Map.Map("id", addLayerCallback: async (layerCallback, beforeCallback) => { }, removeLayersCallback: async layers => assertRemoveLayerCallback = layers.Contains(layer1.Id) && layers.Contains(layer2.Id));
            await map.AddLayerAsync(layer1);
            await map.AddLayerAsync(layer2);
            await map.RemoveLayersAsync(new List<Layer> { layer1, layer2 });

            Assert.True(assertRemoveLayerCallback);
            Assert.DoesNotContain(layer1, map.Layers);
            Assert.DoesNotContain(layer2, map.Layers);
        }

        [Fact]
        public async void Should_RemoveMultipleLayers_ParamsVersion_Async()
        {
            var assertRemoveLayerCallback = false;
            var layer1 = new BubbleLayer();
            var layer2 = new BubbleLayer();

            var map = new Components.Map.Map("id", addLayerCallback: async (layerCallback, beforeCallback) => { }, removeLayersCallback: async layers => assertRemoveLayerCallback = layers.Contains(layer1.Id) && layers.Contains(layer2.Id));
            await map.AddLayerAsync(layer1);
            await map.AddLayerAsync(layer2);
            await map.RemoveLayersAsync(layer1, layer2);

            Assert.True(assertRemoveLayerCallback);
            Assert.DoesNotContain(layer1, map.Layers);
            Assert.DoesNotContain(layer2, map.Layers);
        }

        [Fact]
        public async void Should_RemoveMultipleLayers_ViaId_Async()
        {
            var assertRemoveLayerCallback = false;
            var layer1 = new BubbleLayer();
            var layer2 = new BubbleLayer();

            var map = new Components.Map.Map("id", addLayerCallback: async (layerCallback, beforeCallback) => { }, removeLayersCallback: async layers => assertRemoveLayerCallback = layers.Contains(layer1.Id) && layers.Contains(layer2.Id));
            await map.AddLayerAsync(layer1);
            await map.AddLayerAsync(layer2);
            await map.RemoveLayersAsync(new List<string> { layer1.Id, layer2.Id });

            Assert.True(assertRemoveLayerCallback);
            Assert.DoesNotContain(layer1, map.Layers);
            Assert.DoesNotContain(layer2, map.Layers);
        }

        [Fact]
        public async void Should_RemoveMultipleLayers_ViaId_ParamsVersion_Async()
        {
            var assertRemoveLayerCallback = false;
            var layer1 = new BubbleLayer();
            var layer2 = new BubbleLayer();

            var map = new Components.Map.Map("id", addLayerCallback: async (layerCallback, beforeCallback) => { }, removeLayersCallback: async layers => assertRemoveLayerCallback = layers.Contains(layer1.Id) && layers.Contains(layer2.Id));
            await map.AddLayerAsync(layer1);
            await map.AddLayerAsync(layer2);
            await map.RemoveLayersAsync(layer1.Id, layer2.Id);

            Assert.True(assertRemoveLayerCallback);
            Assert.DoesNotContain(layer1, map.Layers);
            Assert.DoesNotContain(layer2, map.Layers);
        }

        [Fact]
        public async void Should_AddDataSource_Async()
        {
            var dataSource = new DataSource();

            var map = new Map("id", _jsRuntimeMock.Object, _loggerMock.Object);
            await map.AddSourceAsync(dataSource);

            Assert.Single(map.Sources, dataSource);
            Assert.Equal(_jsRuntimeMock.Object, dataSource.JsRuntime);
            Assert.Equal(_loggerMock.Object, dataSource.Logger);

            _jsRuntimeMock.Verify(runtime => runtime.InvokeVoidAsync(Constants.JsConstants.Methods.Core.AddSource.ToCoreNamespace(), It.Is<object[]>(parameters =>
                parameters[0] as string == dataSource.Id
                && parameters[1] == null
                && parameters[2] as string == dataSource.SourceType.ToString()
            )), Times.Once);
            _jsRuntimeMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void Should_AddVectorTileSourceAsync()
        {
            var source = new VectorTileSource();

            var map = new Map("id", _jsRuntimeMock.Object, _loggerMock.Object);
            await map.AddSourceAsync(source);

            Assert.Single(map.Sources, source);

            _jsRuntimeMock.Verify(runtime => runtime.InvokeVoidAsync(Constants.JsConstants.Methods.Core.AddSource.ToCoreNamespace(), It.Is<object[]>(parameters =>
                parameters[0] as string == source.Id
                && parameters[1] == null
                && parameters[2] as string == source.SourceType.ToString()
            )), Times.Once);
            _jsRuntimeMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void Should_NotAddDataSource_Async()
        {
            var dataSource = new DataSource();

            var map = new Map("id", _jsRuntimeMock.Object, _loggerMock.Object);
            await map.AddSourceAsync(dataSource);
            await Assert.ThrowsAnyAsync<SourceAlreadyExistingException>(async () => await map.AddSourceAsync(dataSource));
            _jsRuntimeMock.Verify(runtime => runtime.InvokeVoidAsync(Constants.JsConstants.Methods.Core.AddSource.ToCoreNamespace(), It.Is<object[]>(parameters =>
                parameters[0] as string == dataSource.Id
                && parameters[1] == null
                && parameters[2] as string == dataSource.SourceType.ToString()
            )), Times.Once);
            _jsRuntimeMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void Should_RemoveDataSource_Async()
        {
            var dataSource = new DataSource();
            var dataSource2 = new DataSource();
            var map = new Map("id", _jsRuntimeMock.Object, _loggerMock.Object);

            await map.AddSourceAsync(dataSource);
            await map.AddSourceAsync(dataSource2);
            await map.RemoveDataSourceAsync(dataSource);

            Assert.DoesNotContain(dataSource, map.Sources);
            Assert.Contains(dataSource2, map.Sources);

            _jsRuntimeMock.Verify(runtime => runtime.InvokeVoidAsync(Constants.JsConstants.Methods.Core.AddSource.ToCoreNamespace(), It.Is<object[]>(parameters =>
                parameters[0] as string == dataSource.Id
                && parameters[1] == null
                && parameters[2] as string == dataSource.SourceType.ToString()
            )), Times.Once);
            _jsRuntimeMock.Verify(runtime => runtime.InvokeVoidAsync(Constants.JsConstants.Methods.Core.AddSource.ToCoreNamespace(), It.Is<object[]>(parameters =>
                parameters[0] as string == dataSource2.Id
                && parameters[1] == null
                && parameters[2] as string == dataSource2.SourceType.ToString()
            )), Times.Once);
            _jsRuntimeMock.Verify(runtime => runtime.InvokeVoidAsync(Constants.JsConstants.Methods.Core.RemoveSource.ToCoreNamespace(), dataSource.Id), Times.Once);
            _jsRuntimeMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void Should_NotRemoveDataSource_Async()
        {
            var dataSource = new DataSource();
            var dataSource2 = new DataSource();
            var map = new Map("id", _jsRuntimeMock.Object, _loggerMock.Object);

            await map.AddSourceAsync(dataSource);
            await map.RemoveDataSourceAsync(dataSource2);

            Assert.DoesNotContain(dataSource2, map.Sources);
            Assert.Contains(dataSource, map.Sources);

            _jsRuntimeMock.Verify(runtime => runtime.InvokeVoidAsync(Constants.JsConstants.Methods.Core.AddSource.ToCoreNamespace(), It.Is<object[]>(parameters =>
                parameters[0] as string == dataSource.Id
                && parameters[1] == null
                && parameters[2] as string == dataSource.SourceType.ToString()
            )), Times.Once);
            _jsRuntimeMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void Should_RemoveDataSource_ViaId_Async()
        {
            var dataSource = new DataSource();
            var dataSource2 = new DataSource();
            var map = new Map("id", _jsRuntimeMock.Object, _loggerMock.Object);

            await map.AddSourceAsync(dataSource);
            await map.AddSourceAsync(dataSource2);
            await map.RemoveDataSourceAsync(dataSource.Id);

            Assert.DoesNotContain(dataSource, map.Sources);
            Assert.Contains(dataSource2, map.Sources);

            _jsRuntimeMock.Verify(runtime => runtime.InvokeVoidAsync(Constants.JsConstants.Methods.Core.AddSource.ToCoreNamespace(), It.Is<object[]>(parameters =>
                parameters[0] as string == dataSource.Id
                && parameters[1] == null
                && parameters[2] as string == dataSource.SourceType.ToString()
            )), Times.Once);
            _jsRuntimeMock.Verify(runtime => runtime.InvokeVoidAsync(Constants.JsConstants.Methods.Core.AddSource.ToCoreNamespace(), It.Is<object[]>(parameters =>
                parameters[0] as string == dataSource2.Id
                && parameters[1] == null
                && parameters[2] as string == dataSource2.SourceType.ToString()
            )), Times.Once);
            _jsRuntimeMock.Verify(runtime => runtime.InvokeVoidAsync(Constants.JsConstants.Methods.Core.RemoveSource.ToCoreNamespace(), dataSource.Id), Times.Once);
            _jsRuntimeMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void Should_NotRemoveDataSource_ViaId_Async()
        {
            var dataSource = new DataSource();
            var dataSource2 = new DataSource();
            var map = new Map("id", _jsRuntimeMock.Object, _loggerMock.Object);

            await map.AddSourceAsync(dataSource);
            await map.RemoveDataSourceAsync(dataSource2.Id);

            Assert.DoesNotContain(dataSource2, map.Sources);
            Assert.Contains(dataSource, map.Sources);

            _jsRuntimeMock.Verify(runtime => runtime.InvokeVoidAsync(Constants.JsConstants.Methods.Core.AddSource.ToCoreNamespace(), It.Is<object[]>(parameters =>
                parameters[0] as string == dataSource.Id
                && parameters[1] == null
                && parameters[2] as string == dataSource.SourceType.ToString()
            )), Times.Once);
            _jsRuntimeMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void Should_ClearMap_Async()
        {
            var assertClearMapCallback = false;
            var map = new Components.Map.Map("id", addLayerCallback: async (_, before) => { }, addPopupCallback: async _ => { }, clearMapCallback: async () => assertClearMapCallback = true);
            await map.AddSourceAsync(new DataSource());
            await map.AddLayerAsync(new BubbleLayer());
            await map.AddHtmlMarkersAsync(new HtmlMarker(null));
            await map.AddPopupAsync(new Popup(new PopupOptions()));

            await map.ClearMapAsync();
            Assert.True(assertClearMapCallback);
            Assert.Null(map.Sources);
            Assert.Null(map.Layers);
            Assert.Null(map.HtmlMarkers);
            Assert.Null(map.Popups);
        }

        [Fact]
        public async void Should_ClearLayers_Async()
        {
            var assertClearLayersCallback = false;
            var map = new Components.Map.Map("id", addLayerCallback: async (_, before) => { }, clearLayersCallback: async () => assertClearLayersCallback = true);
            await map.AddLayerAsync(new BubbleLayer());

            await map.ClearLayersAsync();
            Assert.True(assertClearLayersCallback);
            Assert.Null(map.Layers);
        }

        [Fact]
        public async void Should_ClearDataSources_Async()
        {
            var map = new Map("id", _jsRuntimeMock.Object, _loggerMock.Object);

            await map.AddSourceAsync(new DataSource());
            await map.ClearDataSourcesAsync();
            Assert.Null(map.Sources);
            _jsRuntimeMock.Verify(runtime => runtime.InvokeVoidAsync(Constants.JsConstants.Methods.Core.AddSource.ToCoreNamespace(), It.IsAny<object[]>()), Times.Once);
            _jsRuntimeMock.Verify(runtime => runtime.InvokeVoidAsync(Constants.JsConstants.Methods.Core.ClearSources.ToCoreNamespace()), Times.Once);
            _jsRuntimeMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void Should_ClearHtmlMarkers_Async()
        {
            var map = new Map("id", _jsRuntimeMock.Object, _loggerMock.Object, htmlMarkerInvokeHelper: new HtmlMarkerInvokeHelper(eventArgs => Task.CompletedTask));

            await map.AddHtmlMarkersAsync(new HtmlMarker(null));
            await map.ClearHtmlMarkersAsync();

            Assert.Null(map.HtmlMarkers);

            _jsRuntimeMock.Verify(runtime => runtime.InvokeVoidAsync(Constants.JsConstants.Methods.Core.AddHtmlMarkers.ToCoreNamespace(), It.Is<object[]>(parameters =>
                parameters[0] is IEnumerable<HtmlMarkerCreationOptions> && (parameters[0] as IEnumerable<HtmlMarkerCreationOptions>).Count() == 1
                && parameters[1] is DotNetObjectReference<HtmlMarkerInvokeHelper>
            )), Times.Once);
            _jsRuntimeMock.Verify(runtime => runtime.InvokeVoidAsync(Constants.JsConstants.Methods.Core.ClearHtmlMarkers.ToCoreNamespace()), Times.Once);
            _jsRuntimeMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void Should_AddPopup_Async()
        {
            var assertAddPopup = false;
            var popup = new Popup(new PopupOptions());
            var map = new Components.Map.Map("id", addPopupCallback: async popupCallback => assertAddPopup = popupCallback == popup);
            await map.AddPopupAsync(popup);
            Assert.True(assertAddPopup);
            Assert.Contains(popup, map.Popups);
        }

        [Fact]
        public async void Should_NotAddTwiceTheSamePopup_Async()
        {
            var popup = new Popup(new PopupOptions());
            var map = new Components.Map.Map("id", addPopupCallback: async _ => { });
            await map.AddPopupAsync(popup);
            await Assert.ThrowsAnyAsync<PopupAlreadyExistingException>(async () => await map.AddPopupAsync(popup));
        }

        [Fact]
        public async void Should_RemovePopupFromPopupsCollection_Async()
        {
            var popup = new Popup(new PopupOptions());
            var map = new Components.Map.Map("id", addPopupCallback: async _ => { });
            await map.AddPopupAsync(popup);
            map.RemovePopup(popup.Id);

            Assert.DoesNotContain(popup, map.Popups);
        }

        [Fact]
        public async void Should_RemovePopup_Async()
        {
            var assertRemoveCallback = false;
            var popup = new Popup(new PopupOptions());
            var map = new Components.Map.Map("id", addPopupCallback: async _ => { }, removePopupCallback: async popupId => assertRemoveCallback = popupId == popup.Id);
            await map.AddPopupAsync(popup);
            await map.RemovePopupAsync(popup);

            Assert.True(assertRemoveCallback);
            Assert.DoesNotContain(popup, map.Popups);
        }

        [Fact]
        public async void Should_RemovePopup_IdVersion_Async()
        {
            var assertRemoveCallback = false;
            var popup = new Popup(new PopupOptions());
            var map = new Components.Map.Map("id", addPopupCallback: async _ => { }, removePopupCallback: async popupId => assertRemoveCallback = popupId == popup.Id);
            await map.AddPopupAsync(popup);
            await map.RemovePopupAsync(popup.Id);

            Assert.True(assertRemoveCallback);
            Assert.DoesNotContain(popup, map.Popups);
        }

        [Fact]
        public async void Should_NotRemovePopup_Async()
        {
            var assertRemoveCallback = false;
            var popup = new Popup(new PopupOptions());
            var map = new Components.Map.Map("id", addPopupCallback: async _ => { }, removePopupCallback: async popupId => assertRemoveCallback = true);
            await map.AddPopupAsync(popup);
            await map.RemovePopupAsync(new Popup(new PopupOptions()));

            Assert.False(assertRemoveCallback);
        }

        [Fact]
        public async void Should_ClearPopups_Async()
        {
            var assertClearCallback = false;
            var popup = new Popup(new PopupOptions());
            var map = new Components.Map.Map("id", addPopupCallback: async _ => { }, clearPopupsCallback: async () => assertClearCallback = true);
            await map.AddPopupAsync(popup);
            await map.ClearPopupsAsync();

            Assert.True(assertClearCallback);
            Assert.Null(map.Popups);
        }

        [Fact]
        public async void Should_UpdateCameraOptions_Async()
        {
            var assertOptionsCallback = false;
            var center = new Position(10, 10);
            var initialCameraOptions = new CameraOptions {
                Duration = 10
            };
            var map = new Map("id", setCameraCallback: async options => assertOptionsCallback = options.Center == center && options.Duration == initialCameraOptions.Duration) {
                CameraOptions = initialCameraOptions
            };

            await map.SetCameraOptionsAsync(options => options.Center = center);
            Assert.True(assertOptionsCallback);
        }

        [Fact]
        public async void Should_UpdateCameraOptions_NoCameraOptionsDefined_Async()
        {
            var assertOptionsCallback = false;
            var center = new Position(10, 10);
            var map = new Map("id", setCameraCallback: async options => assertOptionsCallback = options.Center == center);

            await map.SetCameraOptionsAsync(options => options.Center = center);
            Assert.True(assertOptionsCallback);
        }

        [Fact]
        public async void Should_UpdateStyleOptions_Async()
        {
            var assertOptionsCallback = false;
            var language = "fr";
            var initialStyleOptions = new StyleOptions {
                AutoResize = true
            };
            var map = new Map("id", setStyleCallback: async options => assertOptionsCallback = options.AutoResize == initialStyleOptions.AutoResize && options.Language == language) {
                StyleOptions = initialStyleOptions
            };

            await map.SetStyleOptionsAsync(options => options.Language = language);
            Assert.True(assertOptionsCallback);
        }

        [Fact]
        public async void Should_UpdateStyleOptions_NoStyleOptionsDefined_Async()
        {
            var assertOptionsCallback = false;
            var language = "fr";
            var map = new Map("id", setStyleCallback: async options => assertOptionsCallback = options.Language == language);

            await map.SetStyleOptionsAsync(options => options.Language = language);
            Assert.True(assertOptionsCallback);
        }

        [Fact]
        public async void Should_UpdateUserInteraction_Async()
        {
            var assertOptionsCallback = false;
            var initialUserInteractionOptions = new UserInteractionOptions {
                BoxZoomInteraction = true
            };
            var dblClickZoomInteraction = true;
            var map = new Map("id", setUserInteractionCallback: async options => assertOptionsCallback = options.BoxZoomInteraction == initialUserInteractionOptions.BoxZoomInteraction && options.DblclickZoomInteraction == dblClickZoomInteraction) {
                UserInteractionOptions = initialUserInteractionOptions
            };

            await map.SetUserInteractionAsync(options => options.DblclickZoomInteraction = dblClickZoomInteraction);
            Assert.True(assertOptionsCallback);
        }

        [Fact]
        public async void Should_UpdateUserInteraction_NoUserInteractionDefined_Async()
        {
            var assertOptionsCallback = false;
            var dblClickZoomInteraction = true;
            var map = new Map("id", setUserInteractionCallback: async options => assertOptionsCallback = options.DblclickZoomInteraction == dblClickZoomInteraction);

            await map.SetUserInteractionAsync(options => options.DblclickZoomInteraction = dblClickZoomInteraction);
            Assert.True(assertOptionsCallback);
        }

        [Fact]
        public async void Should_UpdateTrafficInteraction_Async()
        {
            var assertOptionsCallback = false;
            var initialTrafficOptions = new TrafficOptions {
                Flow = TrafficFlow.Absolute
            };
            var incidents = true;
            var map = new Map("id", setTrafficOptions: async options => assertOptionsCallback = options.Flow == initialTrafficOptions.Flow && options.Incidents == incidents) {
                TrafficOptions = initialTrafficOptions
            };

            await map.SetTrafficOptionsAsync(options => options.Incidents = incidents);
            Assert.True(assertOptionsCallback);
        }

        [Fact]
        public async void Should_UpdateTrafficInteraction_NoTrafficOptionsDefined_Async()
        {
            var assertOptionsCallback = false;
            var incidents = true;
            var map = new Map("id", setTrafficOptions: async options => assertOptionsCallback = options.Incidents == incidents);

            await map.SetTrafficOptionsAsync(options => options.Incidents = incidents);
            Assert.True(assertOptionsCallback);
        }

        [Fact]
        public void Should_DispatchBoxZoomEndEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "boxzoomend" };
            map.OnBoxZoomEnd += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchBoxZoomStartEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "boxzoomstart" };
            map.OnBoxZoomStart += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchClickEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "click" };
            map.OnClick += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchContextMenuEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "contextmenu" };
            map.OnContextMenu += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchDataEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "data" };
            map.OnData += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchDragEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "drag" };
            map.OnDrag += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchDragEndEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "dragend" };
            map.OnDragEnd += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchDragStartEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "dragstart" };
            map.OnDragStart += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchErrorEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "error" };
            map.OnError += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchIdleEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "idle" };
            map.OnIdle += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchLayerAddedEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "layeradded" };
            map.OnLayerAdded += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchLayerRemovedEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "layerremoved" };
            map.OnLayerRemoved += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchLoadEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "load" };
            map.OnLoad += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchMouseDownEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "mousedown" };
            map.OnMouseDown += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchMouseMoveEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "mousemove" };
            map.OnMouseMove += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
        }

        [Fact]
        public void Should_DispatchMouseOutEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "mouseout" };
            map.OnMouseOut += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchMouseOverEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "mouseover" };
            map.OnMouseOver += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchMouseUpEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "mouseup" };
            map.OnMouseUp += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchMoveEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "move" };
            map.OnMove += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchMoveEndEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "moveend" };
            map.OnMoveEnd += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchMoveStartEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "movestart" };
            map.OnMoveStart += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchPitchEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "pitch" };
            map.OnPitch += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchPitchEndEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "pitchend" };
            map.OnPitchEnd += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchPitchStartEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "pitchstart" };
            map.OnPitchStart += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchReadyEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "ready" };
            map.OnReady += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchRenderEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "render" };
            map.OnRender += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchResizeEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "resize" };
            map.OnResize += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchRotateEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "rotate" };
            map.OnRotate += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchRotateEndEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "rotateend" };
            map.OnRotateEnd += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchRotateStartEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "rotatestart" };
            map.OnRotateStart += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchSourceAddedEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "sourceadded" };
            map.OnSourceAdded += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchSourceDataEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "sourcedata" };
            map.OnSourceData += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchSourceRemovedEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "sourceremoved" };
            map.OnSourceRemoved += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchStyleDataEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "styledata" };
            map.OnStyleData += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchStyleImageMissingEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "styleimagemissing" };
            map.OnStyleImageMissing += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchTokenAcquiredEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "tokenacquired" };
            map.OnTokenAcquired += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchTouchCancelEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "touchcancel" };
            map.OnTouchCancel += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchTouchEndEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "touchend" };
            map.OnTouchEnd += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchTouchMoveEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "touchmove" };
            map.OnTouchMove += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchTouchStartEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "touchstart" };
            map.OnTouchStart += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchWheelEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "wheel" };
            map.OnWheel += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchZoomEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "zoom" };
            map.OnZoom += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchZoomEndEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "zoomend" };
            map.OnZoomEnd += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchZoomStartEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new MapJsEventArgs { Type = "zoomstart" };
            map.OnZoomStart += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchDrawingChangedEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new DrawingToolbarJsEventArgs { Type = "drawingchanged" };
            map.OnDrawingChanged += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchDrawingToolbarEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchDrawingChangingEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new DrawingToolbarJsEventArgs { Type = "drawingchanging" };
            map.OnDrawingChanging += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchDrawingToolbarEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchDrawingCompleteEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new DrawingToolbarJsEventArgs { Type = "drawingcomplete" };
            map.OnDrawingComplete += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchDrawingToolbarEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchDrawingModeChangedEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new DrawingToolbarJsEventArgs { Type = "drawingmodechanged" };
            map.OnDrawingModeChanged += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchDrawingToolbarEvent(jsEventArgs);
            Assert.True(assertEvent);
        }

        [Fact]
        public void Should_DispatchDrawingStartedEvent()
        {
            var assertEvent = false;
            var map = new Map("id");
            var jsEventArgs = new DrawingToolbarJsEventArgs { Type = "drawingstarted" };
            map.OnDrawingStarted += eventArgs => assertEvent = eventArgs.Map == map && eventArgs.Type == jsEventArgs.Type;
            map.DispatchDrawingToolbarEvent(jsEventArgs);
            Assert.True(assertEvent);
        }
    }
}

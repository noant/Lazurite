using AVerCap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MediaHost.AverMedia.Wrapper
{
    public class LocalStreamer
    {
        public uint DeviceId { get; }

        private readonly IntPtr _captureObject;

        public LocalStreamer(uint deviceId, IntPtr controlHandle, VIDEOSOURCE videoSource, CAPTURETYPE captureType, uint imageWidth, uint imageHeight, VIDEORESOLUTION resolution, VIDEOFORMAT format = VIDEOFORMAT.VIDEOFORMAT_NTSC)
        {
            DeviceId = deviceId;

            AVerCapAPI.AVerCreateCaptureObjectEx(DeviceId, (uint)captureType, controlHandle, ref _captureObject);

            var videoResolution = new VIDEO_RESOLUTION()
            {
                dwVideoResolution = (uint)resolution,
                dwVersion = 1
            };
            
            AVerCapAPI.AVerSetVideoFormat(_captureObject, (uint)format);
            AVerCapAPI.AVerSetVideoSource(_captureObject, (uint)videoSource);
            AVerCapAPI.AVerSetVideoResolutionEx(_captureObject, ref videoResolution);
            AVerCapAPI.AVerSetVideoRenderer(_captureObject, (uint)VIDEORENDERER.VIDEORENDERER_VMR9);
            AVerCapAPI.AVerSetVideoPreviewEnabled(_captureObject, 1);
            AVerCapAPI.AVerSetVideoInputFrameRate(_captureObject, 5900);
            AVerCapAPI.AVerSetVideoOutputFrameRate(_captureObject, 5900);
            AVerCapAPI.AVerSetVideoEnhanceMode(_captureObject, (uint)VIDEOENHANCE.VIDEOENHANCE_NONE);
            AVerCapAPI.AVerSetMaintainAspectRatioEnabled(_captureObject, 1);
            
            var rectClient = new RECT
            {
                Left = 0,
                Top = 0,
                Right = (int)imageWidth,
                Bottom = (int)imageHeight
            };

            AVerCapAPI.AVerSetVideoWindowPosition(_captureObject, rectClient);
        }

        public bool StartStreaming()
        {
            return AVerCapAPI.AVerStartStreaming(_captureObject) != (int)ERRORCODE.CAP_EC_SUCCESS;
        }

        public void StopStreaming()
        {
            AVerCapAPI.AVerStopStreaming(_captureObject);
            AVerCapAPI.AVerDeleteCaptureObject(_captureObject);
        }
    }
}

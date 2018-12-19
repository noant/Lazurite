
/////////////////////////////////////////////////////////////////////////////
// (c) Copyright AVerMedia Technologies, Inc. 2011. All Rights Reserved.
//
//  Module:
//
//    AVerCapAPI.cs
//
//  Date:
//
//	  12/01/2014
//
//  Abstract:
//
//    Header file for AVerCapAPI.dll
//
/////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace AVerCap
{
#if _X64_MACHINE
    using LONGPTR = System.Int64;
#else
    using LONGPTR = System.Int32;
#endif

    public enum DEMOSTATE
    {
        DEMO_STATE_STOP = 0,
        DEMO_STATE_PREVIEW = 1,
        DEMO_STATE_CAP_IMAGE = 2,
        DEMO_STATE_RECORD = 4,
        DEMO_STATE_CALLBACK_VIDEO = 8,
        DEMO_STATE_CALLBACK_AUDIO = 16,
        DEMO_STATE_CALLBACK_ESTS = 32
    }
    // error code
    public enum ERRORCODE
    {
        CAP_EC_SUCCESS = 0,
        CAP_EC_INIT_DEVICE_FAILED = -1,
        CAP_EC_DEVICE_IN_USE = -2,
        CAP_EC_NOT_SUPPORTED = -3,
        CAP_EC_INVALID_PARAM = -4,
        CAP_EC_TIMEOUT = -5,
        CAP_EC_NOT_ENOUGH_MEMORY = -6,
        CAP_EC_UNKNOWN_ERROR = -7,
        CAP_EC_ERROR_STATE = -8,
        CAP_EC_HDCP_PROTECTED_CONTENT = -9
    }

    //CARDTYPE
    public enum CARDTYPE
    {
        CARDTYPE_NULL = 0,
        CARDTYPE_C727 = 1,
        CARDTYPE_C729 = 2,
        CARDTYPE_CD331 = 21,
        CARDTYPE_C129 = 11,
        CARDTYPE_V1A8C = 10,
        CARDTYPE_V1A8D = 6,
        CARDTYPE_C039P = 7,
        CARDTYPE_C725A = 8,
        CARDTYPE_C725B = 9,
        CARDTYPE_CE310B = 25, 
        CARDTYPE_C968 = 12,
        CARDTYPE_C351 = 14,
        CARDTYPE_C351_PAL_SQ = 16,
        CARDTYPE_C199 = 3,  
        CARDTYPE_C199X = 4, 
        CARDTYPE_CD910 = 15,
        CARDTYPE_CD110 = 19,
        CARDTYPE_CD750 = 20,
        CARDTYPE_CU511B = 28,
        CARDTYPE_CE511B_4K = 29,
        CARDTYPE_CE511B_HD = 32,
        CARDTYPE_CE511B_SD = 33,
        CARDTYPE_CE513B = 31,
        CARDTYPE_CD530 = 22,
        CARDTYPE_CD311 = 23,
        CARDTYPE_CD311B = 24,
        CARDTYPE_C353 = 13,
        CARDTYPE_CM313B = 26,
        CARDTYPE_CE330B = 27,
        CARDTYPE_CD910_DUO = 17,
        CARDTYPE_CD910_QUAD = 18,
		CARDTYPE_CU331_HN = 34,
        CARDTYPE_CL334_SN = 36,
    }

    //device type
    public enum CAPTURETYPE
    {
        CAPTURETYPE_SD = 1,
        CAPTURETYPE_HD = 2,
        CAPTURETYPE_ALL = 3
    }

    // video input source
    public enum VIDEOSOURCE
    {
        COMPOSITE = 0,
        SVIDEO = 1,
        COMPONENT = 2,
        HDMI = 3,
        VGA = 4,
        SDI = 5,
        ASI = 6,
        DVI = 7
    }

    // audio input source
    public enum AUDIOSOURCE:uint
    {
        AUDIOSOURCE_NONE = 0,
        AUDIOSOURCE_COMPOSITE = 0x00000001,
        AUDIOSOURCE_SVIDEO = 0x00000002,
        AUDIOSOURCE_COMPONENT = 0x00000004,
        AUDIOSOURCE_HDMI = 0x00000008,
        AUDIOSOURCE_VGA = 0x00000010,
        AUDIOSOURCE_SDI = 0x00000020,
        AUDIOSOURCE_ASI = 0x00000040,
        AUDIOSOURCE_DVI = 0x00000080,
        AUDIOSOURCE_DEFAULT = uint.MaxValue
    }

    // video format
    public enum VIDEOFORMAT:uint
    {
        VIDEOFORMAT_NTSC = 0,
        VIDEOFORMAT_PAL = 1
    }

    // video resolution
    public enum VIDEORESOLUTION:uint
    {
        _640X480 = 0,
        _704X576 = 1,
        _720X480 = 2,
        _720X576 = 4,
        _1920X1080 = 7,
        _160X120 = 20,
        _176X144 = 21,
        _240X176 = 22,
        _240X180 = 23,
        _320X240 = 24,
        _352X240 = 25,
        _352X288 = 26,
        _640X240 = 27,
        _640X288 = 28,
        _720X240 = 29,
        _720X288 = 30,
        _80X60 = 31,
        _88X72 = 32,
        _128X96 = 33,
        _640X576 = 34,
        _180X120 = 37,
        _180X144 = 38,
        _360X240 = 39,
        _360X288 = 40,
        _768X576 = 41,
        _384x288 = 42,
        _192x144 = 43,
        _1280X720 = 44,
        _1024X768 = 45,
        _1280X800 = 46,
        _1280X1024 = 47,
        _1440X900 = 48,
        _1600X1200 = 49,
        _1680X1050 = 50,
        _800X600 = 51,
        _1280X768 = 52,
        _1360X768 = 53,
        _1152X864 = 54,
        _1280X960 = 55,

        _702X576 = 56,
        _720X400 = 57,
        _1152X900 = 58,
        _1360X1024 = 59,
        _1366X768 = 60,
        _1400X1050 = 61,
        _1440X480 = 62,
        _1440X576 = 63,
        _1600X900 = 64,
        _1920X1200 = 65,
        _1440X1080 = 66,
        _1600X1024 = 67,
        _3840X2160 = 68,
        _1152X768 = 69,
        _176X120 = 70,
        _704X480 = 71,
        _1792X1344 = 72,
        _1856X1392 = 73,
        _1920X1440 = 74,
        _2048X1152 = 75,
        _2560X1080 = 76,
        _2560X1440 = 77,
        _2560X1600 = 78,
        _4096X2160 = 79,
    }

    // video adjustment property
    public enum VIDEOPROCAMPPROPERTY
    {
        VIDEOPROCAMPPROPERTY_BRIGHTNESS = 0,
        VIDEOPROCAMPPROPERTY_CONTRAST = 1,
        VIDEOPROCAMPPROPERTY_HUE = 2,
        VIDEOPROCAMPPROPERTY_SATURATION = 3
    }

    // deinterlace mode
    public enum DEINTERLACEMODE
    {
        DEINTERLACE_NONE = 0,
        DEINTERLACE_WEAVE = 1,
        DEINTERLACE_BOB = 2,
        DEINTERLACE_BLEND = 3
    }

    // downscale mode
    public enum DOWNSCALEMODE
    {
        DSMODE_NONE = 0,
        DSMODE_HALFHEIGHT = 1,
        DSMODE_HALFWIDTH = 2,
        DSMODE_HALFBOTH = 3,
        DSMODE_CUSTOM = 5
    }

    // overlay settings
    public enum OVERLAYSETTINGS
    {
        OVERLAY_TEXT = 0,
        OVERLAY_TIME = 1,
        OVERLAY_IMAGE = 2
    }

    public enum FONTSIZE
    {
        FONTSIZE_SMALL = 0,
        FONTSIZE_BIG = 1
    }

    public enum TIMEFORMAT
    {
        FORMAT_TIMEONLY = 0,
        FORMAT_DATEANDTIME = 1
    }

    public enum ALIGNMENT
    {
        ALIGNMENT_LEFT = 0,
        ALIGNMENT_CENTER = 1,
        ALIGNMENT_RIGHT = 2
    }

    // video frame/field capture settings
    public enum CT_SEQUENCE
    {
        CT_SEQUENCE_FIELD = 0,
        CT_SEQUENCE_FRAME = 1
    }

    public enum SAVETYPE
    {
        ST_BMP = 0,
        ST_JPG = 1,
        ST_AVI = 2,
        ST_CALLBACK = 3,
        ST_WAV = 4,
        ST_WMV = 5,
        ST_PNG = 6,
        ST_MPG = 7,
        ST_MP4 = 8,
        ST_TIFF = 9,
        ST_CALLBACK_RGB24 = 10,
        ST_CALLBACK_ARGB = 11,
    }
  
    public enum RCMODE
    {
        RCMODE_CBR = 0x01, //固定码率模式
        RCMODE_VBR = 0x02, //可变码率模式
        RCMODE_ABR = 0x03, //平均码率模式
        RCMODE_STRICT_ABR = 0x04,//严格平均码率模式
    }
    public enum VIDEOENHANCE
    {
        VIDEOENHANCE_NONE = 0,
        VIDEOENHANCE_NORMAL = 1,
        VIDEOENHANCE_SPLIT = 2,
        VIDEOENHANCE_COMPARE = 3
    }

    public enum VIDEOMIRROR
    {
        VIDEOMIRROR_NONE = 0,
        VIDEOMIRROR_HORIZONTAL = 1,
        VIDEOMIRROR_VERTICAL = 2,
        VIDEOMIRROR_BOTH = 3
    }

    public enum DURATIONMODE
    {
        DURATION_TIME = 1,
        DURATION_COUNT = 2
    }

    public enum AUDIOBITRATE
    {
        AUDIOBITRATE_96 = 0,
        AUDIOBITRATE_112 = 1,
        AUDIOBITRATE_128 = 2,
        AUDIOBITRATE_144 = 3,
        AUDIOBITRATE_160 = 4,
        AUDIOBITRATE_176 = 5,
        AUDIOBITRATE_192 = 6,
        AUDIOBITRATE_224 = 7,
        AUDIOBITRATE_256 = 8,
        AUDIOBITRATE_288 = 9,
        AUDIOBITRATE_320 = 10,
        AUDIOBITRATE_352 = 11,
        AUDIOBITRATE_384 = 12,
        AUDIOBITRATE_64 = 13,
        AUDIOBITRATE_32 = 14
    }

    public enum VIDEOROTATE
    {
        VIDEOROTATE_NONE = 0,
        VIDEOROTATE_CW90 = 1,
        VIDEOROTATE_CCW90 = 2
    }

    public enum ENCODERTYPE
    {
        ENCODERTYPE_MPEGAUDIO = 0x00000001,
        ENCODERTYPE_H264 = 0x00000002,
        ENCODERTYPE_MPEG2AAC = 0x00000004
    }

    //Capture type
    public enum CT
    {
        CT_CALLBACK_MPEGAUDIO = 0x00000001,
        CT_CALLBACK_H264 = 0x00000002,
        CT_CALLBACK_TS = 0x00000004,
        CT_FILE_TS = 0x01000000,
        CT_FILE_MP4 = 0x02000000,
        CT_CALLBACK_MPEG2AAC = 0x00000008
    }

    //Sample type
    public enum SAMPLETYPE
    {
        SAMPLETYPE_NULL = 0,
        SAMPLETYPE_RAWVIDEO = 0x01,
        SAMPLETYPE_PCMAUDIO = 0x02,
        SAMPLETYPE_TS = 0x10,
        SAMPLETYPE_ES_H264 = 0x20,
        SAMPLETYPE_ES_MPEG4AAC = 0x30,
        SAMPLETYPE_ES_MPEGAUDIO = 0x31,
        SAMPLETYPE_ES_MPEG2AAC = 0x32,
    }

    // video renderer
    public enum VIDEORENDERER
    {
        VIDEORENDERER_DEFAULT = 0,//VMR9
        VIDEORENDERER_VMR7 = 1,
        VIDEORENDERER_VMR9 = 2,
        VIDEORENDERER_EVR = 3//vista, win7, server 2008
    }

    public enum IMAGEQUALITY
    {
        IMAGEQUALITY_BEST = 0,
        IMAGEQUALITY_NORMAL = 1,
        IMAGEQUALITY_LOW = 2
    }

    public enum HWVA
    {
        HWVA_ENCODER_H264 = 0x01
    }

    public enum IMAGETYPE
    {
        IMAGETYPE_BMP = 1,
        IMAGETYPE_JPG = 2,
        IMAGETYPE_PNG = 3,
        IMAGETYPE_TIFF = 4
    }

    public enum COPPERRCODE:uint
    {
        COPP_ERR_UNKNOWN = 0x80000001,
        COPP_ERR_NO_COPP_HW = 0x80000002,
        COPP_ERR_NO_MONITORS_CORRESPOND_TO_DISPLAY_DEVICE = 0x80000003,
        COPP_ERR_CERTIFICATE_CHAIN_FAILED = 0x80000004,
        COPP_ERR_STATUS_LINK_LOST = 0x80000005,
        COPP_ERR_NO_HDCP_PROTECTION_TYPE = 0x80000006,
        COPP_ERR_HDCP_REPEATER = 0x80000007,
        COPP_ERR_HDCP_PROTECTED_CONTENT = 0x80000008,
        COPP_ERR_GET_CRL_FAILED = 0x80000009
    }

    public enum RECORD_STATUS:uint
    {
        STATUS_RESUME = 0,
        STATUS_PAUSE = 1
    }
   
    public struct OVERLAY_POSITION
    {
        public uint dwXPos;
        public uint dwYPos;
        public uint dwAlignment;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct OVERLAY_IMAGE_INFO
    {
        public string lpFileName;
        public uint dwImageType;//obsolete
    }

    public struct OVERLAY_INFO
    {
        public int bEnableOverlay;
        public uint dwFontSize;
        public uint dwFontColor;
        public uint dwTransparency;
        public OVERLAY_POSITION WindowPosition;
    }

    public struct VIDEO_SAMPLE_INFO
    {
        public uint dwWidth;
        public uint dwHeight;
        public uint dwStride;
        public uint dwPixelFormat;
    }

	[StructLayout(LayoutKind.Sequential)]
    public struct AUDIO_PRCS_FORMAT
    {
	    int  bUseOriginal;
	    uint dwChannels;
        uint dwBitsPerSample;
        uint dwSamplingRate;
        uint cbExtraData;
        IntPtr pbExtraData;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AUDIO_PRCS_DATA
    {
        int		nLineID;
        uint	dwDataSize;
        IntPtr	pbData;
        int	    bFixVideoLatency;
        long	llRefTime;
        long	llStartTime;
        long	llVideoLatency;
        IntPtr  pNewFormat;//AUDIO_PRCS_FORMAT
    }
	
    public delegate int VIDEOCAPTURECALLBACK(VIDEO_SAMPLE_INFO VideoInfo, IntPtr pbData, int lLength, long tRefTime, LONGPTR lUserData);

    public delegate int AUDIOCAPTURECALLBACK(AUDIO_PRCS_DATA apData, LONGPTR lpUserData);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct VIDEO_CAPTURE_INFO
    {
        public uint dwCaptureType;
        public uint dwSaveType;
        public int bOverlayMix;
        public uint dwDurationMode;
        public uint dwDuration;
        public string lpFileName;
        public VIDEOCAPTURECALLBACK lpCallback;
        public LONGPTR lCallbackUserData;
    }

    // audio sample capture settings
    public struct AUDIO_SAMPLE_INFO
    {
        public uint dwChannels;
        public uint dwBitsPerSample;
        public uint dwSamplingRate;
    }

   

    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct VIDEO_COMPRESSOR_INFO
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public char[] szName;
        public int bPropertyPageSupported;
        public int bQualitySupported;
        public int bPropertyPageVisabled;
        public uint dwQuality;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AUDIO_SAMPLE_FORMAT
    {
        public uint dwChannels;
        public uint dwBitsPerSample;
        public uint dwSamplingRate;
        public uint dwAvgBytesPerSec;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct AUDIO_COMPRESSOR_INFO
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public char[] szName;
        public AUDIO_SAMPLE_FORMAT AudioSampleFormat;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct AUDIO_CAPTURE_INFO
    {
        public uint  dwSaveType;
        public string lpFileName;
        public AUDIOCAPTURECALLBACK lpCallback;
        public LONGPTR lCallbackUserData;
        public int   lReserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BITMAPFILEHEADER
    {
        public ushort bfType;
        public uint   bfSize;
        public ushort bfReserved1;
        public ushort bfReserved2;
        public uint   bfOffBits;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BITMAPINFOHEADER
    {
        public uint biSize;
        public int biWidth;
        public int biHeight;
        public ushort biPlanes;
        public ushort biBitCount;
        public uint biCompression;
        public uint biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public uint biClrUsed;
        public uint biClrImportant;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VIDEO_STREAM_INFO
    {
        public int bEnableMix;
        public uint dwWidth;
        public uint dwHeight;
        public uint dwPixelFormat;
        public RECT rcMixPosition;
        public uint dwTransparency;
        public uint dwReserved1;
        public uint dwReserved2;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MPEG2_VIDEOENCODER_INFO
    {
        public uint dwVersion;
        public uint dwBitrate;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MPEG2_AUDIOENCODER_INFO
    {
        public uint dwVersion;
        public uint dwBitrate;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MPEG4_VIDEOENCODER_INFO
    {
        public uint dwVersion;//must set to 2
        public uint dwBitrate;
        public uint dwGOPLength;
        public int  bHardwareAcceleration;
        public uint dwQuality;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MPEG4_AUDIOENCODER_INFO
    {
        public uint dwVersion;
        public uint dwBitrate;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct INPUT_VIDEO_INFO
    {
        public uint dwVersion;
        public uint dwWidth;
        public uint dwHeight;
        public int  bProgressive;
        public uint dwFormat;
        public uint dwFramerate;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HW_VIDEOENCODER_INFO
    {
        public uint dwVersion;
        public uint dwEncoderType;
        public uint dwRcMode;
        public uint dwBitrate;
        public uint dwMinBitrate;
        public uint dwMaxBitrate;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HW_AUDIOENCODER_INFO
    {
        public uint dwVersion;
        public uint dwEncoderType;
        public uint dwBitrate;
        public uint dwSamplingRate;  //Must be set to 0 (default)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SAMPLE_INFO
    {
        public uint dwSampleType;
        public IntPtr lpSampleInfo;
    }

    public delegate int CAPTURECALLBACK(SAMPLE_INFO SampleInfo, IntPtr pbData, int lLength, long tRefTime, IntPtr lpUserData);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct HW_STREAM_CAPTURE_INFO
    {
        public uint dwVersion;
        public uint dwCaptureType;
        public CAPTURECALLBACK lpMainCallback;
        public CAPTURECALLBACK lpSecondaryCallback;
        public string lpFileName;
        public IntPtr lpMainCallbackUserData;
        public IntPtr lpSecondaryCallbackUserData;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct TS_STREAM_CAPTURE_INFO
    {
        public uint dwVersion;
        public string lpFileName;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct TS_STREAM_CALLBACK_INFO
    {
        public uint dwVersion;
        public CAPTURECALLBACK lpCallback;
        public IntPtr lpCallbackUserData;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VIDEO_RESOLUTION
    {
        public uint dwVersion;//must set to 1
        public uint dwVideoResolution;//Index
        public int  bCustom;//是否自定义
        public uint dwWidth;
        public uint dwHeight;
    }

    //third audio capture
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct AUDIOCAPTURESOURCE_INFO
    {
        public uint dwVersion;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public char[] szName;
        public uint dwIndex;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct AUDIOCAPTURESOURCE_INPUTTYPE_INFO
    {
        public uint dwVersion;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public char[] szName;
        public uint dwIndex;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct AUDIOCAPTURESOURCE_FORMAT_INFO
    {
        public uint dwVersion;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public char[] szName;
        public uint dwIndex;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AUDIOCAPTURESOURCE_SETTING
    {
        public uint dwVersion;
        public uint dwCapSourceIndex;
        public uint dwInputTypeIndex;
        public uint dwFormatIndex;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct OVERLAY_CONTENT_INFO
    {
        public uint dwVersion;
        public uint dwContentType;
        public IntPtr lpContent;
        public uint dwDuration;
        public uint dwID;
        public uint dwPriority;
        public OVERLAY_INFO OverlayInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct OVERLAY_DC_INFO
    {
        public uint dwVersion;
        public int bClear;
        public IntPtr hDC;
        public uint dwDCWidth;
        public uint dwDCHeight;
        public uint dwBkColor;
        public uint dwBkTransparency;
        public OVERLAY_POSITION WindowPosition;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct CAPTUREIMAGE_NOTIFY_INFO
    {
        public uint dwVersion;
        public uint dwImageType;
        public int bFinished;
        public uint dwImageIndex;
        public string lpFileName;
    }

    public enum CAPTUREEVENT
    {
        EVENT_CAPTUREIMAGE = 1,
        EVENT_CHECKCOPP = 2
    }

    public delegate int NOTIFYEVENTCALLBACK(uint dwEventCode, IntPtr lpEventData, IntPtr lpUserData);

    public delegate int MotionDetectCallback(uint dwID, long tRefTime, int bIsMotion, IntPtr lpUserData);

    [StructLayout(LayoutKind.Sequential)]
    public struct MOTION_DETECT_INFO
    {
        public uint dwVersion;
        public uint dwDetectID;
        public int bEnableDetect;
        public uint dwRefFrameDistance;
        public uint dwNoMotionTransitionTime;
        public uint dwMotionBlockWidth;
        public uint dwMotionBlockHeight;
        public IntPtr pwMotionSensitivityMap;
        public uint dwMapSize;
        public MotionDetectCallback lpNotifyMotion;
        public IntPtr lpCallbackUserData;
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct CAPTURE_IMAGE_INFO
    {
        public uint dwVersion;
        public uint dwImageType;
        public uint dwCaptureType;
        public int bOverlayMix;
        public uint dwDurationMode;
        public uint dwDuration;
        public uint dwCapNumPerSec;
        public RECT rcCapRect;
        public string lpFileName;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct INPUT_AUDIO_INFO
    {
        public uint dwVersion;//must set to 1
        public uint dwSamplingRate;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct RESOLUTION_RANGE_INFO
    {
        public uint dwVersion;
        public int bRange;
        public uint dwWidthMin;
        public uint dwWidthMax;
        public uint dwHeightMin;
        public uint dwHeightMax;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct FRAMERATE_RANGE_INFO
    {
        public uint dwVersion;
        public int bRange;
        public uint dwFramerateMin;
        public uint dwFramerateMax;
    }
   
    static class AVerCapAPI
    {
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetDeviceNum(ref uint pDeviceNum);
        [DllImport("AVerCapAPI.dll", CharSet = CharSet.Unicode)]
        public static extern int AVerGetDeviceName(uint nDeviceIndex, StringBuilder szDeviceName);
        [DllImport("AVerCapAPI.dll", CharSet = CharSet.Ansi)]
        public static extern int AVerGetDeviceSerialNum(uint dwDeviceIndex, StringBuilder pbySerialNum);

        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerCreateCaptureObject(uint nDeviceIndex, IntPtr hWnd, ref IntPtr phCaptureObject);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerCreateCaptureObjectEx(uint dwDeviceIndex, uint dwType, IntPtr hWnd, ref IntPtr phCaptureObject);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerDeleteCaptureObject(IntPtr hCaptureObject);

        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerSetVideoWindowMultiple(IntPtr hCaptureObject, /*ref*/ IntPtr[] phWndArray, uint dwWndNum);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerSetVideoWindowPositionMultiple(IntPtr hCaptureObject, uint dwWndIndex, RECT rectVideoWnd);

        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerSetVideoWindowPosition(IntPtr hCaptureObject, RECT rectVideoWnd);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerRepaintVideo(IntPtr hCaptureObject);

        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerSetVideoRenderer(IntPtr hCaptureObject, uint dwVideoRenderer);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetVideoRenderer(IntPtr hCaptureObject, ref uint pdwVideoRenderer);

        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerSetMaintainAspectRatioEnabled(IntPtr hCaptureObject, int bEnabled);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetMaintainAspectRatioEnabled(IntPtr hCaptureObject, ref int pbEnabled);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetAspectRatio(IntPtr hCaptureObject, ref uint pdwAspectRatioX, ref uint pdwAspectRatioY);

        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerSetVideoSource(IntPtr hCaptureObject, uint dwVideoSource);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetVideoSource(IntPtr hCaptureObject, ref uint pdwVideoSource);

        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetAudioSourceSupported(IntPtr hCaptureObject, ref uint pdwSupported);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerSetAudioSource(IntPtr hCaptureObject, uint dwAudioSource);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetAudioSource(IntPtr hCaptureObject, ref uint pdwAudioSource);

        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerSetVideoFormat(IntPtr hCaptureObject, uint dwVideoFormat);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetVideoFormat(IntPtr hCaptureObject, ref uint pdwVideoFormat);

      
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerSetVideoResolutionEx(IntPtr hCaptureObject, ref VIDEO_RESOLUTION pVideoResolution);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetVideoResolutionEx(IntPtr hCaptureObject, ref VIDEO_RESOLUTION pVideoResolution);

        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetVideoInputFrameRateSupported(IntPtr hCaptureObject, uint[] pdwSupported, ref uint pdwNum);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerSetVideoInputFrameRate(IntPtr hCaptureObject, uint dwFrameRate);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetVideoInputFrameRate(IntPtr hCaptureObject, ref uint pdwFrameRate);

        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerSetAudioSamplingRate(IntPtr hCaptureObject, uint dwSamplingRate);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetAudioSamplingRate(IntPtr hCaptureObject, ref uint pdwSamplingRate);

        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerStartStreaming(IntPtr hCaptureObject);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerStopStreaming(IntPtr hCaptureObject);

        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerSetVideoProcAmp(IntPtr hCaptureObject, uint dwVideoProcAmpProperty, uint dwPropertyValue);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetVideoProcAmp(IntPtr hCaptureObject, uint dwVideoProcAmpProperty, ref uint pdwPropertyValue);



       
      
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetAudioInfo(IntPtr hCaptureObject, ref INPUT_AUDIO_INFO pAudioInfo);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetVideoInfo(IntPtr hCaptureObject, ref INPUT_VIDEO_INFO pVideoInfo);
      
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetMacroVisionMode(IntPtr hCaptureObject, ref uint pdwMode);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetSignalPresence(IntPtr hCaptureObject, ref int pbSignalPresence);

        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerSetVideoPreviewEnabled(IntPtr hCaptureObject, int bEnabled);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetVideoPreviewEnabled(IntPtr hCaptureObject, ref int pbEnabled);

        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerSetAudioPreviewEnabled(IntPtr hCaptureObject, int bEnabled);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetAudioPreviewEnabled(IntPtr hCaptureObject, ref int pbEnabled);


        [DllImport("AVerCapAPI.dll", CharSet = CharSet.Unicode)]
        public static extern int AVerEnumThirdPartyAudioCapSource(IntPtr hCaptureObject, IntPtr pAudioCapSourceInfo, ref uint pdwNum);
        [DllImport("AVerCapAPI.dll", CharSet = CharSet.Unicode)]
        public static extern int AVerEnumThirdPartyAudioCapSourceInputType(IntPtr hCaptureObject, uint dwCapIndex, IntPtr pInputTypeInfo, ref uint pdwNum);
        [DllImport("AVerCapAPI.dll", CharSet = CharSet.Unicode)]
        public static extern int AVerEnumThirdPartyAudioCapSourceSampleFormat(IntPtr hCaptureObject, uint dwCapIndex, IntPtr pFormatInfo, ref uint pdwNum);

        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerSetThirdPartyAudioCapSource(IntPtr hCaptureObject, ref AUDIOCAPTURESOURCE_SETTING pAudioCapSourceSetting);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetThirdPartyAudioCapSource(IntPtr hCaptureObject, ref AUDIOCAPTURESOURCE_SETTING pAudioCapSourceSetting);

       
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerSetEventCallback(IntPtr hCaptureObject, NOTIFYEVENTCALLBACK lpCallback, uint dwOptions, IntPtr lpUserData);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerCaptureImageStart(IntPtr hCaptureObject, ref CAPTURE_IMAGE_INFO pCaptrueImageInfo);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerCaptureImageStop(IntPtr hCaptureObject, uint dwImageType);


        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerSetNoiseReductionEnabled(IntPtr hCaptureObject, int bEnabled);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetNoiseReductionEnabled(IntPtr hCaptureObject, ref int pbEnabled);

        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerSetDeinterlaceMode(IntPtr hCaptureObject, uint dwMode);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetDeinterlaceMode(IntPtr hCaptureObject, ref uint pdwMode);

        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerSetVideoDownscaleMode(IntPtr hCaptureObject, uint dwMode, uint dwWidth, uint dwHeight);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetVideoDownscaleMode(IntPtr hCaptureObject, ref uint pdwMode, ref uint pdwWidth, ref uint pdwHeight);


        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerSetVideoEnhanceMode(IntPtr hCaptureObject, uint dwMode);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetVideoEnhanceMode(IntPtr hCaptureObject, ref uint pdwMode);

        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerSetVideoClippingRect(IntPtr hCaptureObject, RECT rcClippingRect);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetVideoClippingRect(IntPtr hCaptureObject, ref RECT prcClippingRect);

        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerSetVideoRotateMode(IntPtr hCaptureObject, uint dwMode);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetVideoRotateMode(IntPtr hCaptureObject, ref uint pdwMode);

        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerSetVideoUpscaleBlackRect(IntPtr hCaptureObject, RECT rcUpscaleRect);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetVideoUpscaleBlackRect(IntPtr hCaptureObject, ref RECT prcUpscaleRect);

        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerSetVideoOutputFrameRate(IntPtr hCaptureObject, uint dwFrameRate);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetVideoOutputFrameRate(IntPtr hCaptureObject, ref uint pdwFrameRate);

       
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerSetEmbeddedAudioChannel(IntPtr hCaptureObject, uint dwChannels);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetEmbeddedAudioChannel(IntPtr hCaptureObject, ref uint pdwChannels);

        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetDeviceType(uint iCurrentDeviceIndex, ref uint nCardType);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetVideoSourceSupported(IntPtr hCaptureObject, uint[] pdwSupported, ref uint pdwNum);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetVideoResolutionSupported(IntPtr hCaptureObject, uint dwVideoSource, uint dwFormat, uint[] pdwSupported, ref uint pdwNum);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetAudioSourceSupportedEx(IntPtr hCaptureObject, uint dwVideoSource, ref uint dwSupported);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetVideoResolutionRangeSupported(IntPtr hCaptureObject, uint dwVideoSource, uint dwFormat, ref RESOLUTION_RANGE_INFO ResolutionRangeInfo);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetVideoInputFrameRateRangeSupported(IntPtr hCaptureObject, uint dwVideoSource, uint dwFormat, uint dwWidth, uint dwHeight, ref FRAMERATE_RANGE_INFO FrameRateRangeInfo);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerGetVideoInputFrameRateSupportedEx(IntPtr hCaptureObject, uint dwVideoSource, uint dwVideoFormat, uint dwVideoResolution, uint[] pdwSupported, ref uint pdwNum);


        //Record File
        [DllImport("AVerCapAPI.dll", CharSet = CharSet.Unicode)]
	    public static extern int AVerStartRecordFile(IntPtr hCaptureObject,ref IntPtr phRecordObject,string szRecordConfigFilePath);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerPauseRecordFile(IntPtr hRecordObject, uint dwstatus);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerSplitRecordFile(IntPtr hRecordObject);
        [DllImport("AVerCapAPI.dll")]
        public static extern int AVerStopRecordFile(IntPtr hRecordObject);

    }
}




























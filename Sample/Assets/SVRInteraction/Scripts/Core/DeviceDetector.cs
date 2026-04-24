using System;

namespace Skyworth.XR.Interaction
{

public enum SVRDeviceType { Unknown, V0920, V0930 }

public static class DeviceDetector
{
    private static SVRDeviceType s_device;
    public static SVRDeviceType CurrentDevice => s_device;

    static DeviceDetector()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        string board = SystemProperties.get("ro.product.board", "");

        if (board.Equals("anorak", StringComparison.OrdinalIgnoreCase))
            s_device = SVRDeviceType.V0930;
        else if (board.Equals("kona", StringComparison.OrdinalIgnoreCase))
            s_device = SVRDeviceType.V0920;
        else
            s_device = SVRDeviceType.Unknown;
#elif UNITY_EDITOR
        s_device = SVRDeviceType.V0920; // Editor 默认 920
#else
        s_device = SVRDeviceType.Unknown; // 非 Android 非 Editor 平台
#endif
    }
}
}

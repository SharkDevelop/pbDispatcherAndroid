using System;
namespace DataUtils
{
    public static class Settings
    {
        public const int maxPacketSize = 500000;
        public const int userTokenLen = 50;
        public const ushort serverVersion = 1;
        public const ushort clientVersion = 1;

        public const int updatePeriodMs = 1000; //in milliseconds
        public const int sensorHistoryUpdatePeriodMs = 1000; //in milliseconds
        public const int pingValueLifeTime = 10000; //in milliseconds

        public const int greyOfflineMinutes = 22;

        public const ushort machineStatesLogMaxElements = 100;
        public const ushort sensorHistoryPointsCount = 100;
    }
}

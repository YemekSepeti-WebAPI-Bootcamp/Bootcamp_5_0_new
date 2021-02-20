using System.Reflection.Metadata;
using System;

namespace Hotels.API.Caches
{
    public class CacheKeys
    {

        private static string RoomListCache => "RoomListCache";
        private static string CustomerCache => "Customer";


        public static string GetCacheKeyCustomer(int key)
                        => string.Concat(CustomerCache, ":", key);

        public static string GetCacheKeyRoomList(string key)
                        => string.Concat(RoomListCache, ":", key);
    }
}

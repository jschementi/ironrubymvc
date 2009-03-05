#region Usings

using System.Threading;

#endregion

namespace System.Web.Mvc.IronRuby.Helpers
{
    internal static class DescriptorUtil
    {
        public static TDescriptor[] LazilyFetchOrCreateDescriptors<TReflection, TDescriptor>(
            ref TDescriptor[] cacheLocation, Func<TReflection[]> initializer, Func<TReflection, TDescriptor> converter)
        {
            // did we already calculate this once?
            var existingCache = Interlocked.CompareExchange(ref cacheLocation, null, null);
            if (existingCache != null)
            {
                return existingCache;
            }

            var memberInfos = initializer();
            var descriptors = new TDescriptor[memberInfos.Length];
            var i = 0;
            foreach (var memberInfo in memberInfos)
            {
                var descriptor = converter(memberInfo);
                if (descriptor != null) descriptors[i++] = descriptor;
            }
            //memberInfos.Select(converter).Where(descriptor => descriptor != null).ToArray();
            var updatedCache = Interlocked.CompareExchange(ref cacheLocation, descriptors, null);
            return updatedCache ?? descriptors;
        }
    }
}
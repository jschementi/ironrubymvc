#region Usings

using System;
using System.Threading;

#endregion

namespace IronRubyMvcLibrary.Helpers
{
    internal static class DescriptorUtil
    {
        public static TDescriptor[] LazilyFetchOrCreateDescriptors<TReflection, TDescriptor>(
            ref TDescriptor[] cacheLocation, Func<TReflection[]> initializer, Func<TReflection, TDescriptor> converter)
        {
            // did we already calculate this once?
            TDescriptor[] existingCache = Interlocked.CompareExchange(ref cacheLocation, null, null);
            if (existingCache != null)
            {
                return existingCache;
            }

            TReflection[] memberInfos = initializer();
            var descriptors = new TDescriptor[memberInfos.Length];
            int i = 0;
            foreach (var memberInfo in memberInfos)
            {
                TDescriptor descriptor = converter(memberInfo);
                if (descriptor != null) descriptors[i++] = descriptor;
            }
            //memberInfos.Select(converter).Where(descriptor => descriptor != null).ToArray();
            TDescriptor[] updatedCache = Interlocked.CompareExchange(ref cacheLocation, descriptors, null);
            return updatedCache ?? descriptors;
        }
    }
}
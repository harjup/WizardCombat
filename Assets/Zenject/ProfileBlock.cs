#define PROFILING_ENABLED

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ModestTree
{
    public class ProfileBlock : IDisposable
    {
#if PROFILING_ENABLED
        public ProfileBlock(string sampleName)
        {
            Profiler.BeginSample(sampleName);
        }

        public static ProfileBlock Start(string sampleName)
        {
            return new ProfileBlock(sampleName);
        }

        public static ProfileBlock Start(string sampleNameFormat, object obj)
        {
            return new ProfileBlock(string.Format(sampleNameFormat, obj));
        }

        public static ProfileBlock Start(string sampleNameFormat, object obj1, object obj2)
        {
            return new ProfileBlock(string.Format(sampleNameFormat, obj1, obj2));
        }

        public void Dispose()
        {
            Assert.That(Application.isEditor);
            Profiler.EndSample();
        }
#else
        public static ProfileBlock Start()
        {
            return null;
        }

        public static ProfileBlock Start(string sampleName)
        {
            return null;
        }

        public static ProfileBlock Start(string sampleNameFormat, object obj)
        {
            return null;
        }

        public static ProfileBlock Start(string sampleNameFormat, object obj1, object obj2)
        {
            return null;
        }

        public void Dispose()
        {
        }
#endif
    }
}

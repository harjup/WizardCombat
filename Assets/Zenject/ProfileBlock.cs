using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using UnityEngine;

//#define PROFILING_ENABLED

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

        public void Dispose()
        {
        }
#endif
    }
}

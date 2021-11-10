//#define USEHANDLE

using AOT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class TestLoopBack : MonoBehaviour
{
    private unsafe class Disco : IDisposable
    {
        private sealed class InternalCallbackHelper
        {
            private static readonly Lazy<InternalCallbackHelper>
                lazy =
                new Lazy<InternalCallbackHelper>
                    (() => new InternalCallbackHelper());

            public static InternalCallbackHelper Instance { get { return lazy.Value; } }

            private InternalCallbackHelper()
            {
                m_cb = null;
            }
            internal ForwardDataCallback m_cb;

            public void Reset()
            {
                m_cb = null;
            }
        }
#if USEHANDLE
        private AtomicSafetyHandle m_Safety;
#endif
        const string exlibn = "cppdll";
        [DllImport(exlibn, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern bool LoopBackCbWithBuffer([MarshalAs(UnmanagedType.FunctionPtr)] ForwardDataCallback cb, IntPtr buffer = default(IntPtr), Int32 size = 0);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public delegate void ForwardDataCallback(IntPtr dataptr, Int32 offset, Int32 size);

        [MonoPInvokeCallback(typeof(ForwardDataCallback))]
        private static void InternalCallbackDelegate(IntPtr srcbuf, Int32 offset, Int32 rawbytessize)
        {
            InternalCallbackHelper.Instance.m_cb(srcbuf, offset, rawbytessize);
        }

        private void ProcessData(IntPtr srcbuf, Int32 offset, Int32 rawbytessize)
        {
#if USEHANDLE
            AtomicSafetyHandle.CheckGetSecondaryDataPointerAndThrow(m_Safety);
#endif
            NativeArray<byte> m_destinationArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>(srcbuf.ToPointer(), m_destinationBufferSize, Allocator.None);
#if USEHANDLE
            AtomicSafetyHandle safety = m_Safety;
            AtomicSafetyHandle.CheckWriteAndBumpSecondaryVersion(safety);
            AtomicSafetyHandle.UseSecondaryVersion(ref safety);
            AtomicSafetyHandle.SetAllowSecondaryVersionWriting(safety, false);

            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref m_destinationArray, safety);
#endif
            Debug.Log("Reading 4 bytes: " + m_destinationArray[0] + " " + m_destinationArray[1] + " " + m_destinationArray[2] + " " + m_destinationArray[3]);
            Debug.Log("Reading 4 bytes: " + m_destinationArray[4] + " " + m_destinationArray[5] + " " + m_destinationArray[6] + " " + m_destinationArray[7]);
            Debug.Log("Reading 4 bytes: " + m_destinationArray[8] + " " + m_destinationArray[9] + " " + m_destinationArray[10] + " " + m_destinationArray[11]);
            Debug.Log("Reading 4 bytes: " + m_destinationArray[12] + " " + m_destinationArray[13] + " " + m_destinationArray[14] + " " + m_destinationArray[15]);
        }

        Int32 m_destinationBufferSize; // 2MB
        IntPtr m_unmanagedDestinationBuffer;
        private byte* _ptr;

        public Disco()
        {
#if USEHANDLE
            m_Safety = AtomicSafetyHandle.Create();
#endif
            m_destinationBufferSize = 1024; // 2MB
            m_unmanagedDestinationBuffer = Marshal.AllocHGlobal(m_destinationBufferSize);
        }

        public void GoTo()
        {
            InternalCallbackHelper.Instance.m_cb = ProcessData;
            bool ok = LoopBackCbWithBuffer(InternalCallbackDelegate, m_unmanagedDestinationBuffer, m_destinationBufferSize);
        }

        public void Dispose()
        {
#if USEHANDLE
            AtomicSafetyHandle.Release(m_Safety);
#endif
            Marshal.FreeHGlobal(m_unmanagedDestinationBuffer);
        }
    }

    Disco d;

    private void Awake()
    {
        d = new Disco();
    }

    // Start is called before the first frame update
    void Start()
    {
        d.GoTo();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        d.Dispose();
    }
}

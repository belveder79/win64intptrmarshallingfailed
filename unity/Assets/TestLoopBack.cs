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
    private class Disco
    {
        const string exlibn = "cppdll";
        [DllImport(exlibn, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern bool LoopBackCbWithBuffer([MarshalAs(UnmanagedType.FunctionPtr)] ForwardDataCallback cb, IntPtr buffer = default(IntPtr), Int32 size = 0);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public delegate void ForwardDataCallback(IntPtr dataptr, Int32 offset, Int32 size);

        [MonoPInvokeCallback(typeof(ForwardDataCallback))]
        private static void InternalCallbackDelegate(IntPtr srcbuf, Int32 offset, Int32 rawbytessize)
        {
            // just try to do something useful with srcbuf here!
            Debug.Log("Testing this!");
        }

        public void GoTo()
        {
            NativeArray<byte> m_destinationArray;
            Int32 m_destinationBufferSize = 2048 * 1024; // 2MB
            IntPtr m_unmanagedDestinationBuffer = Marshal.AllocHGlobal(m_destinationBufferSize);
            unsafe
            {
                m_destinationArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>(m_unmanagedDestinationBuffer.ToPointer(), m_destinationBufferSize, Allocator.Persistent);
            }
            bool ok = LoopBackCbWithBuffer(InternalCallbackDelegate, m_unmanagedDestinationBuffer, m_destinationBufferSize);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Disco d = new Disco();
        d.GoTo();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

#include "cppdll.h"

extern "C" {
    bool STDCALL LoopBackCbWithBuffer(void* callback, const void* outputBufPtr, const int32_t bufferSize)
    {
        callBack m_cb = (callBack)callback;
        m_cb(outputBufPtr, 0, bufferSize);
        return true;
    }
}
#pragma once
#include <cstdint>

#if defined(WIN32) || defined(WINAPI_FAMILY)
#define STDCALL _cdecl
#ifdef _WINDLL
#define DECLDIR __declspec(dllexport)
#else
#define DECLDIR __declspec(dllimport)
#endif
#else
#define STDCALL
#define DECLDIR __attribute__ ((visibility("default")))
#endif

typedef bool(STDCALL* callBack)(const void* bufPtr, const int32_t offset, const int32_t len);


extern "C"
{
    DECLDIR bool STDCALL LoopBackCbWithBuffer(void* callback, void* outputBufPtr, const int32_t bufferSize);
}
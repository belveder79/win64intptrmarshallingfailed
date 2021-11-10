#include "cppdll.h"
#include <thread>

extern "C" {

    bool STDCALL LoopBackCbWithBuffer(void* callback, void* outputBufPtr, const int32_t bufferSize)
    {
        callBack m_cb = (callBack)callback;
		uint8_t* dPtr = (uint8_t*)outputBufPtr;
		/*
		std::thread([m_cb, dPtr, bufferSize]() {
			using namespace std::chrono_literals;
			std::this_thread::sleep_for(2000ms);
		*/
			uint8_t* p = dPtr;
			for (uint8_t i = 0; i < 255; i++)
				*p++ = i;
			m_cb(dPtr, 0, bufferSize);
		//	}).detach();		
        return true;
    }
}
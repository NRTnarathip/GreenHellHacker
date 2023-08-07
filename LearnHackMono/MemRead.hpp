#include <iostream>
#include <memoryapi.h>
#include <cstring>
#include <wchar.h>
#include <windows.h>
#include <iostream>
#include <psapi.h>
#include <string>
#include <format>
#include <TlHelp32.h>
#include <iostream>
#include <iostream>
#include <stdio.h>
#include <tchar.h>
#include <tlhelp32.h>
#include <codecvt>

class Mem {
public:
	static HANDLE hProc;
	static void initProcess(HANDLE hProc) {
		Mem::hProc = hProc;
	}
	static uintptr_t readIntPtr(uintptr_t addr) {
		uintptr_t result = 0;
		BYTE buffer[sizeof(uintptr_t)];
		SIZE_T bytesRead;
		ReadProcessMemory(hProc, (LPCVOID)addr, buffer, sizeof(buffer), &bytesRead);
		std::memcpy(&result, buffer, sizeof(buffer));
		return result;
	}
	static float readFloat(uintptr_t addr) {
		float result = 0;
		BYTE buffer[sizeof(float)];
		SIZE_T bytesRead;
		ReadProcessMemory(hProc, (LPCVOID)addr, buffer, sizeof(buffer), &bytesRead);
		std::memcpy(&result, buffer, sizeof(buffer));
		return result;
	}
};

HANDLE Mem::hProc = 0;
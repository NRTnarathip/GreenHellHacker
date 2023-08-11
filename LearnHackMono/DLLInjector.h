#pragma once
#include "Headers.h"

namespace DLLInjector {
	static bool inject(HANDLE gameProcHandle, HMODULE hKernel32, std::string path) {
		auto kernelAddr = reinterpret_cast<uintptr_t>(hKernel32);
		std::wcout << "Kernel32 handle: " << std::hex << hKernel32 << "\n";
		if (hKernel32 == 0) {
			cout << "Error!! kernel32 is not found" endline;
			return false;
		}

		//DLL Injection
		auto dllInjectionFullPath = path.c_str();
		size_t dllPathLen = path.length();
		//Dll reader to buffer
		// auto dllFile = ifstream(dllInjectionFullPath);
		// dllFile.seekg(0, std::ifstream::end);
		// std::streampos fileSize = dllFile.tellg();
		// dllFile.seekg(0, std::ifstream::beg);

		// cout << "DLL file size: 0x" << fileSize endline;
		// char* dllBuffer = new char[fileSize];
		// dllFile.read(dllBuffer, fileSize);

		cout << "DLL path length: " << dllPathLen endline;

		auto dllAllocateAddress = VirtualAllocEx(gameProcHandle, 0, dllPathLen, MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_READWRITE);
		if (dllAllocateAddress == nullptr) {
			cout << "Error AllocateEx: " << dllAllocateAddress endline;
			return false;
		}
		wcout << "Allocate DLL Injection address: " << dllAllocateAddress endline;

		if (!WriteProcessMemory(gameProcHandle, dllAllocateAddress,
			dllInjectionFullPath, dllPathLen, 0)) {
			cout << "Error write dll buffer to address: " << dllAllocateAddress endline;
			return false;
		}
		printf("Write Dll Buffer To Process Success\n");

		cout << "kernel32  addr: " << kernelAddr << '\n';
		//0x195C0 is offset function kernel32
		auto loadLibraryAddr = reinterpret_cast<LPVOID>(kernelAddr + 0x195C0);
		cout << "load library addr: " << loadLibraryAddr << '\n';

		// Create Remote Thread, LoadLibraryA
		auto dllThreadHandle = CreateRemoteThread(gameProcHandle, NULL, 0,
			(LPTHREAD_START_ROUTINE)loadLibraryAddr,
			dllAllocateAddress, 0, NULL);
		cout << "dll thread: " << dllThreadHandle << '\n';

		//Free Virtual memory Dll Injected
		if (dllThreadHandle == 0) {
			VirtualFreeEx(gameProcHandle, dllAllocateAddress, 0x0, MEM_RELEASE);
			printf("Error can't create remote thread\n");
			return false;
		}
		cout << "Success Inject DLL\n";
		return true;
	}

}


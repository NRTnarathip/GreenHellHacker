#pragma once
#include <iostream>
#include <Windows.h>
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
namespace ProcessHack {
	inline uintptr_t getProcessBaseAddress(DWORD procID) {
		HANDLE procHandle = OpenProcess(
			PROCESS_QUERY_INFORMATION
			| PROCESS_VM_READ
			| PROCESS_VM_OPERATION
			| PROCESS_VM_WRITE,
			FALSE, procID);

		HMODULE handleModules[1024];
		DWORD cbNeeded;
		uintptr_t baseAddress = 0;
		if (EnumProcessModules(procHandle, handleModules, sizeof(handleModules), &cbNeeded)) {
			//get module at first index
			auto moduleHandle = handleModules[0];
			MODULEINFO moduleInfoBuffer;
			GetModuleInformation(procHandle, moduleHandle, &moduleInfoBuffer, sizeof(MODULEINFO));
			return uintptr_t(moduleInfoBuffer.lpBaseOfDll);
		}

		return 0;
	}
	inline	uintptr_t getProcessIdByName(const std::wstring& processName) {
		uintptr_t processId = 0;

		HANDLE snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
		if (snapshot != INVALID_HANDLE_VALUE) {
			PROCESSENTRY32 processEntry = { sizeof(PROCESSENTRY32) };
			if (Process32First(snapshot, &processEntry)) {
				do {
					if (processName.compare(processEntry.szExeFile) == 0) {
						processId = processEntry.th32ProcessID;
						break;
					}
				} while (Process32Next(snapshot, &processEntry));
			}
			CloseHandle(snapshot);
		}

		std::wcout << L"Process ID of " << processName << L": " << processId << std::endl;

		return processId;
	}
}

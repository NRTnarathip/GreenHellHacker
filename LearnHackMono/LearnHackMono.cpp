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

#define UIntPtr uintptr_t;

void hex(uintptr_t n) {
	printf("%X\n", n);
}
uintptr_t GetProcessIdByName(const std::wstring& processName) {
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
int main()
{
	auto gameName = (LPCWSTR)L"GH.exe";
	const wchar_t* monoDllName = L"mono-2.0-bdwgc.dll";
	uintptr_t procID = GetProcessIdByName(gameName);
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
	}
	if (cbNeeded / sizeof(HMODULE) > 0) {
		baseAddress = (uintptr_t)handleModules[0];
	}
	HANDLE monoHandle = 0;
	uintptr_t monoBaseAddress = 0;
	MODULEINFO* monoModuleInfo;

	for (int i = 0; i < (cbNeeded / sizeof(HMODULE)); i++)
	{
		MODULEINFO moduleInfoBuffer;
		wchar_t szModName[MAX_PATH];
		auto moduleHandle = handleModules[i];
		GetModuleInformation(procHandle, moduleHandle,
			&moduleInfoBuffer, 
			sizeof(MODULEINFO));

		auto moduleName = GetModuleBaseNameW(procHandle, moduleHandle, 
			szModName, sizeof(szModName) / sizeof(wchar_t));

		//std::wcout << szModName << " < this mod name\n";

		if (std::wstring(szModName).compare(monoDllName) == 0) {
			monoHandle = moduleHandle;
			monoModuleInfo = &moduleInfoBuffer;
			monoBaseAddress = reinterpret_cast<uintptr_t>(moduleInfoBuffer.lpBaseOfDll);
		}
	}

	std::cout << "Base Address: 0x" << std::hex << baseAddress << std::endl;
	std::wcout << "mono-2.0-bwgc.dll base address 0x" << std::hex << monoBaseAddress << '\n';


	CloseHandle(procHandle);
	return 0;
}
